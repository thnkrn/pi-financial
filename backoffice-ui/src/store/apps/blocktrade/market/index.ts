import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import APP_CONSTANTS from 'src/constants/app'
import { MarketDataInitialState } from '@/constants/blocktrade/InitialState'
import { getOrderBook } from '@/lib/api/clients/blocktrade/market'
import { datadogLogs } from '@datadog/browser-logs'
import { IOrderBookResponse } from '@/lib/api/clients/blocktrade/market/types'

interface IState {
  symbol: string | null
  data: IOrderBookResponse
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  symbol: null,
  data: MarketDataInitialState,
  isLoading: false,
  errorMessage: '',
}

export const fetchMarketData = createAsyncThunk('appBlocktrade/fetchMarketData', async (symbol: string) => {
  datadogLogs.logger.info('blocktrade/fetchMarketData', {
    action: 'blocktrade/fetchMarketData',
    payload: { symbol: symbol },
    action_status: 'request',
  })

  const response = await getOrderBook({ symbol: symbol })

  return { ...response, isLoading: true }
})

const marketSlice = createSlice({
  name: 'market',
  initialState,
  reducers: {
    resetState: () => initialState,
    updateMktSymbol: (state, action) => {
      state.symbol = action.payload
    },
    updateDataState: (state, action) => {
      state.data = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchMarketData.pending, state => {
      state.isLoading = true
      state.data = MarketDataInitialState
    })
    builder.addCase(fetchMarketData.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
    })
    builder.addCase(fetchMarketData.fulfilled, (state, action) => {
      state.data = action.payload
      state.isLoading = false
      state.errorMessage = ''
    })
  },
})

export const { resetState, updateMktSymbol, updateDataState } = marketSlice.actions

export default marketSlice.reducer
