import { Role } from '@/lib/auth/role'
import { useSession } from 'next-auth/react'

export const useUserRole = (...allowedRoles: Array<Role>): boolean => {
  const { data } = useSession()

  if (!data) return false
  const userRoles = data?.roles ?? []

  return checkRoles(allowedRoles, userRoles)
}

export const checkRoles = (allowedRoles: Array<Role>, userRoles: Array<string> = []): boolean => {
  return allowedRoles.length === 0 || allowedRoles.some(role => userRoles.includes(role))
}
