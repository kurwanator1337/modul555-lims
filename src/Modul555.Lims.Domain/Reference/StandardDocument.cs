using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;

namespace Modul555.Lims.Domain.Reference;

/// <summary>
/// Нормативный документ: норматив на продукцию, метод испытаний либо правила оценки.
/// Код совпадает с обозначением документа, например <c>ГОСТ 10180-2012</c>.
/// </summary>
public class StandardDocument : ReferenceEntity
{
    public StandardKind Kind { get; set; }

    /// <summary>Год введения или редакции.</summary>
    public int? Year { get; set; }

    /// <summary>Документ действует; снятые с действия остаются для ранее выпущенных протоколов.</summary>
    public bool IsCurrent { get; set; } = true;
}
