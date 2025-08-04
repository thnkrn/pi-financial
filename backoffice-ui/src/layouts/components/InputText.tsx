import { TextField } from '@mui/material'
import { useController } from 'react-hook-form'

const InputText = ({
  name,
  label,
  placeholder,
  control,
  required,
  editable
}: React.PropsWithoutRef<{
  name: string
  label?: string
  placeholder?: string
  control: any
  editable: boolean
  required: boolean
}>) => {
  const { field, fieldState, formState } = useController({ name, control })

  return (
    <TextField
      {...field}
      placeholder={placeholder}
      label={label}
      multiline
      fullWidth
      rows={4}
      variant='outlined'
      required={required}
      error={fieldState.isTouched && !!fieldState.error}
      helperText={fieldState.isTouched && fieldState.error?.message}
      disabled={!editable || formState.isSubmitting}
    />
  )
}

export default InputText
