import SinglePicker from '@/views/forms/form-elements/pickers/SinglePicker'
import { Button, FormControl, Grid } from '@mui/material'
import InputLabel from '@mui/material/InputLabel'
import MenuItem from '@mui/material/MenuItem'
import OutlinedInput from '@mui/material/OutlinedInput'
import Select, { SelectChangeEvent } from '@mui/material/Select'
import { useEffect, useRef } from 'react'
import { Filter, ReportFilterProps } from './types'

const initialFilterState: Filter = {
  atsUploadType: null,
  requestDate: null,
}

const ReportFilters = ({ onFilter, filter, onApplyFilter }: ReportFilterProps) => {
  const uploadTypeOptions = [
    { value: 'All', label: 'All' },
    { value: 'UpdateEffectiveDate', label: 'UpdateEffectiveDate' },
    { value: 'OverrideBankInfo', label: 'OverrideBankInfo' },
  ]
  const handleDateChange = (event: Date) => {
    onFilter({
      ...filter,
      requestDate: event.toISOString(),
    })
  }

  const handleSelectChange = (event: SelectChangeEvent) => {
    const selectedValue = event.target.value

    onFilter({
      ...filter,
      atsUploadType: selectedValue === 'All' ? null : selectedValue,
    })
  }

  const handleResetFilter = () => {
    onFilter(initialFilterState)
  }

  const onFilterRef = useRef(onFilter)

  useEffect(() => {
    onFilterRef.current = onFilter
  }, [onFilter])

  useEffect(() => {
    if (filter) {
      onFilterRef.current(filter)
    }
  }, [filter])

  return (
    <>
      <Grid container spacing={6}>
        <Grid item xs={6} sm={6}>
          <FormControl sx={{ minWidth: '180px' }} fullWidth>
            <InputLabel id={'demo-simple-select-label'}>{'Type'}</InputLabel>
            <Select
              size={'small'}
              labelId={'demo-simple-select-label'}
              id={'demo-simple-select'}
              value={filter.atsUploadType ?? 'All'}
              label={'Filter type'}
              onChange={handleSelectChange}
              fullWidth
              input={<OutlinedInput label={'Name'} />}
            >
              {uploadTypeOptions.map(({ value, label }) => (
                <MenuItem key={value} value={value}>
                  {label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>
        <Grid item xs={6} sm={6}>
          <FormControl fullWidth>
            <SinglePicker
              label={'Date'}
              date={filter.requestDate ? new Date(filter.requestDate) : null}
              onChange={handleDateChange}
              popperProps={{
                strategy: 'fixed',
              }}
            />
          </FormControl>
        </Grid>
      </Grid>

      <FormControl sx={{ display: 'flex', justifyContent: 'flex-end', flexDirection: 'row', mt: 6 }}>
        <Button
          type={'reset'}
          size={'medium'}
          sx={{ width: 250, mr: 2 }}
          variant={'outlined'}
          onClick={handleResetFilter}
        >
          {'Reset Filter'}
        </Button>
        <Button
          variant={'contained'}
          size={'medium'}
          fullWidth
          type={'button'}
          sx={{ width: 250 }}
          onClick={onApplyFilter}
        >
          {'Filter Now'}
        </Button>
      </FormControl>
    </>
  )
}

export default ReportFilters
