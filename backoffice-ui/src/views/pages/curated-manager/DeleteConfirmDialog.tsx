import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Typography } from '@mui/material'

interface DeleteConfirmDialogProps {
  open: boolean
  onClose: () => void
  onConfirm: () => void
  itemName: string
}

const DeleteConfirmDialog = ({ open, onClose, onConfirm, itemName }: DeleteConfirmDialogProps) => {
  return (
    <Dialog open={open} onClose={onClose} maxWidth='xs' fullWidth>
      <DialogTitle>Confirm Deletion</DialogTitle>
      <DialogContent>
        <Typography>Are you sure you want to delete "{itemName}" ?</Typography>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} variant='outlined'>
          Cancel
        </Button>
        <Button onClick={onConfirm} variant='contained' color='primary'>
          Delete
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default DeleteConfirmDialog
