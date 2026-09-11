using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;

namespace Modul555.Lims.Domain.Reference;
public class ControlProgram : ReferenceEntity
{
    public Guid NomenclatureId { get; set; }
    public Nomenclature? Nomenclature { get; set; }

    public ControlKind ControlKind { get; set; }

    public Guid? ProductionLineId { get; set; }
    public ProductionLine? ProductionLine { get; set; }
    public Guid? StandardDocumentId { get; set; }
    public StandardDocument? StandardDocument { get; set; }

    public bool IsApproved { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }

    public ICollection<ControlProgramParameter> Parameters { get; set; } = [];
}

public class ControlProgramParameter : Entity
{
    public Guid ControlProgramId { get; set; }
    public ControlProgram? ControlProgram { get; set; }

    public Guid QualityParameterId { get; set; }
    public QualityParameter? QualityParameter { get; set; }

    public Guid? TestMethodId { get; set; }
    public TestMethod? TestMethod { get; set; }

    public NormKind NormKind { get; set; }

    public decimal? NormMin { get; set; }
    public decimal? NormMax { get; set; }

    public decimal? Tolerance { get; set; }
    public decimal? PercentOfDesign { get; set; }

    public string? NormText { get; set; }

    public ControlFrequency Frequency { get; set; } = ControlFrequency.EveryBatch;

    public int Replicates { get; set; } = 1;

    public AggregationKind Aggregation { get; set; } = AggregationKind.Average;

    public bool IsMandatory { get; set; } = true;

    public int SortOrder { get; set; }
}
