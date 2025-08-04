import { yupResolver } from '@hookform/resolvers/yup'
import LoopIcon from '@mui/icons-material/Loop'
import { Button, FormControl, FormHelperText, TextField, Typography } from '@mui/material'
import Box from '@mui/material/Box'
import Checkbox from '@mui/material/Checkbox'
import Divider from '@mui/material/Divider'
import FormControlLabel from '@mui/material/FormControlLabel'
import InputLabel from '@mui/material/InputLabel'
import MenuItem from '@mui/material/MenuItem'
import OutlinedInput from '@mui/material/OutlinedInput'
import Select, { SelectChangeEvent } from '@mui/material/Select'
import Stack from '@mui/material/Stack'
import dayjs from 'dayjs'
import useTranslation from 'next-translate/useTranslation'
import * as React from 'react'
import { useEffect, useRef, useState } from 'react'
import { Controller, useForm } from 'react-hook-form'
import * as yup from 'yup'
import { ApplicationFilterProps, Filter, KeyValue } from './types'

const defaultValues = {
  custCode: '',
}

interface FilterValueType {
  by: string
  value: string
}

const initialFilterState: Filter = {
  status: '',
  bpmReceived: true,
  custCode: '',
  userId: '',
  errorCheck: false,
}

const filterButtons: string[] = ['all', 'created', 'pending', 'completed', 'failed', 'cancelled']

