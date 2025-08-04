import PickersRange from '@/views/forms/form-elements/pickers/PickersRange'
import SelectOption from '@/views/forms/select/SelectOption'
import LoadingButton from '@mui/lab/LoadingButton'
import { CardActions, FormHelperText } from '@mui/material'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import { useTheme } from '@mui/material/styles'
import { ReactDatePickerProps } from 'react-datepicker'
import { Controller, SubmitHandler, useForm } from 'react-hook-form'
import { IFormInput } from './index'

const defaultValues = {
  type: '',
}

interface Props {
  onSubmit: SubmitHandler<IFormInput>
  isDownloading: boolean
  dateFrom: Date | null
  dateTo: Date | null
  onDateRangeChange: (start: Date, end: Date) => void
}

const ReportGeneration = ({ onSubmit, isDownloading, dateFrom, dateTo, onDateRangeChange }: Props) => {
  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({
    defaultValues,
    mode: 'onChange',
  })

  const theme = useTheme()
  const { direction } = theme
  const popperPlacement: ReactDatePickerProps['popperPlacement'] = direction === 'ltr' ? 'bottom-start' : 'bottom-end'

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Grid container spacing={3}>
                <Grid item xs={12} sm={6}>
                  <Controller
                    name='type'
                    control={control}
                    rules={{ required: 'Please select a report type' }}
                    render={({ field: { onChange } }) => (
                      <SelectOption
                        id={'type'}
                        label={'Report Type'}
                        labelId={'reportType'}
                        disabledAll
                        onChange={e => {
                          onChange(e)
                        }}
                        placeholder='Select Report'
                        remote={{
                          field: 'reportType',
                          url: 'reports/types?generatedType=DepositWithdraw',
                          key: 'alias',
                          value: 'name',
                        }}
                      />
                    )}
                  />
                  {errors.type && (
                    <FormHelperText sx={{ color: 'error.main' }} id='validation-schema-transaction-id'>
                      {errors.type.message}
                    </FormHelperText>
                  )}
                </Grid>
                <Grid item xs={12} sm={6}>
                  <PickersRange
                    onChange={onDateRangeChange}
                    popperPlacement={popperPlacement}
                    start={dateFrom}
                    end={dateTo}
                    label={'Date'}
                    maxDateRange={90}
                  />
                </Grid>
              </Grid>
            </CardContent>

            <CardActions sx={{ display: 'flex', alignItems: 'right', justifyContent: 'right' }}>
              <LoadingButton
                size='medium'
                loading={isDownloading}
                type='submit'
                sx={{ width: 250 }}
                fullWidth
                variant='contained'
              >
                <span>Download Report</span>
              </LoadingButton>
            </CardActions>
          </Card>
        </Grid>
      </Grid>
    </form>
  )
}

export default ReportGeneration
