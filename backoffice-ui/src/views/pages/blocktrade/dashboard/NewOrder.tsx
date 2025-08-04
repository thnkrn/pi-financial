// ** React Imports
import { useEffect, useState, useCallback, SyntheticEvent } from 'react'

// ** MUI Imports
import { Box, Grid, TextField, Typography } from '@mui/material'
import Autocomplete from '@mui/material/Autocomplete'
import Button from '@mui/material/Button'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import CircularProgress from '@mui/material/CircularProgress'
import InputAdornment from '@mui/material/InputAdornment'
import { useTheme } from '@mui/material/styles'

// ** Custom Components Imports
import SelectOption from '../SelectOption'
import PhaseSideRadio from './PhaseSideRadio'
import { handleNewOrder } from 'src/hooks/blocktrade/useNewOrder'
import { handleAmendOrder } from 'src/hooks/blocktrade/useAmendOrder'
import { DecimalNumber } from 'src/utils/blocktrade/decimal'
import map from 'lodash/map'
import SelectOptionPre from '@/views/pages/blocktrade/SelectOptionPre'
import { ActivityLogsInitialState } from '@/constants/blocktrade/InitialState'

// ** Redux Imports
import { useDispatch, useSelector } from 'react-redux'
import {
  resetState,
  updateSymbol,
  updateCustomer,
  updateFuturesProperty,
  updateContractAmount,
  updateEquityPrice,
  updateOrderType,
  updateIC,
} from '@/store/apps/blocktrade/order'
import { fetchEquityOrder } from '@/store/apps/blocktrade/equity'
import { updateMktSymbol } from '@/store/apps/blocktrade/market'
import { fetchSymbolListData } from '@/store/apps/blocktrade/symbol-list'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { fetchActivityLog } from '@/store/apps/blocktrade/log'
import { fetchCustomerListData } from '@/store/apps/blocktrade/customer-list'
import { IGetCustomerListResponse } from '@/lib/api/clients/blocktrade/customers/types'
import { validateAndRoundPrice } from '@/utils/blocktrade/validateEquityPrice'
import { OC, Side } from '@/constants/blocktrade/GlobalEnums'
import { ISymbolList } from '@/views/pages/blocktrade/dashboard/types'

