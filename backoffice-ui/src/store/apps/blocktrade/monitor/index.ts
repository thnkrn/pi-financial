import { createSlice } from '@reduxjs/toolkit'
import findIndex from 'lodash/findIndex'
import { MonitorDataType } from '@/types/blocktrade/monitor/types'

type MonitorStateType = {
  data: MonitorDataType[]
  blockId: number | null
  pos: number | null
  side: string | null
  symbol: string | null
  series: string | null
  customer: string | null
  isLoading: boolean
  errorMessage: string
}

const initialState: MonitorStateType = {
  data: [],
  blockId: null,
  pos: null,
  side: null,
  symbol: null,
  series: null,
  customer: null,
  isLoading: false,
  errorMessage: '',
}

const monitorSlice = createSlice({
  name: 'monitor',
  initialState,
  reducers: {
    resetState: () => initialState,
    addOrder: (state, action) => {
      state.data.push(action.payload)
    },
    removeOrder: (state, action) => {
      const index = findIndex(state.data, item => item.id === action.payload.id)
      if (index !== -1) {
        state.data.splice(index, 1)
      }
    },
    updateData: (state, action) => {
      Object.assign(state, action.payload)
    },
    updateSeries: (state, action) => {
      state.series = action.payload
    },
    updateCustomer: (state, action) => {
      state.customer = action.payload
    },
  },
})

export const { updateData, resetState, addOrder, removeOrder, updateSeries, updateCustomer } = monitorSlice.actions

export default monitorSlice.reducer
