import { backofficeAxiosInstance } from '@/lib/api'
import { PiClientOnboardServiceModelPiOnboardServiceAPIModelsAtsAtsRequestsPaginated } from '@pi-financial/backoffice-srv/src/models/PiClientOnboardServiceModelPiOnboardServiceAPIModelsAtsAtsRequestsPaginated'
import { AxiosError } from 'axios'
import {
  FilterObjectURLParamType,
  IGetAtsReportDownloadResponse,
  IGetAtsReportsData,
  IGetAtsReportsRequest,
  IGetAtsReportsResponse,
  IUploadAtsErrorResponse,
  IUploadAtsRequest,
  IUploadAtsResponse
} from './types'

export const getAtsReports = async (filter: IGetAtsReportsRequest): Promise<IGetAtsReportsResponse> => {
  Object.keys(filter).forEach(key => {
    if (!filter[key]) {
      delete filter[key];
    }
  })
  const objString = '?' + new URLSearchParams(filter as FilterObjectURLParamType).toString()

  const instance = await backofficeAxiosInstance()
  const apiResponse = (await instance.get(`ats_registration/requests${objString}`)).data as PiClientOnboardServiceModelPiOnboardServiceAPIModelsAtsAtsRequestsPaginated
  const defaultColValue = 'N/A'
  const mapResponse: IGetAtsReportsData[] = apiResponse.atsRequests?.map(v => {
    return {
      id: v.id ?? defaultColValue,
      atsUploadType: v.uploadType ?? defaultColValue,
      requestDate: v.createdDate ?? defaultColValue,
      status: v.status ?? defaultColValue,
      reportName: v.reportName ?? defaultColValue,
      userName: v.userName ?? defaultColValue,
    }
  }) || []

  return {
    data: mapResponse,
    page: apiResponse.currentPage,
    pageSize: apiResponse.pageSize,
    hasNextPage: apiResponse.hasNextPage ?? false,
    hasPreviousPage: apiResponse.hasPreviousPage ?? false,
    totalPages: apiResponse.totalPages
  }
}

export const downloadAtsReport = async (reportId: string): Promise<IGetAtsReportDownloadResponse> => {
  const instance = await backofficeAxiosInstance()
  const response = await instance.get(`ats_registration/${reportId}/download`, { responseType: 'blob' })
  const blob = new Blob([response.data], { type: response.headers['content-type'] });
  const filename = response.headers['content-disposition']
        ?.split('filename=')[1]
        ?.split(';')[0]
        ?.replace(/['"]/g, '') || 'downloaded_file.xlsx';
  return {
    blobData: blob,
    fileName: filename
  }
}

export const uploadAtsRequest = async (payload: IUploadAtsRequest): Promise<IUploadAtsResponse> => {
  try {
    const { uploadFile, uploadType, userName } = payload

    const formdata = new FormData()
    const fields = { uploadFile, uploadType, userName };
    Object.entries(fields).forEach(([key, value]) => formdata.append(key, value));

    const instance = await backofficeAxiosInstance()

    const res = await instance.post('ats_registration/internal/ats/upload', formdata, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    return {
      rowCount: res?.data?.data?.recordCount ?? 0,
      error: null,
    }
  } catch (error) {
    const apiError = error instanceof AxiosError ? error.response?.data as IUploadAtsErrorResponse : null;
    return {
      rowCount: null,
      error: apiError?.detail ?? 'Unknown error'
    }
  }
}