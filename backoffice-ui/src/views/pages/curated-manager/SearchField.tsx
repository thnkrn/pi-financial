import { TextField } from '@mui/material'
import { memo } from 'react'

interface SearchFieldProps {
  value: string
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
  disabled: boolean
  placeholder: string
}

const SearchField = memo(({ value, onChange, disabled, placeholder }: SearchFieldProps) => (
  <TextField fullWidth size='small' placeholder={placeholder} disabled={disabled} value={value} onChange={onChange} />
))

export default SearchField
