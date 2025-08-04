export type IGetMarginDateResponse = string[]

export type IGetMarginPDFRequest =  {
  effectiveDate: string,
  institute: boolean,
  effectiveDateFrom?: string
}

export type IGetMarginPDFResponse =  {
  filename: string,
  pdfBase64: string
}

export type IImportMarginRequest =  {
  fileBase64: string,
  effectiveDate: string
}
