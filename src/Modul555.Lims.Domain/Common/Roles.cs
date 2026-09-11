namespace Modul555.Lims.Domain.Common;

/// <summary>Роли: Keycloak (auth) + доменные (шаблоны документов).</summary>
public static class Roles
{
    // Keycloak realm roles
    public const string Admin = "admin";
    public const string Manager = "manager";
    public const string Tester = "tester";
    public const string Developer = "developer";

    // Роли в шаблонах протоколов / назначениях
    public const string Laborant = "Laborant";
    public const string LabHead = "LabHead";
    public const string QualityInspector = "QualityInspector";
    public const string Technologist = "Technologist";
    public const string ElectricalLaborant = "ElectricalLaborant";

    public static readonly string[] Keycloak =
    [
        Admin,
        Manager,
        Tester,
        Developer,
    ];

    public static readonly string[] Document =
    [
        Laborant,
        LabHead,
        QualityInspector,
        Technologist,
        ElectricalLaborant,
    ];

    public static readonly string[] All = [.. Keycloak, .. Document];

    public static readonly string[] Administrators = [Admin, Manager];
}
