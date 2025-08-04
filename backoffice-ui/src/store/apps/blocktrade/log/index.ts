import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import APP_CONSTANTS from 'src/constants/app'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'
import { IActivityLog, IActivityLogRequest } from '@/lib/api/clients/blocktrade/activity-logs/types'
import { datadogLogs } from '@datadog/browser-logs'
import { getActivityLog } from '@/lib/api/clients/blocktrade/activity-logs'

interface IState {
  data: IActivityLog[] | []
  filter: {
    page: number
    pageSize: number
    orderBy: string
    orderDir: string
  }
  lastUpdated: string
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  data: [],
  filter: {
    page: 1,
    pageSize: 200,
    orderBy: 'createdAt',
    orderDir: 'desc',
  },
  lastUpdated: '',
  isLoading: false,
  errorMessage: '',
}

export const fetchActivityLog = createAsyncThunk(
  'appBlocktrade/fetchActivityLog',
  async (params: IActivityLogRequest) => {
    let currentFilter: Partial<IActivityLogRequest> = params

    currentFilter = omitBy(currentFilter, v => {
      return v === 'ALL' || v === null || v === ''
    })

    datadogLogs.logger.info('blocktrade/fetchActivityLog', {
      action: 'blocktrade/fetchActivityLog',
      payload: currentFilter,
      action_status: 'request',
    })

    const response = await getActivityLog(currentFilter as IActivityLogRequest)

    return { ...response, isLoading: true }
  }
)

const logSlice = createSlice({
  name: 'log',
  initialState,
  reducers: {
    resetState: (state, action) => {
      state.data = []
      state.filter = action.payload
      state.isLoading = false
    },
    updateFilterState: (state, action) => {
      state.filter = action.payload
    },
    updateLastUpdated: (state, action) => {
      state.lastUpdated = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchActivityLog.pending, state => {
      state.isLoading = true
    })
    builder.addCase(fetchActivityLog.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
    })
    builder.addCase(fetchActivityLog.fulfilled, (state, action) => {
      state.data = action.payload.data
      state.filter = extend({}, state.filter, {
        page: action.payload.page,
        pageSize: action.payload.pageSize,
        total: action.payload.total,
        orderBy: action.payload.orderBy,
        orderDir: action.payload.orderDir,
      })
      state.isLoading = false
      state.errorMessage = ''
    })
  },
})

export const { resetState, updateFilterState, updateLastUpdated } = logSlice.actions

export default logSlice.reducer
