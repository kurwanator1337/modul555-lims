using Modul555.Lims.Domain.Common;

namespace Modul555.Lims.Domain.Reference;

/// <summary>
/// Группа номенклатуры: сырьё входного контроля (цемент, песок, щебень, вода, добавки,
/// металлопрокат) и продукция операционного контроля (смеси, изделия, модули).
/// </summary>
public class NomenclatureCategory : ReferenceEntity
{
    /// <summary>Категория относится к сырью входного контроля.</summary>
    public bool IsRawMaterial { get; set; }

    public ICollection<Nomenclature> Items { get; set; } = [];
}

/// <summary>Номенклатурная позиция — объект контроля.</summary>
public class Nomenclature : ReferenceEntity
{
    public Guid CategoryId { get; set; }
    public NomenclatureCategory? Category { get; set; }

    /// <summary>Единица измерения количества: т, м3, кг, шт.</summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>Марка, класс или фракция: ЦЕМ I 42,5Н, D600, фракция 5-20.</summary>
    public string? Grade { get; set; }

    /// <summary>Нормативный документ на продукцию.</summary>
    public Guid? RequirementStandardId { get; set; }
    public StandardDocument? RequirementStandard { get; set; }

    /// <summary>Требуется серийный (партионный) учёт.</summary>
    public bool TrackBatches { get; set; } = true;

    /// <summary>Срок годности в сутках, если применим (для добавок и цемента).</summary>
    public int? ShelfLifeDays { get; set; }

    /// <summary>Проектное значение для показателей вида «доля от проектного» (класс бетона, МПа).</summary>
    public decimal? DesignValue { get; set; }

    public ICollection<ControlProgram> ControlPrograms { get; set; } = [];
}
