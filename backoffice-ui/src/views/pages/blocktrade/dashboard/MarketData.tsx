// ** React Imports
import { useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

// ** MUI Imports
import Box from '@mui/material/Box'
import Grid from '@mui/material/Grid'
import Typography from '@mui/material/Typography'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import Paper from '@mui/material/Paper'
import Table from '@mui/material/Table'
import TableBody from '@mui/material/TableBody'
import TableContainer from '@mui/material/TableContainer'
import TableHead from '@mui/material/TableHead'
import TableRow from '@mui/material/TableRow'
import Skeleton from '@mui/material/Skeleton'

// ** Custom Components Imports
import SelectOption from '../SelectOption'
import { StyledTableCell, StyledTableRow } from '../styled/table'
import { fetchMarketData, updateMktSymbol, resetState } from '@/store/apps/blocktrade/market'
import { DecimalNumber } from '@/utils/blocktrade/decimal'
import { colorTextPnL } from '@/utils/blocktrade/color'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { IOrderBookResponse } from '@/lib/api/clients/blocktrade/market/types'
import { ISymbolList } from '@/views/pages/blocktrade/dashboard/types'

interface SymbolPart {
  key: string
  value: string
}

const OrderBookRow = ({ dataRow, isLoading }: { dataRow: IOrderBookResponse; isLoading: boolean }) => {
  return (
    <>
      {dataRow.orderBook?.map(data => (
        <StyledTableRow key={`orderbook-row-${data.id}`}>
          <StyledTableCell align='right'>
            {isLoading ? (
              <Skeleton variant='text' sx={{ fontSize: '1rem' }} />
            ) : data.bidVolume ? (
              DecimalNumber(data.bidVolume, 0)
            ) : (
              '-'
            )}
          </StyledTableCell>
          <StyledTableCell align='center'>
            {isLoading ? (
              <Skeleton variant='text' sx={{ fontSize: '1rem' }} />
            ) : data.bidPrice ? (
              DecimalNumber(data.bidPrice, 2)
            ) : (
              '-'
            )}
          </StyledTableCell>
          <StyledTableCell align='center'>
            {isLoading ? (
              <Skeleton variant='text' sx={{ fontSize: '1rem' }} />
            ) : data.askPrice ? (
              DecimalNumber(data.askPrice, 2)
            ) : (
              '-'
            )}
          </StyledTableCell>
          <StyledTableCell align='right'>
            {isLoading ? (
              <Skeleton variant='text' sx={{ fontSize: '1rem' }} />
            ) : data.askVolume ? (
              DecimalNumber(data.askVolume, 0)
            ) : (
              '-'
            )}
          </StyledTableCell>
        </StyledTableRow>
      ))}
    </>
  )
}

const MarketData = () => {
  const marketStore = useSelector((state: any) => state.btMarket)
  const symbolListStore = useSelector((state: any) => state.btSymbolList)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const handleSymbolSelected = async (value: string) => {
    dispatch(updateMktSymbol(value))
  }

  useEffect(() => {
    if (marketStore.symbol) {
      dispatch(fetchMarketData(marketStore.symbol))
    } else {
      dispatch(resetState())
    }
  }, [marketStore.symbol, dispatch])

  const symbolPartsMap: { [key: string]: SymbolPart } = {}
  Object.values(symbolListStore.data as ISymbolList[]).forEach((symbol: ISymbolList) => {
    const value: string = symbol.symbol
    if (!symbolPartsMap[value]) {
      symbolPartsMap[value] = { key: value, value }
    }
  })
  const uniqueSymbolParts = Object.values(symbolPartsMap)

  const renderChange = () => {
    if (marketStore.isLoading) {
      return <Skeleton variant='text' width={40} sx={{ fontSize: '1rem' }} />
    }

    if (marketStore.data.symbolInfo.lastPrice !== null) {
      return colorTextPnL(marketStore.data.symbolInfo.lastPrice - marketStore.data.symbolInfo.lastOpen)
    }

    return '-'
  }

  return (
    <div>
      <Card sx={{ textAlign: 'center', marginLeft: 0, marginRight: { xs: 0, lg: 0 }, marginTop: 0, height: '100%' }}>
        <CardHeader
          sx={{ backgroundColor: 'primary.main', paddingY: 2 }}
          title={
            <Typography variant='h6' sx={{ color: 'white', fontWeight: 700 }}>
              Market Data
            </Typography>
          }
        />
        <CardContent sx={{ paddingY: 0 }}>
          <Grid container spacing={2} sx={{ marginTop: 2 }} rowGap={0}>
            <Grid item xs={12} md={6}>
              <Grid container spacing={2}>
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
                  <SelectOption
                    id={'symbol'}
                    label={''}
                    labelId={'symbol'}
                    options={uniqueSymbolParts}
                    defaultValue={marketStore.symbol}
                    disabled={false}
                    onChange={value => {
                      handleSymbolSelected(value)
                    }}
                  />
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12} md={6} sx={{ marginY: 'auto' }}>
              <Box sx={{ display: 'flex', justifyContent: 'right' }}>
                <Box sx={{ marginRight: 1 }}>Change:</Box>
                <Box>{renderChange()}</Box>
              </Box>
            </Grid>
          </Grid>
        </CardContent>
        <CardContent sx={{ padding: 0, marginBottom: -5 }}>
          <TableContainer component={Paper}>
            <Table size='small'>
              <TableHead>
                <TableRow>
                  <StyledTableCell align='right'>Volume</StyledTableCell>
                  <StyledTableCell align='center'>Bid</StyledTableCell>
                  <StyledTableCell align='center'>Offer</StyledTableCell>
                  <StyledTableCell align='right'>Volume</StyledTableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                <OrderBookRow dataRow={marketStore.data} isLoading={marketStore.isLoading} />
              </TableBody>
            </Table>
          </TableContainer>
        </CardContent>
      </Card>
    </div>
  )
}

export default MarketData
