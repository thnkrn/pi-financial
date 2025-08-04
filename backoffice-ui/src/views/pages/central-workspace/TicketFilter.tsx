import { IGetTicketsRequest } from '@/lib/api/clients/backoffice/central-workspace/types'
import { fetchTickets, resetState, updateFilterState } from '@/store/apps/central-workspace'
import { showErrors } from '@/utils/fmt'
import ErrorModal from '@/views/components/ErrorModal'
import SelectOption from '@/views/forms/select/SelectOption'
import { yupResolver } from '@hookform/resolvers/yup'
import { Icon } from '@iconify/react'
import { CardActions, FormHelperText, ListItemText } from '@mui/material'
import Button from '@mui/material/Button'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Divider from '@mui/material/Divider'
import FormControl from '@mui/material/FormControl'
import Grid from '@mui/material/Grid'
import List from '@mui/material/List'
import ListItem from '@mui/material/ListItem'
import ListItemIcon from '@mui/material/ListItemIcon'
import TextField from '@mui/material/TextField'
import Typography from '@mui/material/Typography'
import { ThunkDispatch } from '@reduxjs/toolkit'
import extend from 'lodash/extend'
import isEmpty from 'lodash/isEmpty'
import isEqual from 'lodash/isEqual'
import map from 'lodash/map'
import omitBy from 'lodash/omitBy'
import { useEffect, useState } from 'react'
import { Controller, useForm } from 'react-hook-form'
import { useDispatch, useSelector } from 'react-redux'
import { RootState } from 'src/store'
import Swal from 'sweetalert2'
import withReactContent from 'sweetalert2-react-content'
import * as yup from 'yup'

const defaultValues = {
  customerCode: '',
}

const schema = yup.object().shape({
  customerCode: yup.string().max(10, obj => showErrors('Customer Code', obj.value.length, obj.max)),
})

const initialState = {
  responseCodeId: 'ALL',
  customerCode: '',
  status: 'Pending',
  page: 1,
  pageSize: 10,
  orderBy: 'createdAt',
  orderDir: 'desc',
}

const TicketFilter = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const store = useSelector((state: any) => state.centralWorkspace)

  const [filter, setFilter] = useState<IGetTicketsRequest>(initialState)
  const error = useSelector((state: RootState) => state.centralWorkspace.errorMessage)
  const dropdownError = useSelector((state: RootState) => state.dropdown.errorMessage)

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
    dispatch(updateFilterState(initialState))

    dispatch(fetchTickets(initialState))

    return () => {
      setFilter(initialState)
      dispatch(resetState(initialState))
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  useEffect(() => {
    setFilter(initialState)
    if (isEqual(store.filter, initialState)) {
      reset()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store, resetState])

  const updateFilter = (filterValue: any) => {
    const currentFilter = store.filter
    const newFilter = extend({}, currentFilter, filterValue)

    const storeFilter = extend({}, filter, newFilter)
    setFilter(storeFilter)

    dispatch(updateFilterState(storeFilter))
  }

  const onResetButtonClicked = () => {
    dispatch(resetState(initialState))
    dispatch(fetchTickets(initialState))
  }

  const onSubmit = () => {
    let currentFilter = { ...store.filter }
    currentFilter = omitBy(currentFilter, v => {
      return v === 'ALL' || v === null || v === ''
    })

    dispatch(updateFilterState(currentFilter))

    dispatch(fetchTickets(currentFilter))
  }

  const onSubmitButtonClicked = () => {
    if (!isEmpty(errors)) {
      const ListItems = map(errors, e => {
        return (
          <ListItem disablePadding key={e?.message}>
            <ListItemIcon>
              <Icon icon='mdi:alert-circle-outline' fontSize={20} color={'#FF4D49'} />
            </ListItemIcon>
            <ListItemText sx={{ color: '#FF4D49' }}>
              <span className={'swal-list-text'}>{e?.message}</span>
            </ListItemText>
          </ListItem>
        )
      })

      const Lists = <List>{ListItems}</List>

      ReactSwal.fire({
        icon: 'error',
        title: 'Fields validation errors',
        html: <div>{Lists}</div>,
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
                <Grid container spacing={2}>
                  <Grid item xs={6}>
                    <Typography variant='body2' sx={{ fontWeight: 600 }}>
                      Filter Ticket Detail
                    </Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant='body2' sx={{ fontWeight: 600 }}>
                      Customer code
                    </Typography>
                  </Grid>
                  <Grid item xs={12} sm={6}>
                    <SelectOption
                      id={'responseCodeId'}
                      label={'Response Code'}
                      labelId={'responseCodeId'}
                      remote={{
                        field: 'responseCodes',
                        url: 'response_codes?HasAction=true',
                        key: 'id',
                        value: 'description',
                        extension: 'productType',
                        prefix: 'machine',
                      }}
                      defaultValue={store.filter?.responseCodeId}
                      onChange={value => {
                        updateFilter({ responseCodeId: value })
                      }}
                    />
                  </Grid>

                  <Grid item xs={12} sm={6}>
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
                </Grid>
              </CardContent>
              <Divider sx={{ m: '0 !important' }} />
              <CardActions sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <Button type='reset' size='small' color='secondary' variant='outlined' onClick={onResetButtonClicked}>
                  Reset Filter
                </Button>
                <Button size='small' type='submit' sx={{ mr: 2 }} variant='contained' onClick={onSubmitButtonClicked}>
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

export default TicketFilter
