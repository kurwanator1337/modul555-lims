import { Button, Card, Descriptions, Space, Table, Typography, message } from "antd";
import { useParams } from "react-router-dom";
import {
  useGetOrderQuery,
  useIssueProtocolMutation,
  useOrderProtocolsQuery,
  useOrderTestsQuery,
  useRegisterSampleMutation,
  useSamplesQuery,
  useSignProtocolMutation,
} from "../api/endpoints";
import {
  renderOrderStatus,
  renderProtocolStatus,
  renderSampleState,
  renderVerdict,
} from "../components/StatusTag";
import { documentKindRu, label } from "../i18n/labels";

export default function OrderPage() {
  const { id = "" } = useParams();
  const { data: order } = useGetOrderQuery(id, { skip: !id });
  const { data: samples } = useSamplesQuery(id, { skip: !id });
  const { data: tests } = useOrderTestsQuery(id, { skip: !id });
  const { data: protocols } = useOrderProtocolsQuery(id, { skip: !id });
  const [register, reg] = useRegisterSampleMutation();
  const [issue] = useIssueProtocolMutation();
  const [sign] = useSignProtocolMutation();

  if (!order) return null;
  const kind =
    order.controlKind === "Incoming"
      ? "IncomingProtocolCement"
      : "BatchConformityConclusion";

  return (
    <Space orientation="vertical" size={16} style={{ width: "100%" }}>
      <Typography.Title level={3} style={{ margin: 0 }}>
        Распоряжение {String(order.number)}
      </Typography.Title>
      <Descriptions bordered size="small" column={3}>
        <Descriptions.Item label="Программа">
          {String(order.controlProgramName ?? "")}
        </Descriptions.Item>
        <Descriptions.Item label="Партия">
          {String(order.materialBatchNumber ?? order.productBatchNumber ?? "")}
        </Descriptions.Item>
        <Descriptions.Item label="Статус">
          {renderOrderStatus(order.status)}
        </Descriptions.Item>
      </Descriptions>

      <Card
        title="Пробы"
        extra={
          <Button
            type="primary"
            loading={reg.isLoading}
            onClick={() => register({ id, body: {} })}
          >
            Зарегистрировать пробу
          </Button>
        }
      >
        <Table
          rowKey="id"
          size="small"
          pagination={false}
          dataSource={samples ?? []}
          columns={[
            { title: "Номер", dataIndex: "number" },
            { title: "Штрихкод", dataIndex: "barcode" },
            { title: "Отобрана", dataIndex: "sampledAt" },
            {
              title: "Состояние",
              dataIndex: "state",
              render: renderSampleState,
            },
          ]}
        />
      </Card>

      <Card title="Испытания">
        <Table
          rowKey="id"
          size="small"
          pagination={false}
          dataSource={tests ?? []}
          rowClassName={(r) =>
            r.verdict === "NotConforms"
              ? "lims-row-not-conforms"
              : r.verdict === "Conforms"
                ? "lims-row-conforms"
                : ""
          }
          columns={[
            { title: "Показатель", dataIndex: "parameterName" },
            { title: "Метод", dataIndex: "methodName" },
            { title: "Норматив", dataIndex: "normFormatted" },
            { title: "Факт", dataIndex: "resultValue" },
            { title: "Вердикт", dataIndex: "verdict", render: renderVerdict },
            { title: "Исполнитель", dataIndex: "assigneeName" },
          ]}
        />
      </Card>

      <Card
        title="Протоколы и акты"
        extra={
          <Button
            onClick={async () => {
              await issue({ id, kind }).unwrap();
              message.success("Протокол сформирован");
            }}
          >
            Сформировать протокол
          </Button>
        }
      >
        <Table
          rowKey="id"
          size="small"
          pagination={false}
          dataSource={protocols ?? []}
          columns={[
            { title: "Номер", dataIndex: "number" },
            {
              title: "Вид",
              dataIndex: "kind",
              render: (v: string) => label(documentKindRu, v),
            },
            {
              title: "Статус",
              dataIndex: "status",
              render: renderProtocolStatus,
            },
            { title: "Заключение", dataIndex: "conclusion" },
            {
              title: "",
              render: (_, r) => (
                <Space>
                  <Button
                    size="small"
                    onClick={() => sign({ id: String(r.id), role: "Performer" })}
                  >
                    Подписать
                  </Button>
                  <Button
                    size="small"
                    onClick={() => sign({ id: String(r.id), role: "Approver" })}
                  >
                    Утвердить
                  </Button>
                </Space>
              ),
            },
          ]}
        />
      </Card>
    </Space>
  );
}
