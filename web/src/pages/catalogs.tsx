import CatalogPage from "../components/CatalogPage";
import { renderEquipmentStatus } from "../components/StatusTag";
import { controlStageRu, documentKindRu, label as ruLabel, standardKindRu } from "../i18n/labels";

const codeName = [
  { title: "Код", dataIndex: "code", width: 140 },
  { title: "Наименование", dataIndex: "name" },
];

export function NomenclaturesPage() {
  return (
    <CatalogPage
      title="Номенклатура"
      path="nomenclatures"
      showInactive
      drawerWidth={520}
      columns={[
        ...codeName,
        { title: "Группа", dataIndex: "categoryName", width: 180 },
        { title: "Марка", dataIndex: "grade", width: 140 },
        { title: "Ед.", dataIndex: "unitOfMeasure", width: 70 },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        {
          name: "categoryId",
          label: "Группа",
          type: "lookup",
          lookup: "nomenclature-categories",
          required: true,
        },
        { name: "unitOfMeasure", label: "Ед. изм.", required: true },
        { name: "grade", label: "Марка / класс" },
        {
          name: "requirementStandardId",
          label: "Норматив",
          type: "lookup",
          lookup: "standards",
        },
        { name: "designValue", label: "Проектное значение", type: "number" },
        { name: "shelfLifeDays", label: "Срок годности, сут.", type: "number" },
        { name: "trackBatches", label: "Партионный учёт", type: "switch" },
      ]}
    />
  );
}

export function StandardsPage() {
  return (
    <CatalogPage
      title="Нормативные документы"
      path="standards"
      showInactive
      columns={[
        ...codeName,
        { title: "Год", dataIndex: "year", width: 80 },
        {
          title: "Вид",
          dataIndex: "kind",
          width: 160,
          render: (v: string) => ruLabel(standardKindRu, v),
        },
      ]}
      fields={[
        { name: "code", label: "Обозначение", required: true },
        { name: "name", label: "Наименование", required: true },
        { name: "year", label: "Год", type: "number" },
        {
          name: "kind",
          label: "Вид",
          type: "select",
          options: [
            { value: "Requirement", label: "Норматив" },
            { value: "TestMethod", label: "Метод испытаний" },
            { value: "EvaluationRules", label: "Правила оценки" },
            { value: "CodeOfPractice", label: "Свод правил" },
          ],
        },
        { name: "isCurrent", label: "Действующий", type: "switch" },
      ]}
    />
  );
}

export function ParametersPage() {
  return (
    <CatalogPage
      title="Показатели качества"
      path="quality-parameters"
      showInactive
      columns={[
        ...codeName,
        { title: "Ед.", dataIndex: "unitOfMeasure", width: 80 },
        { title: "Тип", dataIndex: "valueKind", width: 120 },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        { name: "unitOfMeasure", label: "Ед. изм." },
        { name: "precision", label: "Знаков", type: "number" },
        {
          name: "valueKind",
          label: "Тип",
          type: "select",
          options: [
            { value: "Numeric", label: "Число" },
            { value: "Boolean", label: "Да / нет" },
            { value: "Enumerated", label: "Справочник" },
            { value: "Text", label: "Текст" },
          ],
        },
        {
          name: "allowedValues",
          label: "Допустимые значения (через ;)",
          type: "textarea",
        },
      ]}
    />
  );
}

export function MethodsPage() {
  return (
    <CatalogPage
      title="Методы испытаний"
      path="test-methods"
      showInactive
      drawerWidth={560}
      columns={[
        ...codeName,
        { title: "Формула", dataIndex: "formula" },
        { title: "ГОСТ", dataIndex: "standardDocumentName", width: 140 },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        {
          name: "standardDocumentId",
          label: "Норматив на метод",
          type: "lookup",
          lookup: "standards",
        },
        { name: "formula", label: "Формула" },
        {
          name: "quantityIds",
          label: "Измеряемые величины",
          type: "multiLookup",
          lookup: "measured-quantities",
        },
        {
          name: "equipmentTypeIds",
          label: "Типы СИ",
          type: "multiLookup",
          lookup: "equipment-types",
        },
        { name: "laborMinutes", label: "Трудоёмкость, мин", type: "number" },
        { name: "defaultReplicates", label: "Повторности", type: "number" },
        { name: "isDestructive", label: "Разрушающий", type: "switch" },
        {
          name: "requiresCompetency",
          label: "Требует аттестации",
          type: "switch",
        },
      ]}
    />
  );
}

