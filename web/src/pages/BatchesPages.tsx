import { useState } from "react";
import {
  Button,
  DatePicker,
  Drawer,
  Form,
  Input,
  InputNumber,
  Select,
  Space,
  Table,
  Typography,
} from "antd";
import { useNavigate } from "react-router-dom";
import dayjs from "dayjs";
import {
  useCreateMutation,
  useIncomingOrderMutation,
  useListQuery,
  useLookupQuery,
  useOperationalOrderMutation,
} from "../api/endpoints";
import { renderBatchStatus, renderOrderStatus } from "../components/StatusTag";

export function IncomingBatchesPage() {
  const [page, setPage] = useState(1);
  const [open, setOpen] = useState(false);
  const { data, isFetching } = useListQuery({ path: "material-batches", page });
  const { data: nomenclatures } = useLookupQuery("nomenclatures");
  const { data: suppliers } = useLookupQuery("suppliers");
  const { data: warehouses } = useLookupQuery("warehouses");
  const [create, c] = useCreateMutation();
  const [start] = useIncomingOrderMutation();
  const [form] = Form.useForm();
  const navigate = useNavigate();

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
          Партии сырья — входной контроль
        </Typography.Title>
        <Button
          type="primary"
          onClick={() => {
            form.resetFields();
            setOpen(true);
          }}
        >
          Приход партии
        </Button>
      </Space>
      <Table
        rowKey="id"
        loading={isFetching}
        dataSource={data?.items ?? []}
        columns={[
          { title: "Номер", dataIndex: "number", width: 140 },
          { title: "Материал", dataIndex: "nomenclatureName" },
          { title: "Поставщик", dataIndex: "supplierName" },
          { title: "Дата", dataIndex: "arrivalDate", width: 120 },
          { title: "Кол-во", dataIndex: "quantity", width: 90 },
          {
            title: "Статус",
            dataIndex: "status",
            width: 160,
            render: renderBatchStatus,
          },
          {
            title: "",
            width: 220,
            render: (_, r) => (
              <Space>
                <Button
                  size="small"
                  onClick={async () => {
                    const order = await start({
                      materialBatchId: r.id,
                    }).unwrap();
                    navigate(`/orders/${order.id}`);
                  }}
                >
                  На контроль
                </Button>
              </Space>
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
        title="Приход партии сырья"
        open={open}
        onClose={() => setOpen(false)}
        size={420}
        extra={
          <Button
            type="primary"
            loading={c.isLoading}
            onClick={async () => {
              const v = await form.validateFields();
              await create({
                path: "material-batches",
                body: {
                  ...v,
                  arrivalDate: v.arrivalDate?.format?.("YYYY-MM-DD") ?? v.arrivalDate,
                },
              }).unwrap();
              setOpen(false);
            }}
          >
            Сохранить
          </Button>
        }
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ arrivalDate: dayjs(), quantity: 1 }}
        >
          <Form.Item
            name="nomenclatureId"
            label="Номенклатура"
            rules={[{ required: true }]}
          >
            <Select
              showSearch
              optionFilterProp="label"
              options={nomenclatures?.map((x) => ({
                value: x.id,
                label: x.name,
              }))}
            />
          </Form.Item>
          <Form.Item name="supplierId" label="Поставщик" rules={[{ required: true }]}>
            <Select
              showSearch
              optionFilterProp="label"
              options={suppliers?.map((x) => ({ value: x.id, label: x.name }))}
            />
          </Form.Item>
          <Form.Item name="arrivalDate" label="Дата прихода">
            <DatePicker style={{ width: "100%" }} />
          </Form.Item>
          <Form.Item name="quantity" label="Количество">
            <InputNumber style={{ width: "100%" }} />
          </Form.Item>
          <Form.Item name="quantityUnit" label="Ед.">
            <Input />
          </Form.Item>
          <Form.Item name="supplierDocumentNumber" label="Паспорт / сертификат">
            <Input />
          </Form.Item>
          <Form.Item name="warehouseId" label="Склад">
            <Select
              allowClear
              options={warehouses?.map((x) => ({ value: x.id, label: x.name }))}
            />
          </Form.Item>
        </Form>
      </Drawer>
    </>
  );
}

export function ProductBatchesPage() {
  const [page, setPage] = useState(1);
  const [open, setOpen] = useState(false);
  const { data, isFetching } = useListQuery({ path: "product-batches", page });
  const { data: nomenclatures } = useLookupQuery("nomenclatures");
  const { data: lines } = useLookupQuery("production-lines");
  const { data: programList } = useListQuery({
    path: "control-programs",
    page: 1,
  });
  const [create, c] = useCreateMutation();
  const [start] = useOperationalOrderMutation();
  const [form] = Form.useForm();
  const navigate = useNavigate();

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
          Производственные партии
        </Typography.Title>
        <Button
          type="primary"
          onClick={() => {
            form.resetFields();
            setOpen(true);
          }}
        >
          Новая партия
        </Button>
      </Space>
      <Table
        rowKey="id"
        loading={isFetching}
        dataSource={data?.items ?? []}
        columns={[
          { title: "Номер", dataIndex: "number", width: 140 },
          { title: "Продукция", dataIndex: "nomenclatureName" },
          { title: "Участок", dataIndex: "productionLineName" },
          { title: "Дата", dataIndex: "productionDate", width: 120 },
          {
            title: "Статус",
            dataIndex: "status",
            width: 160,
            render: renderBatchStatus,
          },
          {
            title: "",
            width: 220,
            render: (_, r) => (
              <Button
                size="small"
                onClick={() => {
                  const program = (programList?.items ?? []).find(
                    (p) =>
                      p.nomenclatureId === r.nomenclatureId &&
                      p.controlKind === "Operational",
                  );
                  if (!program) return;
                  start({ productBatchId: r.id, controlProgramId: program.id })
                    .unwrap()
                    .then((order) => navigate(`/orders/${order.id}`));
                }}
              >
                На контроль
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
        title="Производственная партия"
        open={open}
        onClose={() => setOpen(false)}
        size={420}
        extra={
          <Button
            type="primary"
            loading={c.isLoading}
            onClick={async () => {
              const v = await form.validateFields();
              await create({
                path: "product-batches",
                body: {
                  ...v,
                  productionDate:
                    v.productionDate?.format?.("YYYY-MM-DD") ?? v.productionDate,
                },
              }).unwrap();
              setOpen(false);
            }}
          >
            Сохранить
          </Button>
        }
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ productionDate: dayjs(), kind: "FinishedGoods" }}
        >
          <Form.Item
            name="nomenclatureId"
            label="Номенклатура"
            rules={[{ required: true }]}
          >
            <Select
              showSearch
              optionFilterProp="label"
              options={nomenclatures?.map((x) => ({
                value: x.id,
                label: x.name,
              }))}
            />
          </Form.Item>
          <Form.Item name="productionLineId" label="Участок">
            <Select options={lines?.map((x) => ({ value: x.id, label: x.name }))} />
          </Form.Item>
          <Form.Item name="kind" label="Вид">
            <Select
              options={[
                { value: "Mix", label: "Замес смеси" },
                { value: "FinishedGoods", label: "Готовые изделия" },
                { value: "RebarAssembly", label: "Каркасы" },
                { value: "Module", label: "Модуль СТМ/ИМ" },
              ]}
            />
          </Form.Item>
          <Form.Item name="productionDate" label="Дата">
            <DatePicker style={{ width: "100%" }} />
          </Form.Item>
          <Form.Item name="designValue" label="Проектное значение">
            <InputNumber style={{ width: "100%" }} />
          </Form.Item>
        </Form>
      </Drawer>
    </>
  );
}

export function OrdersPage({ kind }: { kind: "Incoming" | "Operational" }) {
  const [page, setPage] = useState(1);
  const { data, isFetching } = useListQuery({
    path: "control-orders",
    page,
    extra: `kind=${kind}`,
  });
  const navigate = useNavigate();
  return (
    <>
      <Typography.Title level={3}>
        {kind === "Incoming"
          ? "Распоряжения входного контроля"
          : "Распоряжения операционного контроля"}
      </Typography.Title>
      <Table
        rowKey="id"
        loading={isFetching}
        dataSource={data?.items ?? []}
        onRow={(r) => ({
          onClick: () => navigate(`/orders/${r.id}`),
          style: { cursor: "pointer" },
        })}
        columns={[
          { title: "Номер", dataIndex: "number", width: 140 },
          { title: "Программа", dataIndex: "controlProgramName" },
          {
            title: "Партия",
            dataIndex: kind === "Incoming" ? "materialBatchNumber" : "productBatchNumber",
            width: 140,
          },
          {
            title: "Статус",
            dataIndex: "status",
            width: 140,
            render: renderOrderStatus,
          },
          {
            title: "Испытания",
            width: 140,
            render: (_, r) => `${r.completedTestCount}/${r.testRunCount}`,
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
    </>
  );
}
