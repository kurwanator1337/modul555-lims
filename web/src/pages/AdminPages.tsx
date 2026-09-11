import { useMemo, useState } from "react";
import {
  Button,
  Card,
  DatePicker,
  Drawer,
  Form,
  Input,
  Popconfirm,
  Select,
  Space,
  Switch,
  Table,
  Typography,
  message,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import dayjs from "dayjs";
import CatalogPage from "../components/CatalogPage";
import { useRoleOptions } from "../hooks/useRoleOptions";
import { renderBatchStatus } from "../components/StatusTag";
import {
  batchDecisionRu,
  batchStatusRu,
  controlStageRu,
  documentKindRu,
  label as ruLabel,
} from "../i18n/labels";
import {
  useAdminAuditQuery,
  useAdminMaterialBatchesQuery,
  useAdminProductBatchesQuery,
  useAdminUsersQuery,
  useClearDemoMutation,
  useCompetenciesQuery,
  useDeleteCompetencyMutation,
  useGenerateDemoMutation,
  useLookupQuery,
  useSaveAdminUserMutation,
  useSaveCompetencyMutation,
  useUpdateAdminMaterialBatchMutation,
  useUpdateAdminProductBatchMutation,
} from "../api/endpoints";

const codeName = [
  { title: "Код", dataIndex: "code", width: 140 },
  { title: "Наименование", dataIndex: "name" },
];

const roleRu: Record<string, string> = {
  Laborant: "Лаборант",
  LabHead: "Нач. лаборатории",
  QualityInspector: "ОТК",
  Technologist: "Технолог",
  ElectricalLaborant: "Электролаборант",
  admin: "Администратор",
  manager: "Менеджер",
  tester: "Тестировщик",
  developer: "Разработчик",
};

export function AdminHomePage() {
  const [clearDemo, clearState] = useClearDemoMutation();
  const [generateDemo, genState] = useGenerateDemoMutation();

  return (
    <Space orientation="vertical" size={16} style={{ width: "100%" }}>
      <Typography.Title level={3} style={{ margin: 0 }}>
        Администрирование
      </Typography.Title>
      <Typography.Paragraph type="secondary" style={{ marginBottom: 0 }}>
        Полное редактирование НСИ, учёток, аттестаций и корректировка статусов партий без
        доступа к БД.
      </Typography.Paragraph>

      <Card title="Демонстрационные данные" size="small">
        <Typography.Paragraph type="secondary">
          Операционный контур для показа: партии, распоряжения, пробы, испытания,
          протоколы, несоответствия. Справочники НСИ не затрагиваются.
        </Typography.Paragraph>
        <Space wrap>
          <Popconfirm
            title="Очистить все тестовые данные?"
            description="Будут удалены партии, распоряжения, пробы, испытания, протоколы, НС и связи. НСИ останется."
            okText="Очистить"
            okButtonProps={{ danger: true }}
            onConfirm={async () => {
              try {
                const r = await clearDemo().unwrap();
                message.success(r.message);
              } catch (e: unknown) {
                const err = e as { data?: { message?: string } };
                message.error(err?.data?.message ?? "Не удалось очистить");
              }
            }}
          >
            <Button danger loading={clearState.isLoading}>
              Очистить тестовые данные
            </Button>
          </Popconfirm>
          <Popconfirm
            title="Сгенерировать демо-сценарий?"
            description="Партия цемента, распоряжение входного контроля, проба и испытания на АРМ. База должна быть без партий."
            okText="Сгенерировать"
            onConfirm={async () => {
              try {
                const r = await generateDemo().unwrap();
                message.success(r.message);
              } catch (e: unknown) {
                const err = e as { data?: { message?: string } };
                message.error(err?.data?.message ?? "Не удалось сгенерировать");
              }
            }}
          >
            <Button type="primary" loading={genState.isLoading}>
              Сгенерировать демо-данные
            </Button>
          </Popconfirm>
        </Space>
      </Card>

      <Card size="small">
        <ul style={{ margin: 0, paddingLeft: 18, lineHeight: 1.8 }}>
          <li>Организация: участки, подразделения, склады</li>
          <li>
            НСИ: номенклатура, нормативы, показатели, методы, программы, СИ, шаблоны
          </li>
          <li>Персонал: сотрудники, учётные записи, роли, пароли, аттестации</li>
          <li>Операции: статусы партий сырья и продукции</li>
          <li>Журнал аудита изменений</li>
        </ul>
      </Card>
    </Space>
  );
}

export function ProductionLinesAdminPage() {
  return (
    <CatalogPage
      title="Производственные участки"
      path="production-lines"
      showInactive
      columns={codeName}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        { name: "description", label: "Описание", type: "textarea" },
        { name: "sortOrder", label: "Порядок", type: "number" },
      ]}
    />
  );
}

