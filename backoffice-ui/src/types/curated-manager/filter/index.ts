export interface CuratedFilterItem {
  idString: string
  filterId: number
  filterName: string
  filterCategory: string
  filterType: string
  categoryPriority: number
  groupName: string
  subGroupName: string
  curatedListId: number
  isDefault: boolean
  highlight: boolean
  ordering: number
}

export interface CuratedFilterGroup {
  name: string
  data: CuratedFilterItem[]
}

export interface CuratedFiltersResponse {
  data: CuratedFilterGroup[]
}

export interface IGetCuratedFiltersRequest {
  groupName: string
}

export const TAB_CONFIG = {
  thaiEquities: 'Thai Equities',
  globalEquities: 'Global Equities',
  derivatives: 'Derivatives',
  funds: 'Funds',
  indices: 'Indices',
  currencies: 'Currencies',
  commodities: 'Commodities',
} as const

export type FilterTabType = keyof typeof TAB_CONFIG

export interface CuratedFilterState {
  activeTab: FilterTabType
  filterGroups: CuratedFilterGroup[]
  isLoading: boolean
  errorMessage: string
  isUploadSuccess: boolean
}

export interface UploadCuratedFilters {
  formData: FormData
  dataSource: string
  groupName: string
}
