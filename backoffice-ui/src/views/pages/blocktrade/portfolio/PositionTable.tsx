import { useEffect, useState } from 'react'
import { Grid } from '@mui/material'
import { columns } from './PositionColumns'
import { PositionRowType } from './types'
import { useDispatch, useSelector } from 'react-redux'
import { DataTable } from '@/views/components/blocktrade/DataTable'
import { PositionInitialState } from 'src/constants/blocktrade/InitialState'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { fetchPosition, updateFilterState, updateLastUpdated, updateRollover } from '@/store/apps/blocktrade/position'
import { CustomToolbar } from '@/views/pages/blocktrade/portfolio/CustomToolbar'

const PositionTable = () => {
  const store = useSelector((state: any) => state.btPosition)
  const userStore = useSelector((state: any) => state.btUser)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const [total, setTotal] = useState<number>(store.filter.total)
  const [rows, setRows] = useState<PositionRowType[]>(store.data)

  const processedRows = (rows: PositionRowType[]) => {
    const processedData = rows.map(item => ({
      ...item,
      symbolLabel: (item.symbol?.symbol ?? '') + (item.series?.series ?? ''),
      equityPrice: Number(item.futuresOrders?.equityPrice ?? 0),
      equityMKTPrice: Number(item.futuresClose?.mktPrice ?? 0),
      xd: Number(item.futuresClose?.xd ?? 0),
      futureProjPrice: Number(item.futuresClose?.projFutures ?? 0),
      unrealizedPL: Number(item.futuresClose?.unrealizedPnlPerCont ?? 0),
      employeeId: item.sales?.employeeId ?? '',
    }))

    return processedData
  }

  useEffect(() => {
    setRows(store.data)
    setTotal(store.filter.total || 0)
  }, [store.data, store.filter.total])

  useEffect(() => {
    dispatch(updateFilterState(PositionInitialState))
    dispatch(fetchPosition(PositionInitialState))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }, [dispatch])

  const renderCustomToolbar = () => <CustomToolbar userRole={userStore.data.role} onSetRollover={handleSetRollover} />

  const handleSetRollover = () => {
    dispatch(updateRollover(!store.rollover))
  }

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
          customToolbar={renderCustomToolbar}
        />
      </Grid>
    </Grid>
  )
}

export default PositionTable
