import { Navigate, Outlet, Route, Routes } from "react-router-dom";
import { useSelector } from "react-redux";
import type { RootState } from "./store";
import AppLayout from "./layout/AppLayout";
import LoginPage from "./pages/LoginPage";
import ProgramsPage from "./pages/ProgramsPage";
import WorkplacePage from "./pages/WorkplacePage";
import OrderPage from "./pages/OrderPage";
import {
  IncomingBatchesPage,
  OrdersPage,
  ProductBatchesPage,
} from "./pages/BatchesPages";
import {
  CausesPage,
  DefectsPage,
  EquipmentCatalogPage,
  MethodsPage,
  NomenclaturesPage,
  ParametersPage,
  StandardsPage,
  SuppliersPage,
  TemplatesPage,
} from "./pages/catalogs";
import {
  DashboardPage,
  IncomingJournalPage,
  MetrologyPage,
  NonconformancesPage,
  PassportPage,
  PersonnelPage,
} from "./pages/OtherPages";
import {
  AdminHomePage,
  AuditAdminPage,
  CategoriesAdminPage,
  CompetenciesAdminPage,
  EmployeesAdminPage,
  EquipmentTypesAdminPage,
  MaterialBatchesAdminPage,
  ProductBatchesAdminPage,
  ProductionLinesAdminPage,
  QuantitiesAdminPage,
  SubdivisionsAdminPage,
  TemplatesAdminPage,
  UsersAdminPage,
  WarehousesAdminPage,
} from "./pages/AdminPages";

function isAdminRole(roles: string[] | undefined) {
  return !!roles?.some((r) => r === "admin" || r === "manager");
}

function RequireAuth() {
  const token = useSelector((s: RootState) => s.auth.token);
  return token ? <Outlet /> : <Navigate to="/login" replace />;
}

function RequireAdmin() {
  const user = useSelector((s: RootState) => s.auth.user);
  return isAdminRole(user?.roles) ? <Outlet /> : <Navigate to="/" replace />;
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<RequireAuth />}>
        <Route element={<AppLayout />}>
          <Route path="/" element={<DashboardPage />} />
          <Route path="/workplace" element={<WorkplacePage />} />
          <Route path="/incoming/batches" element={<IncomingBatchesPage />} />
          <Route path="/incoming/orders" element={<OrdersPage kind="Incoming" />} />
          <Route path="/operational/batches" element={<ProductBatchesPage />} />
          <Route path="/operational/orders" element={<OrdersPage kind="Operational" />} />
          <Route path="/orders/:id" element={<OrderPage />} />
          <Route path="/nonconformances" element={<NonconformancesPage />} />
          <Route path="/metrology" element={<MetrologyPage />} />
          <Route path="/passport" element={<PassportPage />} />
          <Route path="/personnel" element={<PersonnelPage />} />
          <Route path="/journals/incoming" element={<IncomingJournalPage />} />
          <Route path="/nsi/nomenclatures" element={<NomenclaturesPage />} />
          <Route path="/nsi/standards" element={<StandardsPage />} />
          <Route path="/nsi/parameters" element={<ParametersPage />} />
          <Route path="/nsi/methods" element={<MethodsPage />} />
          <Route path="/nsi/programs" element={<ProgramsPage />} />
          <Route path="/nsi/suppliers" element={<SuppliersPage />} />
          <Route path="/nsi/equipment" element={<EquipmentCatalogPage />} />
          <Route path="/nsi/defects" element={<DefectsPage />} />
          <Route path="/nsi/causes" element={<CausesPage />} />
          <Route path="/nsi/templates" element={<TemplatesPage />} />

          <Route element={<RequireAdmin />}>
            <Route path="/admin" element={<AdminHomePage />} />
            <Route path="/admin/lines" element={<ProductionLinesAdminPage />} />
            <Route path="/admin/subdivisions" element={<SubdivisionsAdminPage />} />
            <Route path="/admin/warehouses" element={<WarehousesAdminPage />} />
            <Route path="/admin/categories" element={<CategoriesAdminPage />} />
            <Route path="/admin/quantities" element={<QuantitiesAdminPage />} />
            <Route path="/admin/equipment-types" element={<EquipmentTypesAdminPage />} />
            <Route path="/admin/employees" element={<EmployeesAdminPage />} />
            <Route path="/admin/users" element={<UsersAdminPage />} />
            <Route path="/admin/competencies" element={<CompetenciesAdminPage />} />
            <Route path="/admin/templates" element={<TemplatesAdminPage />} />
            <Route
              path="/admin/material-batches"
              element={<MaterialBatchesAdminPage />}
            />
            <Route path="/admin/product-batches" element={<ProductBatchesAdminPage />} />
            <Route path="/admin/audit" element={<AuditAdminPage />} />
          </Route>
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
