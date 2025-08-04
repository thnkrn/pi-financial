import TRANSACTION_STATUSES from '@/constants/TransactionStatus'
import { IState, resetState } from '@/store/apps/transfer-cash'
import { showErrors } from '@/utils/fmt'
import ErrorModal from '@/views/components/ErrorModal'
import PickersRange from '@/views/forms/form-elements/pickers/PickersRange'
import SelectOption from '@/views/forms/select/SelectOption'
import { yupResolver } from '@hookform/resolvers/yup'
import { Icon } from '@iconify/react'
import { CardActions, FormHelperText, ListItemText } from '@mui/material'
import Button from '@mui/material/Button'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import FormControl from '@mui/material/FormControl'
import Grid from '@mui/material/Grid'
import List from '@mui/material/List'
import ListItem from '@mui/material/ListItem'
import ListItemIcon from '@mui/material/ListItemIcon'
import MUITextField, { TextFieldProps } from '@mui/material/TextField'
import Typography from '@mui/material/Typography'
import { styled, useTheme } from '@mui/material/styles'
import dayjs from 'dayjs'
import isEmpty from 'lodash/isEmpty'
import isEqual from 'lodash/isEqual'
import map from 'lodash/map'
import { useEffect } from 'react'
import { ReactDatePickerProps } from 'react-datepicker'
import { Controller, useForm } from 'react-hook-form'
import Swal from 'sweetalert2'
import withReactContent from 'sweetalert2-react-content'
import * as yup from 'yup'
import {
  IGetTransferCashTransactionsRequest,
  ITransferCashFilters,
} from '@/lib/api/clients/backoffice/transactions/types'
import { DATE_FORMAT, LAST_7_DAYS, TODAY } from '@/views/pages/transfer-cash/constants'

const TextField = styled(MUITextField)<TextFieldProps>(() => ({
  '& .Mui-disabled': {
    cursor: 'not-allowed',
  },
}))

const defaultValues = {
  senderAccountNo: '',
  customerCode: '',
  accountCode: '',
  transactionNumber: '',
}

const schema = yup.object().shape({
  senderAccountNo: yup.string().max(19, obj => showErrors('Sender Account Number', obj.value.length, obj.max)),
  customerCode: yup.string().max(10, obj => showErrors('Customer Code', obj.value.length, obj.max)),
  accountCode: yup.string().max(10, obj => showErrors('Customer Account Number', obj.value.length, obj.max)),
  transactionNumber: yup.string().max(20, obj => showErrors('Transaction No', obj.value.length, obj.max)),
})

interface Props {
  initialState: IGetTransferCashTransactionsRequest
  startCreatedDate: Date | null
  endCreatedDate: Date | null
  startOtpConfirmedDate: Date | null
  endOtpConfirmedDate: Date | null
  store: IState
  setFilter: (value: IGetTransferCashTransactionsRequest) => void
  updateFilter: (value: ITransferCashFilters) => void
  onResetButtonClicked: () => void
  setStartCreatedDate: (value: Date | null) => void
  setEndCreatedDate: (value: Date | null) => void
  setStartOtpConfirmedDate: (value: Date | null) => void
  setEndOtpConfirmedDate: (value: Date | null) => void
  onSubmit: () => void
  error: string
  dropdownError: string
}

