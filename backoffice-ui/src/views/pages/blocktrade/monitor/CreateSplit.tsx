// ** React Imports
import { useEffect, useState } from 'react'
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

// ** Other Imports
import { DecimalNumber } from '@/utils/blocktrade/decimal'
import { resetState } from '@/store/apps/blocktrade/monitor'
import { fetchEquityOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/equity'
import { EquityOrderFilterInitialState } from '@/constants/blocktrade/InitialState'
import Swal from 'sweetalert2'
import APP_CONSTANTS from '@/constants/app'
import { datadogLogs } from '@datadog/browser-logs'
import { MonitorDataType } from '@/types/blocktrade/monitor/types'
import { Container } from '@mui/system'
import { splitOrder } from '@/lib/api/clients/blocktrade/monitor'

const displayAlert = (message: string) => {
  Swal.fire({
    title: 'Error!',
    text: message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE,
    icon: 'error',
  })
}

const CreateSplitModal = () => {
  const router = useRouter()
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const store = useSelector((state: any) => state.btMonitor)

  const [modalOpen, setModalOpen] = useState<boolean>(false)
  const [splitTo, setSplitTo] = useState<number | null>(0)
  const [avgPrice, setAvgPrice] = useState<number | null>(null)
  const [totalQty, setTotalQty] = useState<number | null>(null)
  const [orderQty, setOrderQty] = useState<number[]>([])
  const [orderId, setOrderId] = useState<number | null>(null)

  const handleOpen = () => {
    if (store.symbol) {
      setModalOpen(true)
    }
  }

  const handleClose = () => {
    setModalOpen(false)
    handleResetInfo()
  }

  const handleResetInfo = () => {
    setSplitTo(0)
    setOrderQty([])
  }

  const handleSplit = async () => {
    if (splitTo && splitTo > 0 && store.data.length === 1) {
      let id = 1
      const splitOrders = orderQty
        .filter(qty => qty > 0)
        .map(qty => {
          return { id: id++, qty }
        })

      const payload = {
        equityOrderId: store.data[0].id,
        splitOrders,
      }
      try {
        const response = await splitOrder(payload)
        if (response) {
          handleClose()
          dispatch(resetState())
          dispatch(updateFilterState(EquityOrderFilterInitialState))
          dispatch(fetchEquityOrder(EquityOrderFilterInitialState))
          dispatch(updateLastUpdated(new Date().toLocaleString()))
          datadogLogs.logger.info('equityOrders/splitOrders', {
            action: ['splitOrders', 'equityOrders'],
            payload,
            action_status: 'request',
          })
        }
      } catch (error: any) {
        datadogLogs.logger.error(
          'equityOrders/splitOrders',
          { action: ['splitOrders', 'equityOrders'] },
          error as Error
        )
        if (error.code === 'UNAUTHORIZED') {
          await router.push({ pathname: '/error', query: { reason: 'unauthorized' } })
        } else {
          displayAlert(error.message)
        }
      }
    }
  }

  const handleQtyChange = (index: number, value: number) => {
    if (splitTo === 2) {
      const newQty = [...orderQty]
      newQty[index] = value
      newQty[1 - index] = (totalQty || 0) - value
      setOrderQty(newQty)
    } else {
      const newQty = [...orderQty]
      newQty[index] = value
      setOrderQty(newQty)
    }
  }

  const totalOrderQty = () => {
    return orderQty.reduce((sum, qty) => sum + qty, 0)
  }

  useEffect(() => {
    if (splitTo !== null && splitTo <= 10 && splitTo > 0) {
      if (splitTo === 2) {
        setOrderQty([Math.floor((totalQty || 0) / 2), Math.floor((totalQty || 0) / 2)])
      } else {
        setOrderQty(Array(splitTo).fill(0))
      }
    } else {
      setOrderQty([])
    }
  }, [splitTo, totalQty])

  useEffect(() => {
    if (store.data.length > 0) {
      const sumQty = store.data.reduce((a: number, b: MonitorDataType) => a + b.qty, 0)
      const sumExecutedPrice = store.data.reduce((a: number, b: MonitorDataType) => a + b.executedPrice * b.qty, 0)
      setAvgPrice(sumExecutedPrice / sumQty)
      setTotalQty(sumQty)
      setOrderId(store.data[0].orderId)
    }
  }, [store.data])

  return (
    <Container>
      <Dialog
        open={modalOpen}
        disableEscapeKeyDown
        aria-labelledby='title'
        aria-describedby='description'
        fullWidth={true}
      >
        <DialogTitle id='title'>Split Equity</DialogTitle>
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
                  {orderId}
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
                    Split To (#Orders) :
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <TextField
                  size={'small'}
                  fullWidth
                  value={splitTo}
                  disabled={false}
                  onChange={event => {
                    if (event.target.value && Number(event.target.value) > 0 && Number(event.target.value) <= 10) {
                      setSplitTo(Number(event.target.value))
                    } else {
                      setSplitTo(null)
                    }
                  }}
                />
              </Grid>
            </Grid>
            {splitTo !== null &&
              splitTo > 0 &&
              orderQty.length > 0 &&
              orderQty.map((qty: number, i: number) => {
                return (
                  <Grid container spacing={2} sx={{ marginTop: 0 }} key={`${orderId}-${i}`}>
                    <Grid item xs={4}>
                      <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                        <Typography
                          variant='body1'
                          gutterBottom
                          noWrap
                          sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                        >
                          Order {i + 1} Qty:
                        </Typography>
                      </Box>
                    </Grid>
                    <Grid item xs={8} sx={{ textAlign: 'left' }}>
                      <TextField
                        size='small'
                        fullWidth
                        value={qty ?? 0}
                        onChange={event => handleQtyChange(i, Number(event.target.value))}
                      />
                    </Grid>
                  </Grid>
                )
              })}
          </DialogContentText>
        </DialogContent>
        <DialogActions className='dialog-actions-dense'>
          <Button onClick={handleClose}>Deny</Button>
          <Button onClick={handleSplit} disabled={totalOrderQty() !== totalQty}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
      <Button
        size='medium'
        type='submit'
        sx={{ width: '200px', fontWeight: 700, marginTop: 0 }}
        variant='contained'
        disabled={store.data?.length !== 1 || store.data[0]?.mainId !== null}
        onClick={handleOpen}
      >
        Split Equity
      </Button>
    </Container>
  )
}

export default CreateSplitModal
