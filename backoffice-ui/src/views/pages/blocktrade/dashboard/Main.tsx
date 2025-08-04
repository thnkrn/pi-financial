// ** React Imports
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import extend from 'lodash/extend'

// ** MUI Imports
import Box from '@mui/material/Box'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import Stack from '@mui/material/Stack'
import ActivityLog from 'src/views/pages/blocktrade/dashboard/ActivityLog'
import AvailPos from 'src/views/pages/blocktrade/dashboard/AvailPos'
import MarketData from 'src/views/pages/blocktrade/dashboard/MarketData'
import NewOrder from 'src/views/pages/blocktrade/dashboard/NewOrder'

// ** Custom Components Imports
import { fetchEquityOrder, updateLastUpdated, updateFilterState } from '@/store/apps/blocktrade/equity'
import EquityTable from './EquityTable'
import HeaderPage from '@/views/pages/blocktrade/HeaderPage'

const BlocktradeDashboard = () => {
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
    <Box>
      <HeaderPage
        title={'Blocktrade Dashboard'}
        lastUpdated={equityStore.lastUpdated}
        handleRefresh={handleRefresh}
        updateFilter={updateFilter}
        userValue={userStore.data?.id}
      />
      <CardContent sx={{ paddingTop: 0, paddingBottom: 2 }}>
        <EquityTable />
      </CardContent>
      <CardContent sx={{ paddingY: 2 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} lg={4}>
            <AvailPos />
          </Grid>
          <Grid item xs={12} lg={4}>
            <NewOrder />
          </Grid>
          <Grid item xs={12} lg={4}>
            <Box sx={{ height: '100%', maxHeight: `` }}>
              <Stack sx={{ height: '100%' }} spacing={2}>
                <MarketData />
                <ActivityLog />
              </Stack>
            </Box>
          </Grid>
        </Grid>
      </CardContent>
    </Box>
  )
}

export default BlocktradeDashboard
