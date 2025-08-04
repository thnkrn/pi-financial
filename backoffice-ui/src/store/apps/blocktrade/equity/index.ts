import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import APP_CONSTANTS from '@/constants/app'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'
import { IEquityOrder, IEquityOrderRequest } from '@/lib/api/clients/blocktrade/equity/types'
import { datadogLogs } from '@datadog/browser-logs'
import { getEquityOrders } from '@/lib/api/clients/blocktrade/equity'

interface IState {
  data: IEquityOrder[] | []
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

export const fetchEquityOrder = createAsyncThunk(
  'appBlocktrade/fetchEquityOrder',
  async (params: IEquityOrderRequest) => {
    let currentFilter: Partial<IEquityOrderRequest> = params

    currentFilter = omitBy(currentFilter, v => {
      return v === 'ALL' || v === null || v === ''
    })

    datadogLogs.logger.info('blocktrade/fetchEquityOrder', {
      action: 'blocktrade/fetchEquityOrder',
      payload: currentFilter,
      action_status: 'request',
    })

    const response = await getEquityOrders(currentFilter as IEquityOrderRequest)

    return { ...response, isLoading: true }
  }
)

const equitySlice = createSlice({
  name: 'equity',
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
    builder.addCase(fetchEquityOrder.pending, state => {
      state.isLoading = true
    })
    builder.addCase(fetchEquityOrder.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
    })
    builder.addCase(fetchEquityOrder.fulfilled, (state, action) => {
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

export const { resetState, updateFilterState, updateLastUpdated } = equitySlice.actions

export default equitySlice.reducer
