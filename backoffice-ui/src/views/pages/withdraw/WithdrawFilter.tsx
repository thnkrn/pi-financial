import TRANSACTION_STATUSES from '@/constants/TransactionStatus'
import { WITHDRAW_PRODUCT_TYPE } from '@/constants/withdraw/type'
import { IGetWithdrawTransactionsRequest, IWithdrawFilters } from '@/lib/api/clients/backoffice/withdraw/types'
import { IState, resetState } from '@/store/apps/withdraw'
import { getBankURL } from '@/utils/convert'
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
import TextField from '@mui/material/TextField'
import Typography from '@mui/material/Typography'
import { useTheme } from '@mui/material/styles'
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
import { DATE_FORMAT, LAST_7_DAYS, TODAY } from './constants'

const defaultValues = {
  accountNumber: '',
  customerCode: '',
  accountCode: '',
  transactionNumber: '',
}

const schema = yup.object().shape({
  accountNumber: yup.string().max(19, obj => showErrors('Receiver Bank Account Number', obj.value.length, obj.max)),
  customerCode: yup.string().max(10, obj => showErrors('Customer Code', obj.value.length, obj.max)),
  accountCode: yup.string().max(20, obj => showErrors('Customer Account Number', obj.value.length, obj.max)),
  transactionNumber: yup.string().max(20, obj => showErrors('Transaction No', obj.value.length, obj.max)),
})

interface Props {
  productType: WITHDRAW_PRODUCT_TYPE
  initialState: IGetWithdrawTransactionsRequest
  startCreatedDate: Date | null
  endCreatedDate: Date | null
  starEffectiveDate: Date | null
  endEffectiveDate: Date | null
  store: IState
  setFilter: (value: IGetWithdrawTransactionsRequest) => void
  updateFilter: (value: IWithdrawFilters) => void
  onResetButtonClicked: () => void
  setStartCreatedDate: (value: Date | null) => void
  setEndCreatedDate: (value: Date | null) => void
  setStarEffectiveDate: (value: Date | null) => void
  setEndEffectiveDate: (value: Date | null) => void
  onSubmit: () => void
  error: string
  dropdownError: string
}

const WithdrawFilter = ({
  productType,
  initialState,
  startCreatedDate,
  endCreatedDate,
  starEffectiveDate,
  endEffectiveDate,
  store,
  error,
  dropdownError,
  setFilter,
  updateFilter,
  onResetButtonClicked,
  setStartCreatedDate,
  setEndCreatedDate,
  setStarEffectiveDate,
  setEndEffectiveDate,
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
      setStarEffectiveDate(null)
      setEndEffectiveDate(null)
      reset()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store, resetState])

  const onEffectiveDateRangeChange = (start: Date, end: Date) => {
    updateFilter({
      effectiveDateFrom: start ? dayjs(start).format(DATE_FORMAT) : null,
      effectiveDateTo: end ? dayjs(end).format(DATE_FORMAT) : null,
    })

    setStarEffectiveDate(start ? dayjs(start).toDate() : null)
    setEndEffectiveDate(end ? dayjs(end).toDate() : null)
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
                <Grid container spacing={4}>
                  <Grid item xs={6}>
                    <Typography variant='subtitle1'>Withdrawal Info</Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant='subtitle1'>Payment Info</Typography>
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
                              updateFilter({ transactionNumber: e.target.value })
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
                            label='Customer Code'
                            placeholder='Customer Code'
                            aria-describedby='validation-schema-customer-code'
                            error={Boolean(errors.customerCode)}
                            onChange={e => {
                              onChange(e)
                              updateFilter({ customerCode: e.target.value })
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
                            label='Customer Account Number'
                            placeholder='Customer Account Number'
                            aria-describedby='validation-schema-customer-account-number'
                            error={Boolean(errors.accountCode)}
                            onChange={e => {
                              onChange(e)
                              updateFilter({ accountCode: e.target.value })
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
                      label={'Transaction Created'}
                    />
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <PickersRange
                      onChange={onEffectiveDateRangeChange}
                      popperPlacement={popperPlacement}
                      start={starEffectiveDate}
                      end={endEffectiveDate}
                      label={'Payment Sent'}
                    />
                  </Grid>
                  {productType === WITHDRAW_PRODUCT_TYPE.NonGlobalEquity ? (
                    <Grid item xs={12} sm={6}>
                      <SelectOption
                        id={'accountType'}
                        label={'Account Type'}
                        labelId={'accountType'}
                        remote={{ field: 'accountTypes', url: 'account_types', key: 'alias', value: 'name' }}
                        defaultValue={store.filter.filters?.accountType}
                        onChange={value => {
                          updateFilter({ accountType: value })
                        }}
                      />
                    </Grid>
                  ) : (
                    <Grid item xs={12} sm={6}>
                      <SelectOption
                        id={'accountType'}
                        label={'Account Type'}
                        labelId={'accountType'}
                        options={[{ key: 'Global Equity', value: 'Global Equity' }]}
                        defaultValue={'Global Equity'}
                        disabled
                        onChange={() => {}}
                      />
                    </Grid>
                  )}

                  <Grid item xs={12} sm={6}>
                    <SelectOption
                      id={'channel'}
                      label={'Withdraw by Channel'}
                      labelId={'channel'}
                      defaultValue={store.filter.filters?.channel}
                      remote={{ field: 'channel', url: 'transactions/withdraw/channels', key: 'alias', value: 'name' }}
                      onChange={value => {
                        updateFilter({ channel: value ?? 'ALL', bankCode: 'ALL' })
                      }}
                    />
                  </Grid>
                  <Grid item xs={12} sm={3}>
                    <FormControl fullWidth>
                      <Controller
                        name='accountNumber'
                        control={control}
                        render={({ field: { value, onChange } }) => (
                          <TextField
                            size={'small'}
                            fullWidth
                            value={value}
                            label='Receiver Bank Account Number'
                            placeholder='Receiver Bank Account Number'
                            aria-describedby='validation-schema-receiver-bank-account-no'
                            error={Boolean(errors.accountNumber)}
                            onChange={e => {
                              onChange(e)
                              updateFilter({ accountNumber: e.target.value })
                            }}
                          />
                        )}
                      />
                      {errors.accountNumber && (
                        <FormHelperText sx={{ color: 'error.main' }} id='validation-schema-receiver-bank-account-no'>
                          {errors.accountNumber.message}
                        </FormHelperText>
                      )}
                    </FormControl>
                  </Grid>
                  <Grid item xs={6} sm={3}>
                    <SelectOption
                      id={'bankCode'}
                      label={'Receiver Bank Name'}
                      labelId={'bankCode'}
                      defaultValue={store.filter.filters?.bankCode}
                      remote={{
                        field: 'banks',
                        url: getBankURL(store?.filter?.filters?.channel as string),
                        key: 'code',
                        value: 'abbreviation',
                      }}
                      onChange={value => {
                        updateFilter({ bankCode: value })
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
                  <Grid item xs={12} sm={3}>
                    <SelectOption
                      id={'responseCodeId'}
                      label={'Response Description'}
                      labelId={'responseCode'}
                      remote={{
                        field: 'responseCodes',
                        url: `response_codes?Machine=Withdraw&ProductType=${productType}`,
                        key: 'id',
                        value: 'description',
                      }}
                      defaultValue={store.filter.filters?.responseCodeId}
                      onChange={value => {
                        updateFilter({ responseCodeId: value })
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

export default WithdrawFilter
