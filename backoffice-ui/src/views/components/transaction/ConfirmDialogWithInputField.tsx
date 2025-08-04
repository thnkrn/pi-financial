import { TextField, Typography } from '@mui/material'

interface ConfirmDialogProps {
  additionalInput?: string
  onInputChange?: (value: string) => void
}

const ConfirmDialogText = ({ additionalInput, onInputChange }: ConfirmDialogProps) => {
  return (
    <>
      <Typography mb={2} variant='h5' textAlign='left'>
        Confirm Action
      </Typography>
      <Typography py={1} variant='h6' textAlign='left'>
        Are you sure you want to perform the following action?
      </Typography>

      <TextField
        fullWidth
        label='Additional Information'
        variant='outlined'
        value={additionalInput}
        onChange={e => onInputChange && onInputChange(e.target.value)}
        sx={{ mt: 2 }}
      />
    </>
  )
}

export default ConfirmDialogText