export function SubdivisionsAdminPage() {
  return (
    <CatalogPage
      title="Подразделения"
      path="subdivisions"
      showInactive
      columns={[
        ...codeName,
        { title: "Участок", dataIndex: "productionLineName", width: 180 },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        {
          name: "productionLineId",
          label: "Участок",
          type: "lookup",
          lookup: "production-lines",
        },
        { name: "description", label: "Описание", type: "textarea" },
      ]}
    />
  );
}

export function WarehousesAdminPage() {
  return (
    <CatalogPage
      title="Склады и зоны"
      path="warehouses"
      showInactive
      columns={[
        ...codeName,
        {
          title: "Карантин",
          dataIndex: "isQuarantineZone",
          width: 100,
          render: (v: boolean) => (v ? "да" : ""),
        },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        { name: "isQuarantineZone", label: "Зона карантина", type: "switch" },
        { name: "description", label: "Описание", type: "textarea" },
      ]}
    />
  );
}

export function CategoriesAdminPage() {
  return (
    <CatalogPage
      title="Группы номенклатуры"
      path="nomenclature-categories"
      showInactive
      columns={[
        ...codeName,
        {
          title: "Сырьё",
          dataIndex: "isRawMaterial",
          width: 90,
          render: (v: boolean) => (v ? "да" : ""),
        },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        {
          name: "isRawMaterial",
          label: "Сырьё / комплектующие",
          type: "switch",
        },
      ]}
    />
  );
}

export function QuantitiesAdminPage() {
  return (
    <CatalogPage
      title="Измеряемые величины"
      path="measured-quantities"
      showInactive
      columns={[
        ...codeName,
        { title: "Переменная", dataIndex: "variableName", width: 120 },
        { title: "Ед.", dataIndex: "unitOfMeasure", width: 80 },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        {
          name: "variableName",
          label: "Имя переменной в формуле",
          required: true,
        },
        { name: "unitOfMeasure", label: "Ед. изм." },
        { name: "precision", label: "Знаков", type: "number" },
      ]}
    />
  );
}

export function EquipmentTypesAdminPage() {
  return (
    <CatalogPage
      title="Типы средств измерений"
      path="equipment-types"
      showInactive
      columns={codeName}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        { name: "description", label: "Описание", type: "textarea" },
      ]}
    />
  );
}

export function EmployeesAdminPage() {
  return (
    <CatalogPage
      title="Сотрудники"
      path="employees"
      showInactive
      drawerWidth={520}
      columns={[
        { title: "ФИО", dataIndex: "fullName" },
        { title: "Логин", dataIndex: "userName", width: 120 },
        { title: "Должность", dataIndex: "position", width: 180 },
        { title: "Подразделение", dataIndex: "subdivisionName", width: 160 },
      ]}
      fields={[
        { name: "code", label: "Таб. № / код", required: true },
        { name: "userName", label: "Логин", required: true },
        { name: "lastName", label: "Фамилия", required: true },
        { name: "firstName", label: "Имя", required: true },
        { name: "middleName", label: "Отчество" },
        { name: "position", label: "Должность", required: true },
        {
          name: "subdivisionId",
          label: "Подразделение",
          type: "lookup",
          lookup: "subdivisions",
        },
        { name: "workSchedule", label: "График" },
      ]}
    />
  );
}

