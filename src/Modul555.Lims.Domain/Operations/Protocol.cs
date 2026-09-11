using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;
using Modul555.Lims.Domain.Reference;

namespace Modul555.Lims.Domain.Operations;

/// <summary>Протокол испытаний, акт или заключение, сформированные лабораторией.</summary>
public class Protocol : Entity
{
    public string Number { get; set; } = string.Empty;

    public DocumentKind Kind { get; set; }

    public Guid? TemplateId { get; set; }
    public DocumentTemplate? Template { get; set; }

    public Guid? ControlOrderId { get; set; }
    public ControlOrder? ControlOrder { get; set; }

    public DateTimeOffset IssuedAt { get; set; }

    public ProtocolStatus Status { get; set; } = ProtocolStatus.Draft;

    public Verdict OverallVerdict { get; set; } = Verdict.Pending;

    /// <summary>Снимок результатов на момент формирования документа, JSON.</summary>
    public string? ResultsSnapshot { get; set; }

    public string? Conclusion { get; set; }

    public ICollection<ProtocolSignature> Signatures { get; set; } = [];
}

/// <summary>Подпись документа. Без неё документ не меняет статус партии.</summary>
public class ProtocolSignature : Entity
{
    public Guid ProtocolId { get; set; }
    public Protocol? Protocol { get; set; }

    public SignatureRole Role { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateTimeOffset SignedAt { get; set; }

    /// <summary>Внутренняя подпись пользователя системы. УКЭП — на следующем этапе.</summary>
    public SignatureKind Kind { get; set; } = SignatureKind.Simple;
}
