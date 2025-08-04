// ** React Imports
import toast from 'react-hot-toast'

// ** MUI Imports
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogActions from '@mui/material/DialogActions'
import Button from '@mui/material/Button'
import Dialog from '@mui/material/Dialog'
import { useEffect, useState } from 'react'
import { Typography } from '@mui/material'
import CircularProgress from '@mui/material/CircularProgress'
import Backdrop from '@mui/material/Backdrop'

// ** Custom Components Imports
import { getOrdersFetchingStatus, startOrdersFetching, stopOrdersFetching } from '@/lib/api/clients/blocktrade/monitor'

const FetchOrdersModal = () => {
  const [modalOpen, setModalOpen] = useState<boolean>(false)
  const [loadingOverlay, setLoadingOverlay] = useState<boolean>(false)
  const [ordersFetchingStatus, setOrdersFetchingStatus] = useState<boolean>(false)

  const handleOpen = () => {
    setModalOpen(true)
  }

  const handleClose = () => {
    setModalOpen(false)
  }

  const handleStartFetching = async () => {
    setLoadingOverlay(true)
    try {
      const response = await startOrdersFetching()

      if (response.status === 'ok') {
        toast.success('Fetching Orders Started')
      } else {
        toast.error('Something went wrong')
      }
    } catch (error) {
      toast.error('Something went wrong')
    } finally {
      setOrdersFetchingStatus(true)
      setLoadingOverlay(false)
      handleClose()
    }
  }

  const handleStopFetching = async () => {
    setLoadingOverlay(true)
    try {
      const response = await stopOrdersFetching()

      if (response.status === 'ok') {
        toast.success('Fetching Orders Stopped')
      } else {
        toast.error('Something went wrong')
      }
    } catch (error) {
      toast.error('Something went wrong')
    } finally {
      setOrdersFetchingStatus(false)
      setLoadingOverlay(false)
      handleClose()
    }
  }

  const fetchGetOrdersFetchingStatus = async () => {
    try {
      const response = await getOrdersFetchingStatus()

      if (response.status === 'running') {
        setOrdersFetchingStatus(true)
      } else {
        setOrdersFetchingStatus(false)
      }
    } catch (error) {
      return null
    }
  }

  useEffect(() => {
    fetchGetOrdersFetchingStatus()

    const interval = setInterval(fetchGetOrdersFetchingStatus, 10000)

    return () => clearInterval(interval)
  }, [])

  return (
    <>
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
        <DialogTitle id='title'>Oneport Orders Fetching</DialogTitle>
        <DialogContent>
          <DialogContentText id='description'>
            <Typography
              variant='body1'
              gutterBottom
              noWrap
              sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main.primary' }}
            >
              {ordersFetchingStatus ? 'Do you want to stop fetching orders?' : 'Do you want to start fetching orders?'}
            </Typography>
          </DialogContentText>
        </DialogContent>
        <DialogActions className='dialog-actions-dense'>
          <Button onClick={handleClose}>Deny</Button>
          <Button disabled={false} onClick={ordersFetchingStatus ? handleStopFetching : handleStartFetching}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
      <Button
        size='medium'
        type='submit'
        sx={{ width: '200px', fontWeight: 700, marginTop: 0, backgroundColor: ordersFetchingStatus ? 'orange' : '' }}
        variant='contained'
        disabled={false}
        onClick={handleOpen}
      >
        {ordersFetchingStatus ? 'Stop Fetching' : 'Start Fetching'}
      </Button>
    </>
  )
}

export default FetchOrdersModal
