export type TranslationValueViewModel = {
  key: string | null
  value: string | null
  translation: string | null
}

export type PersonalInfo = {
  title?: string | null
  firstNameEn?: string | null
  lastNameEn?: string | null
  firstNameTh?: string | null
  lastNameTh?: string | null
  dateOfBirth?: string | null
  nationality?: string | null
  email?: string | null
  phone?: string | null
}

export type IdentificationInfo = {
  custCode?: string | null
  userId?: string | null
  id?: string | null
  citizenId?: string | null
  laserCode?: string | null
  idCardExpiryDate?: string | null
  createdDate?: string | null
  updatedDate?: string | null
}

export type BaseDocumentPortalRowType = PersonalInfo & IdentificationInfo

export interface DocumentPortalRowType extends BaseDocumentPortalRowType {
  documents?: AttachmentData[] | null
}

export interface AttachmentData {
  documentType?: string | null
  url?: string | null
  fileName?: string | null
}

export interface AttachmentsDataGridProps {
  documents?: AttachmentData[] | null
}

export interface Filter {
  filterBy: 'customerCode'
  filterValue: string
}

export interface Translations {
  [key: string]: string
}

export interface TabPanelProps {
  children?: React.ReactNode
  index: number
  value: number
}

export interface Action {
  handleDownloadAttachments: (params: AttachmentData[], custCode: string | null) => Promise<void>
}

export interface CustomerDetailProps {
  data: DocumentPortalRowType
  action: Action
  downloading: boolean
}

export interface PersonalInfoData {
  [key: string]: string | null
}

export interface IdentificationData {
  [key: string]: string | null
}

export interface CustomerDetailType {
  open: boolean
  data: DocumentPortalRowType
}

export interface CustomerFilterProps {
  onFilter: (customerCode: string) => void
  error: string
  onChange: () => void
}
