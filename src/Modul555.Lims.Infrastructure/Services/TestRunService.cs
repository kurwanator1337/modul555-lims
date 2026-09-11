using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Application.Operations;
using Modul555.Lims.Domain.Common;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

public sealed class TestRunService
{
    private readonly LimsDbContext _db;
    private readonly ICurrentUser _user;

    public TestRunService(LimsDbContext db, ICurrentUser user)
    {
        _db = db;
        _user = user;
    }

    public async Task<List<TestRunDto>> ForOrder(Guid orderId, CancellationToken ct)
    {
        var runs = await Query().Where(r => r.ControlOrderId == orderId).ToListAsync(ct);
        return runs.Select(Map).ToList();
    }

    public async Task<List<TestRunDto>> Workplace(Guid? assigneeId, CancellationToken ct)
    {
        var id = assigneeId ?? _user.EmployeeId;
        var query = Query()
            .Where(r => r.Status != TestRunStatus.Cancelled && r.Status != TestRunStatus.Completed);
        if (id.HasValue)
            query = query.Where(r => r.AssigneeId == id || r.AssigneeId == null);
        var runs = await query.OrderBy(r => r.CreatedAt).ToListAsync(ct);
        return runs.Select(Map).ToList();
    }

    public async Task<TestRunDto?> Get(Guid id, CancellationToken ct)
    {
        var run = await Query().FirstOrDefaultAsync(r => r.Id == id, ct);
        return run is null ? null : Map(run);
    }

    public async Task<TestRunDto> SaveMeasurements(
        Guid id,
        SaveMeasurementsRequest request,
        CancellationToken ct
    )
    {
        var run =
            await Query().FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new InvalidOperationException("Испытание не найдено.");

        if (request.EquipmentId.HasValue)
        {
            run.EquipmentId = request.EquipmentId;
            await EnsureEquipment(run, request.EquipmentId.Value, ct);
        }

        if (run.EquipmentId.HasValue)
            await EnsureEquipment(run, run.EquipmentId.Value, ct);

        _db.Measurements.RemoveRange(run.Measurements);
        foreach (var m in request.Measurements)
        {
            _db.Measurements.Add(
                new Measurement
                {
                    TestRunId = run.Id,
                    MeasuredQuantityId = m.MeasuredQuantityId,
                    Replicate = m.Replicate <= 0 ? 1 : m.Replicate,
                    NumericValue = m.NumericValue,
                    BooleanValue = m.BooleanValue,
                    TextValue = m.TextValue,
                    Source = ValueSource.Manual,
                    RecordedAt = DateTimeOffset.UtcNow,
                    RecordedById = _user.EmployeeId,
                }
            );
        }

        run.Status = TestRunStatus.InProgress;
        run.StartedAt ??= DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return (await Get(run.Id, ct))!;
    }

    public async Task<TestRunDto> Complete(Guid id, CancellationToken ct)
    {
        var run =
            await Query().FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new InvalidOperationException("Испытание не найдено.");
        if (run.AssigneeId is null && _user.EmployeeId is { } me)
            run.AssigneeId = me;

        var result = Calculate(run);
        if (run.Result is null)
        {
            result.TestRunId = run.Id;
            _db.ParameterResults.Add(result);
            run.Result = result;
        }
        else
        {
            run.Result.NumericValue = result.NumericValue;
            run.Result.BooleanValue = result.BooleanValue;
            run.Result.TextValue = result.TextValue;
            run.Result.NormSnapshot = result.NormSnapshot;
            run.Result.Verdict = result.Verdict;
            run.Result.CalculatedAt = result.CalculatedAt;
        }

        run.Status = TestRunStatus.Completed;
        run.CompletedAt = DateTimeOffset.UtcNow;
        if (
            run.Sample is not null
            && run.Sample.State is SampleState.Registered or SampleState.Prepared
        )
            run.Sample.State = SampleState.InTesting;

        if (result.Verdict == Verdict.NotConforms)
            await OpenNonconformance(run, result, ct);

        await MaybeFinishOrder(run.ControlOrderId, ct);
        await _db.SaveChangesAsync(ct);
        return (await Get(run.Id, ct))!;
    }

