using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;

namespace Modul555.Lims.Domain.Reference;

/// <summary>
/// Показатель качества — то, что оценивается и попадает в протокол:
/// активность цемента, модуль крупности песка, прочность бетона, сопротивление изоляции.
/// Значение либо совпадает с измеряемой величиной, либо рассчитывается по формуле метода.
/// </summary>
public class QualityParameter : ReferenceEntity
{
    public ValueKind ValueKind { get; set; } = ValueKind.Numeric;

    /// <summary>Единица измерения показателя: МПа, %, мин, кг/м3, МОм, град.</summary>
    public string? UnitOfMeasure { get; set; }

    /// <summary>Число знаков после запятой при отображении и округлении результата.</summary>
    public int Precision { get; set; } = 2;

    /// <summary>Допустимые значения для показателя типа «выбор из справочника», через точку с запятой.</summary>
    public string? AllowedValues { get; set; }
}

/// <summary>
/// Измеряемая величина — первичные данные, которые лаборант снимает с прибора:
/// масса, объём, разрушающее усилие, давление, отсчёт по шкале.
/// </summary>
public class MeasuredQuantity : ReferenceEntity
{
    /// <summary>Единица измерения: г, мм, кН, МПа, с.</summary>
    public string? UnitOfMeasure { get; set; }

    /// <summary>Имя переменной в формуле расчёта показателя, например <c>mass</c>.</summary>
    public string VariableName { get; set; } = string.Empty;

    public int Precision { get; set; } = 2;
}

/// <summary>Вид дефекта поверхности — для полей, требующих экспертной оценки.</summary>
public class Defect : ReferenceEntity
{
    /// <summary>Дефект является критическим и сам по себе означает несоответствие.</summary>
    public bool IsCritical { get; set; }

    /// <summary>Область применения: арматура, сварные швы, поверхность изделия.</summary>
    public string? AppliesTo { get; set; }
}

/// <summary>Причина несоответствия — заполняется вручную при разборе.</summary>
public class NonconformanceCause : ReferenceEntity
{
    /// <summary>Группа причины: сырьё, технология, оборудование, персонал.</summary>
    public string? Group { get; set; }
}
