import DatePickerWrapper from '@/@core/styles/libs/react-datepicker'
import Box from '@mui/material/Box'
import TextField from '@mui/material/TextField'
import { forwardRef, Ref, useState } from 'react'
import DatePicker from 'react-datepicker'

interface DatePickerProps {
  defaultDate: Date | null | undefined
  onChange: (value: Date) => any
}

const Datepicker = ({ defaultDate, onChange }: DatePickerProps) => {
  const [date, setDate] = useState<Date>(defaultDate ?? new Date())

  const handleChange = (value: Date) => {
    setDate(value)
    onChange(value)
  }

  const CustomInput = forwardRef(({ value, onClick }: any, ref: Ref<HTMLDivElement>) => (
    <TextField ref={ref} onClick={onClick} value={value} fullWidth sx={{ width: '100%' }} size={'small'} />
  ))

  return (
    <Box sx={{ width: '100%' }}>
      <DatePickerWrapper>
        <DatePicker
          dateFormat='yyyy-MM-dd'
          selected={date}
          shouldCloseOnSelect={true}
          onChange={handleChange}
          customInput={<CustomInput />}
        />
      </DatePickerWrapper>
    </Box>
  )
}

export default Datepicker
