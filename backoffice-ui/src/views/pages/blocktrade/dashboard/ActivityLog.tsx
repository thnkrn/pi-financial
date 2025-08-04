// ** React Imports
import { useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

// ** MUI Imports
import { Box, CircularProgress, Link, Typography } from '@mui/material'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import Table from '@mui/material/Table'
import TableBody from '@mui/material/TableBody'
import TableContainer from '@mui/material/TableContainer'
import TableHead from '@mui/material/TableHead'
import TableRow from '@mui/material/TableRow'

// ** Custom Components
import { ActivityLogsInitialState } from '@/constants/blocktrade/InitialState'
import { FormatDateTime } from '@/utils/blocktrade/date'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { default as NextLink } from 'next/link'
import { fetchActivityLog } from 'src/store/apps/blocktrade/log'
import { StyledTableCell, StyledTableRow } from '../styled/table'

const ActivityLog = () => {
  const initialState = ActivityLogsInitialState

  const storeLog = useSelector((state: any) => state.btLog)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  useEffect(() => {
    dispatch(fetchActivityLog(initialState))
  }, [dispatch, initialState])

  return (
    <Box>
      <Card sx={{ textAlign: 'center', marginLeft: 0, marginRight: { xs: 0, lg: 0 }, marginTop: 0 }}>
        <CardHeader
          sx={{ backgroundColor: 'primary.main', paddingY: 2 }}
          title={
            <Box display='flex' justifyContent='space-between' alignItems='center'>
              <Box>
                <Typography sx={{ visibility: 'hidden' }}>View all</Typography>
              </Box>
              <Typography variant='h6' sx={{ color: 'white', fontWeight: 700 }}>
                Activity Logs
              </Typography>
              <Link
                href='../logs'
                sx={{ color: 'white', fontWeight: 400, fontSize: '16px', textDecoration: 'none' }}
                component={NextLink}
              >
                View all
              </Link>
            </Box>
          }
        />
        <CardContent sx={{ padding: 0, marginBottom: -5 }}>
          <TableContainer sx={{ maxHeight: 200 }}>
            <Table size='small' stickyHeader={true}>
              <TableHead>
                <TableRow>
                  <StyledTableCell align='right'>Date/Time</StyledTableCell>
                  <StyledTableCell align='center'>Actions</StyledTableCell>
                  <StyledTableCell align='center'>Details</StyledTableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {storeLog.data.map((row: any) => (
                  <StyledTableRow key={row.id}>
                    <StyledTableCell align='center'>{FormatDateTime(row.createdAt)}</StyledTableCell>
                    <StyledTableCell align='center'>{row.action.replace(/_/g, ' ')}</StyledTableCell>
                    <StyledTableCell align='left'>{row.detail}</StyledTableCell>
                  </StyledTableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
          {storeLog.isLoading && <CircularProgress sx={{ marginLeft: 'auto', marginRight: 'auto', marginTop: 3 }} />}
        </CardContent>
      </Card>
    </Box>
  )
}

export default ActivityLog
