export interface IAccountInfo {
  id: string // UUID
  username: string
  isSyncPassword: boolean
  isSyncPin: boolean
  loginFailCount: number
  isLock: boolean
  updatedAt: string // ISO date string
  createdAt: string // ISO date string
  userId: string | null // UUID
  email: string | null // null if not available
  mobile: string | null // null if not available
}

export interface IGetAccountInfoResponse {
  currentPage: number // Current page number
  pageSize: number // Number of items per page
  hasNextPage: boolean // Whether there is a next page
  hasPreviousPage: boolean // Whether there is a previous page
  totalPages: number // Total number of pages
  data: IAccountInfo[] | null // Array of account information
}

export interface IGetAccountInfoResponseWrapper {
  code: string // Response code
  msg: string // Message
  data: IAccountInfo[] | null // Array of account info
}

export interface IGetAccountInfoRequest {
  username: string | null
  page?: number
  pageSize?: number
  [key: string]: any
}

export interface IGetAtsReportsResponse {
  data: IGetAtsReportsData[]
  page?: number
  pageSize?: number
  totalPages?: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export type FilterObjectURLParamType = {
  username?: string
  page?: string
  pageSize?: string
}

export interface IGetAtsReportsData {
  id: string | null
  atsUploadType: string | null
  requestDate: string | null
  status: string | null
  reportName: string | null
  userName: string | null
}
