import APP_CONSTANTS from '@/constants/app'
import {
  deleteCuratedListById,
  getCuratedList,
  updateCuratedListById,
  uploadCuratedManualList,
} from '@/lib/api/clients/backoffice/curated-manager'
import {
  ActionType,
  CuratedListState,
  CuratedListTable,
  CuratedType,
  UpdateCuratedList,
  UploadCuratedManualList,
} from '@/types/curated-manager/list'
import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit'

const initialState: CuratedListState = {
  activeTab: CuratedType.Manual,
  isLoading: false,
  errorMessage: '',
  logicalList: [],
  manualList: [],
  successAction: ActionType.NONE,
}

export const fetchCuratedList = createAsyncThunk('curatedList/fetchCuratedList', async () => {
  const response = await getCuratedList()

  return response
})

export const uploadCuratedList = createAsyncThunk(
  'curatedList/uploadCuratedManualList',
  async (uploadValue: UploadCuratedManualList) => {
    await uploadCuratedManualList(uploadValue.formData, uploadValue.dataSource)

    const response = await getCuratedList()

    return response
  }
)

export const updateCuratedList = createAsyncThunk(
  'curatedList/updateCuratedList',
  async (updateValue: UpdateCuratedList) => {
    const { id, dataSource, ...data } = updateValue

    await updateCuratedListById(id, data, dataSource)

    const response = await getCuratedList()

    return response
  }
)

export const deleteCuratedList = createAsyncThunk(
  'curatedList/deleteCuratedList',
  async (curatedList: CuratedListTable) => {
    await deleteCuratedListById(curatedList.id, curatedList.curatedListSource)

    const response = await getCuratedList()

    return response
  }
)

const curatedListSlice = createSlice({
  name: 'curatedList',
  initialState,
  reducers: {
    setActiveTab: (state, action: PayloadAction<CuratedType>) => {
      state.activeTab = action.payload
    },
    updateCuratedListState: (state, action: PayloadAction<CuratedListState>) => {
      state.logicalList = action.payload.logicalList
      state.manualList = action.payload.manualList
    },
    resetState: state => {
      state.activeTab = CuratedType.Manual
      state.isLoading = false
      state.errorMessage = ''
      state.logicalList = []
      state.manualList = []
      state.successAction = ActionType.NONE
    },
    clearSuccessAction: state => {
      state.successAction = ActionType.NONE
    },
  },
  extraReducers: builder => {
    builder
      .addCase(fetchCuratedList.pending, state => {
        state.isLoading = true
        state.logicalList = []
        state.manualList = []
        state.errorMessage = ''
      })
      .addCase(fetchCuratedList.fulfilled, (state, action) => {
        state.isLoading = false
        state.logicalList = action.payload.data.logical
        state.manualList = action.payload.data.manual
        state.errorMessage = ''
      })
      .addCase(fetchCuratedList.rejected, (state, action) => {
        state.isLoading = false
        state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
        state.logicalList = []
        state.manualList = []
      })
      .addCase(uploadCuratedList.pending, state => {
        state.isLoading = true
        state.errorMessage = ''
        state.successAction = ActionType.NONE
      })
      .addCase(uploadCuratedList.fulfilled, (state, action) => {
        state.isLoading = false
        state.errorMessage = ''
        state.successAction = ActionType.UPLOAD
        state.logicalList = action.payload.data.logical
        state.manualList = action.payload.data.manual
      })
      .addCase(uploadCuratedList.rejected, (state, action) => {
        state.isLoading = false
        state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
        state.successAction = ActionType.NONE
      })
      .addCase(updateCuratedList.pending, state => {
        state.isLoading = true
        state.errorMessage = ''
        state.successAction = ActionType.NONE
      })
      .addCase(updateCuratedList.fulfilled, (state, action) => {
        state.isLoading = false
        state.errorMessage = ''
        state.successAction = ActionType.UPDATE
        state.logicalList = action.payload.data.logical
        state.manualList = action.payload.data.manual
      })
      .addCase(updateCuratedList.rejected, (state, action) => {
        state.isLoading = false
        state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
        state.successAction = ActionType.NONE
      })
      .addCase(deleteCuratedList.pending, state => {
        state.isLoading = true
        state.errorMessage = ''
        state.successAction = ActionType.NONE
      })
      .addCase(deleteCuratedList.fulfilled, (state, action) => {
        state.isLoading = false
        state.errorMessage = ''
        state.successAction = ActionType.DELETE
        state.logicalList = action.payload.data.logical
        state.manualList = action.payload.data.manual
      })
      .addCase(deleteCuratedList.rejected, (state, action) => {
        state.isLoading = false
        state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
        state.successAction = ActionType.NONE
      })
  },
})

export const { setActiveTab, updateCuratedListState, resetState, clearSuccessAction } = curatedListSlice.actions
export default curatedListSlice.reducer
