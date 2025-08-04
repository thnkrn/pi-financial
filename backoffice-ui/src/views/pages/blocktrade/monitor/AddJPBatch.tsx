// ** React Imports
import { useCallback, useEffect, useState } from 'react'
import { useDispatch } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { useRouter } from 'next/router'

// ** MUI Imports
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import Dialog from '@mui/material/Dialog'
import CheckIcon from '@mui/icons-material/Check'
import Paper from '@mui/material/Paper'
import TableHead from '@mui/material/TableHead'
import TableRow from '@mui/material/TableRow'
import Table from '@mui/material/Table'
import TableBody from '@mui/material/TableBody'
import TableContainer from '@mui/material/TableContainer'
import Container from '@mui/material/Container'

// ** Other Imports
import Spreadsheet, { Matrix } from 'react-spreadsheet'
import { StyledTableCell, StyledTableRow } from '@/views/pages/blocktrade/styled/table'
import { JPSide, OC, Side } from '@/constants/blocktrade/GlobalEnums'
import { colorTextLS } from '@/utils/blocktrade/color'
import { DecimalNumber } from '@/utils/blocktrade/decimal'
import SelectOption from '@/views/pages/blocktrade/SelectOption'
import { EquityRowType, ListJPOrderPropsType } from '@/views/pages/blocktrade/monitor/types'
import { fetchEquityOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/equity'
import { EquityOrderFilterInitialState } from '@/constants/blocktrade/InitialState'
import { resetState } from '@/store/apps/blocktrade/monitor'
import { datadogLogs } from '@datadog/browser-logs'
import Swal from 'sweetalert2'
import APP_CONSTANTS from '@/constants/app'
import { listJP, submitJP } from '@/lib/api/clients/blocktrade/monitor'
import { IJPDataRequest, IListJPOrderResponse, ISubmitJPRequest } from '@/lib/api/clients/blocktrade/monitor/types'

const OCList = [
  {
    key: OC.OPEN,
    value: OC.OPEN,
  },
  {
    key: OC.CLOSE,
    value: OC.CLOSE,
  },
]

type ReactSpreadsheetProps = { value: string } | undefined

const displayAlert = (message: string) => {
  Swal.fire({
    title: 'Error!',
    text: message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE,
    icon: 'error',
  })
}

const AddJPBatchModal = () => {
  const router = useRouter()
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const [modalOpen, setModalOpen] = useState<boolean>(false)

  const initialData = [[{ value: 'Paste Here' }]]

  const [data, setData] = useState<ReactSpreadsheetProps[][]>(initialData)
  const [isReview, setIsReview] = useState<boolean>(true)
  const [orders, setOrders] = useState<IJPDataRequest[]>([])
  const [jpmOrderData, setJPMOrderData] = useState<ListJPOrderPropsType | {}>({})
  const [submitData, setSubmitData] = useState<IJPDataRequest[]>([])

  const handleOpen = () => {
    setModalOpen(true)
  }

  const handleClose = () => {
    handleResetInfo()
    setModalOpen(false)
  }

  const handleResetInfo = () => {
    setData(initialData)
    setIsReview(false)
    setOrders([])
  }

  const handleConfirm = async () => {
    try {
      const payload: ISubmitJPRequest = {
        jp: submitData,
      }
      const response = await submitJP(payload)

      if (response) {
        handleResetInfo()
        setSubmitData([])
        setModalOpen(false)
        dispatch(updateFilterState(EquityOrderFilterInitialState))
        dispatch(fetchEquityOrder(EquityOrderFilterInitialState))
        dispatch(updateLastUpdated(new Date().toLocaleString()))
        dispatch(resetState())
        datadogLogs.logger.info('equityOrders/submitJP', {
          action: ['submitJP', 'equityOrders'],
          payload,
          action_status: 'request',
        })
      }
    } catch (error: any) {
      datadogLogs.logger.error('equityOrders/submitJP', { action: ['submitJP', 'equityOrders'] }, error as Error)
      if (error.code === 'UNAUTHORIZED') {
        await router.push({ pathname: '/error', query: { reason: 'unauthorized' } })
      } else {
        displayAlert(error.message)
      }
    }
  }

  const handleOCChange = (key: string, orderId: string) => {
    const updatedOrders = orders.map((order: IJPDataRequest) => {
      if (order?.orderId === orderId) {
        return { ...order, oc: key }
      }

      return order
    })

    setOrders(updatedOrders)
    setSubmitData(updatedOrders)
  }

  const handleFetchJPData = useCallback(async () => {
    try {
      const response: IListJPOrderResponse = await listJP()

      if (response) {
        setJPMOrderData(response)
        datadogLogs.logger.info('equityOrders/listJP', { action: ['listJP', 'equityOrders'], action_status: 'request' })
      }
    } catch (error: any) {
      datadogLogs.logger.error('equityOrders/listJP', { action: ['listJP', 'equityOrders'] }, error as Error)
      if (error.code === 'UNAUTHORIZED') {
        await router.push({ pathname: '/error', query: { reason: 'unauthorized' } })
      } else {
        displayAlert(error.message)
      }
    }
  }, [router])

  const arrangeData = (value: Matrix<ReactSpreadsheetProps>) => {
    const reviewDataArray: IJPDataRequest[] = value
      .slice(1)
      .map((row: (ReactSpreadsheetProps | undefined)[]) => {
        if (row.length === 29 && row?.some(cell => cell?.value)) {
          //NOTE: to clarify the type of JPOrderList later
          const JPOrderList: any = Object.values(jpmOrderData)
          const foundJPOrder = JPOrderList.find((order: EquityRowType) => {
            return row[23]?.value === order.orderId || `${row[23]?.value}-1` === order.orderId.toString()
          })

          return {
            symbol: row[6]?.value.slice(0, -2) ?? '',
            oc: getOCValue(row, foundJPOrder),
            side: row[5]?.value === JPSide.SELL ? Side.SHORT : Side.LONG,
            entryPrice: Number(row[8]?.value.replace(/,/g, '')),
            orderType: row[9]?.value ?? '',
            qty: Number(row[10]?.value.replace(/,/g, '')),
            filled: Number(row[11]?.value.replace(/,/g, '')),
            avgPrice: Number(row[14]?.value.replace(/,/g, '')),
            orderId: row[23]?.value ?? '',
            status: row[3]?.value ?? '',
            added: foundJPOrder,
          }
        }

        return {
          symbol: '',
          oc: '',
          side: '',
          entryPrice: 0,
          orderType: '',
          qty: 0,
          filled: 0,
          avgPrice: 0,
          orderId: '',
          status: '',
          added: false,
        }
      })
      .filter((item: IJPDataRequest) => item)

    setSubmitData(reviewDataArray)

    return reviewDataArray
  }

  const getDefaultOCValue = (order: IJPDataRequest) => {
    if (order.added) {
      return order.oc
    } else {
      return order.side === Side.LONG ? OC.CLOSE : OC.OPEN
    }
  }

  const getOCValue = (row: ReactSpreadsheetProps[], foundJPOrder: EquityRowType | undefined) => {
    if (foundJPOrder) {
      return foundJPOrder.blockOrders.openClose
    } else if (row) {
      return row[5]?.value === JPSide.SELL ? OC.OPEN : OC.CLOSE
    }

    return ''
  }

  useEffect(() => {
    setIsReview(true)
  }, [data])

  useEffect(() => {
    if (modalOpen) {
      handleFetchJPData()
    }
  }, [modalOpen, handleFetchJPData])

  return (
    <Container>
      <Dialog
        open={modalOpen}
        disableEscapeKeyDown
        aria-labelledby={'title'}
        aria-describedby={'description'}
        fullWidth={true}
        maxWidth={'lg'}
      >
        <DialogTitle id={'title'}>Update JP Morgan Orders</DialogTitle>
        <DialogContent>
          <DialogContentText sx={{ mb: 3 }}>Update the orders from JP Morgan Terminal</DialogContentText>
          {isReview ? (
            <Spreadsheet
              data={data}
              onChange={(d: Matrix<ReactSpreadsheetProps>) => {
                if (data !== d) {
                  setOrders(arrangeData(d))
                  setIsReview(false)
                }
              }}
            />
          ) : (
            <TableContainer component={Paper}>
              <Table size='small'>
                <TableHead>
                  <TableRow>
                    <StyledTableCell align='center'>Added</StyledTableCell>
                    <StyledTableCell align='center'>O/C</StyledTableCell>
                    <StyledTableCell align='center'>Side</StyledTableCell>
                    <StyledTableCell align='center'>Symbol</StyledTableCell>
                    <StyledTableCell align='center'>Entry Price</StyledTableCell>
                    <StyledTableCell align='center'>Order Type</StyledTableCell>
                    <StyledTableCell align='center'>Qty</StyledTableCell>
                    <StyledTableCell align='center'>Filled</StyledTableCell>
                    <StyledTableCell align='center'>Avg Price</StyledTableCell>
                    <StyledTableCell align='center'>Order ID</StyledTableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {orders.map((order: IJPDataRequest) => (
                    <StyledTableRow key={order.orderId}>
                      <StyledTableCell align='center'>{order.added && <CheckIcon />}</StyledTableCell>
                      <StyledTableCell align='center'>
                        <SelectOption
                          id={'ocList'}
                          labelId={'ocList'}
                          label={''}
                          options={OCList}
                          defaultValue={getDefaultOCValue(order)}
                          onChange={key => handleOCChange(key, order.orderId)}
                        />
                      </StyledTableCell>
                      <StyledTableCell align='center'>{colorTextLS(order.side)}</StyledTableCell>
                      <StyledTableCell align='center'>{order.symbol}</StyledTableCell>
                      <StyledTableCell align='center'>{DecimalNumber(order.entryPrice, 2)}</StyledTableCell>
                      <StyledTableCell align='center'>{order.orderType}</StyledTableCell>
                      <StyledTableCell align='center'>{order.qty}</StyledTableCell>
                      <StyledTableCell align='center'>{order.filled}</StyledTableCell>
                      <StyledTableCell align='center'>{DecimalNumber(order.avgPrice, 4)}</StyledTableCell>
                      <StyledTableCell align='center'>{order.orderId}</StyledTableCell>
                    </StyledTableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          )}
        </DialogContent>
        <DialogActions className='dialog-actions-dense'>
          <Button onClick={handleClose}>Deny</Button>
          <Button disabled={isReview} onClick={handleConfirm}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
      <Button
        size='medium'
        type='submit'
        sx={{ width: '200px', fontWeight: 700, marginTop: 0 }}
        variant='contained'
        disabled={false}
        onClick={handleOpen}
      >
        Add JP Batch
      </Button>
    </Container>
  )
}

export default AddJPBatchModal
