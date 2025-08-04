import { backofficeAxiosInstance } from '@/lib/api'
import { AxiosResponse } from 'axios'
import { IDownloadReportResponse, IGenerateReportRequest, IGetReportsRequest, IGetReportsResponse } from './types'

/**
 * Retrieves reports based on the provided filter.
 *
 * @param {IGetReportsRequest} filter - The filter for the reports
 * @return {Promise<IGetReportsResponse>} The reports and related metadata
 */
export const getReports = async (filter: IGetReportsRequest): Promise<IGetReportsResponse> => {
  // NOTE: need to define as any type since URLSearchParams only accept type string but we prefer to use our own type for filter
  const objString = '?' + new URLSearchParams(filter as any).toString()

  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`reports${objString}`)

  return {
    reports: res?.data?.data,
    page: res?.data?.page,
    pageSize: res?.data?.pageSize,
    total: res?.data?.total,
    orderBy: res?.data?.orderBy,
    orderDir: res?.data?.orderDir,
  }
}

/**
 * Downloads a report with the given report ID.
 *
 * @param {string} reportId - The ID of the report to be downloaded
 * @return {Promise<IDownloadReportResponse>} The download URL of the report
 */
export const downloadReport = async (reportId: string): Promise<IDownloadReportResponse> => {
  const instance = await backofficeAxiosInstance()
  const res = await instance.get(`reports/${reportId}/download_url`)

  return {
    URL: res?.data?.data,
  }
}

/**
 * Generates a report using the provided payload.
 *
 * @param {IGenerateReportRequest} payload - the request payload for generating the report
 * @return {Promise<AxiosResponse<any, any>>} a promise that resolves to the response from the report generation endpoint
 */
export const generateReport = async (payload: IGenerateReportRequest): Promise<AxiosResponse<any, any>> => {
  const instance = await backofficeAxiosInstance()

  return await instance.post('reports', payload)
}

/**
 * Downloads a Pi app dw daily report with the given date range.
 *
 * @param {string} dateFrom
 * @param {string} dateTo
 */
export const downloadPiAppDwDailyReport = async (dateFrom: string,  dateTo: string, reportType: string)=> {
  const instance = await backofficeAxiosInstance()
  try {
    const response: AxiosResponse<Blob> = await instance.get(
      `reports/pi-app-dw-daily/download?dateFrom=${dateFrom}&dateTo=${dateTo}&reportType=${reportType}`,
      { responseType: 'blob' }
    );

    const fileName = `${reportType}_${dateFrom}-${dateTo}.csv`;
    const url = window.URL.createObjectURL(new Blob([response.data], { type: 'application/octet-stream' }));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', fileName);
    document.body.appendChild(link);

    link.click();

    window.URL.revokeObjectURL(url);
    document.body.removeChild(link);
  } catch (error) {
    throw new Error('Unexpected error from download Pi App DW Daily report')
  }
}
