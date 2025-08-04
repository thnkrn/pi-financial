import APP_CONSTANTS from '@/constants/app'
import { getWithdrawTransactions } from '@/lib/api/clients/backoffice/withdraw'
import { IGetWithdrawTransactionsRequest, IWithdrawFilters } from '@/lib/api/clients/backoffice/withdraw/types'
import { datadogLogs } from '@datadog/browser-logs'
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'
import { PiBackofficeServiceAPIModelsTransactionHistoryV2Response } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransactionHistoryV2Response'

export interface IState {
  transactions: PiBackofficeServiceAPIModelsTransactionHistoryV2Response[] | []
  filter: {
    total: number | null
    filters:
      | IWithdrawFilters
      | {
          accountType?: string
          channel?: string
          bankCode?: string
          status?: string
          responseCodeId?: string
        }
    orderBy: string
    orderDir: string
    page: number
    pageSize: number
  }
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  transactions: [],
  filter: {
    total: null,
    filters: {},
    orderBy: 'createdAt',
    orderDir: 'desc',
    page: 1,
    pageSize: 20,
  },
  isLoading: false,
  errorMessage: '',
}

export const fetchWithdraws = createAsyncThunk(
  'appWithdraw/fetchWithdraws',
  async (params: IGetWithdrawTransactionsRequest) => {
    const currentFilter: IGetWithdrawTransactionsRequest = params

    const withdrawFilters: IWithdrawFilters = omitBy(currentFilter.filters, v => {
      return v === 'ALL' || v === null || v === ''
    })

    const payload: IGetWithdrawTransactionsRequest = extend({}, { ...currentFilter }, { filters: withdrawFilters })

    datadogLogs.logger.info('fetchDeposits', { action: 'fetchDeposits', payload, action_status: 'request' })

    const response = await getWithdrawTransactions(payload)

    return { ...response, isLoading: true }
  }
)

const withdrawSlice = createSlice({
  name: 'withdraw',
  initialState,
  reducers: {
    resetState: (state, action) => {
      state.transactions = []
      state.filter = action.payload
      state.isLoading = false
    },
    updateFilterState: (state, action) => {
      state.filter = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchWithdraws.pending, state => {
      state.isLoading = true
    })

    builder.addCase(fetchWithdraws.rejected, (state, action) => {
      state.transactions = []
      state.isLoading = false
      state.errorMessage = action.error.message ?? APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error('fetchWithdraws', { action: 'fetchWithdraws' }, Error(action?.error?.message))
    })

    builder.addCase(fetchWithdraws.fulfilled, (state, action) => {
      state.transactions = action.payload.transactions
      state.filter = extend({}, state.filter, {
        page: action.payload.page,
        pageSize: action.payload.pageSize,
        total: action.payload.total,
        orderBy: action.payload.orderBy,
        orderDir: action.payload.orderDir,
      })
      state.isLoading = false
      state.errorMessage = ''

      datadogLogs.logger.info('fetchWithdraws', { action: 'fetchWithdraws', action_status: 'success' })
    })
  },
})

export const { resetState, updateFilterState } = withdrawSlice.actions

export default withdrawSlice.reducer
