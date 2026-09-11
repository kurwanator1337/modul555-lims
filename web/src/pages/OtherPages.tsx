import { useState } from "react";
import {
  Button,
  Card,
  Col,
  DatePicker,
  Descriptions,
  Form,
  Input,
  Row,
  Select,
  Space,
  Statistic,
  Table,
  Typography,
} from "antd";
import {
  useAddActionMutation,
  useCompetenciesQuery,
  useDashboardQuery,
  useDecideNcMutation,
  useIncomingJournalQuery,
  useLinkTraceMutation,
  useListQuery,
  useLookupQuery,
  usePassportQuery,
  useRegisterEventMutation,
  useRemindersQuery,
} from "../api/endpoints";
import {
  renderActionStatus,
  renderBatchStatus,
  renderBatchStatusText,
  renderEquipmentStatus,
  renderNonconformanceStatus,
  renderStage,
  renderVerdict,
} from "../components/StatusTag";
import { brand, verdictColors } from "../theme";

export function DashboardPage() {
  const { data } = useDashboardQuery();
  const { data: reminders } = useRemindersQuery();
  return (
    <Space orientation="vertical" size={16} style={{ width: "100%" }}>
      <Typography.Title level={3} style={{ margin: 0 }}>
        Сводка лаборатории
      </Typography.Title>
      <Row gutter={16}>
        <Col span={6}>
          <Card>
            <Statistic
              title="Ожидают контроля"
              value={Number(data?.batchesAwaiting ?? 0)}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="На контроле"
              value={Number(data?.batchesInControl ?? 0)}
              styles={{ content: { color: brand.skyBlue } }}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="В карантине"
              value={Number(data?.batchesQuarantine ?? 0)}
              styles={{ content: { color: verdictColors.notConforms } }}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="Открытые несоответствия"
              value={Number(data?.openNonconformances ?? 0)}
            />
          </Card>
        </Col>
      </Row>
      <Row gutter={16}>
        <Col span={8}>
          <Card>
            <Statistic
              title="Допущено партий"
              value={Number(data?.batchesApproved ?? 0)}
              styles={{ content: { color: verdictColors.conforms } }}
            />
          </Card>
        </Col>
        <Col span={8}>
          <Card>
            <Statistic
              title="Поверка истекает ≤ 30 дней"
              value={Number(data?.verificationDueSoon ?? 0)}
            />
          </Card>
        </Col>
        <Col span={8}>
          <Card>
            <Statistic
              title="СИ непригодны"
              value={Number(data?.verificationExpired ?? 0)}
            />
          </Card>
        </Col>
      </Row>
      <Card title="Напоминания о поверке">
        <Table
          rowKey="id"
          size="small"
          pagination={false}
          dataSource={reminders ?? []}
          columns={[
            { title: "Прибор", dataIndex: "name" },
            { title: "Тип", dataIndex: "equipmentTypeName" },
            { title: "Поверка до", dataIndex: "verificationValidUntil" },
            { title: "Дней", dataIndex: "daysToVerification" },
          ]}
        />
      </Card>
    </Space>
  );
}

export function IncomingJournalPage() {
  const { data, isFetching } = useIncomingJournalQuery();
  return (
    <>
      <Typography.Title level={3}>Журнал входного контроля</Typography.Title>
      <Table
        rowKey={(r) => String(r.batchNumber)}
        loading={isFetching}
        dataSource={data ?? []}
        columns={[
          { title: "Дата", dataIndex: "arrivalDate", width: 120 },
          { title: "Партия", dataIndex: "batchNumber", width: 140 },
          { title: "Материал", dataIndex: "material" },
          { title: "Поставщик", dataIndex: "supplier" },
          {
            title: "Статус",
            dataIndex: "status",
            width: 160,
            render: renderBatchStatus,
          },
          { title: "Протокол", dataIndex: "protocolNumber", width: 140 },
          {
            title: "Вердикт",
            dataIndex: "verdict",
            width: 160,
            render: renderVerdict,
          },
          { title: "Исполнитель", dataIndex: "performer" },
        ]}
      />
    </>
  );
}

