export interface FileObjectType {
  file: File
  status: string
}

export interface UploadErrorType {
  error: boolean
  message?: string
}

export interface AcknowledgeDialogProps {
  content: string
  isSuccess: boolean
  open: boolean
}

export type AtsUploadType = 'UpdateEffectiveDate' | 'OverrideBankInfo'
