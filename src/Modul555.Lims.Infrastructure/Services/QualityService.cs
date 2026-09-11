using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Application.Operations;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

public sealed class QualityService
{
    private readonly LimsDbContext _db;
    private readonly ICurrentUser _user;
    private readonly TestRunService _runs;

    public QualityService(LimsDbContext db, ICurrentUser user, TestRunService runs)
    {
        _db = db;
        _user = user;
        _runs = runs;
    }

    public async Task<List<ProtocolDto>> ForOrder(Guid orderId, CancellationToken ct)
    {
        var items = await _db
            .Protocols.Include(p => p.Signatures)
                .ThenInclude(s => s.Employee)
            .Where(p => p.ControlOrderId == orderId)
            .OrderByDescending(p => p.IssuedAt)
            .ToListAsync(ct);
        var runs = await _runs.ForOrder(orderId, ct);
        return items.Select(p => Map(p, runs)).ToList();
    }

    public async Task<PagedResult<ProtocolDto>> List(PagedQuery q, CancellationToken ct)
    {
        var query = _db
            .Protocols.Include(p => p.Signatures)
                .ThenInclude(s => s.Employee)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(p => p.Number.Contains(s));
        }
        query = query.OrderByDescending(p => p.IssuedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<ProtocolDto>
        {
            Items = items.Select(p => Map(p, [])).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<ProtocolDto> Issue(Guid orderId, DocumentKind kind, CancellationToken ct)
    {
        var order =
            await _db
                .ControlOrders.Include(o => o.TestRuns)
                    .ThenInclude(r => r.Result)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new InvalidOperationException("Распоряжение не найдено.");
        var runs = await _runs.ForOrder(orderId, ct);
        var template = await _db.DocumentTemplates.FirstOrDefaultAsync(t => t.Kind == kind, ct);
        var verdict =
            runs.Any(r => r.Verdict == Verdict.NotConforms) ? Verdict.NotConforms
            : runs.All(r => r.Verdict == Verdict.Conforms) ? Verdict.Conforms
            : Verdict.Pending;

        var protocol = new Protocol
        {
            Number = await Numbering.NextAsync(
                _db,
                "ПРТ",
                d => d.Protocols.Select(p => p.Number),
                ct
            ),
            Kind = kind,
            TemplateId = template?.Id,
            ControlOrderId = orderId,
            IssuedAt = DateTimeOffset.UtcNow,
            Status = ProtocolStatus.Draft,
            OverallVerdict = verdict,
            ResultsSnapshot = JsonSerializer.Serialize(
                runs.Select(r => new
                {
                    r.ParameterName,
                    r.ResultValue,
                    r.Verdict,
                    r.NormFormatted,
                })
            ),
            Conclusion =
                verdict == Verdict.Conforms ? "Соответствует требованиям НТД."
                : verdict == Verdict.NotConforms ? "Не соответствует требованиям НТД."
                : "Испытания не завершены.",
        };
        _db.Protocols.Add(protocol);
        await _db.SaveChangesAsync(ct);
        return Map(protocol, runs);
    }

    public async Task<ProtocolDto> Sign(Guid protocolId, SignatureRole role, CancellationToken ct)
    {
        var protocol =
            await _db
                .Protocols.Include(p => p.Signatures)
                .FirstOrDefaultAsync(p => p.Id == protocolId, ct)
            ?? throw new InvalidOperationException("Протокол не найден.");
        if (_user.EmployeeId is null)
            throw new InvalidOperationException(
                "Текущий пользователь не связан с карточкой сотрудника."
            );

        if (protocol.Signatures.Any(s => s.Role == role && s.EmployeeId == _user.EmployeeId))
            throw new InvalidOperationException("Документ уже подписан этой ролью.");

        protocol.Signatures.Add(
            new ProtocolSignature
            {
                Role = role,
                EmployeeId = _user.EmployeeId.Value,
                SignedAt = DateTimeOffset.UtcNow,
                Kind = SignatureKind.Simple,
            }
        );

        if (role == SignatureRole.Performer && protocol.Status == ProtocolStatus.Draft)
            protocol.Status = ProtocolStatus.Signed;
        if (role is SignatureRole.Approver or SignatureRole.QualityInspector)
            protocol.Status = ProtocolStatus.Approved;

        await _db.SaveChangesAsync(ct);
        var runs = protocol.ControlOrderId is { } orderId ? await _runs.ForOrder(orderId, ct) : [];
        return Map(protocol, runs);
    }

    public async Task<PagedResult<NonconformanceDto>> Nonconformances(
        PagedQuery q,
        CancellationToken ct
    )
    {
        var query = _db
            .Nonconformances.Include(n => n.Cause)
            .Include(n => n.MaterialBatch)
            .Include(n => n.ProductBatch)
            .Include(n => n.Actions)
                .ThenInclude(a => a.AssignedTo)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(n => n.Number.Contains(s) || n.ParameterName.Contains(s));
        }
        query = query.OrderByDescending(n => n.DetectedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<NonconformanceDto>
        {
            Items = items.Select(MapNc).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<NonconformanceDto> Decide(
        Guid id,
        DecideNonconformanceRequest request,
        CancellationToken ct
    )
    {
        var nc =
            await _db
                .Nonconformances.Include(n => n.MaterialBatch)
                .Include(n => n.ProductBatch)
                .Include(n => n.Actions)
                .FirstOrDefaultAsync(n => n.Id == id, ct)
            ?? throw new InvalidOperationException("Несоответствие не найдено.");
        nc.Decision = request.Decision;
        nc.DecisionComment = request.DecisionComment;
        nc.CauseId = request.CauseId;
        nc.CauseComment = request.CauseComment;
        nc.Status = NonconformanceStatus.ActionsPlanned;

        ApplyDecision(nc);
        await _db.SaveChangesAsync(ct);
        return MapNc(nc);
    }

    public async Task<CorrectiveActionDto> AddAction(
        Guid ncId,
        CreateCorrectiveActionRequest request,
        CancellationToken ct
    )
    {
        var nc =
            await _db.Nonconformances.FindAsync([ncId], ct)
            ?? throw new InvalidOperationException("Несоответствие не найдено.");
        var action = new CorrectiveAction
        {
            NonconformanceId = nc.Id,
            Description = request.Description,
            AssignedToId = request.AssignedToId,
            DueDate = request.DueDate,
            Status = CorrectiveActionStatus.Planned,
        };
        _db.CorrectiveActions.Add(action);
        nc.Status = NonconformanceStatus.ActionsPlanned;
        await _db.SaveChangesAsync(ct);
        return new CorrectiveActionDto
        {
            Id = action.Id,
            Description = action.Description,
            AssignedToId = action.AssignedToId,
            DueDate = action.DueDate,
            Status = action.Status,
        };
    }

    public async Task CloseAction(Guid actionId, string? result, CancellationToken ct)
    {
        var action =
            await _db
                .CorrectiveActions.Include(a => a.Nonconformance)
                    .ThenInclude(n => n!.Actions)
                .FirstOrDefaultAsync(a => a.Id == actionId, ct)
            ?? throw new InvalidOperationException("Действие не найдено.");
        action.Status = CorrectiveActionStatus.Done;
        action.CompletedAt = DateTimeOffset.UtcNow;
        action.Result = result;
        if (
            action.Nonconformance!.Actions.All(a =>
                a.Status is CorrectiveActionStatus.Done or CorrectiveActionStatus.Cancelled
            )
        )
            action.Nonconformance.Status = NonconformanceStatus.Closed;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<PassportDto> Passport(Guid? productId, Guid? materialId, CancellationToken ct)
    {
        var dto = new PassportDto();
        if (productId.HasValue)
        {
            var product = await _db
                .ProductBatches.Include(p => p.Nomenclature)
                .Include(p => p.ProductionLine)
                .FirstOrDefaultAsync(p => p.Id == productId, ct);
            dto.Product = product is null
                ? null
                : new ProductBatchDto
                {
                    Id = product.Id,
                    Number = product.Number,
                    Kind = product.Kind,
                    NomenclatureId = product.NomenclatureId,
                    NomenclatureName = product.Nomenclature?.Name,
                    ProductionLineId = product.ProductionLineId,
                    ProductionLineName = product.ProductionLine?.Name,
                    ProductionDate = product.ProductionDate,
                    DesignValue = product.DesignValue,
                    Status = product.Status,
                };
            dto.Materials = await _db
                .TraceabilityLinks.Include(l => l.MaterialBatch)
                    .ThenInclude(b => b!.Nomenclature)
                .Where(l => l.ProductBatchId == productId)
                .Select(l => new TraceabilityLinkDto
                {
                    Id = l.Id,
                    ProductBatchId = l.ProductBatchId,
                    MaterialBatchId = l.MaterialBatchId,
                    MaterialBatchNumber = l.MaterialBatch!.Number,
                    MaterialName = l.MaterialBatch.Nomenclature!.Name,
                    Quantity = l.Quantity,
                })
                .ToListAsync(ct);
            dto.Orders = (
                await _db
                    .ControlOrders.Include(o => o.ControlProgram)
                    .Where(o => o.ProductBatchId == productId)
                    .ToListAsync(ct)
            )
                .Select(o => new ControlOrderDto
                {
                    Id = o.Id,
                    Number = o.Number,
                    ControlKind = o.ControlKind,
                    Status = o.Status,
                    ControlProgramId = o.ControlProgramId,
                    ControlProgramName = o.ControlProgram?.Name,
                    PlannedAt = o.PlannedAt,
                })
                .ToList();
        }

        if (materialId.HasValue)
        {
            var material = await _db
                .MaterialBatches.Include(b => b.Nomenclature)
                .Include(b => b.Supplier)
                .FirstOrDefaultAsync(b => b.Id == materialId, ct);
            if (material is not null)
            {
                dto.Material = new MaterialBatchDto
                {
                    Id = material.Id,
                    Number = material.Number,
                    NomenclatureId = material.NomenclatureId,
                    NomenclatureName = material.Nomenclature?.Name,
                    SupplierId = material.SupplierId,
                    SupplierName = material.Supplier?.Name,
                    ArrivalDate = material.ArrivalDate,
                    Quantity = material.Quantity,
                    Status = material.Status,
                };
            }
            dto.Products = await _db
                .TraceabilityLinks.Include(l => l.ProductBatch)
                    .ThenInclude(b => b!.Nomenclature)
                .Where(l => l.MaterialBatchId == materialId)
                .Select(l => new TraceabilityLinkDto
                {
                    Id = l.Id,
                    ProductBatchId = l.ProductBatchId,
                    ProductBatchNumber = l.ProductBatch!.Number,
                    ProductName = l.ProductBatch.Nomenclature!.Name,
                    MaterialBatchId = l.MaterialBatchId,
                    Quantity = l.Quantity,
                })
                .ToListAsync(ct);
        }

        return dto;
    }

    public async Task<TraceabilityLinkDto> Link(
        Guid productId,
        Guid materialId,
        decimal? quantity,
        CancellationToken ct
    )
    {
        var existing = await _db.TraceabilityLinks.FirstOrDefaultAsync(
            l => l.ProductBatchId == productId && l.MaterialBatchId == materialId,
            ct
        );
        if (existing is not null)
        {
            existing.Quantity = quantity;
            await _db.SaveChangesAsync(ct);
            return new TraceabilityLinkDto
            {
                Id = existing.Id,
                ProductBatchId = productId,
                MaterialBatchId = materialId,
                Quantity = quantity,
            };
        }
        var link = new TraceabilityLink
        {
            ProductBatchId = productId,
            MaterialBatchId = materialId,
            Quantity = quantity,
        };
        _db.TraceabilityLinks.Add(link);
        await _db.SaveChangesAsync(ct);
        return new TraceabilityLinkDto
        {
            Id = link.Id,
            ProductBatchId = productId,
            MaterialBatchId = materialId,
            Quantity = quantity,
        };
    }

    public async Task<DashboardDto> Dashboard(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var soon = today.AddDays(30);
        return new DashboardDto
        {
            BatchesAwaiting =
                await _db.MaterialBatches.CountAsync(
                    b => b.Status == BatchStatus.AwaitingControl,
                    ct
                )
                + await _db.ProductBatches.CountAsync(
                    b => b.Status == BatchStatus.AwaitingControl,
                    ct
                ),
            BatchesInControl =
                await _db.MaterialBatches.CountAsync(b => b.Status == BatchStatus.InControl, ct)
                + await _db.ProductBatches.CountAsync(b => b.Status == BatchStatus.InControl, ct),
            BatchesApproved = await _db.MaterialBatches.CountAsync(
                b => b.Status == BatchStatus.Approved,
                ct
            ),
            BatchesQuarantine =
                await _db.MaterialBatches.CountAsync(b => b.Status == BatchStatus.Quarantine, ct)
                + await _db.ProductBatches.CountAsync(b => b.Status == BatchStatus.Quarantine, ct),
            OpenNonconformances = await _db.Nonconformances.CountAsync(
                n => n.Status != NonconformanceStatus.Closed,
                ct
            ),
            VerificationDueSoon = await _db.Equipment.CountAsync(
                e =>
                    e.VerificationValidUntil != null
                    && e.VerificationValidUntil >= today
                    && e.VerificationValidUntil <= soon,
                ct
            ),
            VerificationExpired = await _db.Equipment.CountAsync(
                e =>
                    e.Status == EquipmentStatus.VerificationExpired
                    || (e.VerificationValidUntil != null && e.VerificationValidUntil < today),
                ct
            ),
            AssignedToMe = _user.EmployeeId is { } me
                ? await _db.TestRuns.CountAsync(
                    r =>
                        r.AssigneeId == me
                        && r.Status != TestRunStatus.Completed
                        && r.Status != TestRunStatus.Cancelled,
                    ct
                )
                : 0,
        };
    }

    public async Task<List<IncomingJournalRow>> IncomingJournal(CancellationToken ct)
    {
        var batches = await _db
            .MaterialBatches.Include(b => b.Nomenclature)
            .Include(b => b.Supplier)
            .Include(b => b.ControlOrders)
                .ThenInclude(o => o.Protocols)
                    .ThenInclude(p => p.Signatures)
                        .ThenInclude(s => s.Employee)
            .OrderByDescending(b => b.ArrivalDate)
            .Take(200)
            .ToListAsync(ct);
        return batches
            .Select(b =>
            {
                var protocol = b
                    .ControlOrders.SelectMany(o => o.Protocols)
                    .OrderByDescending(p => p.IssuedAt)
                    .FirstOrDefault();
                return new IncomingJournalRow
                {
                    ArrivalDate = b.ArrivalDate,
                    BatchNumber = b.Number,
                    Material = b.Nomenclature?.Name ?? string.Empty,
                    Supplier = b.Supplier?.Name ?? string.Empty,
                    Status = b.Status,
                    ProtocolNumber = protocol?.Number,
                    Verdict = protocol?.OverallVerdict ?? Verdict.Pending,
                    Performer = protocol
                        ?.Signatures.FirstOrDefault(s => s.Role == SignatureRole.Performer)
                        ?.Employee?.FullName,
                };
            })
            .ToList();
    }

    private void ApplyDecision(Nonconformance nc)
    {
        var batch = nc.MaterialBatch;
        if (batch is null)
            return;
        batch.Decision = nc.Decision;
        batch.DecisionComment = nc.DecisionComment;
        batch.Status = nc.Decision switch
        {
            BatchDecision.ReturnToSupplier => BatchStatus.ReturnedToSupplier,
            BatchDecision.WriteOff => BatchStatus.WrittenOff,
            BatchDecision.RestrictedUse => BatchStatus.RestrictedUse,
            BatchDecision.Sorting => BatchStatus.Quarantine,
            _ => batch.Status,
        };
    }

    private static ProtocolDto Map(Protocol p, IReadOnlyList<TestRunDto> runs) =>
        new()
        {
            Id = p.Id,
            Number = p.Number,
            Kind = p.Kind,
            KindName = p.Kind.ToString(),
            ControlOrderId = p.ControlOrderId,
            IssuedAt = p.IssuedAt,
            Status = p.Status,
            OverallVerdict = p.OverallVerdict,
            Conclusion = p.Conclusion,
            Signatures = p
                .Signatures.Select(s => new ProtocolSignatureDto
                {
                    Role = s.Role,
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee?.FullName,
                    SignedAt = s.SignedAt,
                })
                .ToList(),
            Results = runs.ToList(),
        };

    private static NonconformanceDto MapNc(Nonconformance n) =>
        new()
        {
            Id = n.Id,
            Number = n.Number,
            Stage = n.Stage,
            ControlOrderId = n.ControlOrderId,
            MaterialBatchId = n.MaterialBatchId,
            MaterialBatchNumber = n.MaterialBatch?.Number,
            ProductBatchId = n.ProductBatchId,
            ProductBatchNumber = n.ProductBatch?.Number,
            DetectedAt = n.DetectedAt,
            ParameterName = n.ParameterName,
            ActualValue = n.ActualValue,
            NormValue = n.NormValue,
            CauseId = n.CauseId,
            CauseName = n.Cause?.Name,
            CauseComment = n.CauseComment,
            Status = n.Status,
            Decision = n.Decision,
            DecisionComment = n.DecisionComment,
            Actions = n
                .Actions.Select(a => new CorrectiveActionDto
                {
                    Id = a.Id,
                    Description = a.Description,
                    AssignedToId = a.AssignedToId,
                    AssignedToName = a.AssignedTo?.FullName,
                    DueDate = a.DueDate,
                    Status = a.Status,
                    Result = a.Result,
                })
                .ToList(),
        };
}
