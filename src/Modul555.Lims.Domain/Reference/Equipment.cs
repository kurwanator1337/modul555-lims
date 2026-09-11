using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;

namespace Modul555.Lims.Domain.Reference;

/// <summary>Тип средства измерения: пресс, весы, мегаомметр, склерометр, стенд опрессовки.</summary>
public class EquipmentType : ReferenceEntity
{
    public ICollection<Equipment> Items { get; set; } = [];
    public ICollection<TestMethodEquipmentType> Methods { get; set; } = [];
}

/// <summary>
/// Экземпляр лабораторного оборудования / средства измерения.
/// При истечении поверки статус переводится в «непригоден», ввод данных с него блокируется.
/// </summary>
public class Equipment : ReferenceEntity
{
    public Guid EquipmentTypeId { get; set; }
    public EquipmentType? EquipmentType { get; set; }

    public string? InventoryNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public int? YearOfManufacture { get; set; }

    public EquipmentStatus Status { get; set; } = EquipmentStatus.Operational;

    /// <summary>Дата окончания действия текущей поверки.</summary>
    public DateOnly? VerificationValidUntil { get; set; }

    /// <summary>Межповерочный интервал, месяцы.</summary>
    public int VerificationIntervalMonths { get; set; } = 12;

    /// <summary>Ответственный за прибор.</summary>
    public Guid? ResponsibleEmployeeId { get; set; }
    public Employee? ResponsibleEmployee { get; set; }

    public ICollection<EquipmentEvent> Events { get; set; } = [];
}

/// <summary>Мероприятие по оборудованию: поверка, калибровка, ТО, ремонт.</summary>
public class EquipmentEvent : Entity
{
    public Guid EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public EquipmentEventKind Kind { get; set; }

    public DateOnly EventDate { get; set; }

    /// <summary>Дата, до которой действует результат поверки или калибровки.</summary>
    public DateOnly? ValidUntil { get; set; }

    public string? CertificateNumber { get; set; }
    public string? Organization { get; set; }
    public string? Comment { get; set; }

    public Guid? PerformedById { get; set; }
    public Employee? PerformedBy { get; set; }
}
