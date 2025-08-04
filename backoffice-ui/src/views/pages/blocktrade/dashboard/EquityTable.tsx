import { useEffect, useState } from 'react'
import { Grid } from '@mui/material'
import { columns } from './EquityColumns'
import { EquityRowType } from '@/views/pages/blocktrade/dashboard/types'
import { useDispatch, useSelector } from 'react-redux'
import { fetchEquityOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/equity'
import { DataTable } from '@/views/components/blocktrade/DataTable'
import { EquityOrderFilterInitialState } from '@/constants/blocktrade/InitialState'
import { ThunkDispatch } from '@reduxjs/toolkit'
import processedEquityOrderRows from '@/utils/blocktrade/equityRows'

const EquityTable = () => {
  const store = useSelector((state: any) => state.btEquity)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const [total, setTotal] = useState<number>(store.filter.total)
  const [rows, setRows] = useState<EquityRowType[]>(store.data)

  useEffect(() => {
    setRows(store.data)
    setTotal(store.filter.total || 0)
  }, [store.data, store.filter.total])

  useEffect(() => {
    dispatch(updateFilterState(EquityOrderFilterInitialState))
    dispatch(fetchEquityOrder(EquityOrderFilterInitialState))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }, [dispatch])

  const onPaginate = () => {
    //TODO to update it later
  }

  return (
    <Grid container spacing={2}>
      <Grid item xs={12}>
        <DataTable
          rows={processedEquityOrderRows(rows)}
          total={total}
          columns={columns}
          store={store}
          onPaginate={onPaginate}
        />
      </Grid>
    </Grid>
  )
}

export default EquityTable
