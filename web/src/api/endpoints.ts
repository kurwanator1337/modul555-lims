import { api } from "./api";
import type { AuthUser } from "../features/auth/authSlice";

export interface Paged<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface LookupItem {
  id: string;
  code: string;
  name: string;
}

export type Reference = {
  id: string;
  code: string;
  name: string;
  description?: string;
  isActive: boolean;
  sortOrder: number;
};

const injected = api.injectEndpoints({
  endpoints: (build) => ({
    login: build.mutation<
      { token: string; refreshToken?: string; user: AuthUser },
      { userName: string; password: string }
    >({
      query: (body) => ({ url: "/auth/login", method: "POST", body }),
    }),
    me: build.query<AuthUser, void>({ query: () => "/auth/me" }),

    list: build.query<
      Paged<Record<string, unknown>>,
      { path: string; page?: number; search?: string; extra?: string }
    >({
      query: ({ path, page = 1, search, extra }) => {
        const params = new URLSearchParams({
          page: String(page),
          pageSize: "20",
        });
        if (search) params.set("search", search);
        if (extra)
          extra.split("&").forEach((p) => {
            const [k, v] = p.split("=");
            if (k && v) params.set(k, decodeURIComponent(v));
          });
        return `/${path}?${params}`;
      },
      providesTags: (_r, _e, arg) => [{ type: "Dashboard", id: arg.path }],
    }),
    lookup: build.query<LookupItem[], string>({
      query: (entity) => `/lookups/${entity}`,
    }),
    create: build.mutation<unknown, { path: string; body: unknown }>({
      query: ({ path, body }) => ({ url: `/${path}`, method: "POST", body }),
      invalidatesTags: ["Dashboard"],
    }),
    update: build.mutation<unknown, { path: string; id: string; body: unknown }>({
      query: ({ path, id, body }) => ({
        url: `/${path}/${id}`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Dashboard"],
    }),
    remove: build.mutation<void, { path: string; id: string }>({
      query: ({ path, id }) => ({ url: `/${path}/${id}`, method: "DELETE" }),
      invalidatesTags: ["Dashboard"],
    }),

    getProgram: build.query<Record<string, unknown>, string>({
      query: (id) => `/control-programs/${id}`,
      providesTags: ["ControlProgram"],
    }),
    saveProgram: build.mutation<Record<string, unknown>, { id?: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: id ? `/control-programs/${id}` : "/control-programs",
        method: id ? "PUT" : "POST",
        body,
      }),
      invalidatesTags: ["ControlProgram"],
    }),

    getOrder: build.query<Record<string, unknown>, string>({
      query: (id) => `/control-orders/${id}`,
      providesTags: ["ControlOrder"],
    }),
    incomingOrder: build.mutation<Record<string, unknown>, unknown>({
      query: (body) => ({
        url: "/control-orders/incoming",
        method: "POST",
        body,
      }),
      invalidatesTags: ["ControlOrder", "MaterialBatch", "Dashboard"],
    }),
    operationalOrder: build.mutation<Record<string, unknown>, unknown>({
      query: (body) => ({
        url: "/control-orders/operational",
        method: "POST",
        body,
      }),
      invalidatesTags: ["ControlOrder", "ProductBatch", "Dashboard"],
    }),
    samples: build.query<Record<string, unknown>[], string>({
      query: (id) => `/control-orders/${id}/samples`,
      providesTags: ["Sample"],
    }),
    registerSample: build.mutation<unknown, { id: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: `/control-orders/${id}/samples`,
        method: "POST",
        body,
      }),
      invalidatesTags: ["Sample", "ControlOrder", "TestRun"],
    }),
    orderTests: build.query<Record<string, unknown>[], string>({
      query: (id) => `/control-orders/${id}/tests`,
      providesTags: ["TestRun"],
    }),
    workplace: build.query<Record<string, unknown>[], void>({
      query: () => "/workplace",
      providesTags: ["TestRun"],
    }),
    getTest: build.query<Record<string, unknown>, string>({
      query: (id) => `/tests/${id}`,
      providesTags: ["TestRun"],
    }),
    assignTest: build.mutation<void, { id: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: `/tests/${id}/assign`,
        method: "POST",
        body,
      }),
      invalidatesTags: ["TestRun"],
    }),
    saveMeasurements: build.mutation<
      Record<string, unknown>,
      { id: string; body: unknown }
    >({
      query: ({ id, body }) => ({
        url: `/tests/${id}/measurements`,
        method: "POST",
        body,
      }),
      invalidatesTags: ["TestRun"],
    }),
    completeTest: build.mutation<Record<string, unknown>, string>({
      query: (id) => ({ url: `/tests/${id}/complete`, method: "POST" }),
      invalidatesTags: ["TestRun", "ControlOrder", "Nonconformance", "Dashboard"],
    }),
    issueProtocol: build.mutation<Record<string, unknown>, { id: string; kind: string }>({
      query: ({ id, kind }) => ({
        url: `/control-orders/${id}/protocols?kind=${kind}`,
        method: "POST",
      }),
      invalidatesTags: ["Protocol"],
    }),
    orderProtocols: build.query<Record<string, unknown>[], string>({
      query: (id) => `/control-orders/${id}/protocols`,
      providesTags: ["Protocol"],
    }),
    signProtocol: build.mutation<unknown, { id: string; role: string }>({
      query: ({ id, role }) => ({
        url: `/protocols/${id}/sign`,
        method: "POST",
        body: { role },
      }),
      invalidatesTags: ["Protocol"],
    }),
    decideNc: build.mutation<unknown, { id: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: `/nonconformances/${id}/decide`,
        method: "POST",
        body,
      }),
      invalidatesTags: ["Nonconformance", "Dashboard"],
    }),
    addAction: build.mutation<unknown, { id: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: `/nonconformances/${id}/actions`,
        method: "POST",
        body,
      }),
      invalidatesTags: ["Nonconformance"],
    }),
    closeAction: build.mutation<void, { id: string; result?: string }>({
      query: ({ id, result }) => ({
        url: `/actions/${id}/close`,
        method: "POST",
        body: result,
      }),
      invalidatesTags: ["Nonconformance"],
    }),
    dashboard: build.query<Record<string, unknown>, void>({
      query: () => "/dashboard",
      providesTags: ["Dashboard"],
    }),
    incomingJournal: build.query<Record<string, unknown>[], void>({
      query: () => "/journals/incoming",
    }),
    reminders: build.query<Record<string, unknown>[], void>({
      query: () => "/metrology/reminders",
      providesTags: ["Equipment"],
    }),
    equipmentEvents: build.query<Record<string, unknown>[], string>({
      query: (id) => `/equipment/${id}/events`,
    }),
    registerEvent: build.mutation<unknown, unknown>({
      query: (body) => ({ url: "/equipment/events", method: "POST", body }),
      invalidatesTags: ["Equipment"],
    }),
    passport: build.query<
      Record<string, unknown>,
      { productId?: string; materialId?: string }
    >({
      query: ({ productId, materialId }) => {
        const p = new URLSearchParams();
        if (productId) p.set("productId", productId);
        if (materialId) p.set("materialId", materialId);
        return `/passport?${p}`;
      },
      providesTags: ["Traceability"],
    }),
    linkTrace: build.mutation<
      unknown,
      { productId: string; materialId: string; quantity?: number }
    >({
      query: ({ productId, materialId, quantity }) => ({
        url: `/traceability?productId=${productId}&materialId=${materialId}${quantity != null ? `&quantity=${quantity}` : ""}`,
        method: "POST",
      }),
      invalidatesTags: ["Traceability"],
    }),
    competencies: build.query<Record<string, unknown>[], string | undefined>({
      query: (employeeId) =>
        `/competencies${employeeId ? `?employeeId=${employeeId}` : ""}`,
      providesTags: ["Dashboard"],
    }),
    adminUsers: build.query<
      Paged<Record<string, unknown>>,
      { page?: number; search?: string }
    >({
      query: ({ page = 1, search }) => {
        const p = new URLSearchParams({ page: String(page), pageSize: "20" });
        if (search) p.set("search", search);
        return `/admin/users?${p}`;
      },
      providesTags: ["Dashboard"],
    }),
    saveAdminUser: build.mutation<unknown, { id?: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: id ? `/admin/users/${id}` : "/admin/users",
        method: id ? "PUT" : "POST",
        body,
      }),
      invalidatesTags: ["Dashboard"],
    }),
    saveCompetency: build.mutation<unknown, { id?: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: id ? `/admin/competencies/${id}` : "/admin/competencies",
        method: id ? "PUT" : "POST",
        body,
      }),
      invalidatesTags: ["Dashboard"],
    }),
    deleteCompetency: build.mutation<void, string>({
      query: (id) => ({ url: `/admin/competencies/${id}`, method: "DELETE" }),
      invalidatesTags: ["Dashboard"],
    }),
    adminAudit: build.query<
      Paged<Record<string, unknown>>,
      { page?: number; search?: string }
    >({
      query: ({ page = 1, search }) => {
        const p = new URLSearchParams({ page: String(page), pageSize: "20" });
        if (search) p.set("search", search);
        return `/admin/audit?${p}`;
      },
    }),
    adminMaterialBatches: build.query<
      Paged<Record<string, unknown>>,
      { page?: number; search?: string }
    >({
      query: ({ page = 1, search }) => {
        const p = new URLSearchParams({ page: String(page), pageSize: "20" });
        if (search) p.set("search", search);
        return `/admin/material-batches?${p}`;
      },
      providesTags: ["MaterialBatch"],
    }),
    adminProductBatches: build.query<
      Paged<Record<string, unknown>>,
      { page?: number; search?: string }
    >({
      query: ({ page = 1, search }) => {
        const p = new URLSearchParams({ page: String(page), pageSize: "20" });
        if (search) p.set("search", search);
        return `/admin/product-batches?${p}`;
      },
      providesTags: ["ProductBatch"],
    }),
    updateAdminMaterialBatch: build.mutation<unknown, { id: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: `/admin/material-batches/${id}`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["MaterialBatch", "Dashboard"],
    }),
    updateAdminProductBatch: build.mutation<unknown, { id: string; body: unknown }>({
      query: ({ id, body }) => ({
        url: `/admin/product-batches/${id}`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["ProductBatch", "Dashboard"],
    }),
    clearDemo: build.mutation<{ deleted: number; message: string }, void>({
      query: () => ({ url: "/admin/demo/clear", method: "POST" }),
      invalidatesTags: [
        "Dashboard",
        "MaterialBatch",
        "ProductBatch",
        "ControlOrder",
        "TestRun",
        "Sample",
        "Nonconformance",
        "Protocol",
        "Traceability",
      ],
    }),
    generateDemo: build.mutation<
      {
        deleted: number;
        message: string;
        orderNumber?: string;
        batchNumber?: string;
        testRuns: number;
      },
      void
    >({
      query: () => ({ url: "/admin/demo/generate", method: "POST" }),
      invalidatesTags: [
        "Dashboard",
        "MaterialBatch",
        "ProductBatch",
        "ControlOrder",
        "TestRun",
        "Sample",
        "Traceability",
      ],
    }),
  }),
});

