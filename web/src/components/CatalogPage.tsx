import { useMemo, useState } from "react";
import {
  Button,
  DatePicker,
  Drawer,
  Form,
  Input,
  InputNumber,
  Popconfirm,
  Select,
  Space,
  Switch,
  Table,
  Typography,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";
import dayjs from "dayjs";
import {
  useCreateMutation,
  useListQuery,
  useLookupQuery,
  useRemoveMutation,
  useUpdateMutation,
} from "../api/endpoints";

export interface FieldDef {
  name: string;
  label: string;
  type?:
    | "text"
    | "number"
    | "textarea"
    | "switch"
    | "select"
    | "lookup"
    | "multiLookup"
    | "date"
    | "password";
  options?: { label: string; value: string | number | boolean }[];
  lookup?: string;
  required?: boolean;
  mode?: "multiple" | "tags";
}

interface Props {
  title: string;
  path: string;
  columns: ColumnsType<Record<string, unknown>>;
  fields: FieldDef[];
  creatable?: boolean;
  /** Показывать неактивные записи (для админки). */
  showInactive?: boolean;
  /** Ширина drawer. */
  drawerWidth?: number;
}

function LookupSelect({
  lookup,
  mode,
  ...rest
}: { lookup: string; mode?: "multiple" | "tags" } & Record<string, unknown>) {
  const { data, isFetching } = useLookupQuery(lookup);
  const options = useMemo(
    () =>
      (data ?? []).map((x) => ({
        value: x.id,
        label: `${x.code} · ${x.name}`,
      })),
    [data],
  );
  return (
    <Select
      showSearch
      optionFilterProp="label"
      loading={isFetching}
      options={options}
      mode={mode}
      allowClear
      {...rest}
    />
  );
}

export default function CatalogPage({
  title,
  path,
  columns,
  fields,
  creatable = true,
  showInactive = false,
  drawerWidth = 480,
}: Props) {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [open, setOpen] = useState(false);
  const [current, setCurrent] = useState<Record<string, unknown> | null>(null);
  const extra = showInactive ? "activeOnly=false" : undefined;
  const { data, isFetching } = useListQuery({ path, page, search, extra });
  const [create, createState] = useCreateMutation();
  const [update, updateState] = useUpdateMutation();
  const [remove] = useRemoveMutation();
  const [form] = Form.useForm();

  const onEdit = (row: Record<string, unknown>) => {
    setCurrent(row);
    const values: Record<string, unknown> = {
      ...row,
      isActive: row.isActive !== false,
    };
    for (const f of fields) {
      if (f.type === "date" && values[f.name])
        values[f.name] = dayjs(String(values[f.name]));
    }
    form.setFieldsValue(values);
    setOpen(true);
  };

  const save = async () => {
    const raw = await form.validateFields();
    const values: Record<string, unknown> = { ...raw };
    for (const f of fields) {
      if (f.type === "date" && values[f.name] && dayjs.isDayjs(values[f.name])) {
        values[f.name] = (values[f.name] as dayjs.Dayjs).format("YYYY-MM-DD");
      }
      if (f.type === "password" && !values[f.name]) delete values[f.name];
    }
    if (current)
      await update({
        path,
        id: String(current.id),
        body: { ...current, ...values },
      }).unwrap();
    else await create({ path, body: values }).unwrap();
    setOpen(false);
  };

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
          {title}
        </Typography.Title>
        <Space>
          <Input.Search
            allowClear
            placeholder="Поиск"
            onSearch={(v) => {
              setSearch(v);
              setPage(1);
            }}
            style={{ width: 260 }}
          />
          {creatable && (
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={() => {
                setCurrent(null);
                form.resetFields();
                form.setFieldsValue({ isActive: true });
                setOpen(true);
              }}
            >
              Добавить
            </Button>
          )}
        </Space>
      </Space>
      <Table
        rowKey="id"
        loading={isFetching}
        dataSource={(data?.items ?? []) as Record<string, unknown>[]}
        columns={[
          ...columns,
          ...(creatable
            ? [
                {
                  title: "",
                  width: 200,
                  render: (_: unknown, row: Record<string, unknown>) => (
                    <Space>
                      <Button size="small" onClick={() => onEdit(row)}>
                        Изменить
                      </Button>
                      {row.isActive !== false ? (
                        <Popconfirm
                          title="Снять с использования?"
                          onConfirm={() => remove({ path, id: String(row.id) })}
                        >
                          <Button size="small" danger>
                            Отключить
                          </Button>
                        </Popconfirm>
                      ) : (
                        <Button
                          size="small"
                          onClick={() =>
                            update({
                              path,
                              id: String(row.id),
                              body: { ...row, isActive: true },
                            })
                          }
                        >
                          Включить
                        </Button>
                      )}
                    </Space>
                  ),
                } as ColumnsType<Record<string, unknown>>[number],
              ]
            : []),
        ]}
        pagination={{
          current: page,
          pageSize: data?.pageSize ?? 20,
          total: data?.total ?? 0,
          onChange: setPage,
          showSizeChanger: false,
        }}
      />
      <Drawer
        title={current ? "Изменить запись" : "Новая запись"}
        open={open}
        onClose={() => setOpen(false)}
        size={drawerWidth}
        extra={
          <Button
            type="primary"
            loading={createState.isLoading || updateState.isLoading}
            onClick={save}
          >
            Сохранить
          </Button>
        }
      >
        <Form form={form} layout="vertical">
          {fields.map((f) => (
            <Form.Item
              key={f.name}
              name={f.name}
              label={f.label}
              valuePropName={f.type === "switch" ? "checked" : "value"}
              rules={
                f.required
                  ? [{ required: true, message: "Обязательное поле" }]
                  : undefined
              }
            >
              {f.type === "textarea" ? (
                <Input.TextArea rows={3} />
              ) : f.type === "number" ? (
                <InputNumber style={{ width: "100%" }} />
              ) : f.type === "switch" ? (
                <Switch />
              ) : f.type === "select" ? (
                <Select options={f.options} allowClear mode={f.mode} />
              ) : f.type === "lookup" && f.lookup ? (
                <LookupSelect lookup={f.lookup} />
              ) : f.type === "multiLookup" && f.lookup ? (
                <LookupSelect lookup={f.lookup} mode="multiple" />
              ) : f.type === "date" ? (
                <DatePicker style={{ width: "100%" }} />
              ) : f.type === "password" ? (
                <Input.Password
                  placeholder={current ? "Оставьте пустым, чтобы не менять" : undefined}
                />
              ) : (
                <Input />
              )}
            </Form.Item>
          ))}
          {creatable && !fields.some((f) => f.name === "isActive") && (
            <Form.Item
              name="isActive"
              label="Активна"
              valuePropName="checked"
              initialValue={true}
            >
              <Switch />
            </Form.Item>
          )}
        </Form>
      </Drawer>
    </>
  );
}
