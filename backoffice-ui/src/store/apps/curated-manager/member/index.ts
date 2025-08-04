import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit'

import APP_CONSTANTS from '@/constants/app'
import { getCuratedMembersByCuratedId } from '@/lib/api/clients/backoffice/curated-manager'
import { CuratedMemberState } from '@/types/curated-manager/member'

const initialState: CuratedMemberState = {
  isLoading: false,
  errorMessage: '',
  members: [],
}

export const fetchCuratedMembers = createAsyncThunk(
  'curatedMembers/fetchCuratedMembers',
  async (curatedListId: number) => {
    const response = await getCuratedMembersByCuratedId(curatedListId)

    return response
  }
)

const curatedMemberSlice = createSlice({
  name: 'curatedMembers',
  initialState,
  reducers: {
    updateCuratedMemberState: (state, action: PayloadAction<CuratedMemberState>) => {
      state.members = action.payload.members
    },
    resetState: state => {
      state.isLoading = false
      state.errorMessage = ''
      state.members = []
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchCuratedMembers.pending, state => {
      state.isLoading = true
    })
    builder.addCase(fetchCuratedMembers.fulfilled, (state, action) => {
      state.isLoading = false
      state.members = action.payload.data
    })
    builder.addCase(fetchCuratedMembers.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE
    })
  },
})

export const { resetState, updateCuratedMemberState } = curatedMemberSlice.actions
export default curatedMemberSlice.reducer
