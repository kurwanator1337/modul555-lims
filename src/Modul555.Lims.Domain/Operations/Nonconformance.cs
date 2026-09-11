using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;
using Modul555.Lims.Domain.Reference;

namespace Modul555.Lims.Domain.Operations;

/// <summary>Акт о несоответствии. Создаётся автоматически при вердикте «не соответствует».</summary>
public class Nonconformance : Entity
{
    public string Number { get; set; } = string.Empty;

    public ControlStage Stage { get; set; }

    public Guid? ControlOrderId { get; set; }
    public ControlOrder? ControlOrder { get; set; }

    public Guid? ProtocolId { get; set; }
    public Protocol? Protocol { get; set; }

    public Guid? MaterialBatchId { get; set; }
    public MaterialBatch? MaterialBatch { get; set; }

    public Guid? ProductBatchId { get; set; }
    public ProductBatch? ProductBatch { get; set; }

    public DateTimeOffset DetectedAt { get; set; }

    public Guid? DetectedById { get; set; }
    public Employee? DetectedBy { get; set; }

    /// <summary>Показатель, который не соответствует, и его фактическое значение.</summary>
    public string ParameterName { get; set; } = string.Empty;
    public string? ActualValue { get; set; }
    public string? NormValue { get; set; }

    public Guid? CauseId { get; set; }
    public NonconformanceCause? Cause { get; set; }

    public string? CauseComment { get; set; }

    public NonconformanceStatus Status { get; set; } = NonconformanceStatus.Open;

    public BatchDecision Decision { get; set; } = BatchDecision.None;
    public string? DecisionComment { get; set; }

    public ICollection<CorrectiveAction> Actions { get; set; } = [];
}

/// <summary>Карта корректирующих действий: продление пропарки, выдержка, доработка, списание.</summary>
public class CorrectiveAction : Entity
{
    public Guid NonconformanceId { get; set; }
    public Nonconformance? Nonconformance { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid? AssignedToId { get; set; }
    public Employee? AssignedTo { get; set; }

    public DateOnly? DueDate { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public CorrectiveActionStatus Status { get; set; } = CorrectiveActionStatus.Planned;

    public string? Result { get; set; }
}