export const {
  useLoginMutation,
  useListQuery,
  useLookupQuery,
  useCreateMutation,
  useUpdateMutation,
  useRemoveMutation,
  useGetProgramQuery,
  useSaveProgramMutation,
  useGetOrderQuery,
  useIncomingOrderMutation,
  useOperationalOrderMutation,
  useSamplesQuery,
  useRegisterSampleMutation,
  useOrderTestsQuery,
  useWorkplaceQuery,
  useGetTestQuery,
  useAssignTestMutation,
  useSaveMeasurementsMutation,
  useCompleteTestMutation,
  useIssueProtocolMutation,
  useOrderProtocolsQuery,
  useSignProtocolMutation,
  useDecideNcMutation,
  useAddActionMutation,
  useCloseActionMutation,
  useDashboardQuery,
  useIncomingJournalQuery,
  useRemindersQuery,
  useEquipmentEventsQuery,
  useRegisterEventMutation,
  usePassportQuery,
  useLinkTraceMutation,
  useCompetenciesQuery,
  useAdminUsersQuery,
  useSaveAdminUserMutation,
  useSaveCompetencyMutation,
  useDeleteCompetencyMutation,
  useAdminAuditQuery,
  useAdminMaterialBatchesQuery,
  useAdminProductBatchesQuery,
  useUpdateAdminMaterialBatchMutation,
  useUpdateAdminProductBatchMutation,
  useClearDemoMutation,
  useGenerateDemoMutation,
  useMeQuery,
  useLazyMeQuery,
} = injected;