export function SuppliersPage() {
  return (
    <CatalogPage
      title="Поставщики"
      path="suppliers"
      showInactive
      columns={[
        ...codeName,
        { title: "ИНН", dataIndex: "inn", width: 140 },
        { title: "Контакт", dataIndex: "contactPerson" },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        { name: "inn", label: "ИНН" },
        { name: "contactPerson", label: "Контакт" },
        { name: "phone", label: "Телефон" },
        { name: "email", label: "Эл. почта" },
        { name: "address", label: "Адрес", type: "textarea" },
      ]}
    />
  );
}

export function EquipmentCatalogPage() {
  return (
    <CatalogPage
      title="Средства измерений"
      path="equipment"
      showInactive
      drawerWidth={560}
      columns={[
        ...codeName,
        { title: "Тип", dataIndex: "equipmentTypeName", width: 180 },
        { title: "Инв. №", dataIndex: "inventoryNumber", width: 110 },
        {
          title: "Поверка до",
          dataIndex: "verificationValidUntil",
          width: 120,
        },
        {
          title: "Статус",
          dataIndex: "status",
          width: 160,
          render: renderEquipmentStatus,
        },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        {
          name: "equipmentTypeId",
          label: "Тип СИ",
          type: "lookup",
          lookup: "equipment-types",
          required: true,
        },
        { name: "inventoryNumber", label: "Инв. №" },
        { name: "serialNumber", label: "Зав. №" },
        { name: "manufacturer", label: "Изготовитель" },
        { name: "yearOfManufacture", label: "Год выпуска", type: "number" },
        {
          name: "status",
          label: "Статус",
          type: "select",
          options: [
            { value: "Operational", label: "Исправно" },
            { value: "UnderVerification", label: "На поверке" },
            { value: "UnderMaintenance", label: "На ТО" },
            { value: "UnderRepair", label: "В ремонте" },
            { value: "VerificationExpired", label: "Поверка истекла" },
            { value: "Decommissioned", label: "Снято с эксплуатации" },
          ],
        },
        { name: "verificationValidUntil", label: "Поверка до", type: "date" },
        {
          name: "verificationIntervalMonths",
          label: "Межповерочный интервал, мес.",
          type: "number",
        },
        {
          name: "responsibleEmployeeId",
          label: "Ответственный",
          type: "lookup",
          lookup: "employees",
        },
      ]}
    />
  );
}

export function DefectsPage() {
  return (
    <CatalogPage
      title="Справочник дефектов"
      path="defects"
      showInactive
      columns={[
        ...codeName,
        { title: "Область", dataIndex: "appliesTo" },
        {
          title: "Критический",
          dataIndex: "isCritical",
          render: (v: boolean) => (v ? "да" : ""),
        },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        { name: "appliesTo", label: "Область" },
        { name: "isCritical", label: "Критический", type: "switch" },
      ]}
    />
  );
}

export function CausesPage() {
  return (
    <CatalogPage
      title="Причины несоответствий"
      path="causes"
      showInactive
      columns={[...codeName, { title: "Группа", dataIndex: "group" }]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        { name: "group", label: "Группа" },
      ]}
    />
  );
}

export function TemplatesPage() {
  return (
    <CatalogPage
      title="Шаблоны документов"
      path="document-templates"
      showInactive
      drawerWidth={560}
      columns={[
        ...codeName,
        {
          title: "Вид",
          dataIndex: "kind",
          render: (v: string) => ruLabel(documentKindRu, v),
        },
        {
          title: "Этап",
          dataIndex: "stage",
          width: 160,
          render: (v: string) => ruLabel(controlStageRu, v),
        },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        {
          name: "kind",
          label: "Вид",
          type: "select",
          options: Object.entries(documentKindRu).map(([value, label]) => ({
            value,
            label,
          })),
          required: true,
        },
        {
          name: "stage",
          label: "Этап",
          type: "select",
          options: [
            { value: "Incoming", label: "Входной контроль" },
            { value: "Operational", label: "Операционный контроль" },
          ],
          required: true,
        },
        { name: "productionLineCode", label: "Код участка" },
        { name: "performerRole", label: "Роль исполнителя" },
        { name: "approverRole", label: "Роль утверждающего" },
        {
          name: "requiresQualityInspector",
          label: "Требуется ОТК",
          type: "switch",
        },
      ]}
    />
  );
}