const TransferCashFilter = ({
  initialState,
  startCreatedDate,
  endCreatedDate,
  startOtpConfirmedDate,
  endOtpConfirmedDate,
  store,
  error,
  dropdownError,
  setFilter,
  updateFilter,
  onResetButtonClicked,
  setStartCreatedDate,
  setEndCreatedDate,
  setStartOtpConfirmedDate,
  setEndOtpConfirmedDate,
  onSubmit,
}: Props) => {
  const theme = useTheme()
  const { direction } = theme
  const popperPlacement: ReactDatePickerProps['popperPlacement'] = direction === 'ltr' ? 'bottom-start' : 'bottom-end'

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    defaultValues,
    mode: 'onChange',
    resolver: yupResolver(schema),
  })

  const ReactSwal = withReactContent(Swal)

  useEffect(() => {
    setFilter(initialState)
    if (isEqual(store.filter, initialState)) {
      setStartCreatedDate(LAST_7_DAYS.toDate())
      setEndCreatedDate(TODAY)
      reset()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store, resetState])

  const onOtpConfirmedDateRangeChange = (start: Date, end: Date) => {
    updateFilter({
      otpConfirmedDateFrom: start ? dayjs(start).format(DATE_FORMAT) : null,
      otpConfirmedDateTo: end ? dayjs(end).format(DATE_FORMAT) : null,
    })

    setStartOtpConfirmedDate(start ? dayjs(start).toDate() : null)
    setEndOtpConfirmedDate(end ? dayjs(end).toDate() : null)
  }

  const onCreatedDateRangeChange = (start: Date, end: Date) => {
    updateFilter({
      createdAtFrom: start ? dayjs(start).format(DATE_FORMAT) : null,
      createdAtTo: end ? dayjs(end).format(DATE_FORMAT) : null,
    })

    setStartCreatedDate(start ? dayjs(start).toDate() : null)
    setEndCreatedDate(end ? dayjs(end).toDate() : null)
  }

  const onSubmitButtonClicked = () => {
    if (!isEmpty(errors)) {
      ReactSwal.fire({
        icon: 'error',
        title: 'Fields validation errors',
        html: (
          <div>
            <List>
              {map(errors, e => (
                <ListItem disablePadding>
                  <ListItemIcon>
                    <Icon icon='mdi:alert-circle-outline' fontSize={20} color={'#FF4D49'} />
                  </ListItemIcon>
                  <ListItemText sx={{ color: '#FF4D49' }}>
                    <span className={'swal-list-text'}>{e?.message}</span>
                  </ListItemText>
                </ListItem>
              ))}
            </List>
          </div>
        ),
        confirmButtonColor: '#21CE99',
      })
    }
  }

  return (
    <div>
      <form onSubmit={handleSubmit(onSubmit)}>
        <Grid container spacing={6}>
          <Grid item xs={12}>
            <Card>
              <CardContent>
                <Grid container spacing={6}>
                  <Grid item xs={6}>
                    <Typography variant='body2' sx={{ fontWeight: 600 }} color='textPrimary'>
                      Transfer Info
                    </Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant='body2' sx={{ fontWeight: 600 }} color='textPrimary'>
                      Payment Info
                    </Typography>
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <FormControl fullWidth>
                      <Controller
                        name='transactionNumber'
                        control={control}
                        render={({ field: { value, onChange } }) => (
                          <TextField
                            size={'small'}
                            fullWidth
                            value={value}
                            label='Transaction No'
                            placeholder='Transaction No'
                            aria-describedby='validation-schema-transaction-id'
                            error={Boolean(errors.transactionNumber)}
                            onChange={e => {
                              onChange(e)
                              updateFilter({ transactionNo: e.target.value })
                            }}
                          />
                        )}
                      />
                      {errors.transactionNumber && (
                        <FormHelperText sx={{ color: 'error.main' }} id='validation-schema-transaction-id'>
                          {errors.transactionNumber.message}
                        </FormHelperText>
                      )}
                    </FormControl>
                  </Grid>
                  <Grid item xs={6} sm={3}>
                    <FormControl fullWidth>
                      <Controller
                        name='customerCode'
                        control={control}
                        render={({ field: { value, onChange } }) => (
                          <TextField
                            size={'small'}
                            fullWidth
                            value={value}
                            label='Transfer From Account Code'
                            placeholder='Transfer From Account Code'
                            aria-describedby='validation-schema-customer-code'
                            error={Boolean(errors.customerCode)}
                            onChange={e => {
                              onChange(e)
                              updateFilter({ transferFromAccountCode: e.target.value })
                            }}
                          />
                        )}
                      />
                      {errors.customerCode && (
                        <FormHelperText sx={{ color: 'error.main' }} id='validation-schema-customer-code'>
                          {errors.customerCode.message}
                        </FormHelperText>
                      )}
                    </FormControl>
                  </Grid>
                  <Grid item xs={6} sm={3}>
                    <FormControl fullWidth>
                      <Controller
                        name='accountCode'
                        control={control}
                        render={({ field: { value, onChange } }) => (
                          <TextField
                            size={'small'}
                            fullWidth
                            value={value}
                            label='Transfer To Account Code'
                            aria-describedby='validation-schema-customer-account-number'
                            placeholder='Transfer To Account Code'
                            error={Boolean(errors.accountCode)}
                            onChange={e => {
                              onChange(e)
                              updateFilter({ transferToAccountCode: e.target.value })
                            }}
                          />
                        )}
                      />
                      {errors.accountCode && (
                        <FormHelperText sx={{ color: 'error.main' }} id='validation-schema-customer-account-number'>
                          {errors.accountCode.message}
                        </FormHelperText>
                      )}
                    </FormControl>
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <PickersRange
                      onChange={onCreatedDateRangeChange}
                      popperPlacement={popperPlacement}
                      start={startCreatedDate}
                      end={endCreatedDate}
                      label={'Created Date'}
                    />
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <PickersRange
                      onChange={onOtpConfirmedDateRangeChange}
                      popperPlacement={popperPlacement}
                      start={startOtpConfirmedDate}
                      end={endOtpConfirmedDate}
                      label={'Otp Confirmed'}
                    />
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <SelectOption
                      id={'transferFromExchangeMarket'}
                      labelId={'transferFromExchangeMarket'}
                      label={'Transfer From Product'}
                      defaultValue={store.filter.filters?.transferFromExchangeMarket}
                      remote={{
                        field: 'transferFromExchangeMarket',
                        url: 'account_types',
                        key: 'alias',
                        value: 'name',
                      }}
                      onChange={value => {
                        updateFilter({ transferFromExchangeMarket: value })
                      }}
                    />
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <SelectOption
                      id={'transferToExchangeMarket'}
                      labelId={'transferToExchangeMarket'}
                      label={'Transfer To Product'}
                      defaultValue={store.filter.filters?.transferToExchangeMarket}
                      remote={{
                        field: 'transferToExchangeMarket',
                        url: 'account_types',
                        key: 'alias',
                        value: 'name',
                      }}
                      onChange={value => {
                        updateFilter({ transferToExchangeMarket: value })
                      }}
                    />
                  </Grid>

                  <Grid item xs={12} sm={3}>
                    <SelectOption
                      id={'status'}
                      label={'Status'}
                      labelId={'status-label'}
                      options={TRANSACTION_STATUSES}
                      defaultValue={store.filter.filters?.status}
                      onChange={value => {
                        updateFilter({ status: value })
                      }}
                    />
                  </Grid>
                </Grid>
              </CardContent>

              <CardActions sx={{ display: 'flex', alignItems: 'right', justifyContent: 'right' }}>
                <Button
                  type='reset'
                  size='medium'
                  sx={{ width: 250 }}
                  color='secondary'
                  variant='outlined'
                  onClick={onResetButtonClicked}
                >
                  Reset Filter
                </Button>
                <Button
                  variant='contained'
                  size='medium'
                  fullWidth
                  type='submit'
                  sx={{ mr: 2, width: 250 }}
                  onClick={onSubmitButtonClicked}
                >
                  Filter Now
                </Button>
              </CardActions>
            </Card>
          </Grid>
        </Grid>
      </form>
      <ErrorModal
        isError={!isEmpty(error) || !isEmpty(dropdownError)}
        errorMessage={!isEmpty(error) ? error : dropdownError}
        dependencies={[error, dropdownError]}
      />
    </div>
  )
}

export default TransferCashFilter
