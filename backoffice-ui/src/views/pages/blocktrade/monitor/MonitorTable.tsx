// ** React Imports
import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'

// ** MUI Imports
import Button from '@mui/material/Button'
import { GridFilterModel, GridToolbar } from '@mui/x-data-grid'

// ** Custom Components Imports
import { columns } from './MonitorColumns'
import { DataTable } from '@/views/components/blocktrade/DataTable'
import { EquityOrderFilterInitialState } from 'src/constants/blocktrade/InitialState'
import { EquityRowType } from '@/views/pages/blocktrade/dashboard/types'
import { fetchEquityOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/equity'
import { OrderStatus } from '@/constants/blocktrade/GlobalEnums'
import processedEquityOrderRows from '@/utils/blocktrade/equityRows'

type Props = {
  handleSetFilter: (value: string, condition?: string) => void
}

const CustomToolbar = ({ handleSetFilter }: Props) => {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
      <div>
        <Button
          variant='outlined'
          color='primary'
          size='small'
          style={{ marginLeft: 10, marginTop: 4, marginBottom: 4, padding: 3 }}
          onClick={() => handleSetFilter('')}
        >
          All
        </Button>
        <Button
          variant='outlined'
          color='primary'
          size='small'
          style={{ marginLeft: 10, marginTop: 4, marginBottom: 4, padding: 3 }}
          onClick={() => handleSetFilter(OrderStatus.WORKING)}
        >
          Working
        </Button>
        <Button
          variant='outlined'
          color='primary'
          size='small'
          style={{ marginLeft: 10, marginTop: 4, marginBottom: 4, padding: 3 }}
          onClick={() => handleSetFilter(OrderStatus.S_FILLED)}
        >
          Filled
        </Button>
        <Button
          variant='outlined'
          color='primary'
          size='small'
          style={{ marginLeft: 10, marginTop: 4, marginBottom: 4, padding: 3 }}
          onClick={() => handleSetFilter(OrderStatus.F_MATCHED.slice(0, 2), 'startsWith')}
        >
          Done
        </Button>
      </div>
      <GridToolbar showQuickFilter={true} />
    </div>
  )
}

const MonitorTable = () => {
  const store = useSelector((state: any) => state.btEquity)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const [total, setTotal] = useState<number>(store.filter?.total ?? 0)
  const [rows, setRows] = useState<EquityRowType[]>(store?.data ?? [])
  const [filterModel, setFilterModel] = useState<GridFilterModel>({ items: [] })

  useEffect(() => {
    setRows(store.data)
    setTotal(store.filter.total ?? 0)
  }, [store.data, store.filter.total])

  useEffect(() => {
    const updateData = () => {
      dispatch(updateFilterState(EquityOrderFilterInitialState))
      dispatch(fetchEquityOrder(EquityOrderFilterInitialState))
      dispatch(updateLastUpdated(new Date().toLocaleString()))
    }
    updateData()
    const interval = setInterval(updateData, 10000)

    return () => clearInterval(interval)
  }, [dispatch])

  const handleSetFilter = (value: string, condition?: string) => {
    setFilterModel({
      items: [{ field: 'status', operator: condition ?? 'equals', value: value }],
    })
  }

  const renderCustomToolbar = () => <CustomToolbar handleSetFilter={handleSetFilter} />

  const onPaginate = () => {
    //NOTE: to update it later
  }

  return (
    <DataTable
      height={'600px'}
      rowHeight={50}
      rows={processedEquityOrderRows(rows)}
      total={total}
      columns={columns}
      store={store}
      onPaginate={onPaginate}
      customToolbar={renderCustomToolbar}
      filterModel={filterModel}
      onFilterModelChange={setFilterModel}
    />
  )
}

export default MonitorTable
