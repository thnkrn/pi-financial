import useFetchSblOrders from '@/hooks/backoffice/useFetchSblOrders'
import usePollingWithRef from '@/hooks/usePollingWithRef'
import { updateSblOrderAction } from '@/lib/api/clients/backoffice/sbl'
import {
  IGetOrdersRequest,
  IGetOrdersResponse,
  IUpdateOrderActionRequest,
  SBL_ORDER_STATUS,
} from '@/lib/api/clients/backoffice/sbl/types'
import { ACTION_TYPE, IAction, IPaginationModel } from '@/types/sbl/sblTypes'
import Grid from '@mui/material/Grid'
import { PiBackofficeServiceApplicationModelsSblSblOrder } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsSblSblOrder'
import axios from 'axios'
import extend from 'lodash/extend'
import { useEffect, useRef, useState } from 'react'
import { SubmitHandler } from 'react-hook-form'
import PendingAction from './PendingAction'
import PendingDialog from './PendingDialog'
import PendingTable from './PendingTable'

export interface IFormInput {
  account: string
}

const INITIAL_FILTER = {
  page: 1,
  pageSize: 10,
  orderBy: 'createdAt',
  orderDir: 'desc',
  statues: SBL_ORDER_STATUS.pending,
  tradingAccountNo: '',
}

interface Props {
  setFetchNotification: (status: boolean) => void
}

const PendingTab = ({ setFetchNotification }: Props) => {
  const [total, setTotal] = useState<number>(0)
  const [rows, setRows] = useState<PiBackofficeServiceApplicationModelsSblSblOrder[]>([])
  const [filter, setFilter] = useState<IGetOrdersRequest>(INITIAL_FILTER)
  const [currentDateTime, setCurrentDateTime] = useState(new Date())
  const [paginationModel, setPaginationModel] = useState<IPaginationModel>({ page: 0, pageSize: 10 })
  const [search, setSearch] = useState<string>('')
  const [open, setOpen] = useState<boolean>(false)
  const [action, setAction] = useState<IAction>()
  const [error, setError] = useState<string>('')
  const [isOrderAction, setIsOrderAction] = useState<boolean>(false)
  const [isFirstRender, setIsFirstRender] = useState<boolean>(true)

  const { orderResponse, loading, error: fetchOrdersError, fetchSblOrders } = useFetchSblOrders()

  const filterRef = useRef(filter)
  filterRef.current = filter

  const { start, stop } = usePollingWithRef<IGetOrdersResponse>({
    fetcher: async () => {
      await fetchSblOrders(filterRef.current)
      setCurrentDateTime(new Date())

      return orderResponse!
    },
    interval: 10000,
    autoStart: false,
  })

  useEffect(() => {
    const fetchData = async () => {
      await fetchSblOrders(filter)
    }

    if (isFirstRender) {
      fetchData()
      setCurrentDateTime(new Date())
    } else if (!open) {
      start()
    } else {
      stop()
    }

    setIsFirstRender(false)

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isFirstRender, open])

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
  }

  const handleOpen = (orderId: string, orderNo: string, type: ACTION_TYPE) => {
    setAction({
      orderId,
      orderNo,
      type,
    })
    setOpen(true)
  }

  const onActionClick = async (action: IAction, reason: string = '') => {
    try {
      setIsOrderAction(true)
      const payload: IUpdateOrderActionRequest = {
        status: action.type,
        rejectedReason: reason,
      }

      const { status, data } = await updateSblOrderAction(action.orderId, payload)

      if (status !== 200) {
        throw new Error(`Unexpected error from processing order: ${data}`)
      }

      setError('')
    } catch (error) {
      const message = axios.isAxiosError(error)
        ? error.response?.data?.detail ?? 'An error occurred while processing the order'
        : 'An unexpected error occurred while processing the order'

      setError(message)
    } finally {
      setOpen(false)
      setIsOrderAction(false)
      setFetchNotification(true)

      await fetchSblOrders(filter)
    }
  }

  if (fetchOrdersError || error) {
    ;(async () => {
      const displayAlert = (await import('@/views/components/DisplayAlert')).default
      displayAlert(fetchOrdersError?.message ?? error)
    })()
  }

  return (
    <>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <PendingAction onSubmit={onSubmit} onSearchChange={onSearchChange} />
          <PendingTable
            rows={rows}
            total={total}
            paginationModel={paginationModel}
            setPaginationModel={setPaginationModel}
            isLoading={loading}
            onRefresh={onRefresh}
            currentDateTime={currentDateTime}
            handleOpen={handleOpen}
          />
        </Grid>
      </Grid>
      {!!action && (
        <PendingDialog
          open={open}
          handleClose={() => setOpen(false)}
          action={action}
          onActionClick={onActionClick}
          isOrderAction={isOrderAction}
        />
      )}
    </>
  )
}

export default PendingTab
