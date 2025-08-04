import PickersRange from '@/views/forms/form-elements/pickers/PickersRange'
import SelectOption from '@/views/forms/select/SelectOption'
import { CardActions, FormHelperText } from '@mui/material'
import Button from '@mui/material/Button'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import { useTheme } from '@mui/material/styles'
import { ReactDatePickerProps } from 'react-datepicker'
import { Controller, SubmitHandler, useForm } from 'react-hook-form'
import { IFormInput } from './index'

const defaultValues = {
  type: 'Auto',
}

interface Props {
  onSubmit: SubmitHandler<IFormInput>
  dateFrom: Date | null
  dateTo: Date | null
  onDateRangeChange: (start: Date, end: Date) => void
  updateFilter: (value: any) => void
}

const ReportFilter = ({ onSubmit, dateFrom, dateTo, onDateRangeChange, updateFilter }: Props) => {
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
    <div>
      <form onSubmit={handleSubmit(onSubmit)}>
        <Grid container spacing={6}>
          <Grid item xs={12}>
            <Card>
              <CardContent>
                <Grid container spacing={2}>
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
                          onChange={e => {
                            onChange(e)
                            if (e === 'ALL') {
                              updateFilter({ reportType: undefined })
                            } else {
                              updateFilter({ reportType: e })
                            }
                          }}
                          placeholder='Select Report'
                          remote={{
                            field: 'reportType',

                            // TODO: Calling CME report type endpoint
                            url: 'reports/types?generatedType=Auto',
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
                    />
                  </Grid>
                </Grid>
              </CardContent>

              <CardActions sx={{ display: 'flex', alignItems: 'right', justifyContent: 'right' }}>
                <Button size='medium' type='submit' sx={{ width: 250 }} fullWidth variant='contained'>
                  <span>Filter Report</span>
                </Button>
              </CardActions>
            </Card>
          </Grid>
        </Grid>
      </form>
    </div>
  )
}

export default ReportFilter
