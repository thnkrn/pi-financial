import { useEffect, useState } from 'react'
import { Grid } from '@mui/material'
import { columns } from './ActivityLogsColumns'
import { ActivityLogRowType } from './types'
import { useDispatch, useSelector } from 'react-redux'
import { DataTable } from '@/views/components/blocktrade/DataTable'
import { ActivityLogsInitialState } from 'src/constants/blocktrade/InitialState'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { fetchActivityLog, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/log'

const ActivityLogsTable = () => {
  const store = useSelector((state: any) => state.btLog)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const [total, setTotal] = useState<number>(store.filter.total)
  const [rows, setRows] = useState<ActivityLogRowType[]>(store.data)

  const processedRows = (rows: ActivityLogRowType[]) => {
    const processedData = rows.map(item => ({
      ...item,
      dateTime: item.createdAt ?? '',
      action: item.action ?? '',
      details: item.detail ?? '',
      salesId: item.saleId ?? '',
    }))

    return processedData
  }

  useEffect(() => {
    setRows(store.data)
    setTotal(store.filter.total || 0)
  }, [store.data, store.filter.total])

  useEffect(() => {
    dispatch(updateFilterState(ActivityLogsInitialState))
    dispatch(fetchActivityLog(ActivityLogsInitialState))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }, [dispatch])

  const onPaginate = () => {
    //TODO to update it later
  }

  return (
    <Grid container spacing={2}>
      <Grid item xs={12}>
        <DataTable
          height={'600px'}
          rowHeight={50}
          rows={processedRows(rows)}
          total={total}
          columns={columns}
          store={store}
          onPaginate={onPaginate}
        />
      </Grid>
    </Grid>
  )
}

export default ActivityLogsTable
