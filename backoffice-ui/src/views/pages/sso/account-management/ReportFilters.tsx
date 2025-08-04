import { Button, FormControl, Grid, Input } from '@mui/material'
import {} from '@mui/material/Input'
import InputLabel from '@mui/material/InputLabel'
import { right } from '@popperjs/core'
import { useEffect, useRef } from 'react'
import { Filter, ReportFilterProps } from './types'

const initialFilterState: Filter = {
  username: null,
}

const ReportFilters = ({ onFilter, filter, onApplyFilter, fetchAccountData }: ReportFilterProps) => {
  const handleTextChange = (event: any) => {
    const selectedValue = event.target.value

    onFilter({
      ...filter,
      username: selectedValue,
    })
  }

  const handleResetFilter = () => {
    onFilter(initialFilterState)
    fetchAccountData()
  }

  const onFilterRef = useRef(onFilter)

  useEffect(() => {
    if (filter) {
      onFilterRef.current(filter)
    }
  }, [filter])

  return (
    <>
      <Grid container spacing={6}>
        <Grid item xs={12} sm={12}>
          <FormControl sx={{ minWidth: '180px', textAlign: right, alignContent: right }} fullWidth>
            <InputLabel id={'username-label'}>{'Username'}</InputLabel>
            <Input
              onChange={handleTextChange}
              size={'small'}
              id={'username-label'}
              value={filter.username ?? ''}
              fullWidth
            ></Input>
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
          {'Reset'}
        </Button>
        <Button
          variant={'contained'}
          size={'medium'}
          fullWidth
          type={'button'}
          sx={{ width: 250 }}
          onClick={onApplyFilter}
        >
          {'search'}
        </Button>
      </FormControl>
    </>
  )
}

export default ReportFilters
