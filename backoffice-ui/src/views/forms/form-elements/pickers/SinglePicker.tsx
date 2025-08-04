import DatePickerWrapper from '@/@core/styles/libs/react-datepicker'
import Box from '@mui/material/Box'
import TextField from '@mui/material/TextField'
import { Ref, forwardRef } from 'react'
import DatePicker from 'react-datepicker'

interface PopperProps {
  strategy: string
}

interface DatePickerProps {
  date: Date | null
  onChange: any
  label?: string
  popperProps?: PopperProps
}

const CustomInput = forwardRef(({ value, onClick, label }: any, ref: Ref<HTMLDivElement>) => (
  <TextField
    label={label || ''}
    ref={ref}
    onClick={onClick}
    value={value}
    fullWidth
    sx={{ width: '100%' }}
    size={'small'}
  />
))

const SinglePicker = ({ date, onChange, label, popperProps }: DatePickerProps) => {
  return (
    <Box sx={{ width: '100%' }}>
      <DatePickerWrapper>
        <DatePicker
          dateFormat='yyyy-MM-dd'
          selected={date}
          shouldCloseOnSelect={true}
          onChange={onChange}
          popperProps={popperProps}
          customInput={<CustomInput label={label} />}
        />
      </DatePickerWrapper>
    </Box>
  )
}

export default SinglePicker
