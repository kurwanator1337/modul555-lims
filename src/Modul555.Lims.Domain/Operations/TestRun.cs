using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;
using Modul555.Lims.Domain.Reference;

namespace Modul555.Lims.Domain.Operations;

/// <summary>Проведение испытания по одному показателю программы.</summary>
public class TestRun : Entity
{
    public Guid ControlOrderId { get; set; }
    public ControlOrder? ControlOrder { get; set; }

    public Guid? SampleId { get; set; }
    public Sample? Sample { get; set; }

    public Guid ControlProgramParameterId { get; set; }
    public ControlProgramParameter? ControlProgramParameter { get; set; }

    public Guid? AssigneeId { get; set; }
    public Employee? Assignee { get; set; }

    public Guid? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public TestRunStatus Status { get; set; } = TestRunStatus.Assigned;

    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public ICollection<Measurement> Measurements { get; set; } = [];
    public ParameterResult? Result { get; set; }
}

/// <summary>Одно измерение одной величины в одной повторности.</summary>
public class Measurement : Entity
{
    public Guid TestRunId { get; set; }
    public TestRun? TestRun { get; set; }

    public Guid MeasuredQuantityId { get; set; }
    public MeasuredQuantity? MeasuredQuantity { get; set; }

    /// <summary>Номер повторности, начиная с 1.</summary>
    public int Replicate { get; set; } = 1;

    public decimal? NumericValue { get; set; }
    public bool? BooleanValue { get; set; }
    public string? TextValue { get; set; }

    public ValueSource Source { get; set; } = ValueSource.Manual;

    public DateTimeOffset RecordedAt { get; set; }

    public Guid? RecordedById { get; set; }
    public Employee? RecordedBy { get; set; }
}

/// <summary>Рассчитанный показатель с нормативом и вердиктом.</summary>
public class ParameterResult : Entity
{
    public Guid TestRunId { get; set; }
    public TestRun? TestRun { get; set; }

    public decimal? NumericValue { get; set; }
    public bool? BooleanValue { get; set; }
    public string? TextValue { get; set; }

    /// <summary>Норматив, скопированный с программы испытаний на момент расчёта.</summary>
    public string? NormSnapshot { get; set; }

    public Verdict Verdict { get; set; } = Verdict.Pending;

    public DateTimeOffset CalculatedAt { get; set; }
}
