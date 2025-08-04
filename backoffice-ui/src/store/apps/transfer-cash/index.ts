import APP_CONSTANTS from '@/constants/app'
import { datadogLogs } from '@datadog/browser-logs'
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'
import { PiBackofficeServiceAPIModelsTransferCashResponse } from '@pi-financial/backoffice-srv'
import {
  IGetTransferCashTransactionsRequest,
  ITransferCashFilters,
} from '@/lib/api/clients/backoffice/transactions/types'
import { getTransferCashTransactions } from '@/lib/api/clients/backoffice/transactions'

export interface IState {
  transactions: PiBackofficeServiceAPIModelsTransferCashResponse[] | []
  filter: {
    total: number | null
    filters:
      | ITransferCashFilters
      | {
          status?: string
          transferFromExchangeMarket?: string
          transferToExchangeMarket?: string
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

export const fetchTransferCashTransactions = createAsyncThunk(
  'appTransfer/fetchTransferCashTransactions',
  async (params: IGetTransferCashTransactionsRequest) => {
    const currentFilter: IGetTransferCashTransactionsRequest = params

    const transferCashFilters: ITransferCashFilters = omitBy(currentFilter.filters, v => {
      return v === 'ALL' || v === null || v === ''
    })

    const payload: IGetTransferCashTransactionsRequest = extend(
      {},
      { ...currentFilter },
      { filters: transferCashFilters }
    )

    datadogLogs.logger.info('fetchTransferCashTransactions', {
      action: 'fetchTransferCashTransactions',
      payload,
      action_status: 'request',
    })

    const response = await getTransferCashTransactions(payload)

    return { ...response, isLoading: true }
  }
)

const transferCashSlice = createSlice({
  name: 'transferCash',
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
    builder.addCase(fetchTransferCashTransactions.pending, state => {
      state.isLoading = true
    })

    builder.addCase(fetchTransferCashTransactions.rejected, (state, action) => {
      state.transactions = []
      state.isLoading = false
      state.errorMessage = action.error.message ?? APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error(
        'fetchTransferCashTransactions',
        { action: 'fetchTransferCashTransactions' },
        Error(action?.error?.message)
      )
    })

    builder.addCase(fetchTransferCashTransactions.fulfilled, (state, action) => {
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

      datadogLogs.logger.info('fetchTransferCashTransactions', {
        action: 'fetchTransferCashTransactions',
        action_status: 'success',
      })
    })
  },
})

export const { resetState, updateFilterState } = transferCashSlice.actions

export default transferCashSlice.reducer
