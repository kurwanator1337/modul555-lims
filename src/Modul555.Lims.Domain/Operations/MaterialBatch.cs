using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;
using Modul555.Lims.Domain.Reference;

namespace Modul555.Lims.Domain.Operations;

/// <summary>Партия сырья, поступившая на входной контроль.</summary>
public class MaterialBatch : Entity
{
    public string Number { get; set; } = string.Empty;

    public Guid NomenclatureId { get; set; }
    public Nomenclature? Nomenclature { get; set; }

    public Guid SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public DateOnly ArrivalDate { get; set; }

    public decimal Quantity { get; set; }
    public string? QuantityUnit { get; set; }

    /// <summary>Номер паспорта / сертификата поставщика.</summary>
    public string? SupplierDocumentNumber { get; set; }
    public DateOnly? SupplierDocumentDate { get; set; }

    /// <summary>Дата изготовления — для проверки срока годности.</summary>
    public DateOnly? ManufacturedOn { get; set; }

    public Guid? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public BatchStatus Status { get; set; } = BatchStatus.AwaitingControl;

    public BatchDecision Decision { get; set; } = BatchDecision.None;
    public string? DecisionComment { get; set; }

    public ICollection<ControlOrder> ControlOrders { get; set; } = [];
    public ICollection<TraceabilityLink> ProductLinks { get; set; } = [];
}
