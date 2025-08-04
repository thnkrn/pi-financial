import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import APP_CONSTANTS from '@/constants/app'
import { CustomerListInitialState } from '@/constants/blocktrade/InitialState'
import { getCustomerList } from '@/lib/api/clients/blocktrade/customers'
import { IGetCustomerListResponse } from '@/lib/api/clients/blocktrade/customers/types'
import { datadogLogs } from '@datadog/browser-logs'

interface IState {
  data: IGetCustomerListResponse
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  data: CustomerListInitialState,
  isLoading: false,
  errorMessage: '',
}

export const fetchCustomerListData = createAsyncThunk('appBlocktrade/fetchCustomerListData', async (icId: number) => {
  datadogLogs.logger.info('blocktrade/fetchCustomerListData', {
    action: 'blocktrade/fetchCustomerListData',
    payload: { icId: icId },
    action_status: 'request',
  })

  const response = await getCustomerList(icId)

  return { ...response, isLoading: true }
})

const customerListSlice = createSlice({
  name: 'customerList',
  initialState,
  reducers: {
    resetState: () => initialState,
    updateDataState: (state, action) => {
      state.data = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchCustomerListData.pending, state => {
      state.isLoading = true
    })
    builder.addCase(fetchCustomerListData.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error(
        'blocktrade/fetchCustomerListData',
        { action: 'blocktrade/fetchCustomerListData' },
        Error(action?.error?.message)
      )
    })
    builder.addCase(fetchCustomerListData.fulfilled, (state, action) => {
      state.data = action.payload
      state.isLoading = false
      state.errorMessage = ''
    })
  },
})

export const { resetState, updateDataState } = customerListSlice.actions

export default customerListSlice.reducer
