using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modul555.Lims.Application.Catalogs;
using Modul555.Lims.Application.Common;
using Modul555.Lims.Infrastructure.Services;

namespace Modul555.Lims.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public sealed class CatalogsController : ControllerBase
{
    private readonly CatalogService _catalogs;

    public CatalogsController(CatalogService catalogs) => _catalogs = catalogs;

    [HttpGet("production-lines")]
    public Task<PagedResult<ReferenceDto>> Lines([FromQuery] PagedQuery q, CancellationToken ct) =>
        _catalogs.ProductionLines(q, ct);

    [HttpGet("nomenclature-categories")]
    public Task<PagedResult<NomenclatureCategoryDto>> Categories(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Categories(q, ct);

    [HttpGet("nomenclatures")]
    public Task<PagedResult<NomenclatureDto>> Nomenclatures(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Nomenclatures(q, ct);

    [HttpGet("suppliers")]
    public Task<PagedResult<SupplierDto>> Suppliers(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Suppliers(q, ct);

    [HttpGet("warehouses")]
    public Task<PagedResult<WarehouseDto>> Warehouses(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Warehouses(q, ct);

    [HttpGet("standards")]
    public Task<PagedResult<StandardDocumentDto>> Standards(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Standards(q, ct);

    [HttpGet("quality-parameters")]
    public Task<PagedResult<QualityParameterDto>> Parameters(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Parameters(q, ct);

    [HttpGet("measured-quantities")]
    public Task<PagedResult<MeasuredQuantityDto>> Quantities(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Quantities(q, ct);

    [HttpGet("defects")]
    public Task<PagedResult<DefectDto>> Defects([FromQuery] PagedQuery q, CancellationToken ct) =>
        _catalogs.Defects(q, ct);

    [HttpGet("causes")]
    public Task<PagedResult<NonconformanceCauseDto>> Causes(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Causes(q, ct);

    [HttpGet("equipment-types")]
    public Task<PagedResult<EquipmentTypeDto>> EquipmentTypes(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.EquipmentTypes(q, ct);

    [HttpGet("document-templates")]
    public Task<PagedResult<DocumentTemplateDto>> Templates(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Templates(q, ct);

    [HttpGet("subdivisions")]
    public Task<PagedResult<SubdivisionDto>> Subdivisions(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Subdivisions(q, ct);

    [HttpGet("test-methods")]
    public Task<PagedResult<TestMethodDto>> Methods(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Methods(q, ct);

    [HttpGet("equipment")]
    public Task<PagedResult<EquipmentDto>> Equipment(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Equipment(q, ct);

    [HttpGet("employees")]
    public Task<PagedResult<EmployeeDto>> Employees(
        [FromQuery] PagedQuery q,
        CancellationToken ct
    ) => _catalogs.Employees(q, ct);

    [HttpGet("competencies")]
    public Task<List<CompetencyDto>> Competencies(
        [FromQuery] Guid? employeeId,
        CancellationToken ct
    ) => _catalogs.Competencies(employeeId, ct);

    [HttpGet("lookups/{entity}")]
    public async Task<ActionResult<List<LookupItem>>> Lookups(string entity, CancellationToken ct)
    {
        var items = entity switch
        {
            "production-lines" => await _catalogs.Lookup<ProductionLine>(ct),
            "nomenclature-categories" => await _catalogs.Lookup<NomenclatureCategory>(ct),
            "nomenclatures" => await _catalogs.Lookup<Nomenclature>(ct),
            "suppliers" => await _catalogs.Lookup<Supplier>(ct),
            "warehouses" => await _catalogs.Lookup<Warehouse>(ct),
            "standards" => await _catalogs.Lookup<StandardDocument>(ct),
            "quality-parameters" => await _catalogs.Lookup<QualityParameter>(ct),
            "measured-quantities" => await _catalogs.Lookup<MeasuredQuantity>(ct),
            "defects" => await _catalogs.Lookup<Defect>(ct),
            "causes" => await _catalogs.Lookup<NonconformanceCause>(ct),
            "equipment-types" => await _catalogs.Lookup<EquipmentType>(ct),
            "test-methods" => await _catalogs.Lookup<TestMethod>(ct),
            "equipment" => await _catalogs.Lookup<Equipment>(ct),
            "employees" => await _catalogs.Lookup<Employee>(ct),
            "subdivisions" => await _catalogs.Lookup<Subdivision>(ct),
            _ => null,
        };
        return items is null ? NotFound() : Ok(items);
    }

    [HttpPost("production-lines")]
    public async Task<ReferenceDto> CreateLine([FromBody] ReferenceDto dto, CancellationToken ct)
    {
        var e = await _catalogs.CreateAsync(
            new ProductionLine
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("production-lines/{id:guid}")]
    public async Task<IActionResult> UpdateLine(
        Guid id,
        [FromBody] ReferenceDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<ProductionLine>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("subdivisions")]
    public async Task<SubdivisionDto> CreateSubdivision(
        [FromBody] SubdivisionDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new Subdivision
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                ProductionLineId = dto.ProductionLineId,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("subdivisions/{id:guid}")]
    public async Task<IActionResult> UpdateSubdivision(
        Guid id,
        [FromBody] SubdivisionDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<Subdivision>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.ProductionLineId = dto.ProductionLineId;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("warehouses")]
    public async Task<WarehouseDto> CreateWarehouse(
        [FromBody] WarehouseDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new Warehouse
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                IsQuarantineZone = dto.IsQuarantineZone,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("warehouses/{id:guid}")]
    public async Task<IActionResult> UpdateWarehouse(
        Guid id,
        [FromBody] WarehouseDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<Warehouse>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.IsQuarantineZone = dto.IsQuarantineZone;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("nomenclature-categories")]
    public async Task<NomenclatureCategoryDto> CreateCategory(
        [FromBody] NomenclatureCategoryDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new NomenclatureCategory
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                IsRawMaterial = dto.IsRawMaterial,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("nomenclature-categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] NomenclatureCategoryDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<NomenclatureCategory>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.IsRawMaterial = dto.IsRawMaterial;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("suppliers")]
    public async Task<SupplierDto> CreateSupplier([FromBody] SupplierDto dto, CancellationToken ct)
    {
        var e = await _catalogs.CreateAsync(
            new Supplier
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                Inn = dto.Inn,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("suppliers/{id:guid}")]
    public async Task<IActionResult> UpdateSupplier(
        Guid id,
        [FromBody] SupplierDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<Supplier>(
            id,
            s =>
            {
                s.Code = dto.Code;
                s.Name = dto.Name;
                s.Description = dto.Description;
                s.IsActive = dto.IsActive;
                s.Inn = dto.Inn;
                s.ContactPerson = dto.ContactPerson;
                s.Phone = dto.Phone;
                s.Email = dto.Email;
                s.Address = dto.Address;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("nomenclatures")]
    public async Task<NomenclatureDto> CreateNomenclature(
        [FromBody] NomenclatureDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new Nomenclature
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                UnitOfMeasure = dto.UnitOfMeasure,
                Grade = dto.Grade,
                RequirementStandardId = dto.RequirementStandardId,
                TrackBatches = dto.TrackBatches,
                ShelfLifeDays = dto.ShelfLifeDays,
                DesignValue = dto.DesignValue,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("nomenclatures/{id:guid}")]
    public async Task<IActionResult> UpdateNomenclature(
        Guid id,
        [FromBody] NomenclatureDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<Nomenclature>(
            id,
            n =>
            {
                n.Code = dto.Code;
                n.Name = dto.Name;
                n.Description = dto.Description;
                n.IsActive = dto.IsActive;
                n.CategoryId = dto.CategoryId;
                n.UnitOfMeasure = dto.UnitOfMeasure;
                n.Grade = dto.Grade;
                n.RequirementStandardId = dto.RequirementStandardId;
                n.TrackBatches = dto.TrackBatches;
                n.ShelfLifeDays = dto.ShelfLifeDays;
                n.DesignValue = dto.DesignValue;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("standards")]
    public async Task<StandardDocumentDto> CreateStandard(
        [FromBody] StandardDocumentDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new StandardDocument
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                Kind = dto.Kind,
                Year = dto.Year,
                IsCurrent = dto.IsCurrent,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("standards/{id:guid}")]
    public async Task<IActionResult> UpdateStandard(
        Guid id,
        [FromBody] StandardDocumentDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<StandardDocument>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.Kind = dto.Kind;
                x.Year = dto.Year;
                x.IsCurrent = dto.IsCurrent;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("quality-parameters")]
    public async Task<QualityParameterDto> CreateParameter(
        [FromBody] QualityParameterDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new QualityParameter
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                ValueKind = dto.ValueKind,
                UnitOfMeasure = dto.UnitOfMeasure,
                Precision = dto.Precision,
                AllowedValues = dto.AllowedValues,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("quality-parameters/{id:guid}")]
    public async Task<IActionResult> UpdateParameter(
        Guid id,
        [FromBody] QualityParameterDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<QualityParameter>(
            id,
            p =>
            {
                p.Code = dto.Code;
                p.Name = dto.Name;
                p.Description = dto.Description;
                p.IsActive = dto.IsActive;
                p.ValueKind = dto.ValueKind;
                p.UnitOfMeasure = dto.UnitOfMeasure;
                p.Precision = dto.Precision;
                p.AllowedValues = dto.AllowedValues;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("measured-quantities")]
    public async Task<MeasuredQuantityDto> CreateQuantity(
        [FromBody] MeasuredQuantityDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new MeasuredQuantity
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                UnitOfMeasure = dto.UnitOfMeasure,
                VariableName = dto.VariableName,
                Precision = dto.Precision,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("measured-quantities/{id:guid}")]
    public async Task<IActionResult> UpdateQuantity(
        Guid id,
        [FromBody] MeasuredQuantityDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<MeasuredQuantity>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.UnitOfMeasure = dto.UnitOfMeasure;
                x.VariableName = dto.VariableName;
                x.Precision = dto.Precision;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("defects")]
    public async Task<DefectDto> CreateDefect([FromBody] DefectDto dto, CancellationToken ct)
    {
        var e = await _catalogs.CreateAsync(
            new Defect
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                IsCritical = dto.IsCritical,
                AppliesTo = dto.AppliesTo,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("defects/{id:guid}")]
    public async Task<IActionResult> UpdateDefect(
        Guid id,
        [FromBody] DefectDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<Defect>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.IsCritical = dto.IsCritical;
                x.AppliesTo = dto.AppliesTo;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("causes")]
    public async Task<NonconformanceCauseDto> CreateCause(
        [FromBody] NonconformanceCauseDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new NonconformanceCause
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                Group = dto.Group,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("causes/{id:guid}")]
    public async Task<IActionResult> UpdateCause(
        Guid id,
        [FromBody] NonconformanceCauseDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<NonconformanceCause>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.Group = dto.Group;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("equipment-types")]
    public async Task<EquipmentTypeDto> CreateEquipmentType(
        [FromBody] EquipmentTypeDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new EquipmentType
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("equipment-types/{id:guid}")]
    public async Task<IActionResult> UpdateEquipmentType(
        Guid id,
        [FromBody] EquipmentTypeDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<EquipmentType>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("equipment")]
    public async Task<EquipmentDto> CreateEquipment(
        [FromBody] EquipmentDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new Equipment
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                EquipmentTypeId = dto.EquipmentTypeId,
                InventoryNumber = dto.InventoryNumber,
                SerialNumber = dto.SerialNumber,
                Manufacturer = dto.Manufacturer,
                YearOfManufacture = dto.YearOfManufacture,
                Status = dto.Status,
                VerificationValidUntil = dto.VerificationValidUntil,
                VerificationIntervalMonths = dto.VerificationIntervalMonths,
                ResponsibleEmployeeId = dto.ResponsibleEmployeeId,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("equipment/{id:guid}")]
    public async Task<IActionResult> UpdateEquipment(
        Guid id,
        [FromBody] EquipmentDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<Equipment>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.EquipmentTypeId = dto.EquipmentTypeId;
                x.InventoryNumber = dto.InventoryNumber;
                x.SerialNumber = dto.SerialNumber;
                x.Manufacturer = dto.Manufacturer;
                x.YearOfManufacture = dto.YearOfManufacture;
                x.Status = dto.Status;
                x.VerificationValidUntil = dto.VerificationValidUntil;
                x.VerificationIntervalMonths = dto.VerificationIntervalMonths;
                x.ResponsibleEmployeeId = dto.ResponsibleEmployeeId;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("employees")]
    public async Task<EmployeeDto> CreateEmployee([FromBody] EmployeeDto dto, CancellationToken ct)
    {
        var e = await _catalogs.CreateAsync(
            new Employee
            {
                Code = dto.Code,
                Name = $"{dto.LastName} {dto.FirstName}".Trim(),
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                UserName = dto.UserName,
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                Position = dto.Position,
                SubdivisionId = dto.SubdivisionId,
                WorkSchedule = dto.WorkSchedule,
            },
            ct
        );
        dto.Id = e.Id;
        dto.FullName = e.FullName;
        return dto;
    }

    [HttpPut("employees/{id:guid}")]
    public async Task<IActionResult> UpdateEmployee(
        Guid id,
        [FromBody] EmployeeDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<Employee>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = $"{dto.LastName} {dto.FirstName}".Trim();
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.UserName = dto.UserName;
                x.LastName = dto.LastName;
                x.FirstName = dto.FirstName;
                x.MiddleName = dto.MiddleName;
                x.Position = dto.Position;
                x.SubdivisionId = dto.SubdivisionId;
                x.WorkSchedule = dto.WorkSchedule;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("document-templates")]
    public async Task<DocumentTemplateDto> CreateTemplate(
        [FromBody] DocumentTemplateDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.CreateAsync(
            new DocumentTemplate
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                Kind = dto.Kind,
                Stage = dto.Stage,
                ProductionLineCode = dto.ProductionLineCode,
                PerformerRole = dto.PerformerRole,
                ApproverRole = dto.ApproverRole,
                RequiresQualityInspector = dto.RequiresQualityInspector,
                SignatureKind = dto.SignatureKind,
                CreatedOnNonconformance = dto.CreatedOnNonconformance,
            },
            ct
        );
        dto.Id = e.Id;
        return dto;
    }

    [HttpPut("document-templates/{id:guid}")]
    public async Task<IActionResult> UpdateTemplate(
        Guid id,
        [FromBody] DocumentTemplateDto dto,
        CancellationToken ct
    )
    {
        var e = await _catalogs.UpdateAsync<DocumentTemplate>(
            id,
            x =>
            {
                x.Code = dto.Code;
                x.Name = dto.Name;
                x.Description = dto.Description;
                x.IsActive = dto.IsActive;
                x.SortOrder = dto.SortOrder;
                x.Kind = dto.Kind;
                x.Stage = dto.Stage;
                x.ProductionLineCode = dto.ProductionLineCode;
                x.PerformerRole = dto.PerformerRole;
                x.ApproverRole = dto.ApproverRole;
                x.RequiresQualityInspector = dto.RequiresQualityInspector;
                x.SignatureKind = dto.SignatureKind;
                x.CreatedOnNonconformance = dto.CreatedOnNonconformance;
            },
            ct
        );
        return e is null ? NotFound() : Ok(dto);
    }

    [HttpPost("test-methods")]
    public Task<TestMethodDto> CreateMethod([FromBody] TestMethodDto dto, CancellationToken ct) =>
        _catalogs.SaveMethodAsync(null, dto, ct);

    [HttpPut("test-methods/{id:guid}")]
    public Task<TestMethodDto> UpdateMethod(
        Guid id,
        [FromBody] TestMethodDto dto,
        CancellationToken ct
    ) => _catalogs.SaveMethodAsync(id, dto, ct);

    [HttpDelete("{entity}/{id:guid}")]
    public async Task<IActionResult> Deactivate(string entity, Guid id, CancellationToken ct)
    {
        var ok = entity switch
        {
            "suppliers" => await _catalogs.DeactivateAsync<Supplier>(id, ct),
            "nomenclatures" => await _catalogs.DeactivateAsync<Nomenclature>(id, ct),
            "quality-parameters" => await _catalogs.DeactivateAsync<QualityParameter>(id, ct),
            "warehouses" => await _catalogs.DeactivateAsync<Warehouse>(id, ct),
            "standards" => await _catalogs.DeactivateAsync<StandardDocument>(id, ct),
            "defects" => await _catalogs.DeactivateAsync<Defect>(id, ct),
            "causes" => await _catalogs.DeactivateAsync<NonconformanceCause>(id, ct),
            "equipment" => await _catalogs.DeactivateAsync<Equipment>(id, ct),
            "employees" => await _catalogs.DeactivateAsync<Employee>(id, ct),
            "test-methods" => await _catalogs.DeactivateAsync<TestMethod>(id, ct),
            "production-lines" => await _catalogs.DeactivateAsync<ProductionLine>(id, ct),
            "subdivisions" => await _catalogs.DeactivateAsync<Subdivision>(id, ct),
            "nomenclature-categories" => await _catalogs.DeactivateAsync<NomenclatureCategory>(
                id,
                ct
            ),
            "measured-quantities" => await _catalogs.DeactivateAsync<MeasuredQuantity>(id, ct),
            "equipment-types" => await _catalogs.DeactivateAsync<EquipmentType>(id, ct),
            "document-templates" => await _catalogs.DeactivateAsync<DocumentTemplate>(id, ct),
            _ => false,
        };
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{entity}/{id:guid}/activate")]
    public async Task<IActionResult> Activate(string entity, Guid id, CancellationToken ct)
    {
        var ok = entity switch
        {
            "suppliers" => await _catalogs.ActivateAsync<Supplier>(id, ct),
            "nomenclatures" => await _catalogs.ActivateAsync<Nomenclature>(id, ct),
            "quality-parameters" => await _catalogs.ActivateAsync<QualityParameter>(id, ct),
            "warehouses" => await _catalogs.ActivateAsync<Warehouse>(id, ct),
            "standards" => await _catalogs.ActivateAsync<StandardDocument>(id, ct),
            "defects" => await _catalogs.ActivateAsync<Defect>(id, ct),
            "causes" => await _catalogs.ActivateAsync<NonconformanceCause>(id, ct),
            "equipment" => await _catalogs.ActivateAsync<Equipment>(id, ct),
            "employees" => await _catalogs.ActivateAsync<Employee>(id, ct),
            "test-methods" => await _catalogs.ActivateAsync<TestMethod>(id, ct),
            "production-lines" => await _catalogs.ActivateAsync<ProductionLine>(id, ct),
            "subdivisions" => await _catalogs.ActivateAsync<Subdivision>(id, ct),
            "nomenclature-categories" => await _catalogs.ActivateAsync<NomenclatureCategory>(
                id,
                ct
            ),
            "measured-quantities" => await _catalogs.ActivateAsync<MeasuredQuantity>(id, ct),
            "equipment-types" => await _catalogs.ActivateAsync<EquipmentType>(id, ct),
            "document-templates" => await _catalogs.ActivateAsync<DocumentTemplate>(id, ct),
            _ => false,
        };
        return ok ? NoContent() : NotFound();
    }
}
