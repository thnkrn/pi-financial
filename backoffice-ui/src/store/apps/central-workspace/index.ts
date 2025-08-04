import APP_CONSTANTS from '@/constants/app'
import { getTickets } from '@/lib/api/clients/backoffice/central-workspace'
import { IGetTicketsRequest } from '@/lib/api/clients/backoffice/central-workspace/types'
import { datadogLogs } from '@datadog/browser-logs'
import { PiBackofficeServiceAPIModelsTicketDetailResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTicketDetailResponse'
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'

interface IState {
  tickets: PiBackofficeServiceAPIModelsTicketDetailResponse[] | []
  filter: {
    total: number | null
    orderBy: string
    orderDir: string
  }
  isLoading: boolean
  errorMessage: string
}

const initialState: IState = {
  tickets: [],
  filter: {
    total: null,
    orderBy: 'createdAt',
    orderDir: 'desc',
  },
  isLoading: false,
  errorMessage: '',
}

export const fetchTickets = createAsyncThunk('appCentralWorkspace/fetchTickets', async (params: IGetTicketsRequest) => {
  let currentFilter: Partial<IGetTicketsRequest> = params

  currentFilter = omitBy(currentFilter, v => {
    return v === 'ALL' || v === null || v === ''
  })

  datadogLogs.logger.info('fetchTickets', {
    action: 'fetchTickets',
    payload: currentFilter,
    action_status: 'request',
  })

  const response = await getTickets(currentFilter as IGetTicketsRequest)

  return { ...response, isLoading: true }
})

const centralWorkspaceSlice = createSlice({
  name: 'central-workspace',
  initialState,
  reducers: {
    resetState: (state, action) => {
      state.tickets = []
      state.filter = action.payload
      state.isLoading = false
    },
    updateFilterState: (state, action) => {
      state.filter = action.payload
    },
  },
  extraReducers: builder => {
    builder.addCase(fetchTickets.pending, state => {
      state.isLoading = true
    })

    builder.addCase(fetchTickets.rejected, (state, action) => {
      state.tickets = []
      state.isLoading = false
      state.errorMessage = action.error.message ?? APP_CONSTANTS.GLOBAL_ERROR_MESSAGE

      datadogLogs.logger.error('fetchTickets', { action: 'fetchTickets' }, Error(action?.error?.message))
    })

    builder.addCase(fetchTickets.fulfilled, (state: IState, action) => {
      state.tickets = action.payload.tickets
      state.filter = extend({}, state.filter, {
        page: action.payload.page,
        pageSize: action.payload.pageSize,
        total: action.payload.total,
        orderBy: action.payload.orderBy,
        orderDir: action.payload.orderDir,
      })
      state.isLoading = false
      state.errorMessage = ''

      datadogLogs.logger.info('fetchTickets', { action: 'fetchTickets', action_status: 'success' })
    })
  },
})

export const { resetState, updateFilterState } = centralWorkspaceSlice.actions

export default centralWorkspaceSlice.reducer
