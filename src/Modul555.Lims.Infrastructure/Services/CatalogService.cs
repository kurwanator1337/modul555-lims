using Microsoft.EntityFrameworkCore;
using Modul555.Lims.Application.Catalogs;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Domain.Common;
using Modul555.Lims.Infrastructure.Persistence;

namespace Modul555.Lims.Infrastructure.Services;

public sealed class CatalogService
{
    private readonly LimsDbContext _db;

    public CatalogService(LimsDbContext db) => _db = db;

    public Task<PagedResult<ReferenceDto>> ProductionLines(PagedQuery q, CancellationToken ct) =>
        Page(_db.ProductionLines, q, x => Map(x), ct);

    public Task<PagedResult<NomenclatureCategoryDto>> Categories(
        PagedQuery q,
        CancellationToken ct
    ) =>
        Page(
            _db.NomenclatureCategories,
            q,
            x => new NomenclatureCategoryDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                SortOrder = x.SortOrder,
                IsRawMaterial = x.IsRawMaterial,
            },
            ct
        );

    public async Task<PagedResult<NomenclatureDto>> Nomenclatures(
        PagedQuery q,
        CancellationToken ct
    )
    {
        var query = Apply(
            _db.Nomenclatures.Include(n => n.Category).Include(n => n.RequirementStandard),
            q
        );
        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(n => new NomenclatureDto
            {
                Id = n.Id,
                Code = n.Code,
                Name = n.Name,
                Description = n.Description,
                IsActive = n.IsActive,
                SortOrder = n.SortOrder,
                CategoryId = n.CategoryId,
                CategoryName = n.Category != null ? n.Category.Name : null,
                UnitOfMeasure = n.UnitOfMeasure,
                Grade = n.Grade,
                RequirementStandardId = n.RequirementStandardId,
                RequirementStandardName =
                    n.RequirementStandard != null ? n.RequirementStandard.Code : null,
                TrackBatches = n.TrackBatches,
                ShelfLifeDays = n.ShelfLifeDays,
                DesignValue = n.DesignValue,
            })
            .ToListAsync(ct);
        return new PagedResult<NomenclatureDto>
        {
            Items = items,
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public Task<PagedResult<SupplierDto>> Suppliers(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.Suppliers,
            q,
            s => new SupplierDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Description = s.Description,
                IsActive = s.IsActive,
                SortOrder = s.SortOrder,
                Inn = s.Inn,
                ContactPerson = s.ContactPerson,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
            },
            ct
        );

    public Task<PagedResult<WarehouseDto>> Warehouses(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.Warehouses,
            q,
            w => new WarehouseDto
            {
                Id = w.Id,
                Code = w.Code,
                Name = w.Name,
                Description = w.Description,
                IsActive = w.IsActive,
                SortOrder = w.SortOrder,
                IsQuarantineZone = w.IsQuarantineZone,
            },
            ct
        );

    public Task<PagedResult<StandardDocumentDto>> Standards(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.StandardDocuments,
            q,
            s => new StandardDocumentDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Description = s.Description,
                IsActive = s.IsActive,
                SortOrder = s.SortOrder,
                Kind = s.Kind,
                Year = s.Year,
                IsCurrent = s.IsCurrent,
            },
            ct
        );

    public Task<PagedResult<QualityParameterDto>> Parameters(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.QualityParameters,
            q,
            p => new QualityParameterDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                IsActive = p.IsActive,
                SortOrder = p.SortOrder,
                ValueKind = p.ValueKind,
                UnitOfMeasure = p.UnitOfMeasure,
                Precision = p.Precision,
                AllowedValues = p.AllowedValues,
            },
            ct
        );

    public Task<PagedResult<MeasuredQuantityDto>> Quantities(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.MeasuredQuantities,
            q,
            p => new MeasuredQuantityDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                IsActive = p.IsActive,
                SortOrder = p.SortOrder,
                UnitOfMeasure = p.UnitOfMeasure,
                VariableName = p.VariableName,
                Precision = p.Precision,
            },
            ct
        );

    public Task<PagedResult<DefectDto>> Defects(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.Defects,
            q,
            d => new DefectDto
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                Description = d.Description,
                IsActive = d.IsActive,
                SortOrder = d.SortOrder,
                IsCritical = d.IsCritical,
                AppliesTo = d.AppliesTo,
            },
            ct
        );

    public Task<PagedResult<NonconformanceCauseDto>> Causes(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.NonconformanceCauses,
            q,
            c => new NonconformanceCauseDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                SortOrder = c.SortOrder,
                Group = c.Group,
            },
            ct
        );

    public Task<PagedResult<EquipmentTypeDto>> EquipmentTypes(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.EquipmentTypes,
            q,
            t => new EquipmentTypeDto
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name,
                Description = t.Description,
                IsActive = t.IsActive,
                SortOrder = t.SortOrder,
            },
            ct
        );

    public Task<PagedResult<DocumentTemplateDto>> Templates(PagedQuery q, CancellationToken ct) =>
        Page(
            _db.DocumentTemplates,
            q,
            t => new DocumentTemplateDto
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name,
                Description = t.Description,
                IsActive = t.IsActive,
                SortOrder = t.SortOrder,
                Kind = t.Kind,
                Stage = t.Stage,
                ProductionLineCode = t.ProductionLineCode,
                PerformerRole = t.PerformerRole,
                ApproverRole = t.ApproverRole,
                RequiresQualityInspector = t.RequiresQualityInspector,
                SignatureKind = t.SignatureKind,
                CreatedOnNonconformance = t.CreatedOnNonconformance,
            },
            ct
        );

    public async Task<PagedResult<SubdivisionDto>> Subdivisions(PagedQuery q, CancellationToken ct)
    {
        var query = Apply(_db.Subdivisions.Include(s => s.ProductionLine), q);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<SubdivisionDto>
        {
            Items = items
                .Select(s => new SubdivisionDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    SortOrder = s.SortOrder,
                    ProductionLineId = s.ProductionLineId,
                    ProductionLineName = s.ProductionLine?.Name,
                })
                .ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<PagedResult<TestMethodDto>> Methods(PagedQuery q, CancellationToken ct)
    {
        var query = Apply(
            _db.TestMethods.Include(m => m.StandardDocument)
                .Include(m => m.Quantities)
                .Include(m => m.EquipmentTypes),
            q
        );
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<TestMethodDto>
        {
            Items = items
                .Select(m => new TestMethodDto
                {
                    Id = m.Id,
                    Code = m.Code,
                    Name = m.Name,
                    Description = m.Description,
                    IsActive = m.IsActive,
                    SortOrder = m.SortOrder,
                    StandardDocumentId = m.StandardDocumentId,
                    StandardDocumentName = m.StandardDocument?.Code,
                    Formula = m.Formula,
                    IsDestructive = m.IsDestructive,
                    LaborMinutes = m.LaborMinutes,
                    DefaultReplicates = m.DefaultReplicates,
                    RequiresCompetency = m.RequiresCompetency,
                    QuantityIds = m.Quantities.Select(x => x.MeasuredQuantityId).ToList(),
                    EquipmentTypeIds = m.EquipmentTypes.Select(x => x.EquipmentTypeId).ToList(),
                })
                .ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<PagedResult<EquipmentDto>> Equipment(PagedQuery q, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = Apply(_db.Equipment.Include(e => e.EquipmentType), q);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<EquipmentDto>
        {
            Items = items
                .Select(e => new EquipmentDto
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.Name,
                    Description = e.Description,
                    IsActive = e.IsActive,
                    SortOrder = e.SortOrder,
                    EquipmentTypeId = e.EquipmentTypeId,
                    EquipmentTypeName = e.EquipmentType?.Name,
                    InventoryNumber = e.InventoryNumber,
                    SerialNumber = e.SerialNumber,
                    Manufacturer = e.Manufacturer,
                    YearOfManufacture = e.YearOfManufacture,
                    Status = e.Status,
                    VerificationValidUntil = e.VerificationValidUntil,
                    VerificationIntervalMonths = e.VerificationIntervalMonths,
                    ResponsibleEmployeeId = e.ResponsibleEmployeeId,
                    DaysToVerification = e.VerificationValidUntil.HasValue
                        ? e.VerificationValidUntil.Value.DayNumber - today.DayNumber
                        : 0,
                })
                .ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<PagedResult<EmployeeDto>> Employees(PagedQuery q, CancellationToken ct)
    {
        var query = Apply(_db.Employees.Include(e => e.Subdivision), q);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<EmployeeDto>
        {
            Items = items
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.Name,
                    Description = e.Description,
                    IsActive = e.IsActive,
                    SortOrder = e.SortOrder,
                    UserName = e.UserName,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    MiddleName = e.MiddleName,
                    FullName = e.FullName,
                    Position = e.Position,
                    SubdivisionId = e.SubdivisionId,
                    SubdivisionName = e.Subdivision?.Name,
                    WorkSchedule = e.WorkSchedule,
                })
                .ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }

    public async Task<List<CompetencyDto>> Competencies(Guid? employeeId, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = _db
            .Competencies.Include(c => c.Employee)
            .Include(c => c.TestMethod)
            .AsQueryable();
        if (employeeId.HasValue)
            query = query.Where(c => c.EmployeeId == employeeId.Value);
        var items = await query
            .OrderBy(c => c.Employee!.LastName)
            .ThenBy(c => c.TestMethod!.Name)
            .ToListAsync(ct);
        return items
            .Select(c => new CompetencyDto
            {
                Id = c.Id,
                EmployeeId = c.EmployeeId,
                EmployeeName = c.Employee?.FullName,
                TestMethodId = c.TestMethodId,
                TestMethodName = c.TestMethod?.Name,
                IssuedOn = c.IssuedOn,
                ValidUntil = c.ValidUntil,
                CertificateNumber = c.CertificateNumber,
                IsValid = c.IsValidOn(today),
            })
            .ToList();
    }

    public async Task<T> CreateAsync<T>(T entity, CancellationToken ct)
        where T : ReferenceEntity
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<T?> UpdateAsync<T>(Guid id, Action<T> apply, CancellationToken ct)
        where T : class
    {
        var entity = await _db.Set<T>().FindAsync([id], ct);
        if (entity is null)
            return null;
        apply(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> DeactivateAsync<T>(Guid id, CancellationToken ct)
        where T : ReferenceEntity
    {
        var entity = await _db.Set<T>().FindAsync([id], ct);
        if (entity is null)
            return false;
        entity.IsActive = false;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ActivateAsync<T>(Guid id, CancellationToken ct)
        where T : ReferenceEntity
    {
        var entity = await _db.Set<T>().FindAsync([id], ct);
        if (entity is null)
            return false;
        entity.IsActive = true;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<TestMethodDto> SaveMethodAsync(
        Guid? id,
        TestMethodDto dto,
        CancellationToken ct
    )
    {
        TestMethod method;
        if (id is null)
        {
            method = new TestMethod();
            _db.TestMethods.Add(method);
        }
        else
        {
            method =
                await _db
                    .TestMethods.Include(m => m.Quantities)
                    .Include(m => m.EquipmentTypes)
                    .FirstOrDefaultAsync(m => m.Id == id.Value, ct)
                ?? throw new InvalidOperationException("Метод не найден");
        }

        method.Code = dto.Code;
        method.Name = dto.Name;
        method.Description = dto.Description;
        method.IsActive = dto.IsActive;
        method.SortOrder = dto.SortOrder;
        method.StandardDocumentId = dto.StandardDocumentId;
        method.Formula = dto.Formula;
        method.IsDestructive = dto.IsDestructive;
        method.LaborMinutes = dto.LaborMinutes;
        method.DefaultReplicates = dto.DefaultReplicates;
        method.RequiresCompetency = dto.RequiresCompetency;

        _db.TestMethodQuantities.RemoveRange(method.Quantities);
        method.Quantities = dto
            .QuantityIds.Select(
                (qid, i) => new TestMethodQuantity { MeasuredQuantityId = qid, SortOrder = i }
            )
            .ToList();

        _db.TestMethodEquipmentTypes.RemoveRange(method.EquipmentTypes);
        method.EquipmentTypes = dto
            .EquipmentTypeIds.Select(eid => new TestMethodEquipmentType
            {
                EquipmentTypeId = eid,
                IsRequired = true,
            })
            .ToList();

        await _db.SaveChangesAsync(ct);
        dto.Id = method.Id;
        return dto;
    }

    public Task<List<LookupItem>> Lookup<T>(CancellationToken ct)
        where T : ReferenceEntity =>
        _db.Set<T>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new LookupItem
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            })
            .ToListAsync(ct);

    private static ReferenceDto Map(ReferenceEntity x) =>
        new()
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            SortOrder = x.SortOrder,
        };

    private static IQueryable<T> Apply<T>(IQueryable<T> source, PagedQuery q)
        where T : ReferenceEntity
    {
        if (q.ActiveOnly != false)
            source = source.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            source = source.Where(x => x.Code.Contains(s) || x.Name.Contains(s));
        }
        return source.OrderBy(x => x.SortOrder).ThenBy(x => x.Name);
    }

    private static async Task<PagedResult<TDto>> Page<T, TDto>(
        IQueryable<T> source,
        PagedQuery q,
        Func<T, TDto> map,
        CancellationToken ct
    )
        where T : ReferenceEntity
    {
        var query = Apply(source, q);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
        return new PagedResult<TDto>
        {
            Items = items.Select(map).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize,
        };
    }
}
