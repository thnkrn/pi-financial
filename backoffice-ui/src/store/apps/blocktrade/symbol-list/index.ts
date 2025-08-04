import APP_CONSTANTS from '@/constants/app'
import { SymbolListInitialState } from '@/constants/blocktrade/InitialState'
import { getSymbolList } from '@/lib/api/clients/blocktrade/symbol'
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import { IGetSymbolListResponse } from '@/lib/api/clients/blocktrade/symbol/types'
import { datadogLogs } from '@datadog/browser-logs'

interface IState {
  data: IGetSymbolListResponse
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  data: SymbolListInitialState,
  isLoading: false,
  errorMessage: '',
}

export const fetchSymbolListData = createAsyncThunk('appBlocktrade/fetchSymbolListData', async () => {
  datadogLogs.logger.info('blocktrade/fetchSymbolListData', {
    action: 'blocktrade/fetchSymbolListData',
    action_status: 'request',
  })

  const response = await getSymbolList()

  return { ...response, isLoading: true }
})

const symbolListSlice = createSlice({
  name: 'symbolList',
  initialState,
  reducers: {
    resetState: () => initialState,
    updateDataState: (state, action) => {
      state.data = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchSymbolListData.pending, state => {
      state.isLoading = true
    })
    builder.addCase(fetchSymbolListData.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error(
        'blocktrade/fetchSymbolListData',
        { action: 'blocktrade/fetchSymbolListData' },
        Error(action?.error?.message)
      )
    })
    builder.addCase(fetchSymbolListData.fulfilled, (state, action) => {
      state.data = action.payload
      state.isLoading = false
      state.errorMessage = ''
    })
  },
})

export const { resetState, updateDataState } = symbolListSlice.actions

export default symbolListSlice.reducer
