namespace Modul555.Lims.Domain.Enums;

/// <summary>Статус партии сырья или готовой продукции.</summary>
public enum BatchStatus
{
    /// <summary>Поступила, контроль не начат.</summary>
    AwaitingControl = 1,

    /// <summary>Испытания идут.</summary>
    InControl = 2,

    /// <summary>Контроль пройден, допущена к производству.</summary>
    Approved = 3,

    /// <summary>В зоне карантина до выяснения соответствия.</summary>
    Quarantine = 4,

    /// <summary>Забракована.</summary>
    Rejected = 5,

    /// <summary>Использование с ограничениями по согласованию с технологом.</summary>
    RestrictedUse = 6,

    /// <summary>Возвращена поставщику.</summary>
    ReturnedToSupplier = 7,

    /// <summary>Списана.</summary>
    WrittenOff = 8,
}

/// <summary>Решение по несоответствующей партии.</summary>
public enum BatchDecision
{
    None = 0,
    ReturnToSupplier = 1,
    Sorting = 2,
    RestrictedUse = 3,
    WriteOff = 4,
    Rework = 5,
    AdditionalCuring = 6,
    ExtendedSteaming = 7,
    RepeatTest = 8,
}

/// <summary>Состояние пробы.</summary>
public enum SampleState
{
    Registered = 1,
    InPreparation = 2,
    Prepared = 3,
    InTesting = 4,
    Tested = 5,
    Stored = 6,
    Disposed = 7,
}

/// <summary>Статус распоряжения на контроль.</summary>
public enum ControlOrderStatus
{
    Draft = 1,
    Approved = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
}

/// <summary>Источник распоряжения на проведение проверки.</summary>
public enum ControlOrderSource
{
    /// <summary>Приход партии сырья.</summary>
    MaterialArrival = 1,

    /// <summary>Операционный контроль по производственной партии.</summary>
    ProductionBatch = 2,

    /// <summary>План контроля.</summary>
    Plan = 3,

    /// <summary>Претензия.</summary>
    Claim = 4,

    /// <summary>Повторное испытание после несоответствия.</summary>
    RepeatAfterNonconformance = 5,
}

/// <summary>Статус проведения испытания.</summary>
public enum TestRunStatus
{
    Assigned = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
}

/// <summary>Статус оборудования / средства измерения.</summary>
public enum EquipmentStatus
{
    /// <summary>Исправно, поверка действует, допущено к испытаниям.</summary>
    Operational = 1,

    /// <summary>На поверке или калибровке.</summary>
    UnderVerification = 2,

    /// <summary>На техническом обслуживании.</summary>
    UnderMaintenance = 3,

    /// <summary>В ремонте.</summary>
    UnderRepair = 4,

    /// <summary>Непригодно: поверка истекла — ввод данных заблокирован.</summary>
    VerificationExpired = 5,

    /// <summary>Выведено из эксплуатации.</summary>
    Decommissioned = 6,
}

/// <summary>Вид мероприятия по оборудованию.</summary>
public enum EquipmentEventKind
{
    Verification = 1,
    Calibration = 2,
    Maintenance = 3,
    Repair = 4,
    Commissioning = 5,
    Decommissioning = 6,
}

/// <summary>Статус документа: черновик, подписан исполнителем, утверждён.</summary>
public enum ProtocolStatus
{
    Draft = 1,
    Signed = 2,
    Approved = 3,
    Cancelled = 4,
}

/// <summary>Роль подписанта в документе.</summary>
public enum SignatureRole
{
    /// <summary>Выполнил — лаборант, испытатель, электролаборант.</summary>
    Performer = 1,

    /// <summary>Утвердил — начальник лаборатории.</summary>
    Approver = 2,

    /// <summary>Согласовал — представитель ОТК.</summary>
    QualityInspector = 3,

    /// <summary>Технолог.</summary>
    Technologist = 4,

    /// <summary>Член комиссии.</summary>
    CommissionMember = 5,
}

/// <summary>Статус несоответствия.</summary>
public enum NonconformanceStatus
{
    Open = 1,
    UnderReview = 2,
    ActionsPlanned = 3,
    Closed = 4,
}

/// <summary>Статус корректирующего действия.</summary>
public enum CorrectiveActionStatus
{
    Planned = 1,
    InProgress = 2,
    Done = 3,
    Cancelled = 4,
}

/// <summary>Вид производственной партии в операционном контроле.</summary>
public enum ProductionBatchKind
{
    /// <summary>Партия смеси (замес).</summary>
    Mix = 1,

    /// <summary>Партия готовых изделий.</summary>
    FinishedGoods = 2,

    /// <summary>Партия арматурных каркасов и закладных деталей.</summary>
    RebarAssembly = 3,

    /// <summary>Партия модулей СТМ / ИМ.</summary>
    Module = 4,
}
