namespace Modul555.Lims.Domain.Enums;

public enum DocumentKind
{
    IncomingControlJournal = 101,
    IncomingProtocolCement = 102,
    IncomingProtocolSand = 103,
    IncomingProtocolGravel = 104,
    IncomingProtocolWater = 105,
    IncomingProtocolAdmixture = 106,
    RebarMechanicalTestProtocol = 107,
    RebarVisualInspectionReport = 108,
    IncomingNonconformanceReport = 109,
    SortingReport = 110,
    MixParametersProtocol = 201,
    ScadaVerificationReport = 202,
    HeatMoistureTreatmentProtocol = 203,
    ControlSpecimenProtocolAfterSteaming = 204,
    ControlSpecimenProtocol28Days = 205,
    AeratedConcreteDensityProtocol = 206,
    FinishedGoodsGeometryReport = 207,
    ReleaseMoistureProtocol = 208,
    BatchConformityConclusion = 209,

    PressureTestProtocol = 301,
    DrainageFloodingReport = 302,
    ElectricalMeasurementProtocol = 303,
    ModuleSystemsAcceptanceReport = 304,
    ModuleConformityConclusion = 305,

    WeldedJointTestProtocol = 401,
    ConcreteMixControlProtocol = 402,
    RcHeatTreatmentProtocol = 403,
    NondestructiveStrengthProtocolDemoulding = 404,
    NondestructiveStrengthProtocolRelease = 405,
    CubeTestProtocol28Days = 406,
    FrostResistanceProtocol = 407,
    WaterTightnessProtocol = 408,
    ConcreteCoverProtocol = 409,
    RcBatchConformityConclusion = 410,

    OperationalNonconformanceReport = 501,
    CorrectiveActionCard = 502,
    NonconformanceJournal = 503,
    WriteOffReport = 504,

    VerificationSchedule = 601,
}

public enum SignatureKind
{
    Simple = 1,
    Qualified = 2,
}