    private ParameterResult Calculate(TestRun run)
    {
        var parameter = run.ControlProgramParameter!;
        var method = parameter.TestMethod;
        var quality = parameter.QualityParameter!;
        var now = DateTimeOffset.UtcNow;
        var norm = VerdictRules.FormatNorm(
            parameter.NormKind,
            parameter.NormMin,
            parameter.NormMax,
            parameter.Tolerance,
            parameter.PercentOfDesign,
            quality.UnitOfMeasure,
            parameter.NormText
        );

        if (quality.ValueKind == ValueKind.Boolean)
        {
            var flags = run
                .Measurements.Where(m => m.BooleanValue.HasValue)
                .Select(m => m.BooleanValue!.Value)
                .ToList();
            var ok = flags.Count > 0 && flags.All(v => v);
            return new ParameterResult
            {
                BooleanValue = ok,
                NumericValue = ok ? 1 : 0,
                NormSnapshot = norm,
                Verdict = VerdictRules.Evaluate(
                    NormKind.MustBeTrue,
                    ok ? 1 : 0,
                    null,
                    null,
                    null,
                    null,
                    null
                ),
                CalculatedAt = now,
            };
        }

        if (
            quality.ValueKind is ValueKind.Enumerated or ValueKind.Text
            || parameter.NormKind == NormKind.Expert
        )
        {
            var text = run
                .Measurements.Select(m => m.TextValue)
                .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
            return new ParameterResult
            {
                TextValue = text,
                NormSnapshot = norm,
                Verdict = Verdict.Pending,
                CalculatedAt = now,
            };
        }

        var quantities =
            method?.Quantities.Select(q => q.MeasuredQuantity!).Where(q => q is not null).ToList()
            ?? run.Measurements.Select(m => m.MeasuredQuantity!).DistinctBy(q => q.Id).ToList();

        var replicateValues = new List<decimal>();
        var maxRep = run.Measurements.Select(m => m.Replicate).DefaultIfEmpty(1).Max();
        for (var r = 1; r <= maxRep; r++)
        {
            var vars = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var quantity in quantities)
            {
                var measurement = run.Measurements.FirstOrDefault(m =>
                    m.Replicate == r && m.MeasuredQuantityId == quantity.Id
                );
                if (measurement?.NumericValue is { } value)
                    vars[quantity.VariableName] = value;
            }
            if (vars.Count == 0)
                continue;
            replicateValues.Add(FormulaCalculator.Evaluate(method?.Formula, vars));
        }

        if (replicateValues.Count == 0)
            throw new InvalidOperationException("Нет числовых измерений для расчёта показателя.");

        var aggregated = parameter.Aggregation switch
        {
            AggregationKind.Minimum => replicateValues.Min(),
            AggregationKind.Maximum => replicateValues.Max(),
            AggregationKind.Last => replicateValues[^1],
            _ => replicateValues.Average(),
        };

        var design =
            run.ControlOrder?.ProductBatch?.DesignValue
            ?? run.ControlOrder?.ProductBatch?.Nomenclature?.DesignValue
            ?? run.ControlOrder?.MaterialBatch?.Nomenclature?.DesignValue
            ?? run.ControlOrder?.ControlProgram?.Nomenclature?.DesignValue;

