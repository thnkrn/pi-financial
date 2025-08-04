// ** React Imports
import { ElementType } from 'react'

// ** MUI Imports
import Box from '@mui/material/Box'
import FormControl from '@mui/material/FormControl'
import InputLabel from '@mui/material/InputLabel'
import MenuItem from '@mui/material/MenuItem'
import Select, { SelectChangeEvent } from '@mui/material/Select'
import { SxProps } from '@mui/system'

// Other Imports
import { IChoice } from './types'

const DropdownPDFDownload = ({
  choices,
  value,
  setValue,
  label,
  IconComponent,
  sx,
}: {
  choices: IChoice[]
  value: string
  setValue(value: string): void
  label?: string
  IconComponent?: ElementType
  sx?: SxProps
}) => {
  const handleChange = (event: SelectChangeEvent) => {
    setValue(event.target.value)
  }

  return (
    <Box sx={{ minWidth: 120, ...sx }}>
      <FormControl fullWidth>
        <InputLabel id={`${label}-label`}>{label}</InputLabel>
        <Select
          labelId={`${label}-label`}
          id={label}
          value={value}
          label={label}
          onChange={handleChange}
          IconComponent={IconComponent}
        >
          <MenuItem value={''} />
          {choices.map((choice: IChoice, i: number) => {
            const key = `${choice.name}-${i}`

            return (
              <MenuItem key={key} value={choice.value ?? choice.name}>
                {choice.name}
              </MenuItem>
            )
          })}
        </Select>
      </FormControl>
    </Box>
  )
}

export default DropdownPDFDownload
