import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import { useRouter } from 'next/router'
import APP_CONSTANTS from '@/constants/app'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'
import { getFuturesOrders } from '@/lib/api/clients/blocktrade/allocation'
import { IFuturesOrderRequest } from '@/lib/api/clients/blocktrade/allocation/types'
import { datadogLogs } from '@datadog/browser-logs'
import { IFuturesOrder } from '@/types/blocktrade/futures/types'

interface IState {
  data: IFuturesOrder[] | []
  filter: {
    total: number | null
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
    total: null,
    orderBy: 'createdAt',
    orderDir: 'desc',
  },
  lastUpdated: '',
  isLoading: false,
  errorMessage: '',
}

export const fetchFuturesOrder = createAsyncThunk(
  'appBlocktrade/fetchFuturesOrder',
  async (params: IFuturesOrderRequest) => {
    let currentFilter: Partial<IFuturesOrderRequest> = params

    currentFilter = omitBy(currentFilter, v => {
      return v === 'ALL' || v === null || v === ''
    })

    datadogLogs.logger.info('blocktrade/fetchFuturesOrders', {
      action: 'blocktrade/fetchFuturesOrders',
      payload: currentFilter,
      action_status: 'request',
    })

    const response = await getFuturesOrders(currentFilter as IFuturesOrderRequest)

    return { ...response, isLoading: true }
  }
)

const futuresSlice = createSlice({
  name: 'futures',
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
    builder.addCase(fetchFuturesOrder.pending, state => {
      state.isLoading = true
    })

    builder.addCase(fetchFuturesOrder.rejected, (state, action) => {
      state.data = []
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
      datadogLogs.logger.error(
        'blocktrade/fetchFuturesOrders',
        { action: 'blocktrade/fetchFuturesOrders' },
        Error(action?.error?.message)
      )

      if (action.error.code === 'UNAUTHORIZED') {
        const router = useRouter()
        router.push('/signin')
      }
    })

    builder.addCase(fetchFuturesOrder.fulfilled, (state, action) => {
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

      datadogLogs.logger.info('blocktrade/fetchFuturesOrders', {
        action: 'blocktrade/fetchFuturesOrders',
        action_status: 'success',
      })
    })
  },
})

export const { resetState, updateFilterState, updateLastUpdated } = futuresSlice.actions

export default futuresSlice.reducer
