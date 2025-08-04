import APP_CONSTANTS from '@/constants/app'
import { getReports } from '@/lib/api/clients/backoffice/report'
import { IGetReportsRequest } from '@/lib/api/clients/backoffice/report/types'
import { datadogLogs } from '@datadog/browser-logs'
import { PiBackofficeServiceAPIModelsReportResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsReportResponse'
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import extend from 'lodash/extend'
import isNil from 'lodash/isNil'
import omitBy from 'lodash/omitBy'

interface IState {
  reports: PiBackofficeServiceAPIModelsReportResponse[] | []
  filter: {
    total: number | null
    orderBy: string
    orderDir: string
  }
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  reports: [],
  filter: {
    total: null,
    orderBy: 'createdAt',
    orderDir: 'desc',
  },
  isLoading: false,
  errorMessage: '',
}

export const fetchReport = createAsyncThunk('appReport/fetchReport', async (filter: IGetReportsRequest) => {
  const currentFilter: Partial<IGetReportsRequest> = omitBy(filter, isNil)

  datadogLogs.logger.info('fetchReport', { action: 'fetchReport', payload: currentFilter, action_status: 'request' })

  const response = await getReports(currentFilter as IGetReportsRequest)

  return { ...response, isLoading: true }
})

const reportSlice = createSlice({
  name: 'report',
  initialState,
  reducers: {
    resetState: (state, action) => {
      state.reports = []
      state.filter = action.payload
      state.isLoading = false
    },
    updateFilterState: (state, action) => {
      state.filter = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchReport.pending, state => {
      state.isLoading = true
    })

    builder.addCase(fetchReport.rejected, (state, action) => {
      state.reports = []
      state.isLoading = false
      state.errorMessage = action.error.message ?? APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error('fetchReport', { action: 'fetchReport' }, Error(action?.error?.message))
    })

    builder.addCase(fetchReport.fulfilled, (state, action) => {
      state.reports = action.payload.reports
      state.filter = extend({}, state.filter, {
        page: action.payload.page,
        pageSize: action.payload.pageSize,
        total: action.payload.total,
        orderBy: action.payload.orderBy,
        orderDir: action.payload.orderDir,
      })
      state.isLoading = false
      state.errorMessage = ''

      datadogLogs.logger.info('fetchReport', { action: 'fetchReport', action_status: 'success' })
    })
  },
})

export const { resetState, updateFilterState } = reportSlice.actions

export default reportSlice.reducer
