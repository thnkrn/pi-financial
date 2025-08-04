// ** React Imports
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import extend from 'lodash/extend'

// ** MUI Imports
import CardContent from '@mui/material/CardContent'

// ** Custom Components
import { fetchActivityLog, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/log'
import ActivityLogsTable from './ActivityLogsTable'
import HeaderPage from '@/views/pages/blocktrade/HeaderPage'

const BlocktradeActivityLogs = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const userStore = useSelector((state: any) => state.btUser)
  const logStore = useSelector((state: any) => state.btLog)

  const updateFilter = (filterValue: any) => {
    const currentFilter = logStore.filter
    const newFilter = extend({}, currentFilter, filterValue)
    dispatch(updateFilterState(newFilter))
    dispatch(fetchActivityLog(newFilter))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }

  const handleRefresh = () => {
    dispatch(fetchActivityLog(logStore.filter))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }

  return (
    <>
      <HeaderPage
        title={'Blocktrade Activity Logs'}
        lastUpdated={logStore.lastUpdated}
        handleRefresh={handleRefresh}
        updateFilter={updateFilter}
        userValue={userStore.data?.id}
      />
      <CardContent sx={{ paddingTop: 0, paddingBottom: 2 }}>
        <ActivityLogsTable />
      </CardContent>
    </>
  )
}

export default BlocktradeActivityLogs
