// React Imports
import { useCallback, useEffect, useState } from 'react'
import { useDispatch } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'

// MUI Imports
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import Dialog from '@mui/material/Dialog'
import Box from '@mui/material/Box'
import Grid from '@mui/material/Grid'
import Typography from '@mui/material/Typography'
import FlipCameraAndroidIcon from '@mui/icons-material/FlipCameraAndroid'
import IconButton from '@mui/material/IconButton'
import CircularProgress from '@mui/material/CircularProgress'
import Chip from '@mui/material/Chip'
import TextField from '@mui/material/TextField'
import Autocomplete from '@mui/material/Autocomplete'
import Checkbox from '@mui/material/Checkbox'
import FormControlLabel from '@mui/material/FormControlLabel'
import FormGroup from '@mui/material/FormGroup'

// Other Imports
import SelectOptionPre from '@/views/pages/blocktrade/SelectOptionPre'
import { DecimalNumber } from '@/utils/blocktrade/decimal'
import { PositionRowType } from '@/views/pages/blocktrade/portfolio/types'
import { OC, Side } from '@/constants/blocktrade/GlobalEnums'
import { PositionInitialState } from '@/constants/blocktrade/InitialState'
import { fetchPosition, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/position'
import {
  getRolloverExpectedFuturesPrice,
  getRolloverList,
  submitRollover,
} from '@/lib/api/clients/blocktrade/portfolio'
import { IRolloverExpectedFuturesPriceRequest } from '@/lib/api/clients/blocktrade/portfolio/types'

const RolloverModal = ({ row, mktPrice }: { row: PositionRowType; mktPrice: number }) => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const [futuresPrice, setFuturesPrice] = useState<number | null>(null)
  const [loadingPrice, setLoadingPrice] = useState<boolean>(false)
  const [modalOpen, setModalOpen] = useState<boolean>(false)
  const [avgPrice, setAvgPrice] = useState<number>(0)
  const [orderList, setOrderList] = useState<any>([])
  const [orderValue, setOrderValue] = useState<any>([])
  const [fixedOptions, setFixedOptions] = useState<any>([])
  const [isPartialRollover, setIsPartialRollover] = useState<boolean>(false)
  const [rolloverQty, setRolloverQty] = useState<number | null>(null)
  const [rolloverPriceString, setRolloverPriceString] = useState<string | null>(null)
  const [rolloverPrice, setRolloverPrice] = useState<number | null>(null)
  const [rolloverSeries, setRolloverSeries] = useState<string>('')
  const [loadingOrder, setLoadingOrder] = useState<boolean>(true)

  const handleOpen = () => {
    setModalOpen(true)
    fetchOrderList()
    setRolloverQty(row.availContract ?? 0)
    setAvgPrice(row.futuresOrders.equityPrice ?? 0)
    setRolloverPrice(mktPrice)
    setRolloverPriceString(mktPrice.toString())
  }

  const handleClose = () => {
    setModalOpen(false)
    handleResetInfo()
  }

  const handleResetInfo = () => {
    setLoadingOrder(true)
    setOrderList([])
    setOrderValue([])
    setFixedOptions([])
    setRolloverQty(0)
    setRolloverPriceString(null)
    setRolloverPrice(null)
    setRolloverSeries('')
    setIsPartialRollover(false)
  }

  const fetchOrderList = async () => {
    try {
      const response = await getRolloverList({ blockId: row.id ?? 0 })
      const ordersArray = Object.values(response)
      const fetchedOrders = ordersArray.map((order: any) => ({
        title: `${order.availContract}C@${order.futuresOrders.equityPrice}(${new Date(
          order.createdAt
        ).toLocaleDateString()})`,
        detail: `${order.availContract} Conts @ ${order.futuresOrders.equityPrice} (${new Date(
          order.createdAt
        ).toLocaleDateString()})`,
        equityPrice: order.futuresOrders.equityPrice,
        qty: order.availContract,
        createdAt: order.createdAt,
        blockId: order.id,
      }))
      setOrderList(fetchedOrders)

      const fixedOrder = fetchedOrders.find(order => order.blockId === row.id)
      if (fixedOrder) {
        setFixedOptions([fixedOrder])
        setOrderValue([fixedOrder])
      }
      setLoadingOrder(false)
    } catch (error) {
      setOrderList([])
    }
  }

  const fetchExpectedFuturesPrice = useCallback(async () => {
    setLoadingPrice(true)
    setFuturesPrice(null)
    if (row.openClose === OC.OPEN) {
      const payload: IRolloverExpectedFuturesPriceRequest = {
        openClose: OC.CLOSE,
        side: row.clientSide === Side.LONG ? Side.SHORT : Side.LONG,
        symbol: row.symbol.symbol ?? '',
        series: row.series.series ?? '',
        rollToSeries: rolloverSeries,
        rollPrice: Number(rolloverPrice),
        customerAccount: row.customerAccount ?? '',
        blocks: orderValue.map((item: any) => ({ blockOrderId: item.blockId })),
      }
      try {
        const response = await getRolloverExpectedFuturesPrice(payload)
        if (response) {
          setFuturesPrice(response.expectedFuturesPrice)
        }
      } catch (error) {
        setFuturesPrice(null)
      }
    } else {
      setFuturesPrice(avgPrice)
    }
    setLoadingPrice(false)
  }, [
    row.customerAccount,
    row.clientSide,
    row.series.series,
    row.symbol.symbol,
    orderValue,
    rolloverSeries,
    rolloverPrice,
    avgPrice,
    row.openClose,
  ])

  const handleRollover = async () => {
    if (futuresPrice && rolloverPrice && rolloverQty && row.customerAccount) {
      const payload = {
        rollToSeries: rolloverSeries,
        rollPrice: rolloverPrice,
        rollQty: rolloverQty,
        customerAccount: row.customerAccount,
        blocks: orderValue.map((item: { blockId: number }) => ({ blockOrderId: item.blockId })),
      }
      try {
        const response = await submitRollover(payload)
        if (response) {
          handleClose()
          dispatch(updateFilterState(PositionInitialState))
          dispatch(fetchPosition(PositionInitialState))
          dispatch(updateLastUpdated(new Date().toLocaleString()))
        }
      } catch (error) {
        return null
      }
    }
  }

  const displayFuturesPrice = (futuresPrice: number | null) => {
    if (futuresPrice) {
      return DecimalNumber(futuresPrice, 4)
    } else {
      return '-'
    }
  }

  useEffect(() => {
    if (orderValue.length > 1) {
      setIsPartialRollover(false)
    }
    const sumQty = orderValue.reduce((a: any, b: any) => a + b.qty, 0)
    const sumExecutedPrice = orderValue.reduce((a: any, b: any) => a + b.equityPrice * b.qty, 0)
    setRolloverQty(sumQty)
    setAvgPrice(sumExecutedPrice / sumQty)
  }, [orderValue])

  useEffect(() => {
    if (
      row.customerAccount &&
      row.clientSide &&
      row.series.series &&
      row.symbol.symbol &&
      orderValue.length > 0 &&
      rolloverSeries &&
      rolloverPrice
    ) {
      fetchExpectedFuturesPrice()
    }
  }, [
    fetchExpectedFuturesPrice,
    row.customerAccount,
    row.clientSide,
    row.series.series,
    row.symbol.symbol,
    orderValue.length,
    rolloverSeries,
    rolloverPrice,
  ])

  return (
    <>
      <Dialog open={modalOpen} disableEscapeKeyDown fullWidth={true}>
        <DialogTitle id='title'>Rollover</DialogTitle>
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
                  {(row.symbol.symbol ?? '') + (row.series.series ?? '')}
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
                  {row.clientSide} {row.openClose}
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
                    Order
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Autocomplete
                  multiple
                  id='fixed-tags-order'
                  fullWidth
                  size={'small'}
                  value={orderValue}
                  onChange={(event, newValue) => {
                    setOrderValue([...fixedOptions, ...newValue.filter(option => fixedOptions.indexOf(option) === -1)])
                  }}
                  options={orderList}
                  getOptionLabel={option => option.detail}
                  renderTags={(tagValue, getTagProps) =>
                    tagValue.map((option, index) => (
                      // eslint-disable-next-line react/jsx-key
                      <Chip
                        label={option.title}
                        {...getTagProps({ index })}
                        disabled={fixedOptions.indexOf(option) !== -1}
                      />
                    ))
                  }
                  renderInput={params => (
                    <>
                      {loadingOrder ? (
                        <CircularProgress size={42} thickness={8} />
                      ) : (
                        <TextField {...params} label='' placeholder='' />
                      )}
                    </>
                  )}
                />
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
                    Partial Rollover
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <FormGroup>
                  <FormControlLabel
                    control={
                      <Checkbox
                        sx={{ py: 0 }}
                        checked={isPartialRollover}
                        onChange={e => {
                          setIsPartialRollover(e.target.checked)
                          setRolloverQty(row.availContract ?? 0)
                        }}
                        disabled={orderValue.length > 1}
                      />
                    }
                    label=''
                  />
                </FormGroup>
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
                {isPartialRollover ? (
                  <TextField
                    size={'small'}
                    fullWidth
                    label=''
                    placeholder=''
                    value={rolloverQty}
                    disabled={false}
                    onChange={event => {
                      if (
                        event.target.value &&
                        Number(event.target.value) > 0 &&
                        Number(event.target.value) <= (row.availContract ?? 0)
                      ) {
                        setRolloverQty(Number(event.target.value))
                      } else if (Number(event.target.value) > (row.availContract ?? 0)) {
                        setRolloverQty(row.availContract ?? 0)
                      } else {
                        setRolloverQty(null)
                      }
                    }}
                  />
                ) : (
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                  >
                    {rolloverQty}
                  </Typography>
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
                    Open Price
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
                    Account No.
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
                  {row.customerAccount}
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
                  {row.sales.employeeId + ' - ' + row.sales.name}
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
                    Rollover Price
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <TextField
                  size={'small'}
                  fullWidth
                  label=''
                  placeholder=''
                  value={rolloverPriceString}
                  disabled={false}
                  onChange={event => {
                    if (event.target.value) {
                      setRolloverPriceString(event.target.value)
                      setRolloverPrice(Number(event.target.value))
                    } else {
                      setRolloverPrice(null)
                    }
                  }}
                />
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
                    Rollover Series
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <SelectOptionPre
                  id={'seriesList'}
                  labelId={'seriesList'}
                  label={''}
                  defaultValue={rolloverSeries}
                  remote={{
                    field: 'seriesList',
                    url: `seriesProperties/getList?except=${row.series.series ?? null}`,
                    key: 'series',
                    value: 'series',
                  }}
                  onChange={key => {
                    setRolloverSeries(key)
                  }}
                />
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
                    Futures Price (Close)
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
                  {loadingPrice ? <CircularProgress size={18} thickness={8} /> : displayFuturesPrice(futuresPrice)}
                </Typography>
              </Grid>
            </Grid>
          </DialogContentText>
        </DialogContent>
        <DialogActions className='dialog-actions-dense'>
          <Button onClick={handleClose}>Deny</Button>
          <Button disabled={!futuresPrice} onClick={handleRollover}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
      <IconButton sx={{ mx: -1 }} onClick={handleOpen}>
        <FlipCameraAndroidIcon />
      </IconButton>
    </>
  )
}

export default RolloverModal
