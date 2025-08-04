import { Box, Select, SelectChangeEvent } from '@mui/material'
import FormControl from '@mui/material/FormControl'
import InputLabel from '@mui/material/InputLabel'
import MenuItem from '@mui/material/MenuItem'
import { useEffect, useState } from 'react'

const ITEM_HEIGHT = 48
const ITEM_PADDING_TOP = 8
const MenuProps = {
  PaperProps: {
    style: {
      width: 150,
      maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
    },
  },
}

interface SelectMultipleProps {
  id: string
  label: string
  labelId: string
  options: any[]
  onChange: (value: string) => any
  defaultValue?: string
  disabled?: boolean
}
const SelectOption = (props: SelectMultipleProps) => {
  const initialValue = props.defaultValue ?? null
  const [value, setValue] = useState<string[] | null>(initialValue ? [initialValue] : null)

  const handleChange = (event: SelectChangeEvent<string[]>) => {
    setValue(event.target.value as string[])
    props.onChange(event.target.value as string)
  }

  useEffect(() => {
    setValue(props.defaultValue ? [props.defaultValue] : null)
  }, [props.defaultValue])

  return (
    <Box>
      <FormControl fullWidth size={'small'}>
        <InputLabel id={props.labelId}>{props.label}</InputLabel>
        <Select
          label={props.label}
          value={value || ''}
          MenuProps={MenuProps}
          id={props.id}
          onChange={handleChange}
          labelId={props.labelId}
          disabled={props.disabled}
        >
          {props.options.map(option => (
            <MenuItem key={option.key} value={option.key}>
              {option.value}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </Box>
  )
}

export default SelectOption
