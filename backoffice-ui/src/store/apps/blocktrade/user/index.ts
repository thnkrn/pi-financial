import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import APP_CONSTANTS from '@/constants/app'
import { UserInitialState } from '@/constants/blocktrade/InitialState'
import { getMyUserData } from '@/lib/api/clients/blocktrade/users'
import { IGetMyUserDataResponse } from '@/lib/api/clients/blocktrade/users/types'
import { datadogLogs } from '@datadog/browser-logs'
import { useRouter } from 'next/router'

interface IState {
  data: IGetMyUserDataResponse
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  data: UserInitialState,
  isLoading: false,
  errorMessage: '',
}

export const fetchUserData = createAsyncThunk('appBlocktrade/fetchUserData', async () => {
  datadogLogs.logger.info('blocktrade/fetchUserData', {
    action: 'blocktrade/fetchUserData',
    action_status: 'request',
  })

  const response = await getMyUserData()

  return { ...response, isLoading: true }
})

const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    resetState: () => initialState,
    updateDataState: (state, action) => {
      state.data = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchUserData.pending, state => {
      state.isLoading = true
    })
    builder.addCase(fetchUserData.rejected, (state, action) => {
      state.isLoading = false
      state.errorMessage = action.error.message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error(
        'blocktrade/fetchUserData',
        { action: 'blocktrade/fetchUserData' },
        Error(action?.error?.message)
      )

      if (action.error.code === 'UNAUTHORIZED') {
        const router = useRouter()
        router.push('/signin')
      }
    })
    builder.addCase(fetchUserData.fulfilled, (state, action) => {
      state.data = action.payload
      state.isLoading = false
      state.errorMessage = ''
    })
  },
})

export const { resetState, updateDataState } = userSlice.actions

export default userSlice.reducer
