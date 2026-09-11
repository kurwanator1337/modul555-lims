using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;
using Modul555.Lims.Domain.Reference;

namespace Modul555.Lims.Domain.Operations;

/// <summary>Проба, отобранная для испытаний.</summary>
public class Sample : Entity
{
    public string Number { get; set; } = string.Empty;

    /// <summary>Штрихкод для этикетки пробы.</summary>
    public string Barcode { get; set; } = string.Empty;

    public Guid ControlOrderId { get; set; }
    public ControlOrder? ControlOrder { get; set; }

    public DateTimeOffset SampledAt { get; set; }

    public Guid? SampledById { get; set; }
    public Employee? SampledBy { get; set; }

    public Guid? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public SampleState State { get; set; } = SampleState.Registered;

    public decimal? Quantity { get; set; }
    public string? QuantityUnit { get; set; }

    public ICollection<SampleStateHistory> History { get; set; } = [];
    public ICollection<TestRun> TestRuns { get; set; } = [];
}

/// <summary>История смены состояния пробы.</summary>
public class SampleStateHistory : Entity
{
    public Guid SampleId { get; set; }
    public Sample? Sample { get; set; }

    public SampleState FromState { get; set; }
    public SampleState ToState { get; set; }

    public DateTimeOffset ChangedAt { get; set; }

    public Guid? ChangedById { get; set; }
    public Employee? ChangedBy { get; set; }

    public string? Comment { get; set; }
}
