import APP_CONSTANTS from '@/constants/app'
import { getDropdown } from '@/lib/api/clients/backoffice/dropdown'
import { IGetDropdownRequest } from '@/lib/api/clients/backoffice/dropdown/types'
import { PiBackofficeServiceAPIModelsNameAliasResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsNameAliasResponse'
import { PiBackofficeServiceAPIModelsResponseCodeResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsResponseCodeResponse'
import { PiBackofficeServiceApplicationModelsReportTypeNameAliasResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsReportTypeNameAliasResponse'
import { PiBackofficeServiceDomainAggregateModelsTransactionAggregateBank } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceDomainAggregateModelsTransactionAggregateBank'
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'

export interface IState {
  values: {
    [key: string]:
      | PiBackofficeServiceAPIModelsResponseCodeResponse[]
      | PiBackofficeServiceApplicationModelsReportTypeNameAliasResponse[]
      | PiBackofficeServiceDomainAggregateModelsTransactionAggregateBank[]
      | PiBackofficeServiceAPIModelsNameAliasResponse[]
  }
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  values: {},
  isLoading: false,
  errorMessage: '',
}

export const fetchDropdown = createAsyncThunk(
  'appDropdown/fetchDropdown',
  async (input: { field: string; url: string }) => {
    const response = await getDropdown({ field: input.field, url: input.url } as IGetDropdownRequest)

    return { values: response, isLoading: true }
  }
)

const dropdownSlice = createSlice({
  name: 'dropdown',
  initialState,
  reducers: {},
  extraReducers: builder => {
    builder.addCase(fetchDropdown.pending, state => {
      state.isLoading = true
    })
    builder.addCase(fetchDropdown.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
    })
    builder.addCase(fetchDropdown.fulfilled, (state: IState, action) => {
      state.values[action.payload.values.field] = action.payload.values.data
      state.isLoading = false
      state.errorMessage = ''
    })
  },
})

export default dropdownSlice.reducer
