import { DataSource } from '@/types/curated-manager'

export enum CuratedType {
  Logical = 'Logical',
  Manual = 'Manual',
}

export enum ActionType {
  UPLOAD = 'UPLOAD',
  UPDATE = 'UPDATE',
  DELETE = 'DELETE',
  NONE = 'NONE',
}

export interface CuratedListItem {
  id: string
  curatedListId: number
  curatedListCode: string | null
  curatedType: CuratedType
  relevantTo: string
  name: string
  hashtag: string
  ordering: number | null
  curatedListSource: DataSource
  createTime?: string
  updateTime?: string
  updateBy?: string
}

export interface CuratedListResponse {
  data: {
    logical: CuratedListItem[]
    manual: CuratedListItem[]
  }
}

export interface CuratedListTable extends Omit<CuratedListItem, 'id' | 'createTime' | 'updateTime' | 'updateBy'> {
  id: string
}

export interface ExtendedCuratedListTable extends CuratedListTable {
  onDelete: () => void
}

export interface CuratedListState {
  activeTab: CuratedType
  isLoading: boolean
  errorMessage: string
  logicalList: CuratedListItem[]
  manualList: CuratedListItem[]
  successAction: ActionType
}

export interface UpdateCuratedList {
  id: string
  dataSource: string
  [key: string]: string
}

export interface UploadCuratedManualList {
  formData: FormData
  dataSource: string
}
