import APP_CONSTANTS from '@/constants/app'
import { getCuratedFilterGroups, uploadCuratedFilters } from '@/lib/api/clients/backoffice/curated-manager'
import { CuratedFilterState, FilterTabType, UploadCuratedFilters } from '@/types/curated-manager/filter'
import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit'

const initialState: CuratedFilterState = {
  activeTab: 'thaiEquities',
  filterGroups: [],
  isLoading: false,
  errorMessage: '',
  isUploadSuccess: false,
}

export const fetchFilterGroups = createAsyncThunk('curatedFilter/fetchFilterGroups', async (groupName: string) => {
  const response = await getCuratedFilterGroups({ groupName })

  return response
})

export const uploadCuratedFiltersThunk = createAsyncThunk(
  'curatedFilter/uploadCuratedFilters',
  async (uploadValue: UploadCuratedFilters) => {
    await uploadCuratedFilters(uploadValue.formData, uploadValue.dataSource)

    const response = await getCuratedFilterGroups({ groupName: uploadValue.groupName })

    return response
  }
)

const curatedFilterSlice = createSlice({
  name: 'curatedFilter',
  initialState,
  reducers: {
    setActiveTab: (state, action: PayloadAction<FilterTabType>) => {
      state.activeTab = action.payload
      state.filterGroups = []
    },
    resetState: () => initialState,
  },
  extraReducers: builder => {
    builder
      .addCase(fetchFilterGroups.pending, state => {
        state.isLoading = true
        state.filterGroups = []
        state.errorMessage = ''
      })
      .addCase(fetchFilterGroups.fulfilled, (state, action) => {
        state.filterGroups = action.payload
        state.isLoading = false
        state.errorMessage = ''
      })
      .addCase(fetchFilterGroups.rejected, (state, action) => {
        state.isLoading = false
        state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
        state.filterGroups = []
      })
      .addCase(uploadCuratedFiltersThunk.pending, state => {
        state.isLoading = true
        state.errorMessage = ''
        state.isUploadSuccess = false
      })
      .addCase(uploadCuratedFiltersThunk.fulfilled, (state, action) => {
        state.isLoading = false
        state.errorMessage = ''
        state.isUploadSuccess = true
        state.filterGroups = action.payload
      })
      .addCase(uploadCuratedFiltersThunk.rejected, (state, action) => {
        state.isLoading = false
        state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
        state.isUploadSuccess = false
      })
  },
})

export const { setActiveTab, resetState } = curatedFilterSlice.actions
export default curatedFilterSlice.reducer
