using Modul555.Lims.Domain.Common;
using Modul555.Lims.Domain.Enums;

namespace Modul555.Lims.Domain.Reference;
public class DocumentTemplate : ReferenceEntity
{
    public DocumentKind Kind { get; set; }

    public ControlStage Stage { get; set; }

    public string? ProductionLineCode { get; set; }

    public string PerformerRole { get; set; } = "Laborant";

    public string? ApproverRole { get; set; } = "LabHead";

    public bool RequiresQualityInspector { get; set; }

    public SignatureKind SignatureKind { get; set; } = SignatureKind.Simple;

    public bool CreatedOnNonconformance { get; set; }
}
