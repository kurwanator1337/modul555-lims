using Modul555.Lims.Domain.Common;

namespace Modul555.Lims.Domain.Reference;

/// <summary>
/// Метод испытаний: ссылка на нормативный документ, набор измеряемых величин
/// и формула, по которой из них рассчитывается показатель качества.
/// </summary>
public class TestMethod : ReferenceEntity
{
    /// <summary>Нормативный документ на метод, например ГОСТ 10180.</summary>
    public Guid? StandardDocumentId { get; set; }
    public StandardDocument? StandardDocument { get; set; }

    /// <summary>
    /// Формула расчёта показателя над переменными измеряемых величин, например <c>mass / volume</c>.
    /// Пустая формула означает, что показатель равен единственной измеряемой величине.
    /// </summary>
    public string? Formula { get; set; }

    /// <summary>Метод разрушающий (испытание образца до разрушения).</summary>
    public bool IsDestructive { get; set; }

    /// <summary>Трудоёмкость проведения испытания в минутах — для планирования смены.</summary>
    public int LaborMinutes { get; set; } = 30;

    /// <summary>Число повторностей по методике.</summary>
    public int DefaultReplicates { get; set; } = 1;

    /// <summary>Требуется действующая аттестация исполнителя по этому методу.</summary>
    public bool RequiresCompetency { get; set; } = true;

    public ICollection<TestMethodQuantity> Quantities { get; set; } = [];
    public ICollection<TestMethodEquipmentType> EquipmentTypes { get; set; } = [];
    public ICollection<TestMethodMaterial> Materials { get; set; } = [];
    public ICollection<Competency> Competencies { get; set; } = [];
}

/// <summary>Измеряемая величина в составе метода.</summary>
public class TestMethodQuantity : Entity
{
    public Guid TestMethodId { get; set; }
    public TestMethod? TestMethod { get; set; }

    public Guid MeasuredQuantityId { get; set; }
    public MeasuredQuantity? MeasuredQuantity { get; set; }

    public int SortOrder { get; set; }
}

/// <summary>Тип оборудования, применимого в методе: пресс, весы, мегаомметр, склерометр.</summary>
public class TestMethodEquipmentType : Entity
{
    public Guid TestMethodId { get; set; }
    public TestMethod? TestMethod { get; set; }

    public Guid EquipmentTypeId { get; set; }
    public EquipmentType? EquipmentType { get; set; }

    /// <summary>Указание оборудования этого типа обязательно для проведения испытания.</summary>
    public bool IsRequired { get; set; } = true;
}

/// <summary>Плановый расход расходного материала на одно испытание.</summary>
public class TestMethodMaterial : Entity
{
    public Guid TestMethodId { get; set; }
    public TestMethod? TestMethod { get; set; }

    public Guid NomenclatureId { get; set; }
    public Nomenclature? Nomenclature { get; set; }

    public decimal PlannedQuantity { get; set; }
}
