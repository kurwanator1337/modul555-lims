using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Application.Catalogs;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Domain.Common;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

public sealed class ProgramService
{
    private readonly LimsDbContext _db;

    public ProgramService(LimsDbContext db) => _db = db;

    public async Task<PagedResult<ControlProgramListDto>> ListAsync(
        PagedQuery q,
        CancellationToken ct
    )
    {
        var query = _db
            .ControlPrograms.Include(p => p.Nomenclature)
            .Include(p => p.ProductionLine)
            .Include(p => p.Parameters)
            .AsQueryable();
        if (q.ActiveOnly != false)
            query = query.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            query = query.Where(x =>
                x.Code.Contains(s) || x.Name.Contains(s) || x.Nomenclature!.Name.Contains(s)
            );
        }
        query = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Name);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<ControlProgramListDto>
        {
            Items = items
                .Select(p => new ControlProgramListDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    SortOrder = p.SortOrder,
                    NomenclatureId = p.NomenclatureId,
                    NomenclatureName = p.Nomenclature?.Name,
                    ControlKind = p.ControlKind,
                    ProductionLineId = p.ProductionLineId,
                    ProductionLineName = p.ProductionLine?.Name,
                    IsApproved = p.IsApproved,
                    ParameterCount = p.Parameters.Count,
                })
                .ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<ControlProgramDto?> GetAsync(Guid id, CancellationToken ct)
    {
        var p = await _db
            .ControlPrograms.Include(x => x.Nomenclature)
            .Include(x => x.ProductionLine)
            .Include(x => x.StandardDocument)
            .Include(x => x.Parameters)
                .ThenInclude(pp => pp.QualityParameter)
            .Include(x => x.Parameters)
                .ThenInclude(pp => pp.TestMethod)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return p is null ? null : Map(p);
    }

    public async Task<ControlProgramDto> UpsertAsync(
        Guid? id,
        ControlProgramWriteDto dto,
        CancellationToken ct
    )
    {
        ControlProgram entity;
        if (id is null)
        {
            entity = new ControlProgram { Id = Guid.CreateVersion7() };
            _db.ControlPrograms.Add(entity);
        }
        else
        {
            entity =
                await _db
                    .ControlPrograms.Include(p => p.Parameters)
                    .FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new InvalidOperationException("Программа испытаний не найдена.");
            _db.ControlProgramParameters.RemoveRange(entity.Parameters);
        }

        entity.Code = dto.Code;
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;
        entity.SortOrder = dto.SortOrder;
        entity.NomenclatureId = dto.NomenclatureId;
        entity.ControlKind = dto.ControlKind;
        entity.ProductionLineId = dto.ProductionLineId;
        entity.StandardDocumentId = dto.StandardDocumentId;
        entity.IsApproved = dto.IsApproved;
        if (dto.IsApproved && entity.ApprovedAt is null)
        {
            entity.ApprovedAt = DateTimeOffset.UtcNow;
            entity.ApprovedBy = "user";
        }

        foreach (var row in dto.Parameters.OrderBy(p => p.SortOrder))
        {
            entity.Parameters.Add(
                new ControlProgramParameter
                {
                    QualityParameterId = row.QualityParameterId,
                    TestMethodId = row.TestMethodId,
                    NormKind = row.NormKind,
                    NormMin = row.NormMin,
                    NormMax = row.NormMax,
                    Tolerance = row.Tolerance,
                    PercentOfDesign = row.PercentOfDesign,
                    NormText = row.NormText,
                    Frequency = row.Frequency,
                    Replicates = row.Replicates <= 0 ? 1 : row.Replicates,
                    Aggregation = row.Aggregation,
                    IsMandatory = row.IsMandatory,
                    SortOrder = row.SortOrder,
                }
            );
        }

        await _db.SaveChangesAsync(ct);
        return (await GetAsync(entity.Id, ct))!;
    }

    public async Task<bool> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.ControlPrograms.FindAsync([id], ct);
        if (entity is null)
            return false;
        entity.IsActive = false;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static ControlProgramDto Map(ControlProgram p) =>
        new()
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            IsActive = p.IsActive,
            SortOrder = p.SortOrder,
            NomenclatureId = p.NomenclatureId,
            NomenclatureName = p.Nomenclature?.Name,
            ControlKind = p.ControlKind,
            ProductionLineId = p.ProductionLineId,
            ProductionLineName = p.ProductionLine?.Name,
            IsApproved = p.IsApproved,
            ParameterCount = p.Parameters.Count,
            StandardDocumentId = p.StandardDocumentId,
            StandardDocumentName = p.StandardDocument?.Code,
            ApprovedAt = p.ApprovedAt,
            ApprovedBy = p.ApprovedBy,
            Parameters = p
                .Parameters.OrderBy(x => x.SortOrder)
                .Select(x => new ControlProgramParameterDto
                {
                    Id = x.Id,
                    QualityParameterId = x.QualityParameterId,
                    QualityParameterName = x.QualityParameter?.Name,
                    UnitOfMeasure = x.QualityParameter?.UnitOfMeasure,
                    TestMethodId = x.TestMethodId,
                    TestMethodName = x.TestMethod?.Name,
                    NormKind = x.NormKind,
                    NormMin = x.NormMin,
                    NormMax = x.NormMax,
                    Tolerance = x.Tolerance,
                    PercentOfDesign = x.PercentOfDesign,
                    NormText = x.NormText,
                    NormFormatted = VerdictRules.FormatNorm(
                        x.NormKind,
                        x.NormMin,
                        x.NormMax,
                        x.Tolerance,
                        x.PercentOfDesign,
                        x.QualityParameter?.UnitOfMeasure,
                        x.NormText
                    ),
                    Frequency = x.Frequency,
                    Replicates = x.Replicates,
                    Aggregation = x.Aggregation,
                    IsMandatory = x.IsMandatory,
                    SortOrder = x.SortOrder,
                })
                .ToList(),
        };
}