export function NonconformancesPage() {
  const { data, isFetching } = useListQuery({
    path: "nonconformances",
    page: 1,
  });
  const { data: causes } = useLookupQuery("causes");
  const [decide] = useDecideNcMutation();
  const [add] = useAddActionMutation();
  return (
    <>
      <Typography.Title level={3}>Журнал несоответствий</Typography.Title>
      <Table
        rowKey="id"
        loading={isFetching}
        expandable={{
          expandedRowRender: (r) => (
            <Space orientation="vertical" style={{ width: "100%" }}>
              <Space>
                <Select
                  placeholder="Причина"
                  style={{ width: 260 }}
                  options={causes?.map((c) => ({ value: c.id, label: c.name }))}
                  onChange={(causeId) =>
                    decide({
                      id: String(r.id),
                      body: { causeId, decision: "ReturnToSupplier" },
                    })
                  }
                />
                <Button
                  onClick={() =>
                    decide({
                      id: String(r.id),
                      body: { decision: "ReturnToSupplier" },
                    })
                  }
                >
                  Возврат поставщику
                </Button>
                <Button
                  onClick={() =>
                    decide({
                      id: String(r.id),
                      body: { decision: "RestrictedUse" },
                    })
                  }
                >
                  С ограничениями
                </Button>
                <Button
                  onClick={() =>
                    decide({ id: String(r.id), body: { decision: "WriteOff" } })
                  }
                >
                  Списание
                </Button>
                <Button
                  onClick={() =>
                    add({
                      id: String(r.id),
                      body: {
                        description: "Повторное испытание / дополнительная выдержка",
                      },
                    })
                  }
                >
                  Карта действий
                </Button>
              </Space>
              <Table
                size="small"
                pagination={false}
                dataSource={(r.actions as unknown[]) ?? []}
                rowKey="id"
                columns={[
                  { title: "Действие", dataIndex: "description" },
                  {
                    title: "Статус",
                    dataIndex: "status",
                    render: renderActionStatus,
                  },
                ]}
              />
            </Space>
          ),
        }}
        dataSource={data?.items ?? []}
        columns={[
          { title: "Номер", dataIndex: "number", width: 140 },
          {
            title: "Этап",
            dataIndex: "stage",
            width: 180,
            render: renderStage,
          },
          { title: "Партия", dataIndex: "materialBatchNumber" },
          { title: "Показатель", dataIndex: "parameterName" },
          { title: "Факт", dataIndex: "actualValue", width: 90 },
          { title: "Норматив", dataIndex: "normValue" },
          {
            title: "Статус",
            dataIndex: "status",
            width: 180,
            render: renderNonconformanceStatus,
          },
        ]}
      />
    </>
  );
}

export function MetrologyPage() {
  const { data, isFetching } = useListQuery({ path: "equipment", page: 1 });
  const { data: reminders } = useRemindersQuery();
  const [register] = useRegisterEventMutation();
  const [form] = Form.useForm();
  return (
    <Space orientation="vertical" size={16} style={{ width: "100%" }}>
      <Typography.Title level={3} style={{ margin: 0 }}>
        Метрология — график поверок
      </Typography.Title>
      <Card title="Истекает в ближайшие 30 дней">
        <Table
          rowKey="id"
          size="small"
          pagination={false}
          dataSource={reminders ?? []}
          columns={[
            { title: "Прибор", dataIndex: "name" },
            { title: "Поверка до", dataIndex: "verificationValidUntil" },
            { title: "Дней", dataIndex: "daysToVerification" },
          ]}
        />
      </Card>
      <Card title="Реестр средств измерений">
        <Table
          rowKey="id"
          loading={isFetching}
          dataSource={data?.items ?? []}
          columns={[
            { title: "Наименование", dataIndex: "name" },
            { title: "Тип", dataIndex: "equipmentTypeName" },
            { title: "Инв. №", dataIndex: "inventoryNumber" },
            { title: "Поверка до", dataIndex: "verificationValidUntil" },
            {
              title: "Статус",
              dataIndex: "status",
              render: renderEquipmentStatus,
            },
          ]}
        />
      </Card>
      <Card title="Зарегистрировать поверку / ТО">
        <Form
          form={form}
          layout="inline"
          onFinish={async (v) => {
            await register({
              ...v,
              eventDate: v.eventDate?.format?.("YYYY-MM-DD"),
              validUntil: v.validUntil?.format?.("YYYY-MM-DD"),
            }).unwrap();
            form.resetFields();
          }}
        >
          <Form.Item name="equipmentId" rules={[{ required: true }]}>
            <Select
              placeholder="Прибор"
              style={{ width: 260 }}
              options={(data?.items ?? []).map((e) => ({
                value: e.id,
                label: e.name as string,
              }))}
            />
          </Form.Item>
          <Form.Item name="kind" initialValue="Verification">
            <Select
              style={{ width: 180 }}
              options={[
                { value: "Verification", label: "Поверка" },
                { value: "Calibration", label: "Калибровка" },
                { value: "Maintenance", label: "ТО" },
                { value: "Repair", label: "Ремонт" },
              ]}
            />
          </Form.Item>
          <Form.Item name="eventDate">
            <DatePicker />
          </Form.Item>
          <Form.Item name="validUntil">
            <DatePicker placeholder="Действует до" />
          </Form.Item>
          <Form.Item name="certificateNumber">
            <Input placeholder="№ свидетельства" />
          </Form.Item>
          <Button type="primary" htmlType="submit">
            Записать
          </Button>
        </Form>
      </Card>
    </Space>
  );
}

