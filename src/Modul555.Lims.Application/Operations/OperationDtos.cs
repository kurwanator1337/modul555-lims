using Modul555.Lims.Domain.Enums;

namespace Modul555.Lims.Application.Operations;

public sealed class MaterialBatchDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Guid NomenclatureId { get; set; }
    public string? NomenclatureName { get; set; }
    public Guid SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public DateOnly ArrivalDate { get; set; }
    public decimal Quantity { get; set; }
    public string? QuantityUnit { get; set; }
    public string? SupplierDocumentNumber { get; set; }
    public DateOnly? SupplierDocumentDate { get; set; }
    public DateOnly? ManufacturedOn { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public BatchStatus Status { get; set; }
    public BatchDecision Decision { get; set; }
    public string? DecisionComment { get; set; }
}

public sealed class ProductBatchDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public ProductionBatchKind Kind { get; set; }
    public Guid NomenclatureId { get; set; }
    public string? NomenclatureName { get; set; }
    public Guid? ProductionLineId { get; set; }
    public string? ProductionLineName { get; set; }
    public DateOnly ProductionDate { get; set; }
    public decimal? DesignValue { get; set; }
    public BatchStatus Status { get; set; }
}

public sealed class ControlOrderDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public ControlKind ControlKind { get; set; }
    public ControlOrderSource Source { get; set; }
    public ControlOrderStatus Status { get; set; }
    public Guid ControlProgramId { get; set; }
    public string? ControlProgramName { get; set; }
    public Guid? MaterialBatchId { get; set; }
    public string? MaterialBatchNumber { get; set; }
    public Guid? ProductBatchId { get; set; }
    public string? ProductBatchNumber { get; set; }
    public DateTimeOffset PlannedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public Guid? ResponsibleEmployeeId { get; set; }
    public string? ResponsibleName { get; set; }
    public string? Comment { get; set; }
    public int SampleCount { get; set; }
    public int TestRunCount { get; set; }
    public int CompletedTestCount { get; set; }
}

public sealed class CreateIncomingOrderRequest
{
    public Guid MaterialBatchId { get; set; }
    public Guid? ControlProgramId { get; set; }
    public Guid? ResponsibleEmployeeId { get; set; }
    public string? Comment { get; set; }
}

public sealed class CreateOperationalOrderRequest
{
    public Guid ProductBatchId { get; set; }
    public Guid ControlProgramId { get; set; }
    public Guid? ResponsibleEmployeeId { get; set; }
    public string? Comment { get; set; }
}

public sealed class SampleDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public Guid ControlOrderId { get; set; }
    public DateTimeOffset SampledAt { get; set; }
    public Guid? SampledById { get; set; }
    public string? SampledByName { get; set; }
    public SampleState State { get; set; }
    public decimal? Quantity { get; set; }
    public string? QuantityUnit { get; set; }
}

public sealed class RegisterSampleRequest
{
    public Guid? SampledById { get; set; }
    public Guid? WarehouseId { get; set; }
    public decimal? Quantity { get; set; }
    public string? QuantityUnit { get; set; }
}

public sealed class TestRunDto
{
    public Guid Id { get; set; }
    public Guid ControlOrderId { get; set; }
    public string? ControlOrderNumber { get; set; }
    public Guid? SampleId { get; set; }
    public string? SampleNumber { get; set; }
    public Guid ControlProgramParameterId { get; set; }
    public string ParameterName { get; set; } = string.Empty;
    public string? UnitOfMeasure { get; set; }
    public string? MethodName { get; set; }
    public Guid? MethodId { get; set; }
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public Guid? EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public TestRunStatus Status { get; set; }
    public int Replicates { get; set; }
    public string? NormFormatted { get; set; }
    public Verdict Verdict { get; set; }
    public decimal? ResultValue { get; set; }
    public bool? ResultBoolean { get; set; }
    public string? ResultText { get; set; }
    public List<MeasurementDto> Measurements { get; set; } = [];
    public List<QuantityHintDto> Quantities { get; set; } = [];
}

public sealed class QuantityHintDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string VariableName { get; set; } = string.Empty;
    public string? UnitOfMeasure { get; set; }
}

