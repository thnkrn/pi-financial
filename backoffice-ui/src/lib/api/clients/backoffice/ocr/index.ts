import { base64ToBlob } from '@/@core/utils/files'
import { backofficeAxiosInstance } from '@/lib/api'
import { IGetOcrResultsRequest, IGetOcrResultsResponse } from './types'

const extractDocumentFormat = (fileType: string): string | null => {
  if (fileType.trim() !== '') {
    const documentFormat = fileType.split('/')[1]

    return documentFormat ?? null
  }

  return null
}

export const getOcrResults = async (payload: IGetOcrResultsRequest): Promise<IGetOcrResultsResponse> => {
  const { files, documentType, output, password } = payload

  const fileArray = files.map(base64String => {
    return base64ToBlob(base64String)
  })

  const formdata = new FormData()
  fileArray.forEach((file: Blob, index: number) => {
    formdata.append('files', file, `document-${index}.${extractDocumentFormat(file.type)}`)
  })

  formdata.append('documentType', documentType)
  formdata.append('output', output)
  formdata.append('password', password as string)

  const instance = await backofficeAxiosInstance()

  const res = await instance.post('ocr/process', formdata, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })

  return {
    data: res?.data?.data,
    balanceData: res?.data?.balanceData,
    metadata: res?.data?.metadata,
    ocrConfidence: res?.data?.ocrConfidence,
    error: res?.data?.error,
  }
}