        var verdict = VerdictRules.Evaluate(
            parameter.NormKind,
            aggregated,
            parameter.NormMin,
            parameter.NormMax,
            parameter.Tolerance,
            parameter.PercentOfDesign,
            design
        );
        return new ParameterResult
        {
            NumericValue = decimal.Round(aggregated, quality.Precision),
            NormSnapshot = norm,
            Verdict = verdict,
            CalculatedAt = now,
        };
    }

    private async Task OpenNonconformance(TestRun run, ParameterResult result, CancellationToken ct)
    {
        var exists = await _db.Nonconformances.AnyAsync(
            n =>
                n.ControlOrderId == run.ControlOrderId
                && n.ParameterName == run.ControlProgramParameter!.QualityParameter!.Name
                && n.Status != NonconformanceStatus.Closed,
            ct
        );
        if (exists)
            return;

        var number = await Numbering.NextAsync(
            _db,
            "НС",
            d => d.Nonconformances.Select(n => n.Number),
            ct
        );
        var order = run.ControlOrder!;
        _db.Nonconformances.Add(
            new Nonconformance
            {
                Number = number,
                Stage =
                    order.ControlKind == ControlKind.Incoming
                        ? ControlStage.Incoming
                        : ControlStage.Operational,
                ControlOrderId = order.Id,
                MaterialBatchId = order.MaterialBatchId,
                ProductBatchId = order.ProductBatchId,
                DetectedAt = DateTimeOffset.UtcNow,
                DetectedById = _user.EmployeeId,
                ParameterName = run.ControlProgramParameter!.QualityParameter!.Name,
                ActualValue =
                    result.NumericValue?.ToString()
                    ?? result.TextValue
                    ?? (result.BooleanValue is true ? "да" : "нет"),
                NormValue = result.NormSnapshot,
                Status = NonconformanceStatus.Open,
            }
        );

        if (order.MaterialBatch is not null)
        {
            order.MaterialBatch.Status = BatchStatus.Quarantine;
            var quarantine = await _db.Warehouses.FirstOrDefaultAsync(w => w.IsQuarantineZone, ct);
            if (quarantine is not null)
                order.MaterialBatch.WarehouseId = quarantine.Id;
        }
        if (order.ProductBatch is not null)
            order.ProductBatch.Status = BatchStatus.Quarantine;
    }

    private async Task MaybeFinishOrder(Guid orderId, CancellationToken ct)
    {
        var order = await _db
            .ControlOrders.Include(o => o.TestRuns)
                .ThenInclude(r => r.Result)
            .Include(o => o.MaterialBatch)
            .Include(o => o.ProductBatch)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order is null)
            return;
        if (
            order.TestRuns.Any(r =>
                r.Status != TestRunStatus.Completed && r.Status != TestRunStatus.Cancelled
            )
        )
            return;

        order.Status = ControlOrderStatus.Completed;
        order.CompletedAt = DateTimeOffset.UtcNow;
        var failed = order.TestRuns.Any(r => r.Result?.Verdict == Verdict.NotConforms);
        if (!failed)
        {
            if (
                order.MaterialBatch is not null
                && order.MaterialBatch.Status != BatchStatus.Quarantine
            )
                order.MaterialBatch.Status = BatchStatus.Approved;
            if (
                order.ProductBatch is not null
                && order.ProductBatch.Status != BatchStatus.Quarantine
            )
                order.ProductBatch.Status = BatchStatus.Approved;
        }
    }

    private async Task EnsureEquipment(TestRun run, Guid equipmentId, CancellationToken ct)
    {
        var equipment =
            await _db.Equipment.FindAsync([equipmentId], ct)
            ?? throw new InvalidOperationException("Оборудование не найдено.");
        if (
            equipment.Status != EquipmentStatus.Operational
            || (
                equipment.VerificationValidUntil is { } until
                && until < DateOnly.FromDateTime(DateTime.UtcNow)
            )
        )
            throw new InvalidOperationException(
                "Средство измерения непригодно: поверка истекла или прибор не в работе."
            );
    }

    private IQueryable<TestRun> Query() =>
        _db
            .TestRuns.Include(r => r.ControlOrder)!
                .ThenInclude(o => o!.MaterialBatch)!
                    .ThenInclude(b => b!.Nomenclature)
            .Include(r => r.ControlOrder)!
                .ThenInclude(o => o!.ProductBatch)!
                    .ThenInclude(b => b!.Nomenclature)
            .Include(r => r.ControlOrder)!
                .ThenInclude(o => o!.ControlProgram)!
                    .ThenInclude(p => p!.Nomenclature)
            .Include(r => r.Sample)
            .Include(r => r.Assignee)
            .Include(r => r.Equipment)
            .Include(r => r.Result)
            .Include(r => r.Measurements)
                .ThenInclude(m => m.MeasuredQuantity)
            .Include(r => r.ControlProgramParameter)!
                .ThenInclude(p => p!.QualityParameter)
            .Include(r => r.ControlProgramParameter)!
                .ThenInclude(p => p!.TestMethod)!
                    .ThenInclude(m => m!.Quantities)
                        .ThenInclude(q => q.MeasuredQuantity);

    private static TestRunDto Map(TestRun r)
    {
        var p = r.ControlProgramParameter;
        return new TestRunDto
        {
            Id = r.Id,
            ControlOrderId = r.ControlOrderId,
            ControlOrderNumber = r.ControlOrder?.Number,
            SampleId = r.SampleId,
            SampleNumber = r.Sample?.Number,
            ControlProgramParameterId = r.ControlProgramParameterId,
            ParameterName = p?.QualityParameter?.Name ?? string.Empty,
            UnitOfMeasure = p?.QualityParameter?.UnitOfMeasure,
            MethodName = p?.TestMethod?.Name,
            MethodId = p?.TestMethodId,
            AssigneeId = r.AssigneeId,
            AssigneeName = r.Assignee?.FullName,
            EquipmentId = r.EquipmentId,
            EquipmentName = r.Equipment?.Name,
            Status = r.Status,
            Replicates = p?.Replicates ?? 1,
            NormFormatted = p is null
                ? null
                : VerdictRules.FormatNorm(
                    p.NormKind,
                    p.NormMin,
                    p.NormMax,
                    p.Tolerance,
                    p.PercentOfDesign,
                    p.QualityParameter?.UnitOfMeasure,
                    p.NormText
                ),
            Verdict = r.Result?.Verdict ?? Verdict.Pending,
            ResultValue = r.Result?.NumericValue,
            ResultBoolean = r.Result?.BooleanValue,
            ResultText = r.Result?.TextValue,
            Measurements = r
                .Measurements.OrderBy(m => m.Replicate)
                .ThenBy(m => m.CreatedAt)
                .Select(m => new MeasurementDto
                {
                    Id = m.Id,
                    MeasuredQuantityId = m.MeasuredQuantityId,
                    QuantityName = m.MeasuredQuantity?.Name,
                    Replicate = m.Replicate,
                    NumericValue = m.NumericValue,
                    BooleanValue = m.BooleanValue,
                    TextValue = m.TextValue,
                    Source = m.Source,
                })
                .ToList(),
            Quantities =
                p?.TestMethod?.Quantities.OrderBy(q => q.SortOrder)
                    .Select(q => new QuantityHintDto
                    {
                        Id = q.MeasuredQuantityId,
                        Name = q.MeasuredQuantity?.Name ?? string.Empty,
                        VariableName = q.MeasuredQuantity?.VariableName ?? string.Empty,
                        UnitOfMeasure = q.MeasuredQuantity?.UnitOfMeasure,
                    })
                    .ToList()
                ?? [],
        };
    }
}
