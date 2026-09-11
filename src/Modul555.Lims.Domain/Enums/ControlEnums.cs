namespace Modul555.Lims.Domain.Enums;

public enum ControlStage
{
    Incoming = 1,
    Operational = 2,
}

public enum ControlKind
{
    Incoming = 1,
    Operational = 2,
    Acceptance = 3,
    Periodic = 4,
    Repeat = 5,
}

public enum StandardKind
{
    Requirement = 1,
    TestMethod = 2,
    EvaluationRules = 3,
    CodeOfPractice = 4,
}

public enum ValueKind
{
    Numeric = 1,
    Boolean = 2,
    Enumerated = 3,
    Text = 4,
}

public enum NormKind
{
    Min = 1,
    Max = 2,
    Range = 3,
    Nominal = 4,
    PercentOfDesign = 5,
    MustBeTrue = 6,
    Expert = 7,
}

public enum AggregationKind
{
    Average = 1,
    Minimum = 2,
    Maximum = 3,
    Last = 4,
}

public enum ControlFrequency
{
    EveryBatch = 1,
    Sampled = 2,
    Full = 3,
    Quarterly = 4,
    PerShift = 5,
    ByPlan = 6,
}

public enum Verdict
{
    Pending = 0,
    Conforms = 1,
    NotConforms = 2,
    NotApplicable = 3,
}

public enum ValueSource
{
    Manual = 1,
    Instrument = 2,
    Scada = 3,
    FileImport = 4,
}
