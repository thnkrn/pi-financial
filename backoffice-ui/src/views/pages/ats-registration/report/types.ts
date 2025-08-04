export interface Filter {
  atsUploadType: string | null
  requestDate: string | null
}

export interface ReportFilterProps {
  onFilter: (filter: Filter) => void
  filter: Filter
  onApplyFilter: () => void
}

export interface KeyValue {
  key: string
  value: string | boolean
}

export interface AtsRegistrationReportDataTableProps {
  rows: AtsRegistrationReportRowType[]
  onPageFilter: (pageFilter: PaginationDataType) => void
  loading: boolean
  pagination: PaginationDataType
}

export interface AtsRegistrationReportRowType {
  id: string | null | undefined
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
