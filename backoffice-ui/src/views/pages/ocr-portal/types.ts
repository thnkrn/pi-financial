export interface FileObjectType {
  file: File
  status: string
}

export interface UploadErrorType {
  error: boolean
  message?: string
}

export interface DocumentProcessType {
  processDocumentClick: boolean
  processingStatus: boolean
  processComplete: boolean
}
