using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;
using Modul555.Lims.Domain.Reference;

namespace Modul555.Lims.Domain.Operations;

public class ControlOrder : Entity
{
    public string Number { get; set; } = string.Empty;

    public ControlKind ControlKind { get; set; }
    public ControlOrderSource Source { get; set; }
    public ControlOrderStatus Status { get; set; } = ControlOrderStatus.Draft;

    public Guid? MaterialBatchId { get; set; }
    public MaterialBatch? MaterialBatch { get; set; }

    public Guid? ProductBatchId { get; set; }
    public ProductBatch? ProductBatch { get; set; }

    public Guid? ProductUnitId { get; set; }
    public ProductUnit? ProductUnit { get; set; }

    public Guid ControlProgramId { get; set; }
    public ControlProgram? ControlProgram { get; set; }

    public DateTimeOffset PlannedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public Guid? ResponsibleEmployeeId { get; set; }
    public Employee? ResponsibleEmployee { get; set; }

    public string? Comment { get; set; }

    public ICollection<Sample> Samples { get; set; } = [];
    public ICollection<TestRun> TestRuns { get; set; } = [];
    public ICollection<Protocol> Protocols { get; set; } = [];
}
