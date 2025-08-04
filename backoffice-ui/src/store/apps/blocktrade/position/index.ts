import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import APP_CONSTANTS from 'src/constants/app'
import omitBy from 'lodash/omitBy'
import extend from 'lodash/extend'
import { useRouter } from 'next/router'
import { datadogLogs } from '@datadog/browser-logs'
import { getPortfolio } from '@/lib/api/clients/blocktrade/portfolio'
import { IPortfolioRequest } from '@/lib/api/clients/blocktrade/portfolio/types'
import { IFuturesOrderRequest } from '@/lib/api/clients/blocktrade/allocation/types'
import { IPortfolio } from '@/types/blocktrade/portfolio/types'

interface IState {
  data: IPortfolio[] | []
  filter: {
    total: number | null
    orderBy: string
    orderDir: string
  }
  rollover: boolean
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
  rollover: false,
  lastUpdated: '',
  isLoading: false,
  errorMessage: '',
}

export const fetchPosition = createAsyncThunk('appBlocktrade/fetchPosition', async (params: any) => {
  let currentFilter: Partial<IFuturesOrderRequest> = params

  currentFilter = omitBy(currentFilter, v => {
    return v === 'ALL' || v === null || v === ''
  })

  datadogLogs.logger.info('blocktrade/fetchPosition', {
    action: 'fetchPosition',
    payload: currentFilter,
    action_status: 'request',
  })

  const response = await getPortfolio(currentFilter as IPortfolioRequest)

  return { ...response, isLoading: true }
})

const positionSlice = createSlice({
  name: 'position',
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
    updateRollover: (state, action) => {
      state.rollover = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchPosition.pending, state => {
      state.isLoading = true
    })
    builder.addCase(fetchPosition.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
      datadogLogs.logger.error(
        'blocktrade/fetchPosition',
        { action: 'blocktrade/fetchPosition' },
        Error(action?.error?.message)
      )

      if (action.error.code === 'UNAUTHORIZED') {
        const router = useRouter()
        router.push('/signin')
      }
    })
    builder.addCase(fetchPosition.fulfilled, (state, action) => {
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

      datadogLogs.logger.info('blocktrade/fetchPosition', {
        action: 'blocktrade/fetchPosition',
        action_status: 'success',
      })
    })
  },
})

export const { resetState, updateFilterState, updateLastUpdated, updateRollover } = positionSlice.actions

export default positionSlice.reducer
