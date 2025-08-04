import { fetchTransferCashTransactions, resetState, updateFilterState } from '@/store/apps/transfer-cash'
import { AppDispatch, RootState } from '@/store/index'
import Grid from '@mui/material/Grid'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'
import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import Swal from 'sweetalert2'
import { DATE_FORMAT, LAST_7_DAYS, TODAY } from './constants'
import {
  IGetTransferCashTransactionsRequest,
  ITransferCashFilters,
} from '@/lib/api/clients/backoffice/transactions/types'
import { PiBackofficeServiceAPIModelsTransferCashResponse } from '@pi-financial/backoffice-srv'
import TransferCashFilter from '@/views/pages/transfer-cash/TransferCashFilter'
import TransferCashDataTable from '@/views/pages/transfer-cash/TransferCashDataTable'
import dayjs from 'dayjs'

const Index = () => {
  const initialState: IGetTransferCashTransactionsRequest = {
    filters: {
      status: 'PENDING',
      state: '',
      transactionNo: '',
      transferFromAccountCode: '',
      transferToAccountCode: '',
      transferFromExchangeMarket: '',
      transferToExchangeMarket: '',
      otpConfirmedDateFrom: null,
      otpConfirmedDateTo: null,
      createdAtFrom: dayjs(LAST_7_DAYS).format(DATE_FORMAT),
      createdAtTo: dayjs(TODAY).format(DATE_FORMAT),
    },
    page: 1,
    pageSize: 20,
    orderBy: 'CreatedAt',
    orderDir: 'desc',
  }

  const [total, setTotal] = useState<number>(1)
  const [rows, setRows] = useState<PiBackofficeServiceAPIModelsTransferCashResponse[]>([])
  const [filter, setFilter] = useState<IGetTransferCashTransactionsRequest>(initialState)
  const [startCreatedDate, setStartCreatedDate] = useState<Date | null>(LAST_7_DAYS.toDate())
  const [endCreatedDate, setEndCreatedDate] = useState<Date | null>(TODAY)
  const [startOtpConfirmedDate, setStartOtpConfirmedDate] = useState<Date | null>(null)
  const [endOtpConfirmedDate, setEndOtpConfirmedDate] = useState<Date | null>(null)

  const store = useSelector((state: RootState) => state.transferCash)
  const dispatch = useDispatch<AppDispatch>()
  const error = useSelector((state: RootState) => state.transferCash.errorMessage)
  const dropdownError = useSelector((state: RootState) => state.dropdown.errorMessage)

  useEffect(() => {
    setRows(store.transactions)
    setTotal(store.filter.total ?? 0)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store.transactions])

  useEffect(() => {
    dispatch(updateFilterState(initialState))

    dispatch(fetchTransferCashTransactions(initialState))

    return () => {
      setFilter(initialState)
      dispatch(resetState(initialState))
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const onPaginate = (currentFilter: IGetTransferCashTransactionsRequest) => {
    dispatch(updateFilterState(currentFilter))
    dispatch(fetchTransferCashTransactions(currentFilter))
  }

  const updateFilter = (filterValue: ITransferCashFilters) => {
    const currentFilter = store.filter.filters
    const newFilter = extend({}, currentFilter, filterValue)

    const storeFilter = extend({}, filter, { filters: newFilter })
    setFilter(storeFilter)

    dispatch(updateFilterState(storeFilter))
  }

  const checkDateRange = () => {
    if ((startOtpConfirmedDate && !endOtpConfirmedDate) || (startCreatedDate && !endCreatedDate)) {
      Swal.fire({
        title: 'Error!',
        text: 'Please input a date range',
        icon: 'error',
        confirmButtonText: 'OK',
      })

      return true
    }

    return false
  }

  const onResetButtonClicked = () => {
    dispatch(resetState(initialState))
    dispatch(fetchTransferCashTransactions(initialState))
  }

  const onSubmit = () => {
    const currentFilter = { ...store.filter }

    if (!checkDateRange()) {
      currentFilter.filters = omitBy(currentFilter.filters, v => {
        return v === 'ALL' || v === null || v === ''
      })

      dispatch(updateFilterState(currentFilter))

      dispatch(fetchTransferCashTransactions(currentFilter))
    }
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <TransferCashFilter
          initialState={initialState}
          startCreatedDate={startCreatedDate}
          endCreatedDate={endCreatedDate}
          startOtpConfirmedDate={startOtpConfirmedDate}
          endOtpConfirmedDate={endOtpConfirmedDate}
          store={store}
          error={error}
          dropdownError={dropdownError}
          setFilter={setFilter}
          updateFilter={updateFilter}
          onResetButtonClicked={onResetButtonClicked}
          setStartCreatedDate={setStartCreatedDate}
          setEndCreatedDate={setEndCreatedDate}
          setStartOtpConfirmedDate={setStartOtpConfirmedDate}
          setEndOtpConfirmedDate={setEndOtpConfirmedDate}
          onSubmit={onSubmit}
        />
        <TransferCashDataTable
          total={total}
          rows={rows}
          store={store}
          onPaginate={onPaginate}
          isLoading={store.isLoading}
        />
      </Grid>
    </Grid>
  )
}

export default Index
