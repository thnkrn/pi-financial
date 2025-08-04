// ** React Imports
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import extend from 'lodash/extend'

// ** MUI Imports
import CardContent from '@mui/material/CardContent'

// ** Custom Components
import { fetchFuturesOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/futures'
import FuturesTable from './FuturesTable'
import HeaderPage from '@/views/pages/blocktrade/HeaderPage'

const BlocktradeAllocation = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const userStore = useSelector((state: any) => state.btUser)
  const futuresStore = useSelector((state: any) => state.btFutures)

  const updateFilter = (filterValue: any) => {
    const currentFilter = futuresStore.filter
    const newFilter = extend({}, currentFilter, filterValue)
    dispatch(updateFilterState(newFilter))
    dispatch(fetchFuturesOrder(newFilter))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }

  const handleRefresh = () => {
    dispatch(fetchFuturesOrder(futuresStore.filter))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }

  return (
    <>
      <HeaderPage
        title={'Blocktrade Allocation'}
        lastUpdated={futuresStore.lastUpdated}
        handleRefresh={handleRefresh}
        updateFilter={updateFilter}
        userValue={userStore.data?.id}
      />
      <CardContent sx={{ paddingTop: 0, paddingBottom: 2 }}>
        <FuturesTable />
      </CardContent>
    </>
  )
}

export default BlocktradeAllocation
