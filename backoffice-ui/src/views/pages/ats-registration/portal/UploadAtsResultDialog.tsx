import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material'
import { Stack } from '@mui/system'

interface UploadAtsResultDialogProps {
  open: boolean
  handleDialogClose: () => void
  handleDoAction: () => void
  isSuccess: boolean
  dialogContent: string
}

const UploadAtsResultDialog = ({
  open,
  handleDialogClose,
  isSuccess,
  handleDoAction,
  dialogContent,
}: UploadAtsResultDialogProps) => {
  return (
    <Dialog
      open={open}
      onClose={handleDialogClose}
      fullWidth
      maxWidth='md'
      aria-labelledby='upload-ats-dialog-title'
      aria-describedby='upload-ats-dialog-description'
    >
      <DialogTitle id='upload-ats-dialog-title'>
        <Stack direction={'row'} justifyContent={'space-between'} alignItems={'center'}>
          {`Confirm Action`}
        </Stack>
      </DialogTitle>
      <DialogContent>
        <DialogContentText id='password-dialog-description' sx={{ marginBottom: '15px' }}>
          {dialogContent}
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button variant='contained' onClick={handleDoAction} autoFocus fullWidth>
          {isSuccess ? 'See Result' : 'OK'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default UploadAtsResultDialog
