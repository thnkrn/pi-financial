import { createSlice } from '@reduxjs/toolkit'

const initialState = {
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
  commFee: 0.1,
  openDate: new Date(),
  closeDate: new Date(),
  xd: 0,
  intRate: 5.5,
  minDay: 4,
  openPrice: 0,
  closePrice: 0,
  isLoading: false,
  errorMessage: '',
}

const calculatorSlice = createSlice({
  name: 'calculator',
  initialState,
  reducers: {
    resetState: () => initialState,
    updateId: (state, action) => {
      state.id = action.payload
    },
    updateIC: (state, action) => {
      state.ic = action.payload
    },
    updateSide: (state, action) => {
      state.side = action.payload
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
    updateCommFee: (state, action) => {
      state.commFee = action.payload
    },
    updateOpenDate: (state, action) => {
      state.openDate = action.payload
    },
    updateCloseDate: (state, action) => {
      state.closeDate = action.payload
    },
    updateXD: (state, action) => {
      state.xd = action.payload
    },
    updateIntRate: (state, action) => {
      state.intRate = action.payload
    },
    updateMinDay: (state, action) => {
      state.minDay = action.payload
    },
    updateOpenPrice: (state, action) => {
      state.openPrice = action.payload
    },
    updateClosePrice: (state, action) => {
      state.closePrice = action.payload
    },
    updateData: (state, action) => {
      Object.assign(state, action.payload)
    },
  },
})

export const {
  updateData,
  updateOrderStatus,
  updateIC,
  updateSide,
  updateId,
  updateSymbol,
  updateCustomer,
  updateContractAmount,
  updateEquityPrice,
  updateFuturesPrice,
  updateFuturesProperty,
  updateOrderType,
  updateCommFee,
  updateOpenDate,
  updateCloseDate,
  updateXD,
  updateIntRate,
  updateMinDay,
  updateOpenPrice,
  updateClosePrice,
  resetState,
} = calculatorSlice.actions

export default calculatorSlice.reducer
