// ** React Imports
import { forwardRef, useEffect, useState } from 'react'
import Swal from 'sweetalert2'

// ** MUI Imports
import TextField from '@mui/material/TextField'

// ** Third Party Imports
import differenceInDays from 'date-fns/differenceInDays'
import format from 'date-fns/format'
import isFuture from 'date-fns/isFuture'
import DatePicker from 'react-datepicker'

// ** Types
import Box from '@mui/material/Box'
import _ from 'lodash'
import { DateType } from 'src/types/forms/reactDatepickerTypes'
import DatePickerWrapper from '../../../../@core/styles/libs/react-datepicker'

interface PickerProps {
  label?: string
  end: Date | number
  start: Date | number
  onChange: (start: any, end: any) => {}
}

const today = new Date()
const DATE_RANGE_FORMAT = 'dd/MM/yyyy'

const CustomInput = forwardRef((props: PickerProps, ref) => {
  let value = ''
  let startDate = ''
  let endDate = ''

  if (!_.isNull(props.start)) {
    startDate = format(props.start, DATE_RANGE_FORMAT)
    endDate = props.end !== null ? ` - ${format(props.end, DATE_RANGE_FORMAT)}` : ''
    value = `${startDate}${endDate ?? ''}`
  }

  return (
    <TextField
      size={'small'}
      inputRef={ref}
      label={props.label ?? ''}
      {...props}
      value={value}
      onChange={e => {
        e.preventDefault()
      }}
    />
  )
})

const PickersRange = (props: any) => {
  // ** States
  const [startDateRange, setStartDateRange] = useState<DateType>(props.start)
  const [endDateRange, setEndDateRange] = useState<DateType>(props.end)

  const maxDateRange = props.maxDateRange ?? 90

  useEffect(() => {
    if (_.isNull(props.start) && _.isNull(props.end)) {
      setStartDateRange(null)
      setEndDateRange(null)
    } else if (props.start && _.isNull(props.end)) {
      setStartDateRange(props.start !== today ? props.start : null)
      setEndDateRange(null)
    } else {
      setStartDateRange(props.start)
      setEndDateRange(props.end)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.start, props.end])

  const handleOnChangeRange = (dates: any) => {
    const [start, end] = dates
    setStartDateRange(start)
    setEndDateRange(end)
    if (start && end) {
      if (isFuture(start) || isFuture(end)) {
        Swal.fire({
          title: 'Error!',
          text: "Range can't contain any future date",
          icon: 'error',
          confirmButtonText: 'OK',
        })

        setStartDateRange(null)
        setEndDateRange(null)

        return
      }

      // NOTE: This differenceInDays not include end date in calculation, so need to reduce the cap by 1
      const diff = differenceInDays(end, start)
      if (diff > maxDateRange - 1) {
        Swal.fire({
          title: 'Error!',
          text: `Range must be less than ${maxDateRange} calendar days`,
          icon: 'error',
          confirmButtonText: 'OK',
        })

        setStartDateRange(null)
        setEndDateRange(null)

        return
      }
    }
    if ((start && end) || (start && !end) || (!start && !end)) {
      props.onChange(start, end)
    }
  }

  return (
    <Box>
      <DatePickerWrapper>
        <div className={'customDatePickerWidth'}>
          <DatePicker
            isClearable
            selectsRange
            monthsShown={2}
            endDate={endDateRange}
            selected={startDateRange}
            startDate={startDateRange}
            autoComplete={'off'}
            shouldCloseOnSelect={true}
            onChange={handleOnChangeRange}
            popperPlacement={props.popperPlacement}
            customInput={
              <CustomInput
                label={props.label}
                end={endDateRange as Date | number}
                start={startDateRange as Date | number}
                onChange={() => {
                  return props.onChange(startDateRange, endDateRange)
                }}
              />
            }
          />
        </div>
      </DatePickerWrapper>
    </Box>
  )
}

export default PickersRange