export function UsersAdminPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [open, setOpen] = useState(false);
  const [current, setCurrent] = useState<Record<string, unknown> | null>(null);
  const { data, isFetching } = useAdminUsersQuery({ page, search });
  const { data: employees } = useLookupQuery("employees");
  const [save, state] = useSaveAdminUserMutation();
  const roleOptions = useRoleOptions();
  const [form] = Form.useForm();

  return (
    <>
      <Space
        style={{
          width: "100%",
          justifyContent: "space-between",
          marginBottom: 16,
        }}
      >
        <Typography.Title level={3} style={{ margin: 0 }}>
          Учётные записи
        </Typography.Title>
        <Space>
          <Input.Search
            allowClear
            placeholder="Поиск"
            onSearch={(v) => {
              setSearch(v);
              setPage(1);
            }}
            style={{ width: 240 }}
          />
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => {
              setCurrent(null);
              form.resetFields();
              setOpen(true);
            }}
          >
            Добавить
          </Button>
        </Space>
      </Space>
      <Table
        rowKey="id"
        loading={isFetching}
        dataSource={data?.items ?? []}
        columns={[
          { title: "Логин", dataIndex: "userName", width: 140 },
          { title: "Сотрудник", dataIndex: "employeeName" },
          {
            title: "Роли",
            dataIndex: "roles",
            render: (v: string) =>
              (v ?? "")
                .split(",")
                .filter(Boolean)
                .map((r) => ruLabel(roleRu, r.trim()))
                .join(", "),
          },
          {
            title: "Блокировка",
            dataIndex: "isLocked",
            width: 110,
            render: (v: boolean) => (v ? "да" : ""),
          },
          {
            title: "",
            width: 110,
            render: (_, row) => (
              <Button
                size="small"
                onClick={() => {
                  setCurrent(row);
                  form.setFieldsValue({
                    ...row,
                    roles: String(row.roles ?? "")
                      .split(",")
                      .map((s) => s.trim())
                      .filter(Boolean),
                  });
                  setOpen(true);
                }}
              >
                Изменить
              </Button>
            ),
          },
        ]}
        pagination={{
          current: page,
          total: data?.total,
          pageSize: 20,
          onChange: setPage,
          showSizeChanger: false,
        }}
      />
      <Drawer
        title={current ? "Изменить учётку" : "Новая учётка"}
        open={open}
        onClose={() => setOpen(false)}
        size={460}
        extra={
          <Button
            type="primary"
            loading={state.isLoading}
            onClick={async () => {
              const v = await form.validateFields();
              const body = {
                employeeId: v.employeeId,
                userName: v.userName,
                roles: Array.isArray(v.roles) ? v.roles.join(",") : v.roles,
                isLocked: !!v.isLocked,
              };
              await save({
                id: current ? String(current.id) : undefined,
                body,
              }).unwrap();
              message.success("Сохранено");
              setOpen(false);
            }}
          >
            Сохранить
          </Button>
        }
      >
        <Form form={form} layout="vertical">
          <Form.Item name="employeeId" label="Сотрудник" rules={[{ required: true }]}>
            <Select
              showSearch
              optionFilterProp="label"
              options={(employees ?? []).map((e) => ({
                value: e.id,
                label: `${e.code} · ${e.name}`,
              }))}
            />
          </Form.Item>
          <Form.Item
            name="userName"
            label="Логин Keycloak"
            extra="Должен совпадать с preferred_username в Keycloak"
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>
          <Form.Item name="roles" label="Роли (локальные / доменные)" rules={[{ required: true }]}>
            <Select
              mode="multiple"
              options={
                roleOptions.length
                  ? roleOptions
                  : Object.entries(roleRu).map(([value, label]) => ({
                      value,
                      label,
                    }))
              }
            />
          </Form.Item>
          <Form.Item name="isLocked" label="Заблокирована" valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </Drawer>
    </>
  );
}