const NewOrder = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const orderStore = useSelector((state: any) => state.btOrder)
  const equityStore = useSelector((state: any) => state.btEquity)
  const symbolListStore = useSelector((state: any) => state.btSymbolList)
  const userStore = useSelector((state: any) => state.btUser)
  const icStore = useSelector((state: any) => state.btDropdown.values.accountList)
  const customerListStore = useSelector((state: any) => state.btCustomerList)

  const theme = useTheme()

  const orderType = [
    { key: 'LIMIT', value: 'LIMIT' },
    { key: 'ATO', value: 'ATO' },
    { key: 'ATC', value: 'ATC' },
  ]
  const [symbolList, setSymbolList] = useState<ISymbolList[]>([])
  const [customerName, setCustomerName] = useState<string | null>('-')
  const [ic, setIC] = useState<number>(orderStore.ic || userStore.data?.id)
  const [autoCompleteOpen, setAutoCompleteOpen] = useState<boolean>(false)

  const checkSubmitEnable =
    orderStore.futuresProperty &&
    orderStore.contractAmount &&
    orderStore.contractAmount > 0 &&
    (orderStore.side === Side.LONG || orderStore.side === Side.SHORT) &&
    (orderStore.oc === OC.OPEN || orderStore.oc === OC.CLOSE) &&
    orderStore.equityPrice &&
    orderStore.equityPrice > 0 &&
    orderStore.customer

  const handleOrderSubmit = async () => {
    const result = await handleNewOrder(dispatch, orderStore)

    if (result.success) {
      dispatch(fetchEquityOrder(equityStore.filter))
      dispatch(fetchActivityLog(ActivityLogsInitialState))
      handleReset()
    }
  }

  const handleOrderAmend = async () => {
    const result = await handleAmendOrder(dispatch, orderStore)

    if (result.success) {
      dispatch(fetchEquityOrder(equityStore.filter))
      dispatch(fetchActivityLog(ActivityLogsInitialState))
      handleReset()
    }
  }

  const handleSymbolSelected = async (value: string | null) => {
    dispatch(updateSymbol(value))
  }

  const handleICSelected = useCallback(
    async (userId: number) => {
      if (icStore) {
        setIC(userId)
        dispatch(updateIC(userId))
        const selectedUser = icStore.find((user: { key: number }) => user.key === userId)
        if (!selectedUser) {
          return
        }

        const employeeId = selectedUser.extension
        dispatch(fetchCustomerListData(employeeId))
      }
    },
    [dispatch, icStore]
  )

  const handleCustomerSelected = async (value: string) => {
    await dispatch(updateCustomer(value))

    const customerList: IGetCustomerListResponse = Object.values(customerListStore.data)
    const foundCustomer = customerList.find(customer => String(customer.accountNo) === value)
    setCustomerName(foundCustomer ? foundCustomer.customerName : 'Customer not found')
  }

  const handleReset = () => {
    dispatch(resetState())
    handleICSelected(userStore.data.id)
    setCustomerName('-')
  }

  useEffect(() => {
    const futuresProp = symbolList.find((symbol: { value: string }) => symbol.value === orderStore.symbol)
    dispatch(updateFuturesProperty(futuresProp))
    if (orderStore.symbol) {
      dispatch(updateMktSymbol(futuresProp ? futuresProp.symbol : null))
    }
  }, [orderStore.symbol, dispatch, symbolList])

  useEffect(() => {
    dispatch(fetchSymbolListData())
  }, [dispatch])

  useEffect(() => {
    if (!ic) {
      handleICSelected(userStore.data.id)
    }
  }, [userStore.data.id, handleICSelected, ic])

  useEffect(() => {
    const mapOptions = map(symbolListStore.data, option => {
      return {
        key: option.symbol + option.series,
        value: option.symbol + option.series,
        symbol: option.symbol,
        series: option.series,
        mul: option.multiplier,
        exp: option.expDate,
        im: option.mm * 1.75,
        blocksize: option.blocksize,
      }
    })

    setSymbolList(mapOptions)
  }, [symbolListStore.data])

  return (
    <Box sx={{ height: '100%' }}>
      {orderStore.isLoading && (
        <Box
          sx={{
            position: 'fixed',
            top: 0,
            left: 0,
            width: '100%',
            height: '100%',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            zIndex: 9999,
          }}
        >
          <CircularProgress color='inherit' />
        </Box>
      )}
      <Card
        sx={{
          textAlign: 'center',
          marginLeft: 0,
          marginRight: { xs: 0, lg: 0 },
          marginTop: 0,
          height: '100%',
          backgroundColor:
            orderStore.side === 'LONG'
              ? theme.palette.mode === 'dark'
                ? '#094074'
                : '#DDEAEE'
              : orderStore.side === 'SHORT'
              ? theme.palette.mode === 'dark'
                ? '#FF3B3E'
                : '#E9DDDD'
              : '',
        }}
      >
        <CardHeader
          sx={{ backgroundColor: !orderStore.isAmend ? 'primary.main' : '#FC9D22', paddingY: 2 }}
          title={
            <Typography variant='h6' sx={{ color: 'white', fontWeight: 700 }}>
              {!orderStore.isAmend ? 'New Order' : 'Amend Order'}
            </Typography>
          }
        />
        <CardContent sx={{ marginY: 2 }}>
          <PhaseSideRadio />
          <Grid container spacing={2} sx={{ marginTop: 0 }}>
            <Grid item xs={4}>
              <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  IC Account
                </Typography>
              </Box>
            </Grid>
            <Grid item xs={8} sx={{ textAlign: 'left' }}>
              <SelectOptionPre
                id={'accountList'}
                labelId={'accountList'}
                label={''}
                defaultValue={ic}
                disabled={orderStore.isAmend}
                remote={{
                  field: 'accountList',
                  url: 'users/getMemberInfoFromGroups?dropdown=true',
                  key: 'key',
                  value: 'value',
                  extension: 'extension',
                }}
                onChange={key => {
                  handleICSelected(Number(key))
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
                  Symbol
                </Typography>
              </Box>
            </Grid>
            <Grid item xs={8} sx={{ textAlign: 'left' }}>
              <Autocomplete
                disablePortal
                freeSolo
                open={autoCompleteOpen}
                id='symbol'
                options={symbolList.map((option: { value: string }) => option.value)}
                inputValue={orderStore.symbol ? orderStore.symbol : ''}
                onInputChange={(event: SyntheticEvent<Element, Event>, newInputValue: string) => {
                  if (newInputValue.length === 0) {
                    if (autoCompleteOpen) setAutoCompleteOpen(false)
                  } else {
                    setAutoCompleteOpen(true)
                  }
                  handleSymbolSelected(newInputValue.toUpperCase())
                }}
                size='small'
                renderInput={params => (
                  <TextField
                    {...params}
                    label=''
                    InputProps={{ ...params.InputProps, style: { paddingLeft: 8, paddingRight: 8 } }}
                  />
                )}
                onClose={() => setAutoCompleteOpen(false)}
                onChange={(event: SyntheticEvent<Element, Event>, value: string | null) => {
                  if (autoCompleteOpen) setAutoCompleteOpen(false)
                  handleSymbolSelected(value?.toUpperCase() ?? null)
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
                  Cust. Acc.
                </Typography>
              </Box>
            </Grid>
            <Grid item xs={8} sx={{ textAlign: 'left' }}>
              <TextField
                size={'small'}
                fullWidth
                label=''
                placeholder=''
                value={orderStore.customer || ''}
                disabled={orderStore.isAmend}
                onChange={event => {
                  const newValue = event.target.value
                  if (/^\d*$/.test(newValue) && newValue.length <= 8) {
                    if (newValue.length === 8) {
                      handleCustomerSelected(newValue)
                    } else {
                      dispatch(updateCustomer(newValue))
                      setCustomerName('-')
                    }
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
                  Cust. Info
                </Typography>
              </Box>
            </Grid>
            <Grid item xs={8} sx={{ textAlign: 'left' }}>
              <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {customerName}
                </Typography>
              </Box>
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
                  Qty (Contracts)
                </Typography>
              </Box>
            </Grid>
            <Grid item xs={8} sx={{ textAlign: 'left' }}>
              <TextField
                size={'small'}
                fullWidth
                label=''
                placeholder=''
                value={orderStore.contractAmount ? orderStore.contractAmount : ''}
                disabled={orderStore.isAmend}
                onChange={event => {
                  dispatch(updateContractAmount(Number(event.target.value)))
                }}
                InputProps={{
                  endAdornment: (
                    <InputAdornment position='end'>
                      <Box
                        display='flex'
                        bgcolor='#AEAEAE'
                        color='common.white'
                        mx={-3.5}
                        px={2}
                        py={0.5}
                        height={40}
                        sx={{ borderRadius: '0 10px 10px 0' }}
                      >
                        <Typography color='common.white' sx={{ marginY: 'auto' }}>
                          Min:{' '}
                          {orderStore.futuresProperty
                            ? `${DecimalNumber(orderStore.futuresProperty.blocksize, 0)}`
                            : '-'}
                        </Typography>
                      </Box>
                    </InputAdornment>
                  ),
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
                  Qty (Shares)
                </Typography>
              </Box>
            </Grid>
            <Grid item xs={8}>
              <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {orderStore.contractAmount && orderStore.futuresProperty
                    ? DecimalNumber(orderStore.contractAmount * orderStore.futuresProperty.mul, 0)
                    : '-'}
                </Typography>
              </Box>
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
                  Equity Price
                </Typography>
              </Box>
            </Grid>
            <Grid item xs={8} sx={{ textAlign: 'left' }}>
              <TextField
                size={'small'}
                fullWidth
                label=''
                placeholder=''
                disabled={
                  orderStore.orderType === 'LIMIT'
                    ? orderStore
                      ? orderStore.orderStatus === 'Matched'
                        ? true
                        : false
                      : false
                    : true
                }
                value={orderStore.equityPrice ? orderStore.equityPrice : ''}
                onChange={e => {
                  dispatch(updateEquityPrice(e.target.value))
                }}
                onBlur={e => {
                  const validatedPrice = validateAndRoundPrice(e.target.value)
                  dispatch(updateEquityPrice(validatedPrice))
                }}
                InputProps={{
                  endAdornment: (
                    <InputAdornment position='end'>
                      <Box sx={{ mr: -3.5, width: 90 }}>
                        <SelectOption
                          id={'orderType'}
                          label={''}
                          labelId={'orderType'}
                          options={orderType}
                          disabled={orderStore.isAmend}
                          defaultValue={orderStore.orderType ? orderStore.orderType : 'LIMIT'}
                          onChange={value => {
                            dispatch(updateOrderType(value))
                          }}
                        />
                      </Box>
                    </InputAdornment>
                  ),
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
                  IM
                </Typography>
              </Box>
            </Grid>
            <Grid item xs={8}>
              <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                <Typography
                  variant='body1'
                  gutterBottom
                  noWrap
                  sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
                >
                  {orderStore.contractAmount && orderStore.futuresProperty
                    ? DecimalNumber(orderStore.contractAmount * orderStore.futuresProperty.im, 0)
                    : '-'}
                </Typography>
              </Box>
            </Grid>
          </Grid>
          <Box display='flex' sx={{ marginTop: 4, marginLeft: -1, height: '100%' }}>
            <Button
              size='medium'
              onClick={!orderStore.isAmend ? handleOrderSubmit : handleOrderAmend}
              sx={{
                width: '100%',
                fontWeight: 700,
                marginTop: 0,
                marginLeft: { lg: 1 },
                backgroundColor:
                  orderStore.contractAmount &&
                  orderStore.futuresProperty &&
                  orderStore.futuresProperty.blocksize > orderStore.contractAmount
                    ? '#FC9D22'
                    : !orderStore.isAmend
                    ? ''
                    : '#FC9D22',
                '&:hover': {
                  backgroundColor:
                    orderStore.contractAmount &&
                    orderStore.futuresProperty &&
                    orderStore.futuresProperty.blocksize > orderStore.contractAmount
                      ? '#E37F04'
                      : !orderStore.isAmend
                      ? ''
                      : '#E37F04',
                },
              }}
              variant='contained'
              disabled={!checkSubmitEnable}
            >
              {!orderStore.isAmend
                ? orderStore.contractAmount &&
                  orderStore.futuresProperty &&
                  orderStore.futuresProperty.blocksize > orderStore.contractAmount
                  ? 'Partial Send'
                  : 'Send'
                : 'Amend'}
            </Button>
            <Button
              size='medium'
              onClick={() => {
                handleReset()
              }}
              sx={{ width: '100%', fontWeight: 700, marginTop: 0, marginLeft: { lg: 1 } }}
              color='secondary'
              variant='outlined'
              disabled={false}
            >
              Reset
            </Button>
          </Box>
        </CardContent>
      </Card>
    </Box>
  )
}

export default NewOrder
