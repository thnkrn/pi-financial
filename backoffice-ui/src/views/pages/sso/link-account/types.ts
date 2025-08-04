export interface ISendLinkAccountInfo {
  id: string
  email: string
  custcode: string
  userId: string
  createdAt: string
  usedAt?: string
  isUsed: boolean
}

export interface IPaginationDataType {
  pageSize: number
  page: number
  total: number
  hasNextPage?: boolean
  hasPreviousPage?: boolean
}

export interface IFilter {
  custcode: string | null
}
