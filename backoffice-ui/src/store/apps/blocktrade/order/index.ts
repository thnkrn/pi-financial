import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import APP_CONSTANTS from 'src/constants/app'
import {
  submitAmendEquityOrder,
  submitCancelEquityOrder,
  submitNewEquityOrder,
} from '@/lib/api/clients/blocktrade/equity'
import { ISubmitAmendOrderRequest, ISubmitNewOrderRequest } from '@/lib/api/clients/blocktrade/equity/types'
import { datadogLogs } from '@datadog/browser-logs'

interface IState {
  isAmend: boolean | null
  id: number | null
  ic: number | null
  oc: string | null
  side: string | null
  symbol: string | null
  futuresProperty: {
    id: number | null
    symbolId: number | null
    symbol: string | null
    seriesId: number | null
    series: string | null
    blocksize: number | null
    mm: number | null
    multiplier: number | null
    expDate: string | null
    createdAt: string | null
    updatedAt: string | null
  } | null
  customer: string | null
  contractAmount: number | null
  equityPrice: number | null
  orderType: string
  futuresPrice: number | null
  orderStatus: string | null
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  isAmend: null,
  id: null,
  ic: null,
  oc: null,
  side: null,
  symbol: null,
  futuresProperty: null,
  customer: null,
  contractAmount: null,
  equityPrice: null,
  orderType: 'LIMIT',
  futuresPrice: null,
  orderStatus: null,
  isLoading: false,
  errorMessage: '',
}

export const submitNewOrder = createAsyncThunk(
  'appBlocktrade/submitNewEquityOrder',
  async (orderData: ISubmitNewOrderRequest) => {
    datadogLogs.logger.info('blocktrade/submitNewEquityOrder', {
      action: 'blocktrade/submitNewEquityOrder',
      payload: orderData,
      action_status: 'request',
    })

    const response = await submitNewEquityOrder(orderData)

    return response.data
  }
)

export const submitAmendOrder = createAsyncThunk(
  'appBlocktrade/submitAmendEquityOrder',
  async ({ id, orderData }: { id: number; orderData: ISubmitAmendOrderRequest }) => {
    datadogLogs.logger.info('blocktrade/submitAmendEquityOrder', {
      action: 'blocktrade/submitAmendEquityOrder',
      payload: { orderId: id, payload: orderData },
      action_status: 'request',
    })

    const response = await submitAmendEquityOrder(id, orderData)

    return response.data
  }
)

export const submitCancelOrder = createAsyncThunk(
  'appBlocktrade/submitCancelEquityOrder',
  async ({ id }: { id: number }) => {
    datadogLogs.logger.info('blocktrade/submitCancelEquityOrder', {
      action: 'blocktrade/submitCancelEquityOrder',
      payload: { orderId: id },
      action_status: 'request',
    })

    const response = await submitCancelEquityOrder(id)

    return response.data
  }
)

const orderSlice = createSlice({
  name: 'order',
  initialState,
  reducers: {
    resetState: () => initialState,
    updateId: (state, action) => {
      state.id = action.payload
    },
    updateIC: (state, action) => {
      state.ic = action.payload
    },
    updateSymbol: (state, action) => {
      state.symbol = action.payload
    },
    updateFuturesProperty: (state, action) => {
      state.futuresProperty = action.payload
    },
    updateCustomer: (state, action) => {
      state.customer = action.payload
    },
    updateContractAmount: (state, action) => {
      state.contractAmount = action.payload
    },
    updateEquityPrice: (state, action) => {
      state.equityPrice = action.payload
    },
    updateOrderType: (state, action) => {
      state.orderType = action.payload
    },
    updateFuturesPrice: (state, action) => {
      state.futuresPrice = action.payload
    },
    updateOrderStatus: (state, action) => {
      state.orderStatus = action.payload
    },
    updateData: (state, action) => {
      Object.assign(state, action.payload)
    },
  },
  extraReducers: builder => {
    builder.addCase(submitNewOrder.pending, state => {
      state.isLoading = true
    })
    builder.addCase(submitNewOrder.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error(
        'blocktrade/submitNewEquityOrder',
        { action: 'submitNewEquityOrder' },
        Error(action?.error?.message)
      )
    })
    builder.addCase(submitNewOrder.fulfilled, state => {
      state.isLoading = false
      state.errorMessage = ''
    })
    builder.addCase(submitAmendOrder.pending, state => {
      state.isLoading = true
    })
    builder.addCase(submitAmendOrder.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error(
        'blocktrade/submitAmendEquityOrder',
        { action: 'submitAmendEquityOrder' },
        Error(action?.error?.message)
      )
    })
    builder.addCase(submitAmendOrder.fulfilled, state => {
      state.isLoading = false
      state.errorMessage = ''
    })
    builder.addCase(submitCancelOrder.pending, state => {
      state.isLoading = true
    })
    builder.addCase(submitCancelOrder.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error(
        'blocktrade/submitCancelEquityOrder',
        { action: 'submitCancelEquityOrder' },
        Error(action?.error?.message)
      )
    })
    builder.addCase(submitCancelOrder.fulfilled, state => {
      state.isLoading = false
      state.errorMessage = ''
    })
  },
})

export const {
  updateData,
  updateOrderStatus,
  updateIC,
  updateId,
  updateSymbol,
  updateCustomer,
  updateContractAmount,
  updateEquityPrice,
  updateFuturesPrice,
  updateFuturesProperty,
  updateOrderType,
  resetState,
} = orderSlice.actions

export default orderSlice.reducer
