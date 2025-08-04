import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { Grid } from '@mui/material'

// ** Custom Components Imports
import { columns } from './FuturesColumns'
import { FuturesRowType } from './types'
import { fetchFuturesOrder, updateFilterState, updateLastUpdated } from 'src/store/apps/blocktrade/futures'
import { DataTable } from '@/views/components/blocktrade/DataTable'
import { FuturesOrderFilterInitialState } from 'src/constants/blocktrade/InitialState'
import { Side } from '@/constants/blocktrade/GlobalEnums'

const FuturesTable = () => {
  const store = useSelector((state: any) => state.btFutures)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const [total, setTotal] = useState<number>(store.filter.total)
  const [rows, setRows] = useState<FuturesRowType[]>(store.data)

  const processedRows = (rows: FuturesRowType[]) => {
    return rows.map(item => {
      const symbol = item?.blockOrders?.symbol?.symbol ?? ''
      const series = item?.blockOrders?.series?.series ?? ''

      return {
        ...item,
        orderId: item.blockId,
        openClose: item.blockOrders.openClose,
        piSide: item.blockOrders.clientSide === Side.LONG ? Side.SHORT : Side.LONG,
        clientSide: item.blockOrders.clientSide,
        symbol: `${symbol}${series}`,
        qty: item.blockOrders.numOfContract,
        stockPrice: item.equityPrice,
        customerAccount: item.blockOrders.customerAccount,
        salesId: item.blockOrders.sales.employeeId,
        salesName: item.blockOrders.sales.name,
        status: item.blockOrders.futuresSettrade.length > 0 ? item.blockOrders.futuresSettrade[0].status : 'N/A',
      }
    })
  }

  useEffect(() => {
    setRows(store.data)
    setTotal(store.filter.total || 0)
  }, [store.data, store.filter.total])

  useEffect(() => {
    dispatch(updateFilterState(FuturesOrderFilterInitialState))
    dispatch(fetchFuturesOrder(FuturesOrderFilterInitialState))
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

export default FuturesTable
