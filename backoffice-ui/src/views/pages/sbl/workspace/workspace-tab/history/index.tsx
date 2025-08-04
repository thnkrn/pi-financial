import useFetchSblOrders from '@/hooks/backoffice/useFetchSblOrders'
import { IGetOrdersRequest, SBL_ORDER_STATUS } from '@/lib/api/clients/backoffice/sbl/types'
import { IPaginationModel } from '@/types/sbl/sblTypes'
import Grid from '@mui/material/Grid'
import { PiBackofficeServiceApplicationModelsSblSblOrder } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsSblSblOrder'
import extend from 'lodash/extend'
import { useEffect, useState } from 'react'
import { SubmitHandler } from 'react-hook-form'
import HistoryAction from './HistoryAction'
import HistoryTable from './HistoryTable'

export interface IFormInput {
  account: string
}

const INITIAL_FILTER = {
  page: 1,
  pageSize: 10,
  orderBy: 'createdAt',
  orderDir: 'desc',
  statues: [SBL_ORDER_STATUS.approved, SBL_ORDER_STATUS.rejected],
  tradingAccountNo: '',
}

const HistoryTab = () => {
  const [total, setTotal] = useState<number>(0)
  const [rows, setRows] = useState<PiBackofficeServiceApplicationModelsSblSblOrder[]>([])
  const [filter, setFilter] = useState<IGetOrdersRequest>(INITIAL_FILTER)
  const [currentDateTime, setCurrentDateTime] = useState(new Date())
  const [paginationModel, setPaginationModel] = useState<IPaginationModel>({ page: 0, pageSize: 10 })
  const [search, setSearch] = useState<string>('')
  const [isFirstRender, setIsFirstRender] = useState<boolean>(true)

  const { orderResponse, loading, error, fetchSblOrders } = useFetchSblOrders()

  useEffect(() => {
    const fetchData = async () => {
      await fetchSblOrders(filter)
    }

    if (isFirstRender) {
      fetchData()
      setCurrentDateTime(new Date())
    }

    setIsFirstRender(false)

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isFirstRender])

  useEffect(() => {
    const updatedFilter = extend(
      {},
      { ...filter },
      {
        page: paginationModel.page + 1,
        pageSize: paginationModel.pageSize,
      }
    )
    setFilter(updatedFilter)

    const fetchData = async () => {
      await fetchSblOrders(updatedFilter)
    }

    if (!isFirstRender) {
      fetchData()
      setCurrentDateTime(new Date())
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paginationModel])

  useEffect(() => {
    if (orderResponse?.orders) {
      setRows(orderResponse.orders)
      setTotal(orderResponse.total)
    }
  }, [orderResponse])

  const onRefresh = async () => {
    await fetchSblOrders(filter)
    setCurrentDateTime(new Date())
  }

  const onSearchChange = (searchValue: { account: string }) => {
    setSearch(searchValue?.account)
  }

  const onSubmit: SubmitHandler<IFormInput> = async () => {
    const updatedFilter = extend(
      {},
      { ...filter },
      {
        tradingAccountNo: search,
      }
    )
    setFilter(updatedFilter)

    await fetchSblOrders(updatedFilter)
    setCurrentDateTime(new Date())
  }

  if (error) {
    ;(async () => {
      const displayAlert = (await import('@/views/components/DisplayAlert')).default
      displayAlert(error.message)
    })()
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <HistoryAction onSubmit={onSubmit} onSearchChange={onSearchChange} />
        <HistoryTable
          rows={rows}
          total={total}
          paginationModel={paginationModel}
          setPaginationModel={setPaginationModel}
          isLoading={loading}
          onRefresh={onRefresh}
          currentDateTime={currentDateTime}
        />
      </Grid>
    </Grid>
  )
}

export default HistoryTab
