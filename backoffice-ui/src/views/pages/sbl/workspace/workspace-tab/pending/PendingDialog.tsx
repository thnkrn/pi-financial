import { ACTION_TYPE, IAction } from '@/types/sbl/sblTypes'
import Button from '@mui/material/Button'
import CircularProgress from '@mui/material/CircularProgress'
import Dialog from '@mui/material/Dialog'
import DialogActions from '@mui/material/DialogActions'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogTitle from '@mui/material/DialogTitle'
import TextField from '@mui/material/TextField'
import { ChangeEvent, useState } from 'react'

interface Props {
  open: boolean
  handleClose: () => void
  action: IAction
  onActionClick: (action: IAction, reason?: string) => void
  isOrderAction: boolean
}

const PendingDialog = ({ open, handleClose, action, onActionClick, isOrderAction }: Props) => {
  const [reason, setReason] = useState('')

  if (action.type === ACTION_TYPE.APPROVED) {
    return (
      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>{`Approve Order No. ${action.orderNo} ?`}</DialogTitle>
        <DialogContent>
          <DialogContentText>Are you sure you want to perform the following action?</DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color='secondary'>
            Cancel
          </Button>
          <Button
            onClick={() => onActionClick(action)}
            autoFocus
            disabled={isOrderAction}
            startIcon={isOrderAction ? <CircularProgress size={20} color='inherit' /> : null}
          >
            Approve
          </Button>
        </DialogActions>
      </Dialog>
    )
  }

  return (
    <Dialog open={open} onClose={handleClose}>
      <DialogTitle>{`Reject Order No. ${action.orderNo} ?`}</DialogTitle>
      <DialogContent>
        <DialogContentText component={'div'}>
          <div>
            <span>Are you sure you want to perform the following action?</span>
            <TextField
              id='outlined-basic'
              label='Reason'
              variant='outlined'
              sx={{ width: '100%', marginTop: '10px' }}
              fullWidth
              onChange={(event: ChangeEvent<HTMLInputElement>) => {
                setReason(event.target.value)
              }}
            />
          </div>
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} color='secondary'>
          Cancel
        </Button>
        <Button
          onClick={() => onActionClick(action, reason)}
          autoFocus
          color='error'
          disabled={!reason || reason === '' || reason.trim() === '' || isOrderAction}
          startIcon={isOrderAction ? <CircularProgress size={20} color='inherit' /> : null}
        >
          Reject
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default PendingDialog
