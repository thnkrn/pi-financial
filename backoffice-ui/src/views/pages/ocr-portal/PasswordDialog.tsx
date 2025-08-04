import { Close as CloseIcon } from '@mui/icons-material'
import useTranslation from 'next-translate/useTranslation'

import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  IconButton,
  TextField,
} from '@mui/material'
import { Stack } from '@mui/system'

interface PasswordDialogProps {
  openPasswordDialog: boolean
  handlePasswordDialogClose: () => void
  setOpenPasswordDialog: (arg1: boolean) => void
  setDocumentPassword: (arg1: string) => void
  documentPassword: string
}

const PasswordDialog = ({
  openPasswordDialog,
  handlePasswordDialogClose,
  setOpenPasswordDialog,
  setDocumentPassword,
  documentPassword,
}: PasswordDialogProps) => {
  const { t } = useTranslation('ocr_portal')
  const { t: commonTranslation } = useTranslation('common')

  return (
    <Dialog
      open={openPasswordDialog}
      fullWidth
      maxWidth='md'
      onClose={handlePasswordDialogClose}
      aria-labelledby='password-dialog-title'
      aria-describedby='password-dialog-description'
    >
      <DialogTitle id='password-dialog-title'>
        <Stack direction={'row'} justifyContent='space-between' alignItems={'center'}>
          {commonTranslation('PASSWORD', {}, { default: 'Password' })}
          <IconButton
            aria-label='close'
            size='small'
            onClick={() => {
              setOpenPasswordDialog(false)
              setDocumentPassword('')
            }}
          >
            <CloseIcon />
          </IconButton>
        </Stack>
      </DialogTitle>
      <DialogContent>
        <DialogContentText id='password-dialog-description' sx={{ marginBottom: '15px' }}>
          {t(
            'PASSWORD_PROTECTED_FILE',
            {},
            { default: 'The file is protected by password. Please enter a Open Document Password.' }
          )}
        </DialogContentText>

        <TextField
          id='input-document-password'
          label={commonTranslation('ENTER_PASSWORD', {}, { default: 'Enter password' })}
          value={documentPassword}
          type='password'
          fullWidth
          size='small'
          onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
            setDocumentPassword(event.target.value)
          }}
        />
      </DialogContent>
      <DialogActions>
        <Button
          variant='outlined'
          onClick={() => {
            setOpenPasswordDialog(false)
            setDocumentPassword('')
          }}
        >
          {commonTranslation('CANCEL', {}, { default: 'Cancel' })}
        </Button>
        <Button variant='contained' onClick={() => handlePasswordDialogClose()} autoFocus>
          {commonTranslation('APPLY', {}, { default: 'Apply' })}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default PasswordDialog
