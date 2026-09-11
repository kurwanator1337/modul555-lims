using Modul555.Lims.Domain.Common;

namespace Modul555.Lims.Domain.Reference;

/// <summary>Производственный участок: СТМ, ФСМ, ИМ, ЖБИ.</summary>
public class ProductionLine : ReferenceEntity
{
    public ICollection<ControlProgram> ControlPrograms { get; set; } = [];
}

/// <summary>Подразделение предприятия: лаборатория, ОТК, формовочный цех.</summary>
public class Subdivision : ReferenceEntity
{
    public Guid? ProductionLineId { get; set; }
    public ProductionLine? ProductionLine { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];
}

/// <summary>Поставщик сырья и комплектующих.</summary>
public class Supplier : ReferenceEntity
{
    public string? Inn { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public ICollection<MaterialBatch> Batches { get; set; } = [];
}

/// <summary>Склад или зона хранения, включая зону карантина.</summary>
public class Warehouse : ReferenceEntity
{
    /// <summary>Признак зоны карантина: сюда перемещается несоответствующее сырьё.</summary>
    public bool IsQuarantineZone { get; set; }
}