export function CompetenciesAdminPage() {
  const { data, isFetching, refetch } = useCompetenciesQuery(undefined);
  const { data: employees } = useLookupQuery("employees");
  const { data: methods } = useLookupQuery("test-methods");
  const [save, state] = useSaveCompetencyMutation();
  const [remove] = useDeleteCompetencyMutation();
  const [open, setOpen] = useState(false);
  const [current, setCurrent] = useState<Record<string, unknown> | null>(null);
  const [form] = Form.useForm();

  return (
    <>
      <Space
        style={{
          width: "100%",
          justifyContent: "space-between",
          marginBottom: 16,
        }}
      >
        <Typography.Title level={3} style={{ margin: 0 }}>
          Аттестации
        </Typography.Title>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => {
            setCurrent(null);
            form.resetFields();
            setOpen(true);
          }}
        >
          Добавить
        </Button>
      </Space>
      <Table
        rowKey="id"
        loading={isFetching}
        dataSource={data ?? []}
        columns={[
          { title: "Сотрудник", dataIndex: "employeeName" },
          { title: "Метод", dataIndex: "testMethodName" },
          { title: "С", dataIndex: "issuedOn", width: 120 },
          { title: "По", dataIndex: "validUntil", width: 120 },
          { title: "Сертификат", dataIndex: "certificateNumber", width: 140 },
          {
            title: "Действует",
            dataIndex: "isValid",
            width: 100,
            render: (v: boolean) => (v ? "да" : "нет"),
          },
          {
            title: "",
            width: 180,
            render: (_, row) => (
              <Space>
                <Button
                  size="small"
                  onClick={() => {
                    setCurrent(row);
                    form.setFieldsValue({
                      ...row,
                      issuedOn: row.issuedOn ? dayjs(String(row.issuedOn)) : undefined,
                      validUntil: row.validUntil
                        ? dayjs(String(row.validUntil))
                        : undefined,
                    });
                    setOpen(true);
                  }}
                >
                  Изменить
                </Button>
                <Popconfirm
                  title="Удалить аттестацию?"
                  onConfirm={async () => {
                    await remove(String(row.id));
                    refetch();
                  }}
                >
                  <Button size="small" danger>
                    Удалить
                  </Button>
                </Popconfirm>
              </Space>
            ),
          },
        ]}
      />
      <Drawer
        title={current ? "Изменить аттестацию" : "Новая аттестация"}
        open={open}
        onClose={() => setOpen(false)}
        size={440}
        extra={
          <Button
            type="primary"
            loading={state.isLoading}
            onClick={async () => {
              const v = await form.validateFields();
              await save({
                id: current ? String(current.id) : undefined,
                body: {
                  employeeId: v.employeeId,
                  testMethodId: v.testMethodId,
                  issuedOn: v.issuedOn?.format?.("YYYY-MM-DD") ?? v.issuedOn,
                  validUntil: v.validUntil?.format?.("YYYY-MM-DD") ?? v.validUntil,
                  certificateNumber: v.certificateNumber,
                },
              }).unwrap();
              message.success("Сохранено");
              setOpen(false);
              refetch();
            }}
          >
            Сохранить
          </Button>
        }
      >
        <Form form={form} layout="vertical">
          <Form.Item name="employeeId" label="Сотрудник" rules={[{ required: true }]}>
            <Select
              showSearch
              optionFilterProp="label"
              options={(employees ?? []).map((e) => ({
                value: e.id,
                label: `${e.code} · ${e.name}`,
              }))}
            />
          </Form.Item>
          <Form.Item name="testMethodId" label="Метод" rules={[{ required: true }]}>
            <Select
              showSearch
              optionFilterProp="label"
              options={(methods ?? []).map((e) => ({
                value: e.id,
                label: `${e.code} · ${e.name}`,
              }))}
            />
          </Form.Item>
          <Form.Item name="issuedOn" label="Выдана" rules={[{ required: true }]}>
            <DatePicker style={{ width: "100%" }} />
          </Form.Item>
          <Form.Item name="validUntil" label="Действует до" rules={[{ required: true }]}>
            <DatePicker style={{ width: "100%" }} />
          </Form.Item>
          <Form.Item name="certificateNumber" label="№ сертификата">
            <Input />
          </Form.Item>
        </Form>
      </Drawer>
    </>
  );
}

