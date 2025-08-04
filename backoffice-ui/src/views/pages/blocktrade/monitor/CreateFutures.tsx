// ** React Imports
import { useCallback, useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { useRouter } from 'next/router'

// ** MUI Imports
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import Dialog from '@mui/material/Dialog'
import Box from '@mui/material/Box'
import Grid from '@mui/material/Grid'
import TextField from '@mui/material/TextField'
import Typography from '@mui/material/Typography'
import CircularProgress from '@mui/material/CircularProgress'
import Backdrop from '@mui/material/Backdrop'
import Checkbox from '@mui/material/Checkbox'
import FormControlLabel from '@mui/material/FormControlLabel'
import FormGroup from '@mui/material/FormGroup'
import { Container } from '@mui/system'

// ** Other Imports
import { resetState, updateCustomer, updateSeries } from '@/store/apps/blocktrade/monitor'
import { fetchCustomerListData } from '@/store/apps/blocktrade/customer-list'
import SelectOptionPre from '@/views/pages/blocktrade/SelectOptionPre'
import { OC } from '@/constants/blocktrade/GlobalEnums'
import { formatDate } from '@/utils/fmt'
import { DecimalNumber } from '@/utils/blocktrade/decimal'
import { fetchEquityOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/equity'
import { EquityOrderFilterInitialState } from '@/constants/blocktrade/InitialState'
import Swal from 'sweetalert2'
import APP_CONSTANTS from '@/constants/app'
import { datadogLogs } from '@datadog/browser-logs'
import { MonitorDataType } from '@/types/blocktrade/monitor/types'
import toast from 'react-hot-toast'
import { adminCreateFutures, getExpectedFuturesPrice, getFuturesProp } from '@/lib/api/clients/blocktrade/monitor'
import {
  IAdminCreateFuturesRequest,
  IGetExpectedFuturesPriceRequest,
  IGetFuturesPropResponse,
} from '@/lib/api/clients/blocktrade/monitor/types'
import { IGetCustomerListResponse } from '@/lib/api/clients/blocktrade/customers/types'

type OpenSideDataType = {
  equityPrice: number
  qty: number
  totalQty: number
  createdAt: string
}

const displayAlert = (message: string) => {
  Swal.fire({
    title: 'Error!',
    text: message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE,
    icon: 'error',
  })
}

const CreateFuturesModal = () => {
  const router = useRouter()
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const store = useSelector((state: any) => state.btMonitor)
  const customerListStore = useSelector((state: any) => state.btCustomerList)
  const [customerName, setCustomerName] = useState<string | null>('-')
  const [employeeICID, setEmployeeICID] = useState<number | null>(null)
  const [icUserId, setICUserId] = useState<number | null>(null)
  const [icName, setICName] = useState<string | null>('-')
  const [futuresPrice, setFuturesPrice] = useState<number | null>(null)
  const [loadingPrice, setLoadingPrice] = useState<boolean>(false)
  const [openData, setOpenData] = useState<OpenSideDataType[]>([])
  const [modalOpen, setModalOpen] = useState<boolean>(false)
  const [avgPrice, setAvgPrice] = useState<number | null>(null)
  const [totalQty, setTotalQty] = useState<number | null>(null)
  const [futuresProp, setFuturesProp] = useState<IGetFuturesPropResponse | null>(null)
  const [loadingOverlay, setLoadingOverlay] = useState<boolean>(false)
  const [placeTradeReport, setPlaceTradeReport] = useState<boolean>(false)

  const handleOpen = () => {
    if (store.symbol) {
      setModalOpen(true)
    }
  }

  const handleClose = () => {
    setModalOpen(false)
    dispatch(updateCustomer(null))
    dispatch(updateSeries(null))
    handleResetInfo()
  }

  const handleResetInfo = () => {
    setCustomerName('-')
    setEmployeeICID(null)
    setICUserId(null)
    setICName('-')
    setFuturesPrice(null)
    setLoadingPrice(false)
    setOpenData([])
  }

  const handleCustomerSelected = async (value: string) => {
    await dispatch(updateCustomer(value))
  }

  const handleSeriesSelected = async (value: string) => {
    await dispatch(updateSeries(value))
  }

  const handleCreateFutures = async () => {
    if (futuresPrice && icUserId) {
      setLoadingOverlay(true)
      const payload: IAdminCreateFuturesRequest = {
        targetBlockId: store.blockId,
        series: store.series,
        saleId: icUserId,
        customerAccount: store.customer,
        placeICTradeReport: placeTradeReport,
        equities: store.data.map((item: { id: any }) => ({ equityOrderId: item.id })),
      }
      try {
        const response = await adminCreateFutures(payload)
        if (response) {
          handleClose()
          dispatch(resetState())
          dispatch(updateFilterState(EquityOrderFilterInitialState))
          dispatch(fetchEquityOrder(EquityOrderFilterInitialState))
          dispatch(updateLastUpdated(new Date().toLocaleString()))
          datadogLogs.logger.info('futuresOrders/adminCreate', {
            action: ['adminCreate', 'futuresOrders'],
            payload,
            action_status: 'request',
          })

          const baseMessage = 'Successfully Submitted'
          const message = placeTradeReport ? `${baseMessage} and Requested Trade Report` : baseMessage

          toast.success(message, { duration: 30000 })
        }
      } catch (error: any) {
        datadogLogs.logger.error(
          'futuresOrders/adminCreate',
          { action: ['adminCreate', 'futuresOrders'] },
          error as Error
        )
        if (error.code === 'UNAUTHORIZED') {
          await router.push({ pathname: '/error', query: { reason: 'unauthorized' } })
        } else {
          displayAlert(error.message)
        }
      } finally {
        setLoadingOverlay(false)
      }
    }
  }

  const fetchExpectedFuturesPrice = useCallback(async () => {
    if (store.customer && store.series && store.symbol) {
      setLoadingPrice(true)
      setFuturesPrice(null)
      setOpenData([])
      if (store.pos === OC.CLOSE) {
        const payload: IGetExpectedFuturesPriceRequest = {
          openClose: store.pos,
          side: store.side,
          symbol: store.symbol,
          series: store.series,
          customerAccount: store.customer,
          equities: store.data.map((item: any) => ({ equityOrderId: item.id })),
        }
        try {
          const response = await getExpectedFuturesPrice(payload)
          if (response) {
            setFuturesPrice(response.expectedFuturesPrice)
            for (const order of response.openFutures) {
              setOpenData((prev: any) => [
                ...prev,
                {
                  equityPrice: order.equityPrice,
                  qty: order.qty,
                  totalQty: order.totalQty,
                  createdAt: order.createdAt,
                },
              ])
            }
          }
        } catch (error) {
          setFuturesPrice(null)
          setOpenData([])
        }
      } else {
        setFuturesPrice(avgPrice)
      }
      setLoadingPrice(false)
    }
  }, [avgPrice, store.customer, store.data, store.pos, store.series, store.side, store.symbol])

  const fetchFuturesProp = useCallback(async () => {
    try {
      const req = {
        symbol: store.symbol,
        series: store.series,
      }
      const response = await getFuturesProp(req)
      setFuturesProp(response)
    } catch (error) {
      return null
    }
  }, [store.series, store.symbol])

  useEffect(() => {
    if (store.customer && store.series && store.symbol) {
      fetchExpectedFuturesPrice()
      fetchFuturesProp()
    } else {
      handleResetInfo()
    }
  }, [store.customer, store.series, store.symbol, fetchExpectedFuturesPrice, fetchFuturesProp])

  useEffect(() => {
    const customerList: IGetCustomerListResponse = Object.values(customerListStore.data)
    const foundCustomer = customerList.find(customer => String(customer.accountNo) === store.customer)
    if (foundCustomer) {
      setCustomerName(store.customer ? foundCustomer.customerName : 'Customer not found')
      setEmployeeICID(foundCustomer.icId)
      setICName(foundCustomer.ic ? foundCustomer.ic.name : '-')
      setICUserId(foundCustomer.ic ? foundCustomer.ic.id : null)
    }
  }, [store.customer, customerListStore.data])

  useEffect(() => {
    dispatch(fetchCustomerListData(0))
  }, [dispatch])

  useEffect(() => {
    if (store.data.length > 0) {
      const sumQty = store.data.reduce((a: number, b: MonitorDataType) => a + b.qty, 0)
      const sumExecutedPrice = store.data.reduce((a: number, b: MonitorDataType) => a + b.executedPrice * b.qty, 0)
      setAvgPrice(sumQty ? sumExecutedPrice / sumQty : 0)
      setTotalQty(sumQty)
    }
  }, [store.data])

  const copyToClipboard = () => {
    if (navigator.clipboard) {
      const text = `${store.customer.toString()}\t${store.symbol}\t${totalQty || 0}\t${avgPrice}\t${store.series}\t${
        (totalQty || 0) / (futuresProp?.multiplier || 0)
      }`
      navigator.clipboard.writeText(text)
    }
  }

  const getSeriesListUrl = (pos: string, blockId: number, customer: string) => {
    if (pos === OC.OPEN) {
      return 'seriesProperties/getList'
    } else if (blockId && customer) {
      return `futuresOrders/getCloseSeries?blockId=${blockId}&customer=${customer}`
    } else {
      return 'seriesProperties/getList'
    }
  }

  const renderPriceDisplay = () => {
    if (loadingPrice) {
      return <CircularProgress size={18} thickness={8} />
    } else if (futuresPrice) {
      return DecimalNumber(futuresPrice, 4)
    } else {
      return '-'
    }
  }

  return (
    <Container>
      <Backdrop sx={{ zIndex: theme => Math.max.apply(Math, Object.values(theme.zIndex)) + 1 }} open={loadingOverlay}>
        <CircularProgress color='inherit' />
      </Backdrop>
      <Dialog
        open={modalOpen}
        disableEscapeKeyDown
        aria-labelledby='title'
        aria-describedby='description'
        fullWidth={true}
      >
        <DialogTitle id='title'>Create Futures</DialogTitle>
        <DialogContent>
          <DialogContentText id='description'>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Symbol
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {store.symbol}
                </Typography>
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Side
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {store.side} {store.pos}
                </Typography>
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Order ID
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {store.data.map((item: any) => item.orderId).join(', ')}
                </Typography>
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Average Price
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {DecimalNumber(avgPrice || 0, 4)}
                </Typography>
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Total Qty
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {totalQty}
                </Typography>
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Contract Qty
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {futuresProp?.multiplier ? (totalQty ?? 0) / (futuresProp?.multiplier || 1) : 'N/A'}
                </Typography>
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Cust. Acc.
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                {store.pos === OC.OPEN ? (
                  <TextField
                    size={'small'}
                    fullWidth
                    label=''
                    placeholder=''
                    value={store.customer}
                    disabled={false}
                    onChange={event => {
                      if (event.target.value.length === 8) {
                        handleCustomerSelected(event.target.value)
                      } else {
                        dispatch(updateCustomer(event.target.value))
                        handleResetInfo()
                      }
                    }}
                  />
                ) : (
                  <SelectOptionPre
                    id={'closeToList'}
                    labelId={'closeToList'}
                    label={''}
                    defaultValue={store.customer}
                    remote={{
                      field: 'closeToList',
                      url: `futuresOrders/getCloseCustomers?blockId=${store.blockId}`,
                      key: 'customerAccount',
                      value: 'customerAccount',
                    }}
                    onChange={key => {
                      if (key.length === 8) {
                        dispatch(updateSeries(null))
                        handleCustomerSelected(key)
                      } else {
                        dispatch(updateSeries(null))
                        dispatch(updateCustomer(key))
                        handleResetInfo()
                      }
                    }}
                  />
                )}
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Cust. Info
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {customerName}
                </Typography>
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    IC Info
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {employeeICID ? `${employeeICID} : ${icName}` : '-'}
                </Typography>
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Series
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                {store.customer ? (
                  <SelectOptionPre
                    id={'seriesList'}
                    labelId={'seriesList'}
                    label={''}
                    defaultValue={store.series}
                    disabled={!store.customer}
                    remote={{
                      field: 'seriesList',
                      url: getSeriesListUrl(store.pos, store.blockId, store.customer),
                      key: 'series',
                      value: 'series',
                    }}
                    onChange={key => {
                      handleSeriesSelected(key)
                    }}
                  />
                ) : (
                  <SelectOptionPre
                    id={'seriesList'}
                    labelId={'seriesList'}
                    label={''}
                    defaultValue={''}
                    disabled
                    onChange={() => {}}
                  />
                )}
              </Grid>
            </Grid>
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Futures Price
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary', fontWeight: 700 }}
                >
                  {renderPriceDisplay()}
                </Typography>
              </Grid>
            </Grid>
            {navigator.clipboard && (
              <Grid container spacing={2} sx={{ marginTop: 0 }}>
                <Grid item xs={4}>
                  <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                    <Typography
                      variant='body1'
                      gutterBottom
                      noWrap
                      sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                    >
                      Copy for Excel
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={8} sx={{ textAlign: 'left' }}>
                  <Button variant='contained' disabled={!futuresPrice} onClick={copyToClipboard}>
                    Copy
                  </Button>
                </Grid>
              </Grid>
            )}
            <Grid container spacing={2} sx={{ marginTop: 0 }}>
              <Grid item xs={4}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    Place IC Trade Report
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <FormGroup>
                  <FormControlLabel
                    control={
                      <Checkbox
                        sx={{ py: 0 }}
                        checked={placeTradeReport}
                        onChange={e => {
                          setPlaceTradeReport(e.target.checked)
                        }}
                        disabled={false}
                      />
                    }
                    label={''}
                  />
                </FormGroup>
              </Grid>
            </Grid>
            {store.customer && store.series && store.pos === 'CLOSE' && (
              <>
                <Typography sx={{ fontWeight: 700 }}>Open Position Information</Typography>
                <Grid container spacing={2} sx={{ marginTop: 0 }}>
                  <Grid item xs={4}>
                    <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                      <Typography
                        variant='body1'
                        gutterBottom
                        noWrap
                        sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                      >
                        Equity Price
                      </Typography>
                    </Box>
                  </Grid>
                  <Grid item xs={8} sx={{ textAlign: 'left' }}>
                    <Typography
                      variant='body1'
                      gutterBottom
                      noWrap
                      sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                    >
                      {loadingPrice ? (
                        <CircularProgress size={18} thickness={8} />
                      ) : (
                        openData.map((item: any) => item.equityPrice, 0).join(', ') || '-'
                      )}
                    </Typography>
                  </Grid>
                </Grid>
                <Grid container spacing={2} sx={{ marginTop: 0 }}>
                  <Grid item xs={4}>
                    <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                      <Typography
                        variant='body1'
                        gutterBottom
                        noWrap
                        sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                      >
                        Contract Qty
                      </Typography>
                    </Box>
                  </Grid>
                  <Grid item xs={8} sx={{ textAlign: 'left' }}>
                    <Typography
                      variant='body1'
                      gutterBottom
                      noWrap
                      sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                    >
                      {loadingPrice ? (
                        <CircularProgress size={18} thickness={8} />
                      ) : (
                        openData.map((item: any) => `${item.qty}/${item.totalQty}`, `0/0`).join(', ') ?? '-'
                      )}
                    </Typography>
                  </Grid>
                </Grid>
                <Grid container spacing={2} sx={{ marginTop: 0 }}>
                  <Grid item xs={4}>
                    <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                      <Typography
                        variant='body1'
                        gutterBottom
                        noWrap
                        sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                      >
                        Open Date
                      </Typography>
                    </Box>
                  </Grid>
                  <Grid item xs={8} sx={{ textAlign: 'left' }}>
                    <Typography
                      variant='body1'
                      gutterBottom
                      noWrap
                      sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                    >
                      {loadingPrice ? (
                        <CircularProgress size={18} thickness={8} />
                      ) : (
                        openData.map((item: any) => formatDate(item.createdAt), 0).join(', ') ?? '-'
                      )}
                    </Typography>
                  </Grid>
                </Grid>
              </>
            )}
          </DialogContentText>
        </DialogContent>
        <DialogActions className='dialog-actions-dense'>
          <Button onClick={handleClose}>Deny</Button>
          <Button disabled={!futuresPrice} onClick={handleCreateFutures}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
      <Button
        size='medium'
        type='submit'
        sx={{ width: '200px', fontWeight: 700, marginTop: 0 }}
        variant='contained'
        disabled={!store.symbol}
        onClick={handleOpen}
      >
        Create Futures
      </Button>
    </Container>
  )
}

export default CreateFuturesModal
