export type IGetMyUserDataResponse = {
  id: number | null
  keycloakId: string | null
  role: string | null
  name: string | null
  employeeId: number | null
  brokerId: number | null
  contact: string | null
  verified: boolean | null
  createdAt: string | null
  updatedAt: string | null
  lineToken: string | null
}

export type ICreateUserRequest = {
  employeeId: number
  teamId: number
}