export function TemplatesAdminPage() {
  const kindOptions = useMemo(
    () =>
      Object.entries(documentKindRu).map(([value, label]) => ({
        value,
        label,
      })),
    [],
  );
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
          width: 150,
          render: (v: string) => ruLabel(controlStageRu, v),
        },
        { title: "Участок", dataIndex: "productionLineCode", width: 90 },
      ]}
      fields={[
        { name: "code", label: "Код", required: true },
        { name: "name", label: "Наименование", required: true },
        {
          name: "kind",
          label: "Вид документа",
          type: "select",
          options: kindOptions,
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
        { name: "productionLineCode", label: "Код участка (STM/FSM/IM/RC)" },
        {
          name: "performerRole",
          label: "Роль исполнителя",
          type: "select",
          options: Object.entries(roleRu).map(([value, label]) => ({
            value,
            label,
          })),
        },
        {
          name: "approverRole",
          label: "Роль утверждающего",
          type: "select",
          options: Object.entries(roleRu).map(([value, label]) => ({
            value,
            label,
          })),
        },
        {
          name: "requiresQualityInspector",
          label: "Требуется ОТК",
          type: "switch",
        },
        {
          name: "signatureKind",
          label: "Вид подписи",
          type: "select",
          options: [
            { value: "Simple", label: "Простая" },
            { value: "Qualified", label: "УКЭП" },
          ],
        },
        {
          name: "createdOnNonconformance",
          label: "Создавать при НС",
          type: "switch",
        },
      ]}
    />
  );
}

export function AuditAdminPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const { data, isFetching } = useAdminAuditQuery({ page, search });
  return (
    <>
      <Space
        style={{
          width: "100%",
          justifyContent: "space-between",
          marginBottom: 16,
        }}
      >
        <Typography.Title level={3} style={{ margin: 0 }}>
          Журнал аудита
        </Typography.Title>
        <Input.Search
          allowClear
          placeholder="Поиск"
          onSearch={(v) => {
            setSearch(v);
            setPage(1);
          }}
          style={{ width: 280 }}
        />
      </Space>
      <Table
        rowKey="id"
        loading={isFetching}
        size="small"
        dataSource={data?.items ?? []}
        columns={[
          {
            title: "Когда",
            dataIndex: "occurredAt",
            width: 180,
            render: (v: string) => v?.replace("T", " ").slice(0, 19),
          },
          { title: "Кто", dataIndex: "userName", width: 120 },
          { title: "Сущность", dataIndex: "entityType", width: 160 },
          { title: "Id", dataIndex: "entityId", width: 280, ellipsis: true },
          { title: "Действие", dataIndex: "action", width: 100 },
          { title: "Изменения", dataIndex: "changes", ellipsis: true },
        ]}
        pagination={{
          current: page,
          total: data?.total,
          pageSize: 20,
          onChange: setPage,
          showSizeChanger: false,
        }}
      />
    </>
  );
}

