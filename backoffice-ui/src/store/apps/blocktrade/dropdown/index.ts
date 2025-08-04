import APP_CONSTANTS from '@/constants/app'
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import { getDropdown } from '@/lib/api/clients/blocktrade/dropdown'
import {
  IGetCloseCustomersResponse,
  IGetCloseSeriesResponse,
  IGetDropdownRequest,
  IGetMemberInfoFromGroupsResponse,
  IGetTeamsResponse,
  ISeriesGetListResponse,
} from '@/lib/api/clients/blocktrade/dropdown/types'

export interface IState {
  values: {
    [key: string]:
      | IGetTeamsResponse[]
      | IGetMemberInfoFromGroupsResponse[]
      | IGetCloseCustomersResponse[]
      | ISeriesGetListResponse[]
      | IGetCloseSeriesResponse[]
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
  'appBlocktrade/fetchDropdown',
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
