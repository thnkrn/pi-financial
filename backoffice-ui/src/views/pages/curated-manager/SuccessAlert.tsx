import { Alert, AlertTitle, Snackbar, useTheme } from '@mui/material'
import { AlertColor } from '@mui/material/Alert'

interface SuccessAlertProps {
  open: boolean
  title?: string
  message?: string
  onClose: () => void
  autoHideDuration?: number
  severity?: AlertColor
}

const SuccessAlert = ({
  open,
  title = 'Success',
  message = 'Update successful!',
  onClose,
  autoHideDuration = 5000,
  severity = 'success',
}: SuccessAlertProps) => {
  const theme = useTheme()

  return (
    <Snackbar
      open={open}
      autoHideDuration={autoHideDuration}
      onClose={onClose}
      anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
    >
      <Alert
        onClose={onClose}
        severity={severity}
        variant='outlined'
        sx={{
          width: '100%',
          backgroundColor: 'rgba(255, 255, 255, 0.95)',
          color: '#000',
          '& .MuiAlert-icon': {
            color: theme.palette.primary.main,
          },
          '& .MuiAlert-action': {
            color: theme.palette.primary.main,
          },
          '& .MuiAlert-message': {
            color: '#333',
          },
          borderColor: theme.palette.primary.main,
          boxShadow: '0 2px 10px rgba(0, 0, 0, 0.1)',
        }}
      >
        <AlertTitle>{title}</AlertTitle>
        {message}
      </Alert>
    </Snackbar>
  )
}

export default SuccessAlert