function BatchAdminTable({ kind }: { kind: "material" | "product" }) {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const material = useAdminMaterialBatchesQuery(
    { page, search },
    { skip: kind !== "material" },
  );
  const product = useAdminProductBatchesQuery(
    { page, search },
    { skip: kind !== "product" },
  );
  const { data: warehouses } = useLookupQuery("warehouses");
  const data = kind === "material" ? material.data : product.data;
  const isFetching = kind === "material" ? material.isFetching : product.isFetching;
  const [updateMaterial] = useUpdateAdminMaterialBatchMutation();
  const [updateProduct] = useUpdateAdminProductBatchMutation();
  const [open, setOpen] = useState(false);
  const [current, setCurrent] = useState<Record<string, unknown> | null>(null);
  const [form] = Form.useForm();

  return (
    <>
      <Space
        style={{
          width: "100%",
          justifyContent: "space-between",
          marginBottom: 16,
        }}
      >
        <Typography.Title level={3} style={{ margin: 0 }}>
          {kind === "material"
            ? "Корректировка партий сырья"
            : "Корректировка производственных партий"}
        </Typography.Title>
        <Input.Search
          allowClear
          placeholder="Поиск"
          onSearch={(v) => {
            setSearch(v);
            setPage(1);
          }}
          style={{ width: 260 }}
        />
      </Space>
      <Table
        rowKey="id"
        loading={isFetching}
        dataSource={data?.items ?? []}
        columns={[
          { title: "Номер", dataIndex: "number", width: 140 },
          { title: "Номенклатура", dataIndex: "nomenclatureName" },
          {
            title: "Статус",
            dataIndex: "status",
            width: 160,
            render: renderBatchStatus,
          },
          ...(kind === "material"
            ? [
                {
                  title: "Решение",
                  dataIndex: "decision",
                  width: 160,
                  render: (v: string) => ruLabel(batchDecisionRu, v),
                },
              ]
            : []),
          {
            title: "",
            width: 110,
            render: (_: unknown, row: Record<string, unknown>) => (
              <Button
                size="small"
                onClick={() => {
                  setCurrent(row);
                  form.setFieldsValue(row);
                  setOpen(true);
                }}
              >
                Изменить
              </Button>
            ),
          },
        ]}
        pagination={{
          current: page,
          total: data?.total,
          pageSize: 20,
          onChange: setPage,
          showSizeChanger: false,
        }}
      />
      <Drawer
        title={`Партия ${current?.number ?? ""}`}
        open={open}
        onClose={() => setOpen(false)}
        size={420}
        extra={
          <Button
            type="primary"
            onClick={async () => {
              const v = await form.validateFields();
              if (!current) return;
              if (kind === "material")
                await updateMaterial({
                  id: String(current.id),
                  body: v,
                }).unwrap();
              else
                await updateProduct({
                  id: String(current.id),
                  body: v,
                }).unwrap();
              message.success("Статус обновлён");
              setOpen(false);
            }}
          >
            Сохранить
          </Button>
        }
      >
        <Form form={form} layout="vertical">
          <Form.Item name="status" label="Статус" rules={[{ required: true }]}>
            <Select
              options={Object.entries(batchStatusRu).map(([value, label]) => ({
                value,
                label,
              }))}
            />
          </Form.Item>
          {kind === "material" && (
            <>
              <Form.Item name="decision" label="Решение">
                <Select
                  options={Object.entries(batchDecisionRu).map(([value, label]) => ({
                    value,
                    label,
                  }))}
                />
              </Form.Item>
              <Form.Item name="decisionComment" label="Комментарий">
                <Input.TextArea rows={3} />
              </Form.Item>
              <Form.Item name="warehouseId" label="Склад">
                <Select
                  allowClear
                  showSearch
                  optionFilterProp="label"
                  options={(warehouses ?? []).map((w) => ({
                    value: w.id,
                    label: `${w.code} · ${w.name}`,
                  }))}
                />
              </Form.Item>
            </>
          )}
        </Form>
      </Drawer>
    </>
  );
}

export function MaterialBatchesAdminPage() {
  return <BatchAdminTable kind="material" />;
}

export function ProductBatchesAdminPage() {
  return <BatchAdminTable kind="product" />;
}