export function PersonnelPage() {
  const { data } = useListQuery({ path: "employees", page: 1 });
  const { data: competencies } = useCompetenciesQuery(undefined);
  return (
    <Space orientation="vertical" size={16} style={{ width: "100%" }}>
      <Typography.Title level={3} style={{ margin: 0 }}>
        Персонал лаборатории
      </Typography.Title>
      <Table
        rowKey="id"
        dataSource={data?.items ?? []}
        columns={[
          { title: "ФИО", dataIndex: "fullName" },
          { title: "Должность", dataIndex: "position" },
          { title: "Подразделение", dataIndex: "subdivisionName" },
          { title: "График", dataIndex: "workSchedule" },
        ]}
      />
      <Card title="Аттестации по методам">
        <Table
          rowKey="id"
          size="small"
          dataSource={competencies ?? []}
          columns={[
            { title: "Сотрудник", dataIndex: "employeeName" },
            { title: "Метод", dataIndex: "testMethodName" },
            { title: "С", dataIndex: "issuedOn" },
            { title: "По", dataIndex: "validUntil" },
            {
              title: "Действует",
              dataIndex: "isValid",
              render: (v: boolean) => (v ? "да" : "нет"),
            },
          ]}
        />
      </Card>
    </Space>
  );
}

export function PassportPage() {
  const { data: products } = useListQuery({ path: "product-batches", page: 1 });
  const { data: materials } = useListQuery({
    path: "material-batches",
    page: 1,
  });
  const [productId, setProductId] = useState<string>();
  const [materialId, setMaterialId] = useState<string>();
  const { data } = usePassportQuery(
    { productId, materialId },
    { skip: !productId && !materialId },
  );
  const [link] = useLinkTraceMutation();

  return (
    <Space orientation="vertical" size={16} style={{ width: "100%" }}>
      <Typography.Title level={3} style={{ margin: 0 }}>
        Цифровой паспорт — прослеживаемость
      </Typography.Title>
      <Space>
        <Select
          allowClear
          placeholder="Партия изделия"
          style={{ width: 320 }}
          options={(products?.items ?? []).map((p) => ({
            value: p.id as string,
            label: `${p.number} ${p.nomenclatureName}`,
          }))}
          onChange={setProductId}
        />
        <Select
          allowClear
          placeholder="Партия сырья"
          style={{ width: 320 }}
          options={(materials?.items ?? []).map((p) => ({
            value: p.id as string,
            label: `${p.number} ${p.nomenclatureName}`,
          }))}
          onChange={setMaterialId}
        />
        <Button
          disabled={!productId || !materialId}
          onClick={() => productId && materialId && link({ productId, materialId })}
        >
          Связать
        </Button>
      </Space>
      {data?.product ? (
        <Card title={`Изделие ${(data.product as { number?: string }).number}`}>
          <Descriptions size="small" bordered>
            <Descriptions.Item label="Номенклатура">
              {(data.product as { nomenclatureName?: string }).nomenclatureName}
            </Descriptions.Item>
            <Descriptions.Item label="Статус">
              {renderBatchStatusText((data.product as { status?: string }).status)}
            </Descriptions.Item>
          </Descriptions>
          <Typography.Paragraph style={{ marginTop: 12 }}>
            Сырьё в составе:
          </Typography.Paragraph>
          <Table
            size="small"
            pagination={false}
            rowKey="id"
            dataSource={(data.materials as unknown[]) ?? []}
            columns={[
              { title: "Партия", dataIndex: "materialBatchNumber" },
              { title: "Материал", dataIndex: "materialName" },
              { title: "Кол-во", dataIndex: "quantity" },
            ]}
          />
        </Card>
      ) : null}
      {data?.material ? (
        <Card title={`Сырьё ${(data.material as { number?: string }).number}`}>
          <Descriptions size="small" bordered>
            <Descriptions.Item label="Номенклатура">
              {(data.material as { nomenclatureName?: string }).nomenclatureName}
            </Descriptions.Item>
            <Descriptions.Item label="Поставщик">
              {(data.material as { supplierName?: string }).supplierName}
            </Descriptions.Item>
            <Descriptions.Item label="Статус">
              {renderBatchStatusText((data.material as { status?: string }).status)}
            </Descriptions.Item>
          </Descriptions>
          <Typography.Paragraph style={{ marginTop: 12 }}>
            Изделия, в которых использовано:
          </Typography.Paragraph>
          <Table
            size="small"
            pagination={false}
            rowKey="id"
            dataSource={(data.products as unknown[]) ?? []}
            columns={[
              { title: "Партия", dataIndex: "productBatchNumber" },
              { title: "Изделие", dataIndex: "productName" },
            ]}
          />
        </Card>
      ) : null}
    </Space>
  );
}
