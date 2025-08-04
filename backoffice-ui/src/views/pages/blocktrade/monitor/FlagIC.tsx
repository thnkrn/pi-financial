// ** React Imports
import { SyntheticEvent, useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { useDispatch } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'

// ** MUI Imports
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import Dialog from '@mui/material/Dialog'
import Typography from '@mui/material/Typography'
import CircularProgress from '@mui/material/CircularProgress'
import Backdrop from '@mui/material/Backdrop'
import Grid from '@mui/material/Grid'
import Box from '@mui/material/Box'
import Autocomplete from '@mui/material/Autocomplete'
import TextField from '@mui/material/TextField'
import IconButton from '@mui/material/IconButton'
import ChatIcon from '@mui/icons-material/Chat'

// ** Custom Components Imports
import { resetState } from '@/store/apps/blocktrade/monitor'
import { fetchEquityOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/equity'
import { EquityOrderFilterInitialState } from '@/constants/blocktrade/InitialState'
import { DecimalNumber } from '@/utils/blocktrade/decimal'
import { OrderStatus, OrderType } from '@/constants/blocktrade/GlobalEnums'
import { IEquityOrder } from '@/types/blocktrade/orders/types'
import { datadogLogs } from '@datadog/browser-logs'
import { IFetchAllUserResponse, IFetchUserResponse, IFlagICRequest } from '@/lib/api/clients/blocktrade/monitor/types'
import { flagIC, getAllUser } from '@/lib/api/clients/blocktrade/monitor'

interface Props {
  row: IEquityOrder
}

type IUser = {
  label: string
  value: number
  employeeId: number
} | null

type IUserList = IUser[]

const FlagIC = ({ row }: Props) => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const [modalOpen, setModalOpen] = useState<boolean>(false)
  const [userList, setUserList] = useState<IUserList>([])
  const [userSelected, setUserSelected] = useState<IUser>(null)
  const [loadingOverlay, setLoadingOverlay] = useState<boolean>(false)

  const handleOpen = () => {
    const initialUserSelected: IUser = {
      value: row?.blockOrders?.sales?.id ?? 0,
      label: row?.blockOrders?.sales?.name ?? '',
      employeeId: row?.blockOrders?.sales?.employeeId ?? 0,
    }

    setUserSelected(initialUserSelected)
    setModalOpen(true)
  }

  const handleClose = () => {
    setModalOpen(false)
  }

  const handleConfirm = async () => {
    if (userSelected) {
      setLoadingOverlay(true)
      const payload: IFlagICRequest = {
        blockId: row.blockId,
        userId: userSelected.value,
      }
      try {
        const response = await flagIC(payload)
        if (response) {
          handleClose()
          dispatch(resetState())
          dispatch(updateFilterState(EquityOrderFilterInitialState))
          dispatch(fetchEquityOrder(EquityOrderFilterInitialState))
          dispatch(updateLastUpdated(new Date().toLocaleString()))

          datadogLogs.logger.info('blocktrade/monitor/flagIC/submit', {
            action: 'blocktrade/monitor/flagIC/submit',
            payload: payload,
            action_status: 'request',
          })

          const message = 'Flagged IC Successfully'
          toast.success(message)
        }
      } catch (error: any) {
        datadogLogs.logger.error(
          'blocktrade/monitor/flagIC/submit',
          { action: 'blocktrade/monitor/flagIC/submit' },
          Error(error?.message)
        )

        const message = 'Something went wrong'
        toast.error(message)

        return null
      } finally {
        setLoadingOverlay(false)
      }
    }
  }

  const fetchAllUser = async () => {
    try {
      const response: IFetchAllUserResponse = await getAllUser()

      if (response) {
        const usersArray: IFetchUserResponse[] = Object.values(response)
        const arrangedData: IUserList = usersArray.map((item: IFetchUserResponse) => ({
          label: item.name,
          value: item.id,
          employeeId: item.employeeId,
        }))
        setUserList(arrangedData)

        datadogLogs.logger.info('blocktrade/monitor/flagIC/fetchAllUser', {
          action: 'blocktrade/monitor/flagIC/fetchAllUser',
          action_status: 'request',
        })
      }
    } catch (error: any) {
      datadogLogs.logger.error(
        'blocktrade/monitor/flagIC/fetchAllUser',
        { action: 'blocktrade/monitor/flagIC/fetchAllUser' },
        Error(error?.message)
      )
    }
  }

  useEffect(() => {
    if (modalOpen) {
      fetchAllUser()
    } else {
      setUserList([])
    }
  }, [modalOpen])

  return (
    <>
      <Dialog open={modalOpen} disableEscapeKeyDown fullWidth={true}>
        <DialogTitle id='title'>Flag IC</DialogTitle>
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
                  {row.blockOrders.symbol.symbol + (row.blockOrders.series?.series || '')}
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
                  {row.blockOrders.clientSide} {row.blockOrders.openClose}
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
                  {row.orderId}
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
                    Share Amount
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
                  {DecimalNumber(row.numOfShare, 0)}
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
                    Order Price
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
                  {row.orderType === OrderType.LIMIT ? DecimalNumber(row.orderPrice, 2) : row.orderType}
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
                    IC
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={8} sx={{ textAlign: 'left' }}>
                <Autocomplete
                  id='user-selected'
                  options={userList}
                  getOptionLabel={option => `${option?.employeeId ?? ''} : ${option?.label ?? ''}`}
                  value={userSelected}
                  size='small'
                  renderInput={params => (
                    <TextField
                      {...params}
                      label=''
                      InputProps={{ ...params.InputProps, style: { paddingLeft: 8, paddingRight: 8 } }}
                    />
                  )}
                  renderOption={(props, option) => (
                    <li {...props}>{`${option?.employeeId ?? ''} : ${option?.label ?? ''}`}</li>
                  )}
                  onChange={(event: SyntheticEvent<Element, Event>, newValue: any) => {
                    setUserSelected(newValue)
                  }}
                />
              </Grid>
            </Grid>
          </DialogContentText>
        </DialogContent>
        <DialogActions className='dialog-actions-dense'>
          <Button onClick={handleClose}>Deny</Button>
          <Button onClick={handleConfirm} disabled={!userSelected || userSelected.value === row.blockOrders.sales.id}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
      <Backdrop sx={{ zIndex: theme => Math.max.apply(Math, Object.values(theme.zIndex)) + 1 }} open={loadingOverlay}>
        <CircularProgress color='inherit' />
      </Backdrop>
      {row.status !== OrderStatus.F_SENT && row.status !== OrderStatus.F_MATCHED && !row.mainId && (
        <IconButton onClick={handleOpen}>
          <ChatIcon />
        </IconButton>
      )}
    </>
  )
}

export default FlagIC
