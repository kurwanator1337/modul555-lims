import { Tag } from "antd";
import type { ReactNode } from "react";
import {
  batchStatusRu,
  controlOrderStatusRu,
  controlStageRu,
  correctiveActionStatusRu,
  equipmentStatusRu,
  label,
  nonconformanceStatusRu,
  protocolStatusRu,
  sampleStateRu,
  testRunStatusRu,
  verdictRu,
} from "../i18n/labels";

type TagColor = "default" | "processing" | "success" | "error" | "warning";

function StatusTag({
  value,
  dict,
  colors,
}: {
  value: unknown;
  dict: Record<string, string>;
  colors?: Record<string, TagColor>;
}): ReactNode {
  const key = String(value ?? "");
  return <Tag color={colors?.[key] ?? "default"}>{label(dict, value)}</Tag>;
}

const batchColors: Record<string, TagColor> = {
  AwaitingControl: "default",
  InControl: "processing",
  Approved: "success",
  Quarantine: "error",
  Rejected: "error",
  RestrictedUse: "warning",
  ReturnedToSupplier: "warning",
  WrittenOff: "default",
};

const equipmentColors: Record<string, TagColor> = {
  Operational: "success",
  UnderVerification: "processing",
  UnderMaintenance: "warning",
  UnderRepair: "warning",
  VerificationExpired: "error",
  Decommissioned: "default",
};

const verdictColors: Record<string, TagColor> = {
  Conforms: "success",
  NotConforms: "error",
  Pending: "default",
  NotApplicable: "default",
};

export const renderBatchStatus = (v: unknown) => (
  <StatusTag value={v} dict={batchStatusRu} colors={batchColors} />
);

export const renderOrderStatus = (v: unknown) => (
  <StatusTag value={v} dict={controlOrderStatusRu} />
);

export const renderTestRunStatus = (v: unknown) => (
  <StatusTag value={v} dict={testRunStatusRu} />
);

export const renderSampleState = (v: unknown) => (
  <StatusTag value={v} dict={sampleStateRu} />
);

export const renderVerdict = (v: unknown) => (
  <StatusTag value={v} dict={verdictRu} colors={verdictColors} />
);

export const renderEquipmentStatus = (v: unknown) => (
  <StatusTag value={v} dict={equipmentStatusRu} colors={equipmentColors} />
);

export const renderProtocolStatus = (v: unknown) => (
  <StatusTag value={v} dict={protocolStatusRu} />
);

export const renderNonconformanceStatus = (v: unknown) => (
  <StatusTag value={v} dict={nonconformanceStatusRu} />
);

export const renderActionStatus = (v: unknown) => (
  <StatusTag value={v} dict={correctiveActionStatusRu} />
);

export const renderStage = (v: unknown) => label(controlStageRu, v);

export const renderBatchStatusText = (v: unknown) => label(batchStatusRu, v);
