// ** React Imports
import { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'

// ** MUI Imports
import { Grid, TextField } from '@mui/material'
import Stack from '@mui/material/Stack'
import Box from '@mui/material/Box'
import InputAdornment from '@mui/material/InputAdornment'
import Typography from '@mui/material/Typography'
import Autocomplete from '@mui/material/Autocomplete'

// ** Custom Components Imports
import { updateContractAmount, updateFuturesProperty, updateSymbol } from '@/store/apps/blocktrade/calculator'
import { fetchSymbolListData } from '@/store/apps/blocktrade/symbol-list'
import map from 'lodash/map'
import { DecimalNumber } from '@/utils/blocktrade/decimal'

const TopSecondCol = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const calculatorStore = useSelector((state: any) => state.btCalculator)
  const symbolListStore = useSelector((state: any) => state.btSymbolList)

  const [symbolList, setSymbolList] = useState<any>([])

  const handleSymbolSelected = async (value: any) => {
    const capitalizedValue = value ? value.toUpperCase() : ''
    dispatch(updateSymbol(capitalizedValue))
  }

  useEffect(() => {
    const futuresProp = symbolList.find((symbol: { value: string }) => symbol.value === calculatorStore.symbol)
    dispatch(updateFuturesProperty(futuresProp))
  }, [calculatorStore.symbol, dispatch, symbolList])

  useEffect(() => {
    dispatch(fetchSymbolListData())
  }, [dispatch])

  useEffect(() => {
    const mapOptions = map(symbolListStore.data, option => ({
      key: option.symbol + option.series,
      value: option.symbol + option.series,
      symbol: option.symbol,
      series: option.series,
      mul: option.multiplier,
      exp: option.expDate,
      im: option.mm * 1.75,
      blocksize: option.blocksize,
    }))

    setSymbolList(mapOptions)
  }, [symbolListStore.data])

  return (
    <Grid item xs={12} md={6} lg={3}>
      <Box>
        <Stack spacing={2}>
          <Box sx={{ paddingTop: 0 }}>
            <Grid container spacing={2}>
              <Grid item xs={5}>
                <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                  >
                    Symbol
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={7}>
                <Autocomplete
                  disablePortal
                  freeSolo
                  id='symbol'
                  options={symbolList.map((option: { value: string }) => option.value)}
                  inputValue={calculatorStore.symbol ?? ''}
                  onInputChange={(event, newInputValue) => {
                    handleSymbolSelected(newInputValue)
                  }}
                  size='small'
                  renderInput={params => (
                    <TextField
                      {...params}
                      label=''
                      InputProps={{ ...params.InputProps, style: { paddingLeft: 8, paddingRight: 8 } }}
                    />
                  )}
                  onChange={(event, value) => {
                    handleSymbolSelected(value)
                  }}
                />
              </Grid>
            </Grid>
          </Box>

          <Box sx={{ paddingTop: 0 }}>
            <Grid container spacing={2}>
              <Grid item xs={5}>
                <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                  >
                    QTY (Contracts)
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={7}>
                <TextField
                  size={'small'}
                  fullWidth
                  label=''
                  placeholder=''
                  value={calculatorStore.contractAmount ?? ''}
                  onChange={event => {
                    dispatch(updateContractAmount(Number(event.target.value ?? 0)))
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
                            {calculatorStore.futuresProperty
                              ? `${DecimalNumber(calculatorStore.futuresProperty.blocksize, 0)}`
                              : '-'}
                          </Typography>
                        </Box>
                      </InputAdornment>
                    ),
                  }}
                />
              </Grid>
            </Grid>
          </Box>

          <Box sx={{ paddingTop: 2 }}>
            <Grid container spacing={2}>
              <Grid item xs={5}>
                <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                  >
                    QTY (Shares)
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={7}>
                <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                  <Typography sx={{ marginY: 'auto' }}>
                    {calculatorStore.contractAmount && calculatorStore.futuresProperty
                      ? DecimalNumber(calculatorStore.contractAmount * calculatorStore.futuresProperty.mul, 0)
                      : '-'}
                  </Typography>
                  <Box
                    component='span'
                    display='flex'
                    justifyContent='flex-end'
                    textAlign='right'
                    sx={{ marginLeft: 0, width: '100%', height: '100%' }}
                  >
                    Mul:{' '}
                    {calculatorStore.futuresProperty
                      ? `${Number(calculatorStore.futuresProperty.mul).toLocaleString()}`
                      : '-'}
                  </Box>
                </Box>
              </Grid>
            </Grid>
          </Box>
        </Stack>
      </Box>
    </Grid>
  )
}

export default TopSecondCol
