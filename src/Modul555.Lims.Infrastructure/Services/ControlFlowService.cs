using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Application.Operations;
using Modul555.Lims.Domain.Common;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

public sealed class ControlFlowService
{
    private readonly LimsDbContext _db;
    private readonly ICurrentUser _user;

    public ControlFlowService(LimsDbContext db, ICurrentUser user)
    {
        _db = db;
        _user = user;
    }

    public async Task<PagedResult<MaterialBatchDto>> MaterialBatches(
        PagedQuery q,
        CancellationToken ct
    )
    {
        var query = _db
            .MaterialBatches.Include(b => b.Nomenclature)
            .Include(b => b.Supplier)
            .Include(b => b.Warehouse)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(b =>
                b.Number.Contains(s)
                || b.Nomenclature!.Name.Contains(s)
                || b.Supplier!.Name.Contains(s)
            );
        }
        query = query.OrderByDescending(b => b.ArrivalDate).ThenByDescending(b => b.CreatedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<MaterialBatchDto>
        {
            Items = items.Select(MapBatch).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<MaterialBatchDto> CreateMaterialBatch(
        MaterialBatchDto dto,
        CancellationToken ct
    )
    {
        var entity = new MaterialBatch
        {
            Number = string.IsNullOrWhiteSpace(dto.Number)
                ? await Numbering.NextAsync(
                    _db,
                    "ПТ",
                    d => d.MaterialBatches.Select(b => b.Number),
                    ct
                )
                : dto.Number,
            NomenclatureId = dto.NomenclatureId,
            SupplierId = dto.SupplierId,
            ArrivalDate =
                dto.ArrivalDate == default
                    ? DateOnly.FromDateTime(DateTime.UtcNow)
                    : dto.ArrivalDate,
            Quantity = dto.Quantity,
            QuantityUnit = dto.QuantityUnit,
            SupplierDocumentNumber = dto.SupplierDocumentNumber,
            SupplierDocumentDate = dto.SupplierDocumentDate,
            ManufacturedOn = dto.ManufacturedOn,
            WarehouseId = dto.WarehouseId,
            Status = BatchStatus.AwaitingControl,
        };
        _db.MaterialBatches.Add(entity);
        await _db.SaveChangesAsync(ct);
        return (await GetMaterialBatch(entity.Id, ct))!;
    }

    public async Task<MaterialBatchDto?> GetMaterialBatch(Guid id, CancellationToken ct)
    {
        var b = await _db
            .MaterialBatches.Include(x => x.Nomenclature)
            .Include(x => x.Supplier)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return b is null ? null : MapBatch(b);
    }

    public async Task<PagedResult<ProductBatchDto>> ProductBatches(
        PagedQuery q,
        CancellationToken ct
    )
    {
        var query = _db
            .ProductBatches.Include(b => b.Nomenclature)
            .Include(b => b.ProductionLine)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(b => b.Number.Contains(s) || b.Nomenclature!.Name.Contains(s));
        }
        query = query.OrderByDescending(b => b.ProductionDate);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<ProductBatchDto>
        {
            Items = items.Select(MapProduct).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<ProductBatchDto> CreateProductBatch(ProductBatchDto dto, CancellationToken ct)
    {
        var entity = new ProductBatch
        {
            Number = string.IsNullOrWhiteSpace(dto.Number)
                ? await Numbering.NextAsync(
                    _db,
                    "ПР",
                    d => d.ProductBatches.Select(b => b.Number),
                    ct
                )
                : dto.Number,
            Kind = dto.Kind,
            NomenclatureId = dto.NomenclatureId,
            ProductionLineId = dto.ProductionLineId,
            ProductionDate =
                dto.ProductionDate == default
                    ? DateOnly.FromDateTime(DateTime.UtcNow)
                    : dto.ProductionDate,
            DesignValue = dto.DesignValue,
            Status = BatchStatus.AwaitingControl,
        };
        _db.ProductBatches.Add(entity);
        await _db.SaveChangesAsync(ct);
        return MapProduct(entity);
    }

    public async Task<PagedResult<ControlOrderDto>> Orders(
        PagedQuery q,
        ControlKind? kind,
        CancellationToken ct
    )
    {
        var query = _db
            .ControlOrders.Include(o => o.ControlProgram)
            .Include(o => o.MaterialBatch)
            .Include(o => o.ProductBatch)
            .Include(o => o.ResponsibleEmployee)
            .Include(o => o.Samples)
            .Include(o => o.TestRuns)
            .AsQueryable();
        if (kind.HasValue)
            query = query.Where(o => o.ControlKind == kind);
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(o => o.Number.Contains(s));
        }
        query = query.OrderByDescending(o => o.PlannedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<ControlOrderDto>
        {
            Items = items.Select(MapOrder).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<ControlOrderDto> CreateIncoming(
        CreateIncomingOrderRequest request,
        CancellationToken ct
    )
    {
        var batch =
            await _db.MaterialBatches.FindAsync([request.MaterialBatchId], ct)
            ?? throw new InvalidOperationException("Партия сырья не найдена.");
        var programId =
            request.ControlProgramId
            ?? (
                await _db.ControlPrograms.FirstOrDefaultAsync(
                    p =>
                        p.NomenclatureId == batch.NomenclatureId
                        && p.ControlKind == ControlKind.Incoming
                        && p.IsApproved
                        && p.IsActive,
                    ct
                )
            )?.Id
            ?? throw new InvalidOperationException(
                "Не найдена утверждённая программа входного контроля для этой номенклатуры."
            );

        var order = await CreateOrderCore(
            ControlKind.Incoming,
            ControlOrderSource.MaterialArrival,
            programId,
            request.ResponsibleEmployeeId,
            request.Comment,
            ct
        );
        order.MaterialBatchId = batch.Id;
        batch.Status = BatchStatus.InControl;
        await _db.SaveChangesAsync(ct);
        return (await GetOrder(order.Id, ct))!;
    }

    public async Task<ControlOrderDto> CreateOperational(
        CreateOperationalOrderRequest request,
        CancellationToken ct
    )
    {
        var batch =
            await _db.ProductBatches.FindAsync([request.ProductBatchId], ct)
            ?? throw new InvalidOperationException("Производственная партия не найдена.");
        var order = await CreateOrderCore(
            ControlKind.Operational,
            ControlOrderSource.ProductionBatch,
            request.ControlProgramId,
            request.ResponsibleEmployeeId,
            request.Comment,
            ct
        );
        order.ProductBatchId = batch.Id;
        batch.Status = BatchStatus.InControl;
        await _db.SaveChangesAsync(ct);
        return (await GetOrder(order.Id, ct))!;
    }

    public async Task<ControlOrderDto?> GetOrder(Guid id, CancellationToken ct)
    {
        var o = await _db
            .ControlOrders.Include(x => x.ControlProgram)
            .Include(x => x.MaterialBatch)
            .Include(x => x.ProductBatch)
            .Include(x => x.ResponsibleEmployee)
            .Include(x => x.Samples)
            .Include(x => x.TestRuns)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return o is null ? null : MapOrder(o);
    }

    public async Task<SampleDto> RegisterSample(
        Guid orderId,
        RegisterSampleRequest request,
        CancellationToken ct
    )
    {
        var order =
            await _db
                .ControlOrders.Include(o => o.ControlProgram)
                    .ThenInclude(p => p!.Nomenclature)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new InvalidOperationException("Распоряжение не найдено.");

        var number = await Numbering.NextAsync(
            _db,
            "ПРБ",
            d => d.Samples.Select(s => s.Number),
            ct
        );
        var nom = order.ControlProgram?.Nomenclature?.Code ?? "X";
        var barcode = $"555{DateTime.UtcNow:yyMMdd}{Random.Shared.Next(1000, 9999)}";
        var sample = new Sample
        {
            Number = $"{number}-{nom}",
            Barcode = barcode,
            ControlOrderId = orderId,
            SampledAt = DateTimeOffset.UtcNow,
            SampledById = request.SampledById ?? _user.EmployeeId,
            WarehouseId = request.WarehouseId,
            Quantity = request.Quantity,
            QuantityUnit = request.QuantityUnit,
            State = SampleState.Registered,
        };
        sample.History.Add(
            new SampleStateHistory
            {
                FromState = SampleState.Registered,
                ToState = SampleState.Registered,
                ChangedAt = DateTimeOffset.UtcNow,
                ChangedById = _user.EmployeeId,
                Comment = "Регистрация пробы",
            }
        );
        _db.Samples.Add(sample);

        if (order.Status == ControlOrderStatus.Draft)
            order.Status = ControlOrderStatus.Approved;
        if (order.Status == ControlOrderStatus.Approved)
            order.Status = ControlOrderStatus.InProgress;

        await EnsureTestRuns(order, sample.Id, ct);
        await _db.SaveChangesAsync(ct);
        return MapSample(sample);
    }

    public async Task<List<SampleDto>> Samples(Guid orderId, CancellationToken ct)
    {
        var items = await _db
            .Samples.Include(s => s.SampledBy)
            .Where(s => s.ControlOrderId == orderId)
            .OrderBy(s => s.SampledAt)
            .ToListAsync(ct);
        return items.Select(MapSample).ToList();
    }

    public async Task ChangeSampleState(
        Guid sampleId,
        SampleState to,
        string? comment,
        CancellationToken ct
    )
    {
        var sample =
            await _db.Samples.FindAsync([sampleId], ct)
            ?? throw new InvalidOperationException("Проба не найдена.");
        var from = sample.State;
        sample.State = to;
        _db.SampleStateHistory.Add(
            new SampleStateHistory
            {
                SampleId = sample.Id,
                FromState = from,
                ToState = to,
                ChangedAt = DateTimeOffset.UtcNow,
                ChangedById = _user.EmployeeId,
                Comment = comment,
            }
        );
        await _db.SaveChangesAsync(ct);
    }

    public async Task Assign(Guid testRunId, AssignTestRunRequest request, CancellationToken ct)
    {
        var run =
            await _db
                .TestRuns.Include(r => r.ControlProgramParameter)!
                    .ThenInclude(p => p!.TestMethod)
                .FirstOrDefaultAsync(r => r.Id == testRunId, ct)
            ?? throw new InvalidOperationException("Испытание не найдено.");

        await EnsureAssigneeAllowed(run, request.AssigneeId, ct);
        if (request.EquipmentId.HasValue)
            await EnsureEquipmentAllowed(run, request.EquipmentId.Value, ct);

        run.AssigneeId = request.AssigneeId;
        run.EquipmentId = request.EquipmentId;
        await _db.SaveChangesAsync(ct);
    }

    private async Task<ControlOrder> CreateOrderCore(
        ControlKind kind,
        ControlOrderSource source,
        Guid programId,
        Guid? responsibleId,
        string? comment,
        CancellationToken ct
    )
    {
        var program =
            await _db
                .ControlPrograms.Include(p => p.Parameters)
                .FirstOrDefaultAsync(p => p.Id == programId, ct)
            ?? throw new InvalidOperationException("Программа испытаний не найдена.");
        if (!program.IsApproved)
            throw new InvalidOperationException("Программа испытаний не утверждена.");

        var order = new ControlOrder
        {
            Number = await Numbering.NextAsync(
                _db,
                kind == ControlKind.Incoming ? "ВК" : "ОК",
                d => d.ControlOrders.Select(o => o.Number),
                ct
            ),
            ControlKind = kind,
            Source = source,
            Status = ControlOrderStatus.Approved,
            ControlProgramId = programId,
            PlannedAt = DateTimeOffset.UtcNow,
            ResponsibleEmployeeId = responsibleId ?? _user.EmployeeId,
            Comment = comment,
        };
        _db.ControlOrders.Add(order);
        return order;
    }

    private async Task EnsureTestRuns(ControlOrder order, Guid sampleId, CancellationToken ct)
    {
        var existing = await _db
            .TestRuns.Where(r => r.ControlOrderId == order.Id)
            .Select(r => r.ControlProgramParameterId)
            .ToListAsync(ct);
        var parameters = await _db
            .ControlProgramParameters.Where(p => p.ControlProgramId == order.ControlProgramId)
            .ToListAsync(ct);
        foreach (var parameter in parameters.Where(p => !existing.Contains(p.Id)))
        {
            _db.TestRuns.Add(
                new TestRun
                {
                    ControlOrderId = order.Id,
                    SampleId = sampleId,
                    ControlProgramParameterId = parameter.Id,
                    Status = TestRunStatus.Assigned,
                }
            );
        }
    }

    private async Task EnsureAssigneeAllowed(TestRun run, Guid employeeId, CancellationToken ct)
    {
        var methodId = run.ControlProgramParameter?.TestMethodId;
        if (methodId is null)
            return;
        var method =
            run.ControlProgramParameter!.TestMethod
            ?? await _db.TestMethods.FindAsync([methodId], ct);
        if (method is null || !method.RequiresCompetency)
            return;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var ok = await _db.Competencies.AnyAsync(
            c =>
                c.EmployeeId == employeeId
                && c.TestMethodId == methodId
                && c.IssuedOn <= today
                && c.ValidUntil >= today,
            ct
        );
        if (!ok)
            throw new InvalidOperationException(
                "У сотрудника нет действующей аттестации по этому методу."
            );
    }

    private async Task EnsureEquipmentAllowed(TestRun run, Guid equipmentId, CancellationToken ct)
    {
        var equipment =
            await _db.Equipment.FindAsync([equipmentId], ct)
            ?? throw new InvalidOperationException("Оборудование не найдено.");
        if (equipment.Status != EquipmentStatus.Operational)
            throw new InvalidOperationException(
                $"Оборудование в статусе «{equipment.Status}» и не может использоваться."
            );
        if (
            equipment.VerificationValidUntil is { } until
            && until < DateOnly.FromDateTime(DateTime.UtcNow)
        )
            throw new InvalidOperationException(
                "Поверка средства измерения истекла — ввод данных заблокирован."
            );

        var methodId = run.ControlProgramParameter?.TestMethodId;
        if (methodId is null)
            return;
        var allowed = await _db.TestMethodEquipmentTypes.AnyAsync(
            x => x.TestMethodId == methodId && x.EquipmentTypeId == equipment.EquipmentTypeId,
            ct
        );
        if (
            !allowed
            && await _db.TestMethodEquipmentTypes.AnyAsync(x => x.TestMethodId == methodId, ct)
        )
            throw new InvalidOperationException(
                "Этот тип оборудования не применим в выбранном методе."
            );
    }

    private static MaterialBatchDto MapBatch(MaterialBatch b) =>
        new()
        {
            Id = b.Id,
            Number = b.Number,
            NomenclatureId = b.NomenclatureId,
            NomenclatureName = b.Nomenclature?.Name,
            SupplierId = b.SupplierId,
            SupplierName = b.Supplier?.Name,
            ArrivalDate = b.ArrivalDate,
            Quantity = b.Quantity,
            QuantityUnit = b.QuantityUnit,
            SupplierDocumentNumber = b.SupplierDocumentNumber,
            SupplierDocumentDate = b.SupplierDocumentDate,
            ManufacturedOn = b.ManufacturedOn,
            WarehouseId = b.WarehouseId,
            WarehouseName = b.Warehouse?.Name,
            Status = b.Status,
            Decision = b.Decision,
            DecisionComment = b.DecisionComment,
        };

    private static ProductBatchDto MapProduct(ProductBatch b) =>
        new()
        {
            Id = b.Id,
            Number = b.Number,
            Kind = b.Kind,
            NomenclatureId = b.NomenclatureId,
            NomenclatureName = b.Nomenclature?.Name,
            ProductionLineId = b.ProductionLineId,
            ProductionLineName = b.ProductionLine?.Name,
            ProductionDate = b.ProductionDate,
            DesignValue = b.DesignValue,
            Status = b.Status,
        };

    private static ControlOrderDto MapOrder(ControlOrder o) =>
        new()
        {
            Id = o.Id,
            Number = o.Number,
            ControlKind = o.ControlKind,
            Source = o.Source,
            Status = o.Status,
            ControlProgramId = o.ControlProgramId,
            ControlProgramName = o.ControlProgram?.Name,
            MaterialBatchId = o.MaterialBatchId,
            MaterialBatchNumber = o.MaterialBatch?.Number,
            ProductBatchId = o.ProductBatchId,
            ProductBatchNumber = o.ProductBatch?.Number,
            PlannedAt = o.PlannedAt,
            CompletedAt = o.CompletedAt,
            ResponsibleEmployeeId = o.ResponsibleEmployeeId,
            ResponsibleName = o.ResponsibleEmployee?.FullName,
            Comment = o.Comment,
            SampleCount = o.Samples.Count,
            TestRunCount = o.TestRuns.Count,
            CompletedTestCount = o.TestRuns.Count(r => r.Status == TestRunStatus.Completed),
        };

    private static SampleDto MapSample(Sample s) =>
        new()
        {
            Id = s.Id,
            Number = s.Number,
            Barcode = s.Barcode,
            ControlOrderId = s.ControlOrderId,
            SampledAt = s.SampledAt,
            SampledById = s.SampledById,
            SampledByName = s.SampledBy?.FullName,
            State = s.State,
            Quantity = s.Quantity,
            QuantityUnit = s.QuantityUnit,
        };
}