public sealed class MeasurementDto
{
    public Guid Id { get; set; }
    public Guid MeasuredQuantityId { get; set; }
    public string? QuantityName { get; set; }
    public int Replicate { get; set; }
    public decimal? NumericValue { get; set; }
    public bool? BooleanValue { get; set; }
    public string? TextValue { get; set; }
    public ValueSource Source { get; set; }
}

public sealed class SaveMeasurementsRequest
{
    public Guid? EquipmentId { get; set; }
    public List<MeasurementDto> Measurements { get; set; } = [];
}

public sealed class AssignTestRunRequest
{
    public Guid AssigneeId { get; set; }
    public Guid? EquipmentId { get; set; }
}

public sealed class ProtocolDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DocumentKind Kind { get; set; }
    public string? KindName { get; set; }
    public Guid? ControlOrderId { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public ProtocolStatus Status { get; set; }
    public Verdict OverallVerdict { get; set; }
    public string? Conclusion { get; set; }
    public List<ProtocolSignatureDto> Signatures { get; set; } = [];
    public List<TestRunDto> Results { get; set; } = [];
}

public sealed class ProtocolSignatureDto
{
    public SignatureRole Role { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTimeOffset SignedAt { get; set; }
}

public sealed class SignProtocolRequest
{
    public SignatureRole Role { get; set; }
}

public sealed class NonconformanceDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public ControlStage Stage { get; set; }
    public Guid? ControlOrderId { get; set; }
    public Guid? MaterialBatchId { get; set; }
    public string? MaterialBatchNumber { get; set; }
    public Guid? ProductBatchId { get; set; }
    public string? ProductBatchNumber { get; set; }
    public DateTimeOffset DetectedAt { get; set; }
    public string ParameterName { get; set; } = string.Empty;
    public string? ActualValue { get; set; }
    public string? NormValue { get; set; }
    public Guid? CauseId { get; set; }
    public string? CauseName { get; set; }
    public string? CauseComment { get; set; }
    public NonconformanceStatus Status { get; set; }
    public BatchDecision Decision { get; set; }
    public string? DecisionComment { get; set; }
    public List<CorrectiveActionDto> Actions { get; set; } = [];
}

public sealed class CorrectiveActionDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateOnly? DueDate { get; set; }
    public CorrectiveActionStatus Status { get; set; }
    public string? Result { get; set; }
}

public sealed class DecideNonconformanceRequest
{
    public BatchDecision Decision { get; set; }
    public string? DecisionComment { get; set; }
    public Guid? CauseId { get; set; }
    public string? CauseComment { get; set; }
}

public sealed class CreateCorrectiveActionRequest
{
    public string Description { get; set; } = string.Empty;
    public Guid? AssignedToId { get; set; }
    public DateOnly? DueDate { get; set; }
}

public sealed class TraceabilityLinkDto
{
    public Guid Id { get; set; }
    public Guid ProductBatchId { get; set; }
    public string? ProductBatchNumber { get; set; }
    public string? ProductName { get; set; }
    public Guid MaterialBatchId { get; set; }
    public string? MaterialBatchNumber { get; set; }
    public string? MaterialName { get; set; }
    public decimal? Quantity { get; set; }
}

public sealed class PassportDto
{
    public ProductBatchDto? Product { get; set; }
    public MaterialBatchDto? Material { get; set; }
    public List<TraceabilityLinkDto> Materials { get; set; } = [];
    public List<TraceabilityLinkDto> Products { get; set; } = [];
    public List<ControlOrderDto> Orders { get; set; } = [];
    public List<ProtocolDto> Protocols { get; set; } = [];
}

public sealed class DashboardDto
{
    public int BatchesAwaiting { get; set; }
    public int BatchesInControl { get; set; }
    public int BatchesApproved { get; set; }
    public int BatchesQuarantine { get; set; }
    public int OpenNonconformances { get; set; }
    public int VerificationDueSoon { get; set; }
    public int VerificationExpired { get; set; }
    public int AssignedToMe { get; set; }
}

public sealed class IncomingJournalRow
{
    public DateOnly ArrivalDate { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public BatchStatus Status { get; set; }
    public string? ProtocolNumber { get; set; }
    public Verdict Verdict { get; set; }
    public string? Performer { get; set; }
}
