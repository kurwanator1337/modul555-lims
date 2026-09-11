import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import type { RootState } from "../store";

/** Опции ролей из /api/admin/roles. */
export function useRoleOptions() {
  const token = useSelector((s: RootState) => s.auth.token);
  const [options, setOptions] = useState<{ label: string; value: string }[]>([]);
  useEffect(() => {
    if (!token) {
      setOptions([]);
      return;
    }
    fetch("/api/admin/roles", {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((r) => (r.ok ? r.json() : []))
      .then((data: { value: string; label: string }[]) =>
        setOptions(Array.isArray(data) ? data : []),
      )
      .catch(() => setOptions([]));
  }, [token]);
  return options;
}
