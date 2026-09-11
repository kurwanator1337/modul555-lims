import { Layout, Menu, Dropdown, Typography, Badge } from "antd";
import {
  AppstoreOutlined,
  ExperimentOutlined,
  InboxOutlined,
  LogoutOutlined,
  SafetyCertificateOutlined,
  SettingOutlined,
  TeamOutlined,
  WarningOutlined,
  DashboardOutlined,
  ToolOutlined,
} from "@ant-design/icons";
import { Link, Outlet, useLocation, useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import type { MenuProps } from "antd";
import { useMemo } from "react";
import { Logo } from "../components/Logo";
import { loggedOut } from "../features/auth/authSlice";
import type { RootState } from "../store";
import { brand } from "../theme";

function isAdmin(roles: string[] | undefined) {
  return !!roles?.some((r) => r === "admin" || r === "manager");
}

export default function AppLayout() {
  const location = useLocation();
  const user = useSelector((s: RootState) => s.auth.user);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const admin = isAdmin(user?.roles);

  const items: MenuProps["items"] = useMemo(() => {
    const base: MenuProps["items"] = [
      {
        key: "/",
        icon: <DashboardOutlined />,
        label: <Link to="/">Сводка</Link>,
      },
      {
        key: "/workplace",
        icon: <ExperimentOutlined />,
        label: <Link to="/workplace">АРМ лаборанта</Link>,
      },
      {
        type: "group",
        label: "Входной контроль",
        children: [
          {
            key: "/incoming/batches",
            icon: <InboxOutlined />,
            label: <Link to="/incoming/batches">Партии сырья</Link>,
          },
          {
            key: "/incoming/orders",
            label: <Link to="/incoming/orders">Распоряжения</Link>,
          },
          {
            key: "/journals/incoming",
            label: <Link to="/journals/incoming">Журнал входного контроля</Link>,
          },
        ],
      },
      {
        type: "group",
        label: "Операционный контроль",
        children: [
          {
            key: "/operational/batches",
            icon: <AppstoreOutlined />,
            label: <Link to="/operational/batches">Производственные партии</Link>,
          },
          {
            key: "/operational/orders",
            label: <Link to="/operational/orders">Распоряжения</Link>,
          },
        ],
      },
      {
        key: "/nonconformances",
        icon: <WarningOutlined />,
        label: <Link to="/nonconformances">Несоответствия</Link>,
      },
      {
        key: "/metrology",
        icon: <ToolOutlined />,
        label: <Link to="/metrology">Метрология</Link>,
      },
      {
        key: "/passport",
        icon: <SafetyCertificateOutlined />,
        label: <Link to="/passport">Цифровой паспорт</Link>,
      },
      {
        key: "/personnel",
        icon: <TeamOutlined />,
        label: <Link to="/personnel">Персонал</Link>,
      },
      {
        type: "group",
        label: "Справочники",
        children: [
          {
            key: "/nsi/nomenclatures",
            label: <Link to="/nsi/nomenclatures">Номенклатура</Link>,
          },
          {
            key: "/nsi/standards",
            label: <Link to="/nsi/standards">Нормативные документы</Link>,
          },
          {
            key: "/nsi/parameters",
            label: <Link to="/nsi/parameters">Показатели качества</Link>,
          },
          {
            key: "/nsi/methods",
            label: <Link to="/nsi/methods">Методы испытаний</Link>,
          },
          {
            key: "/nsi/programs",
            label: <Link to="/nsi/programs">Программы испытаний</Link>,
          },
          {
            key: "/nsi/suppliers",
            label: <Link to="/nsi/suppliers">Поставщики</Link>,
          },
          {
            key: "/nsi/equipment",
            label: <Link to="/nsi/equipment">Средства измерений</Link>,
          },
          {
            key: "/nsi/defects",
            label: <Link to="/nsi/defects">Дефекты</Link>,
          },
          {
            key: "/nsi/causes",
            label: <Link to="/nsi/causes">Причины несоответствий</Link>,
          },
          {
            key: "/nsi/templates",
            label: <Link to="/nsi/templates">Шаблоны документов</Link>,
          },
        ],
      },
    ];

    if (admin) {
      base.push({
        type: "group",
        label: "Администрирование",
        children: [
          {
            key: "/admin",
            icon: <SettingOutlined />,
            label: <Link to="/admin">Обзор</Link>,
          },
          {
            key: "/admin/lines",
            label: <Link to="/admin/lines">Участки</Link>,
          },
          {
            key: "/admin/subdivisions",
            label: <Link to="/admin/subdivisions">Подразделения</Link>,
          },
          {
            key: "/admin/warehouses",
            label: <Link to="/admin/warehouses">Склады</Link>,
          },
          {
            key: "/admin/categories",
            label: <Link to="/admin/categories">Группы номенклатуры</Link>,
          },
          {
            key: "/admin/quantities",
            label: <Link to="/admin/quantities">Измеряемые величины</Link>,
          },
          {
            key: "/admin/equipment-types",
            label: <Link to="/admin/equipment-types">Типы СИ</Link>,
          },
          {
            key: "/admin/employees",
            label: <Link to="/admin/employees">Сотрудники</Link>,
          },
          {
            key: "/admin/users",
            label: <Link to="/admin/users">Учётные записи</Link>,
          },
          {
            key: "/admin/competencies",
            label: <Link to="/admin/competencies">Аттестации</Link>,
          },
          {
            key: "/admin/templates",
            label: <Link to="/admin/templates">Шаблоны</Link>,
          },
          {
            key: "/admin/material-batches",
            label: <Link to="/admin/material-batches">Партии сырья (статусы)</Link>,
          },
          {
            key: "/admin/product-batches",
            label: <Link to="/admin/product-batches">Партии продукции (статусы)</Link>,
          },
          {
            key: "/admin/audit",
            label: <Link to="/admin/audit">Журнал аудита</Link>,
          },
        ],
      });
    }

    return base;
  }, [admin]);

  return (
    <Layout style={{ minHeight: "100vh" }} hasSider>
      <Layout.Sider
        width={268}
        theme="dark"
        breakpoint="lg"
        collapsedWidth={0}
        trigger={null}
        className="lims-sider"
        style={{
          background: brand.deepBlue,
          height: "100vh",
          position: "sticky",
          top: 0,
          overflow: "auto",
        }}
      >
        <div style={{ padding: "16px 16px 8px" }}>
          <Logo light markSize={32} />
          <Typography.Text
            style={{
              display: "block",
              color: "#8FC3E4",
              fontSize: 11,
              marginTop: 6,
            }}
          >
            LIMS · лаборатория контроля качества
          </Typography.Text>
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[location.pathname]}
          items={items}
          style={{ background: brand.deepBlue, borderInlineEnd: "none" }}
        />
      </Layout.Sider>
      <Layout>
        <Layout.Header
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "flex-end",
            gap: 16,
          }}
        >
          <Badge
            color={brand.skyBlue}
            text={<span style={{ color: "#fff" }}>{user?.position}</span>}
          />
          <Dropdown
            menu={{
              items: [
                {
                  key: "out",
                  icon: <LogoutOutlined />,
                  label: "Выйти",
                  onClick: () => {
                    dispatch(loggedOut());
                    navigate("/login");
                  },
                },
              ],
            }}
          >
            <Typography.Text style={{ color: "#fff", cursor: "pointer" }}>
              {user?.fullName}
            </Typography.Text>
          </Dropdown>
        </Layout.Header>
        <Layout.Content style={{ padding: 20 }}>
          <Outlet />
        </Layout.Content>
      </Layout>
    </Layout>
  );
}
