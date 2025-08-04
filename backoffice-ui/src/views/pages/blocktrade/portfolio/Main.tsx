// ** React Imports
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import extend from 'lodash/extend'

// ** MUI Imports
import CardContent from '@mui/material/CardContent'

// ** Custom Components
import { fetchPosition, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/position'
import PositionTable from './PositionTable'
import HeaderPage from '@/views/pages/blocktrade/HeaderPage'

const BlocktradePortfolio = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const userStore = useSelector((state: any) => state.btUser)
  const positionStore = useSelector((state: any) => state.btPosition)

  const updateFilter = (filterValue: any) => {
    const currentFilter = positionStore.filter
    const newFilter = extend({}, currentFilter, filterValue)
    dispatch(updateFilterState(newFilter))
    dispatch(fetchPosition(newFilter))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }

  const handleRefresh = () => {
    dispatch(fetchPosition(positionStore.filter))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }

  return (
    <>
      <HeaderPage
        title={'Blocktrade Portfolio'}
        lastUpdated={positionStore.lastUpdated}
        handleRefresh={handleRefresh}
        updateFilter={updateFilter}
        userValue={userStore.data?.id}
      />
      <CardContent sx={{ paddingTop: 0, paddingBottom: 2 }}>
        <PositionTable />
      </CardContent>
    </>
  )
}

export default BlocktradePortfolio
