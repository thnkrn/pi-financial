import { IAccountInfo } from '@/lib/api/clients/sso/account-management/types'

export interface Filter {
  username: string | null
}

export interface ReportFilterProps {
  onFilter: (filter: Filter) => void
  filter: Filter
  onApplyFilter: () => void
  fetchAccountData: any
}

export interface KeyValue {
  key: string
  value: string | boolean
}

export interface DataTableProps {
  rows: IAccountInfo[] | null
  onPageFilter: (pageFilter: PaginationDataType) => void
  loading: boolean
  pagination: PaginationDataType
  fetchAccountData: any
}

export interface AtsRegistrationReportRowType {
  id?: string
  atsUploadType: string | null | undefined
  requestDate: string | null | undefined
  status: string | null | undefined
  reportName: string | null | undefined
  userName: string | null | undefined
}

export interface PaginationDataType {
  pageSize: number
  page: number
  total?: number
  hasNextPage?: boolean
  hasPreviousPage?: boolean
}
