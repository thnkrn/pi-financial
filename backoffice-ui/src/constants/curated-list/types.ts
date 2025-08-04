export interface ICuratedListItem {
  relevantTo: RelevantToType
  listName: string
  listHashtag: string
  instrumentType: InstrumentType
  ordering: OrderingType
  name?: string
  id: string
  curatedListId: number
}

export interface ICuratedListResponse {
  items: ICuratedListItem[]
  total: number
  page: number
  pageSize: number
}

export interface ICuratedListFilters {
  relevantTo?: RelevantToType
  instrumentType?: InstrumentType
  searchTerm?: string
  orderBy?: keyof ICuratedListItem
  orderDirection?: 'asc' | 'desc'
}

export enum RelevantToType {
  Indices = 'Indices',
  ThaiEquities = 'Thai Equities',
  Funds = 'Funds',
  Derivatives = 'Derivatives',
  Bonds = 'Bonds',
}

export enum InstrumentType {
  Index = 'Index',
  Equity = 'Equity',
  Fund = 'Fund',
  DerivativeFund = 'Derivative Fund',
  Bond = 'Bond',
}

export enum OrderingType {
  ManualOrdering = 'Manual Ordering',
  None = '',
}

export interface IBaseCuratedListItem {
  id: number
  relevantTo: RelevantToType
  listName: string
  listHashtag: string
  instrumentType: InstrumentType | ''
  ordering: OrderingType
}

export interface ILogicalCuratedListItem extends IBaseCuratedListItem {
  onEdit?: (id: number) => void
}

export interface IManualCuratedListItem extends IBaseCuratedListItem {
  onDelete?: (id: number) => void
}
