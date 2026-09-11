using Modul555.Lims.Application.Common;
using Modul555.Lims.Domain.Enums;

namespace Modul555.Lims.Application.Catalogs;

public sealed class SupplierDto : ReferenceDto
{
    public string? Inn { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}

public sealed class WarehouseDto : ReferenceDto
{
    public bool IsQuarantineZone { get; set; }
}

public sealed class StandardDocumentDto : ReferenceDto
{
    public StandardKind Kind { get; set; }
    public int? Year { get; set; }
    public bool IsCurrent { get; set; }
}

public sealed class NomenclatureCategoryDto : ReferenceDto
{
    public bool IsRawMaterial { get; set; }
}

public sealed class NomenclatureDto : ReferenceDto
{
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? Grade { get; set; }
    public Guid? RequirementStandardId { get; set; }
    public string? RequirementStandardName { get; set; }
    public bool TrackBatches { get; set; }
    public int? ShelfLifeDays { get; set; }
    public decimal? DesignValue { get; set; }
}

public sealed class QualityParameterDto : ReferenceDto
{
    public ValueKind ValueKind { get; set; }
    public string? UnitOfMeasure { get; set; }
    public int Precision { get; set; }
    public string? AllowedValues { get; set; }
}

public sealed class MeasuredQuantityDto : ReferenceDto
{
    public string? UnitOfMeasure { get; set; }
    public string VariableName { get; set; } = string.Empty;
    public int Precision { get; set; }
}

public sealed class DefectDto : ReferenceDto
{
    public bool IsCritical { get; set; }
    public string? AppliesTo { get; set; }
}

public sealed class NonconformanceCauseDto : ReferenceDto
{
    public string? Group { get; set; }
}

public sealed class EquipmentTypeDto : ReferenceDto;

public sealed class TestMethodDto : ReferenceDto
{
    public Guid? StandardDocumentId { get; set; }
    public string? StandardDocumentName { get; set; }
    public string? Formula { get; set; }
    public bool IsDestructive { get; set; }
    public int LaborMinutes { get; set; }
    public int DefaultReplicates { get; set; }
    public bool RequiresCompetency { get; set; }
    public List<Guid> QuantityIds { get; set; } = [];
    public List<Guid> EquipmentTypeIds { get; set; } = [];
}

public class ControlProgramListDto : ReferenceDto
{
    public Guid NomenclatureId { get; set; }
    public string? NomenclatureName { get; set; }
    public ControlKind ControlKind { get; set; }
    public Guid? ProductionLineId { get; set; }
    public string? ProductionLineName { get; set; }
    public bool IsApproved { get; set; }
    public int ParameterCount { get; set; }
}

public sealed class ControlProgramParameterDto
{
    public Guid Id { get; set; }
    public Guid QualityParameterId { get; set; }
    public string? QualityParameterName { get; set; }
    public string? UnitOfMeasure { get; set; }
    public Guid? TestMethodId { get; set; }
    public string? TestMethodName { get; set; }
    public NormKind NormKind { get; set; }
    public decimal? NormMin { get; set; }
    public decimal? NormMax { get; set; }
    public decimal? Tolerance { get; set; }
    public decimal? PercentOfDesign { get; set; }
    public string? NormText { get; set; }
    public string? NormFormatted { get; set; }
    public ControlFrequency Frequency { get; set; }
    public int Replicates { get; set; }
    public AggregationKind Aggregation { get; set; }
    public bool IsMandatory { get; set; }
    public int SortOrder { get; set; }
}

public sealed class ControlProgramDto : ControlProgramListDto
{
    public Guid? StandardDocumentId { get; set; }
    public string? StandardDocumentName { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public List<ControlProgramParameterDto> Parameters { get; set; } = [];
}

public sealed class ControlProgramWriteDto : ReferenceWriteDto
{
    public Guid NomenclatureId { get; set; }
    public ControlKind ControlKind { get; set; }
    public Guid? ProductionLineId { get; set; }
    public Guid? StandardDocumentId { get; set; }
    public bool IsApproved { get; set; }
    public List<ControlProgramParameterDto> Parameters { get; set; } = [];
}

public sealed class EquipmentDto : ReferenceDto
{
    public Guid EquipmentTypeId { get; set; }
    public string? EquipmentTypeName { get; set; }
    public string? InventoryNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public int? YearOfManufacture { get; set; }
    public EquipmentStatus Status { get; set; }
    public DateOnly? VerificationValidUntil { get; set; }
    public int VerificationIntervalMonths { get; set; }
    public Guid? ResponsibleEmployeeId { get; set; }
    public int DaysToVerification { get; set; }
}

public sealed class EquipmentEventDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public EquipmentEventKind Kind { get; set; }
    public DateOnly EventDate { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public string? CertificateNumber { get; set; }
    public string? Organization { get; set; }
    public string? Comment { get; set; }
}

public sealed class EmployeeDto : ReferenceDto
{
    public string UserName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public Guid? SubdivisionId { get; set; }
    public string? SubdivisionName { get; set; }
    public string? WorkSchedule { get; set; }
}

public sealed class CompetencyDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid TestMethodId { get; set; }
    public string? TestMethodName { get; set; }
    public DateOnly IssuedOn { get; set; }
    public DateOnly ValidUntil { get; set; }
    public string? CertificateNumber { get; set; }
    public bool IsValid { get; set; }
}

public sealed class DocumentTemplateDto : ReferenceDto
{
    public DocumentKind Kind { get; set; }
    public ControlStage Stage { get; set; }
    public string? ProductionLineCode { get; set; }
    public string PerformerRole { get; set; } = string.Empty;
    public string? ApproverRole { get; set; }
    public bool RequiresQualityInspector { get; set; }
    public SignatureKind SignatureKind { get; set; }
    public bool CreatedOnNonconformance { get; set; }
}

public sealed class SubdivisionDto : ReferenceDto
{
    public Guid? ProductionLineId { get; set; }
    public string? ProductionLineName { get; set; }
}

public sealed class UserAccountDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
}

public sealed class UserAccountWriteDto
{
    public Guid EmployeeId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
}

public sealed class CompetencyWriteDto
{
    public Guid EmployeeId { get; set; }
    public Guid TestMethodId { get; set; }
    public DateOnly IssuedOn { get; set; }
    public DateOnly ValidUntil { get; set; }
    public string? CertificateNumber { get; set; }
}

public sealed class AuditLogDto
{
    public long Id { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Changes { get; set; }
}

public sealed class BatchAdminDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string? NomenclatureName { get; set; }
    public BatchStatus Status { get; set; }
    public BatchDecision Decision { get; set; }
    public string? DecisionComment { get; set; }
}

public sealed class BatchAdminWriteDto
{
    public BatchStatus Status { get; set; }
    public BatchDecision Decision { get; set; }
    public string? DecisionComment { get; set; }
    public Guid? WarehouseId { get; set; }
}
