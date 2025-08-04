import { GridSortModel } from '@mui/x-data-grid'

export type TranslationValueViewModel = {
  key: string | null
  value: string | null
  translation: string | null
}

export type PersonalInfo = {
  title?: string
  firstNameEn?: string
  lastNameEn?: string
  firstNameTh?: string
  lastNameTh?: string
  dateOfBirth?: string
  nationality?: string
  email?: string
  phone?: string
}

export type IdentificationInfo = {
  custCode?: string
  id?: string
  accountOpeningRequestId?: string
  citizenId?: string
  laserCode?: string
  idCardExpiryDate?: string
  createdDate?: string
  updatedDate?: string
  userId?: string
  referId?: string
  transId?: string
}

export type ApplicationErrorInfo = {
  bpmReceived: boolean
}

export type OtherApplicationDetails = {
  status?: string
}

export interface CustomerDetailProps {
  data: BaseApplicationSummaryRowType
}

export type BaseApplicationSummaryRowType = PersonalInfo & IdentificationInfo & OtherApplicationDetails

export interface ApplicationSummaryRowType extends BaseApplicationSummaryRowType {
  bpmReceived: boolean
}
export interface Filter {
  status: string
  bpmReceived: boolean
  custCode: string
  userId: string
  orderBy?: string
  orderDir?: string
  errorCheck?: boolean
}

export interface CustomerDetail {
  open: boolean
  data: any
}

export interface ApplicationSummaryTableProps {
  rows: ApplicationSummaryRowType[]
  onPageFilter: (pageFilter: PaginationDataType) => void
  loading: boolean
  onSortModelChange: (sortModel: GridSortModel) => void
  pagination: PaginationDataType
}

export interface KeyValue {
  key: string
  value: string | boolean
}

export interface ApplicationFilterProps {
  onFilter: (filter: Filter) => void
  onSync: (filter: Filter) => void
}

export interface PaginationDataType {
  pageSize: number
  page: number
  total?: number
  orderBy?: string | null
  orderDir?: string | null
}

export interface SortModelType {
  orderBy: string
  orderDir: string
}

export interface FilterObjType {
  pageSize?: number
  page?: number
  orderBy?: string
  orderDir?: string
}
