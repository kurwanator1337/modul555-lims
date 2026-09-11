import {
  Button,
  Card,
  Drawer,
  Form,
  Input,
  InputNumber,
  Select,
  Space,
  Table,
  Typography,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import {
  useGetProgramQuery,
  useListQuery,
  useLookupQuery,
  useSaveProgramMutation,
} from "../api/endpoints";
import { controlKindRu, label } from "../i18n/labels";
import { useState } from "react";

export default function ProgramsPage() {
  const [page, setPage] = useState(1);
  const [openId, setOpenId] = useState<string | "new" | null>(null);
  const { data, isFetching } = useListQuery({ path: "control-programs", page });
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
          Программы испытаний
        </Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setOpenId("new")}>
          Новая программа
        </Button>
      </Space>
      <Table
        rowKey="id"
        loading={isFetching}
        dataSource={data?.items ?? []}
        columns={[
          { title: "Код", dataIndex: "code", width: 140 },
          { title: "Наименование", dataIndex: "name" },
          { title: "Номенклатура", dataIndex: "nomenclatureName" },
          {
            title: "Вид контроля",
            dataIndex: "controlKind",
            width: 150,
            render: (v: string) => label(controlKindRu, v),
          },
          { title: "Участок", dataIndex: "productionLineName", width: 220 },
          { title: "Показателей", dataIndex: "parameterCount", width: 120 },
          {
            title: "",
            width: 110,
            render: (_, r) => (
              <Button size="small" onClick={() => setOpenId(String(r.id))}>
                Открыть
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
      <ProgramEditor id={openId} onClose={() => setOpenId(null)} />
    </>
  );
}

function ProgramEditor({
  id,
  onClose,
}: {
  id: string | "new" | null;
  onClose: () => void;
}) {
  const isNew = id === "new";
  const { data } = useGetProgramQuery(id && !isNew ? id : "", {
    skip: !id || isNew,
  });
  const { data: nomenclatures } = useLookupQuery("nomenclatures");
  const { data: lines } = useLookupQuery("production-lines");
  const { data: standards } = useLookupQuery("standards");
  const { data: parameters } = useLookupQuery("quality-parameters");
  const { data: methods } = useLookupQuery("test-methods");
  const [save, state] = useSaveProgramMutation();
  const [form] = Form.useForm();
  const [rows, setRows] = useState<Record<string, unknown>[]>([]);

  const open = Boolean(id);
  const loaded = data && !isNew;

  return (
    <Drawer
      size={980}
      open={open}
      onClose={onClose}
      title={isNew ? "Новая программа испытаний" : (data?.name as string)}
      extra={
        <Button
          type="primary"
          loading={state.isLoading}
          onClick={async () => {
            const values = await form.validateFields();
            await save({
              id: isNew ? undefined : String(id),
              body: { ...values, isActive: true, parameters: rows },
            }).unwrap();
            onClose();
          }}
        >
          Сохранить
        </Button>
      }
    >
      {open && (
        <Form
          form={form}
          layout="vertical"
          key={String(id) + (loaded ? "l" : "n")}
          initialValues={
            loaded ? data : { controlKind: "Incoming", isApproved: true, parameters: [] }
          }
          onValuesChange={(_, all) => {
            if (all.parameters) setRows(all.parameters);
          }}
        >
          <Space size={12} style={{ display: "flex" }} align="start">
            <Form.Item
              name="code"
              label="Код"
              rules={[{ required: true }]}
              style={{ width: 180 }}
            >
              <Input />
            </Form.Item>
            <Form.Item
              name="name"
              label="Наименование"
              rules={[{ required: true }]}
              style={{ flex: 1 }}
            >
              <Input />
            </Form.Item>
          </Space>
          <Space size={12} style={{ display: "flex" }} wrap>
            <Form.Item
              name="nomenclatureId"
              label="Номенклатура"
              rules={[{ required: true }]}
              style={{ width: 280 }}
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
            <Form.Item name="controlKind" label="Вид контроля" style={{ width: 200 }}>
              <Select
                options={[
                  { value: "Incoming", label: "Входной" },
                  { value: "Operational", label: "Операционный" },
                  { value: "Acceptance", label: "Приёмочный" },
                ]}
              />
            </Form.Item>
            <Form.Item name="productionLineId" label="Участок" style={{ width: 260 }}>
              <Select
                allowClear
                options={lines?.map((x) => ({ value: x.id, label: x.name }))}
              />
            </Form.Item>
            <Form.Item name="standardDocumentId" label="Норматив" style={{ width: 220 }}>
              <Select
                allowClear
                showSearch
                optionFilterProp="label"
                options={standards?.map((x) => ({
                  value: x.id,
                  label: x.code,
                }))}
              />
            </Form.Item>
          </Space>
          <Card
            size="small"
            title="Показатели, нормативы и методы"
            extra={
              <Button
                size="small"
                onClick={() => {
                  const next = [
                    ...(form.getFieldValue("parameters") ?? []),
                    {
                      replicates: 1,
                      aggregation: "Average",
                      frequency: "EveryBatch",
                      isMandatory: true,
                      sortOrder: rows.length,
                      normKind: "Min",
                    },
                  ];
                  form.setFieldValue("parameters", next);
                  setRows(next);
                }}
              >
                Добавить показатель
              </Button>
            }
          >
            <Form.List name="parameters">
              {(fields, { remove }) => (
                <Table
                  size="small"
                  pagination={false}
                  rowKey={(r) => String(r.name)}
                  dataSource={fields}
                  columns={[
                    {
                      title: "Показатель",
                      render: (_, f) => (
                        <Form.Item
                          name={[f.name, "qualityParameterId"]}
                          noStyle
                          rules={[{ required: true }]}
                        >
                          <Select
                            showSearch
                            optionFilterProp="label"
                            style={{ minWidth: 200 }}
                            options={parameters?.map((x) => ({
                              value: x.id,
                              label: x.name,
                            }))}
                          />
                        </Form.Item>
                      ),
                    },
                    {
                      title: "Метод",
                      render: (_, f) => (
                        <Form.Item name={[f.name, "testMethodId"]} noStyle>
                          <Select
                            allowClear
                            showSearch
                            optionFilterProp="label"
                            style={{ minWidth: 180 }}
                            options={methods?.map((x) => ({
                              value: x.id,
                              label: x.name,
                            }))}
                          />
                        </Form.Item>
                      ),
                    },
                    {
                      title: "Норматив",
                      width: 140,
                      render: (_, f) => (
                        <Form.Item name={[f.name, "normKind"]} noStyle>
                          <Select
                            options={[
                              "Min",
                              "Max",
                              "Range",
                              "Nominal",
                              "PercentOfDesign",
                              "MustBeTrue",
                              "Expert",
                            ].map((v) => ({ value: v, label: v }))}
                          />
                        </Form.Item>
                      ),
                    },
                    {
                      title: "Min / %",
                      width: 90,
                      render: (_, f) => (
                        <Form.Item name={[f.name, "normMin"]} noStyle>
                          <InputNumber />
                        </Form.Item>
                      ),
                    },
                    {
                      title: "Max / допуск",
                      width: 100,
                      render: (_, f) => (
                        <Form.Item name={[f.name, "normMax"]} noStyle>
                          <InputNumber />
                        </Form.Item>
                      ),
                    },
                    {
                      title: "Повт.",
                      width: 70,
                      render: (_, f) => (
                        <Form.Item name={[f.name, "replicates"]} noStyle>
                          <InputNumber min={1} />
                        </Form.Item>
                      ),
                    },
                    {
                      title: "",
                      width: 50,
                      render: (_, f) => (
                        <Button size="small" danger onClick={() => remove(f.name)}>
                          ×
                        </Button>
                      ),
                    },
                  ]}
                />
              )}
            </Form.List>
          </Card>
        </Form>
      )}
    </Drawer>
  );
}
