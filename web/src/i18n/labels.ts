/** Словари enum → русский текст для UI. */

export const batchStatusRu: Record<string, string> = {
  AwaitingControl: "Ожидает контроля",
  InControl: "На контроле",
  Approved: "Допущена",
  Quarantine: "Карантин",
  Rejected: "Забракована",
  RestrictedUse: "С ограничениями",
  ReturnedToSupplier: "Возврат поставщику",
  WrittenOff: "Списана",
};

export const batchDecisionRu: Record<string, string> = {
  None: "—",
  ReturnToSupplier: "Возврат поставщику",
  Sorting: "Разбраковка",
  RestrictedUse: "Использование с ограничениями",
  WriteOff: "Списание",
  Rework: "Доработка",
  AdditionalCuring: "Дополнительная выдержка",
  ExtendedSteaming: "Продление пропарки",
  RepeatTest: "Повторное испытание",
};

export const controlOrderStatusRu: Record<string, string> = {
  Draft: "Черновик",
  Approved: "Утверждено",
  InProgress: "В работе",
  Completed: "Завершено",
  Cancelled: "Отменено",
};

export const controlKindRu: Record<string, string> = {
  Incoming: "Входной",
  Operational: "Операционный",
  Acceptance: "Приёмочный",
  Periodic: "Периодический",
  Repeat: "Повторный",
};

export const sampleStateRu: Record<string, string> = {
  Registered: "Зарегистрирована",
  InPreparation: "Пробоподготовка",
  Prepared: "Подготовлена",
  InTesting: "На испытании",
  Tested: "Испытана",
  Stored: "На хранении",
  Disposed: "Утилизирована",
};

export const testRunStatusRu: Record<string, string> = {
  Assigned: "Назначено",
  InProgress: "В работе",
  Completed: "Завершено",
  Cancelled: "Отменено",
};

export const verdictRu: Record<string, string> = {
  Pending: "ожидает",
  Conforms: "соответствует",
  NotConforms: "не соответствует",
  NotApplicable: "не применимо",
};

export const equipmentStatusRu: Record<string, string> = {
  Operational: "Исправно",
  UnderVerification: "На поверке",
  UnderMaintenance: "На ТО",
  UnderRepair: "В ремонте",
  VerificationExpired: "Поверка истекла",
  Decommissioned: "Снято с эксплуатации",
};

export const protocolStatusRu: Record<string, string> = {
  Draft: "Черновик",
  Signed: "Подписан",
  Approved: "Утверждён",
  Cancelled: "Отменён",
};

export const nonconformanceStatusRu: Record<string, string> = {
  Open: "Открыто",
  UnderReview: "На разборе",
  ActionsPlanned: "Действия запланированы",
  Closed: "Закрыто",
};

export const correctiveActionStatusRu: Record<string, string> = {
  Planned: "Запланировано",
  InProgress: "В работе",
  Done: "Выполнено",
  Cancelled: "Отменено",
};

export const controlStageRu: Record<string, string> = {
  Incoming: "Входной контроль",
  Operational: "Операционный контроль",
};

export const productionBatchKindRu: Record<string, string> = {
  Mix: "Замес смеси",
  FinishedGoods: "Готовые изделия",
  RebarAssembly: "Каркасы",
  Module: "Модуль СТМ/ИМ",
};

export const standardKindRu: Record<string, string> = {
  Requirement: "Норматив",
  TestMethod: "Метод испытаний",
  EvaluationRules: "Правила оценки",
  CodeOfPractice: "Свод правил",
};

export const documentKindRu: Record<string, string> = {
  IncomingControlJournal: "Журнал входного контроля",
  IncomingProtocolCement: "Протокол входного контроля цемента",
  IncomingProtocolSand: "Протокол входного контроля песка",
  IncomingProtocolGravel: "Протокол входного контроля щебня",
  IncomingProtocolWater: "Протокол входного контроля воды",
  IncomingProtocolAdmixture: "Протокол входного контроля добавок",
  RebarMechanicalTestProtocol: "Протокол мех. испытаний арматуры",
  RebarVisualInspectionReport: "Акт визуального контроля арматуры",
  IncomingNonconformanceReport: "Акт о несоответствии (входной)",
  SortingReport: "Акт разбраковки",
  MixParametersProtocol: "Протокол параметров смеси",
  ScadaVerificationReport: "Акт сверки с SCADA",
  HeatMoistureTreatmentProtocol: "Протокол ТВО",
  ControlSpecimenProtocolAfterSteaming: "Протокол образцов после пропарки",
  ControlSpecimenProtocol28Days: "Протокол образцов 28 сут.",
  AeratedConcreteDensityProtocol: "Протокол плотности газобетона",
  FinishedGoodsGeometryReport: "Акт геометрии изделий",
  ReleaseMoistureProtocol: "Протокол отпускной влажности",
  BatchConformityConclusion: "Заключение о годности партии",
  PressureTestProtocol: "Протокол опрессовки",
  DrainageFloodingReport: "Акт проверки дренажа/затопления",
  ElectricalMeasurementProtocol: "Протокол электроизмерений",
  ModuleSystemsAcceptanceReport: "Акт приёмки систем модуля",
  ModuleConformityConclusion: "Заключение о годности модуля",
  WeldedJointTestProtocol: "Протокол испытаний сварных соединений",
  ConcreteMixControlProtocol: "Протокол контроля бетонной смеси",
  RcHeatTreatmentProtocol: "Протокол термообработки ЖБИ",
  NondestructiveStrengthProtocolDemoulding: "Протокол прочности при распалубке",
  NondestructiveStrengthProtocolRelease: "Протокол прочности при отпуске",
  CubeTestProtocol28Days: "Протокол кубиков 28 сут.",
  FrostResistanceProtocol: "Протокол морозостойкости",
  WaterTightnessProtocol: "Протокол водонепроницаемости",
  ConcreteCoverProtocol: "Протокол защитного слоя",
  RcBatchConformityConclusion: "Заключение о годности партии ЖБИ",
  OperationalNonconformanceReport: "Акт о несоответствии (операционный)",
  CorrectiveActionCard: "Карта корректирующих действий",
  NonconformanceJournal: "Журнал несоответствий",
  WriteOffReport: "Акт списания",
  VerificationSchedule: "График поверок",
};

export function label(
  dict: Record<string, string>,
  value: unknown,
  fallback?: string,
): string {
  if (value == null || value === "") return fallback ?? "—";
  const key = String(value);
  return dict[key] ?? fallback ?? key;
}
