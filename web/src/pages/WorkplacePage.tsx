import { useMemo, useState } from "react";
import {
  Button,
  Card,
  Form,
  InputNumber,
  Select,
  Space,
  Switch,
  Table,
  Typography,
  message,
} from "antd";
import {
  useAssignTestMutation,
  useCompleteTestMutation,
  useLookupQuery,
  useSaveMeasurementsMutation,
  useWorkplaceQuery,
} from "../api/endpoints";
import { renderTestRunStatus, renderVerdict } from "../components/StatusTag";

export default function WorkplacePage() {
  const { data, isFetching, refetch } = useWorkplaceQuery();
  const { data: employees } = useLookupQuery("employees");
  const { data: equipment } = useLookupQuery("equipment");
  const [currentId, setCurrentId] = useState<string | null>(null);
  const current = useMemo(
    () => (data ?? []).find((r) => r.id === currentId) ?? data?.[0],
    [data, currentId],
  );
  const [assign] = useAssignTestMutation();
  const [save] = useSaveMeasurementsMutation();
  const [complete, completing] = useCompleteTestMutation();
  const [form] = Form.useForm();

  const quantities =
    (current?.quantities as
      { id: string; name: string; unitOfMeasure?: string }[] | undefined) ?? [];
  const replicates = Number(current?.replicates ?? 1);

  return (
    <Space orientation="vertical" size={16} style={{ width: "100%" }}>
      <Typography.Title level={3} style={{ margin: 0 }}>
        АРМ лаборанта
      </Typography.Title>
      <Table
        rowKey="id"
        loading={isFetching}
        size="small"
        dataSource={data ?? []}
        pagination={false}
        rowClassName={(r) =>
          r.id === current?.id
            ? "ant-table-row-selected"
            : r.verdict === "NotConforms"
              ? "lims-row-not-conforms"
              : r.verdict === "Conforms"
                ? "lims-row-conforms"
                : ""
        }
        onRow={(r) => ({
          onClick: () => {
            setCurrentId(String(r.id));
            form.resetFields();
          },
        })}
        columns={[
          {
            title: "Распоряжение",
            dataIndex: "controlOrderNumber",
            width: 140,
          },
          { title: "Проба", dataIndex: "sampleNumber", width: 180 },
          { title: "Показатель", dataIndex: "parameterName" },
          { title: "Норматив", dataIndex: "normFormatted" },
          {
            title: "Статус",
            dataIndex: "status",
            width: 130,
            render: renderTestRunStatus,
          },
          {
            title: "Вердикт",
            dataIndex: "verdict",
            width: 160,
            render: renderVerdict,
          },
        ]}
      />

      {current && (
        <Card title={`${current.parameterName} · ${current.normFormatted ?? ""}`}>
          <Space style={{ marginBottom: 12 }} wrap>
            <Select
              placeholder="Исполнитель"
              style={{ width: 260 }}
              options={employees?.map((x) => ({ value: x.id, label: x.name }))}
              defaultValue={current.assigneeId as string | undefined}
              onChange={(assigneeId) =>
                assign({
                  id: String(current.id),
                  body: { assigneeId, equipmentId: current.equipmentId },
                })
              }
            />
            <Select
              placeholder="Средство измерения"
              allowClear
              style={{ width: 280 }}
              options={equipment?.map((x) => ({ value: x.id, label: x.name }))}
              defaultValue={current.equipmentId as string | undefined}
              onChange={(equipmentId) =>
                assign({
                  id: String(current.id),
                  body: { assigneeId: current.assigneeId, equipmentId },
                })
              }
            />
          </Space>
          <Form form={form} layout="vertical">
            {Array.from({ length: replicates }, (_, i) => i + 1).map((rep) => (
              <Card
                key={rep}
                size="small"
                title={`Повторность ${rep}`}
                style={{ marginBottom: 8 }}
              >
                <Space wrap>
                  {quantities.map((q) => (
                    <Form.Item
                      key={`${rep}-${q.id}`}
                      name={`${rep}:${q.id}`}
                      label={`${q.name}${q.unitOfMeasure ? `, ${q.unitOfMeasure}` : ""}`}
                    >
                      {q.name.toLowerCase().includes("визуал") ||
                      q.name.toLowerCase().includes("соответств") ? (
                        <Switch />
                      ) : (
                        <InputNumber />
                      )}
                    </Form.Item>
                  ))}
                </Space>
              </Card>
            ))}
            <Space>
              <Button
                onClick={async () => {
                  const values = form.getFieldsValue();
                  const measurements: Record<string, unknown>[] = [];
                  for (const rep of Array.from({ length: replicates }, (_, i) => i + 1)) {
                    for (const q of quantities) {
                      const raw = values[`${rep}:${q.id}`];
                      measurements.push({
                        measuredQuantityId: q.id,
                        replicate: rep,
                        numericValue: typeof raw === "number" ? raw : undefined,
                        booleanValue: typeof raw === "boolean" ? raw : undefined,
                      });
                    }
                  }
                  await save({
                    id: String(current.id),
                    body: { equipmentId: current.equipmentId, measurements },
                  }).unwrap();
                  message.success("Измерения сохранены");
                }}
              >
                Сохранить измерения
              </Button>
              <Button
                type="primary"
                loading={completing.isLoading}
                onClick={async () => {
                  const result = await complete(String(current.id)).unwrap();
                  message.info(
                    result.verdict === "NotConforms"
                      ? "Не соответствует — партия в карантине"
                      : "Показатель рассчитан",
                  );
                  refetch();
                }}
              >
                Рассчитать и завершить
              </Button>
            </Space>
          </Form>
        </Card>
      )}
    </Space>
  );
}
