using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;
using Modul555.Lims.Domain.Reference;

namespace Modul555.Lims.Domain.Operations;

/// <summary>
/// Производственная партия: замес смеси, партия изделий, партия каркасов или модуль СТМ/ИМ.
/// </summary>
public class ProductBatch : Entity
{
    public string Number { get; set; } = string.Empty;

    public ProductionBatchKind Kind { get; set; }

    public Guid NomenclatureId { get; set; }
    public Nomenclature? Nomenclature { get; set; }

    public Guid? ProductionLineId { get; set; }
    public ProductionLine? ProductionLine { get; set; }

    public DateOnly ProductionDate { get; set; }

    /// <summary>Проектный класс / марка: B25, D600, проектная прочность, МПа.</summary>
    public decimal? DesignValue { get; set; }

    public BatchStatus Status { get; set; } = BatchStatus.AwaitingControl;

    public ICollection<ProductUnit> Units { get; set; } = [];
    public ICollection<ControlOrder> ControlOrders { get; set; } = [];
    public ICollection<TraceabilityLink> MaterialLinks { get; set; } = [];
}

/// <summary>Отдельное изделие или модуль в составе производственной партии.</summary>
public class ProductUnit : Entity
{
    public Guid ProductBatchId { get; set; }
    public ProductBatch? ProductBatch { get; set; }

    public string SerialNumber { get; set; } = string.Empty;

    public BatchStatus Status { get; set; } = BatchStatus.AwaitingControl;
}
