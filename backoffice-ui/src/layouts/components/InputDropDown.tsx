import { IAction } from '@/lib/api/clients/backoffice/transactions/types'
import { MenuItem, TextField } from '@mui/material'
import { useController } from 'react-hook-form'

interface Props {
  name: string
  label?: string
  control: any
  items: IAction[]
  required?: boolean
}

const InputDropDown = ({ name, label, control, items, required }: Props) => {
  const { field, fieldState, formState } = useController({ name, control })

  return (
    <TextField
      select
      sx={{ mb: 2 }}
      label={label}
      required={required}
      fullWidth
      onChange={event => field.onChange(event.target.value)}
      value={field.value}
      error={fieldState.isTouched && !!fieldState.error}
      helperText={fieldState.isTouched && fieldState.error?.message}
      disabled={formState.isSubmitting}
    >
      {items?.map(item => (
        <MenuItem key={item?.value as string} value={item?.value as string}>
          {item?.label}
        </MenuItem>
      ))}
    </TextField>
  )
}

export default InputDropDown
