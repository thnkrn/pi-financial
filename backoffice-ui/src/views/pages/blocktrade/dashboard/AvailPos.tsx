// ** React Imports
import { useEffect, useCallback } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'

// ** MUI Imports
import { Box, CircularProgress, Typography } from '@mui/material'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import Paper from '@mui/material/Paper'
import Table from '@mui/material/Table'
import TableBody from '@mui/material/TableBody'
import TableContainer from '@mui/material/TableContainer'
import TableHead from '@mui/material/TableHead'
import TableRow from '@mui/material/TableRow'

// ** Custom Components Imports
import { StyledTableCell, StyledTableRow } from '../styled/table'
import { colorTextLS } from '@/utils/blocktrade/color'

// ** Redux Imports
import { fetchPosition } from 'src/store/apps/blocktrade/position'

const AvailPos = () => {
  const positionStore = useSelector((state: any) => state.btPosition)
  const orderStore = useSelector((state: any) => state.btOrder)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const handlePosition = useCallback(
    (userId?: number) => {
      const state = {
        ofUser: userId ?? null,
        page: 1,
        pageSize: 100,
        orderBy: 'CreatedAt',
        orderDir: 'DESC',
      }
      dispatch(fetchPosition(state))
    },
    [dispatch]
  )

  useEffect(() => {
    handlePosition(orderStore?.ic)
  }, [orderStore.ic, handlePosition])

  return (
    <Box sx={{ height: '100%' }}>
      <Card sx={{ textAlign: 'center', marginLeft: 0, marginRight: { xs: 0, lg: 0 }, marginTop: 0, height: '100%' }}>
        <CardHeader
          sx={{ backgroundColor: 'primary.main', paddingY: 2 }}
          title={
            <Typography variant='h6' sx={{ color: 'white', fontWeight: 700 }}>
              Available Positions
            </Typography>
          }
        />
        <CardContent sx={{ padding: 0, marginBottom: -5 }}>
          <TableContainer component={Paper} sx={{ maxHeight: 530 }}>
            <Table size='small'>
              <TableHead>
                <TableRow>
                  <StyledTableCell align='center'>Symbol</StyledTableCell>
                  <StyledTableCell align='center'>L/S</StyledTableCell>
                  <StyledTableCell align='center'>Open Pos</StyledTableCell>
                  <StyledTableCell align='center'>Avail Pos</StyledTableCell>
                  <StyledTableCell align='center'>Cust.</StyledTableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {positionStore.data.map((row: any) => (
                  <StyledTableRow key={row.id}>
                    <StyledTableCell align='center'>
                      {row.symbol.symbol}
                      {row.series.series}
                    </StyledTableCell>
                    <StyledTableCell align='center'>{colorTextLS(row.clientSide)}</StyledTableCell>
                    <StyledTableCell align='right'>{Number(row.numOfContract).toLocaleString()}</StyledTableCell>
                    <StyledTableCell align='right'>{Number(row.availContract).toLocaleString()}</StyledTableCell>
                    <StyledTableCell align='right'>{row.customerAccount}</StyledTableCell>
                  </StyledTableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
          {positionStore.isLoading && (
            <CircularProgress sx={{ marginLeft: 'auto', marginRight: 'auto', marginTop: 3 }} />
          )}
        </CardContent>
      </Card>
    </Box>
  )
}

export default AvailPos
