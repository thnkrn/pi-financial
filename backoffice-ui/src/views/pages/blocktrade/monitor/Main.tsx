// ** React Imports
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import extend from 'lodash/extend'

// ** MUI Imports
import CardContent from '@mui/material/CardContent'
import Box from '@mui/material/Box'

// ** Custom Components
import MonitorTable from './MonitorTable'
import HeaderPage from '@/views/pages/blocktrade/HeaderPage'
import { fetchEquityOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/equity'
import CreateFuturesModal from '@/views/pages/blocktrade/monitor/CreateFutures'
import CreateSplitModal from '@/views/pages/blocktrade/monitor/CreateSplit'
import AddJPBatchModal from '@/views/pages/blocktrade/monitor/AddJPBatch'
import FetchOrdersModal from '@/views/pages/blocktrade/monitor/FetchOrders'

const BlocktradeMonitor = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const userStore = useSelector((state: any) => state.btUser)
  const equityStore = useSelector((state: any) => state.btEquity)

  const updateFilter = (filterValue: any) => {
    const currentFilter = equityStore.filter
    const newFilter = extend({}, currentFilter, filterValue)
    dispatch(updateFilterState(newFilter))
    dispatch(fetchEquityOrder(newFilter))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }

  const handleRefresh = () => {
    dispatch(fetchEquityOrder(equityStore.filter))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }

  return (
    <>
      <HeaderPage
        title={'Blocktrade Monitor'}
        lastUpdated={equityStore.lastUpdated}
        handleRefresh={handleRefresh}
        updateFilter={updateFilter}
        userValue={userStore.data?.id}
      />
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          paddingRight: 5,
        }}
      >
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'flex-start',
            alignItems: 'center',
          }}
        >
          <Box sx={{ marginX: 5, marginBottom: 4, display: 'flex' }}>
            <Box>
              <CreateFuturesModal />
            </Box>
            <Box sx={{ mx: 2 }}>
              <CreateSplitModal />
            </Box>
          </Box>
        </Box>
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'flex-start',
            alignItems: 'center',
          }}
        >
          <Box sx={{ marginBottom: 4, display: 'flex' }}>
            <Box sx={{ mx: 2 }}>
              <FetchOrdersModal />
            </Box>
          </Box>
          <Box sx={{ marginBottom: 4, display: 'flex' }}>
            <Box sx={{ mx: 0 }}>
              <AddJPBatchModal />
            </Box>
          </Box>
        </Box>
      </Box>
      <CardContent sx={{ paddingTop: 0, paddingBottom: 2 }}>
        <MonitorTable />
      </CardContent>
    </>
  )
}

export default BlocktradeMonitor
