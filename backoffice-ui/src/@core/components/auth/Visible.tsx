import { Role } from '@/lib/auth/role';
import { useUserRole } from '@/lib/auth/useUserRole';
import { ReactNode } from 'react';

export const Visible = ({ allowedRoles, children }: { allowedRoles: Role[]; children: ReactNode }) => {
  return !useUserRole(...allowedRoles) ? null : <>{children}</>
}
