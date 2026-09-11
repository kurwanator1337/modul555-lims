using Modul555.Lims.Domain.Common;

namespace Modul555.Lims.Domain.Reference;

/// <summary>Сотрудник лаборатории, ОТК или технологической службы.</summary>
public class Employee : ReferenceEntity
{
    /// <summary>Логин для входа в систему. Совпадает с <see cref="UserAccount.UserName"/>.</summary>
    public string UserName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }

    /// <summary>Должность: лаборант, начальник лаборатории, представитель ОТК, технолог, электролаборант.</summary>
    public string Position { get; set; } = string.Empty;

    public Guid? SubdivisionId { get; set; }
    public Subdivision? Subdivision { get; set; }

    /// <summary>График работы в свободной форме, например «пятидневка 08:00–17:00».</summary>
    public string? WorkSchedule { get; set; }

    public ICollection<Competency> Competencies { get; set; } = [];
    public ICollection<UserAccount> Accounts { get; set; } = [];

    public string FullName =>
        string.Join(
            ' ',
            new[] { LastName, FirstName, MiddleName }.Where(s => !string.IsNullOrWhiteSpace(s))
        );
}

/// <summary>Аттестация сотрудника по методу испытаний: без действующей — назначить нельзя.</summary>
public class Competency : Entity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public Guid TestMethodId { get; set; }
    public TestMethod? TestMethod { get; set; }

    public DateOnly IssuedOn { get; set; }
    public DateOnly ValidUntil { get; set; }
    public string? CertificateNumber { get; set; }

    public bool IsValidOn(DateOnly date) => date >= IssuedOn && date <= ValidUntil;
}

/// <summary>Учётная запись пользователя системы.</summary>
public class UserAccount : Entity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public string UserName { get; set; } = string.Empty;

    /// <summary>Хеш пароля (PBKDF2).</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Список ролей через запятую: Laborant, LabHead, QualityInspector, Technologist, ElectricalLaborant.</summary>
    public string Roles { get; set; } = string.Empty;

    public bool IsLocked { get; set; }
}