const ApplicationFilters = ({ onFilter, onSync }: ApplicationFilterProps) => {
  const [activeButton, setActiveButton] = useState('all')
  const [filter, setFilter] = useState<Filter>(initialFilterState)
  const [filterValue, setFilterValue] = useState<FilterValueType>({
    by: 'custCode',
    value: '',
  })
  const [timer, setTimer] = useState<string>('')
  const { t } = useTranslation('application_summary')

  const schema = yup.object().shape({
    custCode: yup.string(),
  })

  // ** Hook
  const {
    control,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm({
    defaultValues,
    mode: 'onChange',
    resolver: yupResolver(schema),
  })

  const statusTranslations: { [key: string]: string } = {
    all: t('APPLICATION_SUMMARY_FILTER_ALL', {}, { default: 'all' }),
    created: t('APPLICATION_SUMMARY_FILTER_CREATED', {}, { default: 'Created' }),
    pending: t('APPLICATION_SUMMARY_FILTER_PENDING', {}, { default: 'Pending' }),
    completed: t('APPLICATION_SUMMARY_FILTER_COMPLETED', {}, { default: 'Completed' }),
    failed: t('APPLICATION_SUMMARY_FILTER_FAILED', {}, { default: 'Failed' }),
    cancelled: t('APPLICATION_SUMMARY_FILTER_CANCELLED', {}, { default: 'Cancelled' }),
  }

  const handleFilter = ({ key, value }: KeyValue) => {
    setFilter((prev: Filter) => ({
      ...prev,
      [key]: value,
    }))
  }

  const renderFilterButtons = () => {
    return filterButtons.map((value: string) => {
      const variant = activeButton === value ? 'contained' : 'outlined'

      return (
        <Button
          key={value}
          variant={variant}
          onClick={() => {
            setActiveButton(value)
            handleFilter({
              key: 'status',
              value: value === 'all' ? '' : value,
            })
          }}
          data-testid={`application-summary-filter-${value}`}
        >
          {statusTranslations[value] || value}
        </Button>
      )
    })
  }

  const handleFilterSubmit = () => {
    setFilter((prev: Filter) => ({
      ...prev,
      userId: filterValue.by === 'custCode' ? '' : filterValue.value,
      custCode: filterValue.by === 'custCode' ? filterValue.value : '',
    }))
  }

  const handleFilterChange = (event: { target: { value: string } }) => {
    const inputValue = event.target.value
    if (inputValue?.length === 0) {
      setFilter((prev: Filter) => ({
        ...prev,
        userId: '',
        custCode: '',
      }))
    }
    setFilterValue((prev: FilterValueType) => ({
      ...prev,
      value: inputValue,
    }))

    setValue('custCode', inputValue, { shouldValidate: true })
  }

  const handleSelectChange = (event: SelectChangeEvent) => {
    const selectedValue = event.target.value

    setFilterValue((prev: FilterValueType) => ({
      ...prev,
      by: selectedValue,
    }))
  }

  const handleErrorFilter = (event: React.ChangeEvent<HTMLInputElement>) => {
    handleFilter({
      key: 'errorCheck',
      value: event.target.checked,
    })
  }

  const renderSyncTimeStamp = () => {
    const currentTime = dayjs().format('YYYY-MM-DD HH:mm:ss')
    setTimer(currentTime)
    onSync(filter)
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
      <Box sx={{ marginBottom: '20px', width: '100%' }}>
        <Stack sx={{ gap: '10px' }} direction='row' flexWrap='wrap'>
          <Stack sx={{ gap: '10px' }} direction='row' flexWrap='wrap'>
            {renderFilterButtons()}
          </Stack>
          <Divider orientation='vertical' flexItem sx={{ display: { xs: 'none', md: 'block' } }} />
          <FormControlLabel
            control={<Checkbox onChange={handleErrorFilter} checked={filter.errorCheck} />}
            label={t('APPLICATION_SUMMARY_FILTER_SHOW_ERRORS_ONLY', {}, { default: 'Show errors only' })}
          />

          <Box sx={{ marginLeft: 'auto !important' }}>
            {timer && (
              <Typography component='span' sx={{ fontWeight: 'bold' }}>
                {t('APPLICATION_SUMMARY_FILTER_LAST_SYNCED', {}, { default: 'Last synced' })}:{' '}
              </Typography>
            )}
            {timer}
            <Button
              sx={{ marginLeft: '10px' }}
              variant='contained'
              startIcon={<LoopIcon />}
              onClick={renderSyncTimeStamp}
              data-testid={'application-summary-sync'}
            >
              {t('APPLICATION_SUMMARY_FILTER_SYNC', {}, { default: 'Sync' })}
            </Button>
          </Box>
        </Stack>
      </Box>

      <Box sx={{ marginBottom: '20px' }}>
        <form onSubmit={handleSubmit(handleFilterSubmit)}>
          <Stack
            spacing={{ xs: 2, sm: 4 }}
            direction={{ xs: 'column', sm: 'row' }}
            flexWrap={{ xs: 'wrap', sm: 'nowrap' }}
          >
            <FormControl sx={{ minWidth: '180px' }}>
              <InputLabel id='demo-simple-select-label'>By</InputLabel>
              <Select
                size='small'
                defaultValue={filterValue.by}
                labelId='demo-simple-select-label'
                id='demo-simple-select'
                value={filterValue.by}
                label='Filter by'
                onChange={handleSelectChange}
                input={<OutlinedInput label='Name' />}
              >
                <MenuItem value={'custCode'}>
                  {t('APPLICATION_SUMMARY_FILTER_CUSTOMER_CODE', {}, { default: 'Customer code' })}
                </MenuItem>
                <MenuItem value={'userId'}>
                  {t('APPLICATION_SUMMARY_FILTER_USER_ID', {}, { default: 'User ID' })}
                </MenuItem>
              </Select>
            </FormControl>

            <FormControl sx={{ flexGrow: 1 }}>
              <Controller
                name='custCode'
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    size={'small'}
                    label={t('APPLICATION_SUMMARY_FILTER_SEARCH', {}, { default: 'Search' }) + '...'}
                    variant='outlined'
                    value={filterValue.value}
                    onChange={handleFilterChange}
                    style={{ flexGrow: 1 }}
                    error={Boolean(errors.custCode)}
                    data-testid={'application-summary-text-field-search'}
                  />
                )}
              />
              {errors.custCode && (
                <FormHelperText sx={{ color: 'error.main' }}>{errors.custCode.message}</FormHelperText>
              )}
            </FormControl>

            <Button
              type='submit'
              variant='contained'
              sx={{ alignSelf: 'flex-start' }}
              data-testid={'application-summary-search'}
            >
              {t('APPLICATION_SUMMARY_FILTER_SEARCH_BUTTON', {}, { default: 'Search' })}
            </Button>
          </Stack>
        </form>
      </Box>
    </>
  )
}

export default ApplicationFilters
