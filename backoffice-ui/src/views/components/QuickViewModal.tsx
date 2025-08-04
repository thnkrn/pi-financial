import * as React from 'react'
import Box from '@mui/material/Box'
import Dialog, { DialogProps } from '@mui/material/Dialog'
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import CloseIcon from '@mui/icons-material/Close'
import IconButton from '@mui/material/IconButton'

export interface DialogTitleProps {
  id: string
  children?: React.ReactNode
  onClose: () => void
}

export interface ModalProps {
  children: React.ReactNode
  onClose: () => void
  title: string
  open: boolean
  maxWidth: DialogProps['maxWidth']
}

export default function QuickViewModal({ children, open, onClose, title, maxWidth }: ModalProps) {
  const handleClose = () => {
    onClose()
  }

  return (
    <div>
      <Dialog maxWidth={maxWidth} fullWidth onClose={handleClose} aria-labelledby='customized-dialog-title' open={open}>
        <DialogTitle id='customized-dialog-title' data-testid={'quick-view-dialog-title'}>
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
            {title}
            <IconButton aria-label='close' onClick={handleClose}>
              <CloseIcon />
            </IconButton>
          </Box>
        </DialogTitle>
        <DialogContent dividers>{children}</DialogContent>
      </Dialog>
    </div>
  )
}
