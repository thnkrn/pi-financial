import { DEPOSIT_PRODUCT_TYPE } from '@/constants/deposit/type'
import { IDepositFilters, IGetDepositTransactionsRequest } from '@/lib/api/clients/backoffice/deposit/types'
import { fetchDeposits, resetState, updateFilterState } from '@/store/apps/deposit'
import { AppDispatch, RootState } from '@/store/index'
import Grid from '@mui/material/Grid'
import dayjs from 'dayjs'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'
import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import Swal from 'sweetalert2'
import DepositDataTable from './DepositDataTable'
import DepositFilter from './DepositFilter'
import { DATE_FORMAT, LAST_7_DAYS, TODAY } from './constants'
import { PiBackofficeServiceAPIModelsTransactionHistoryV2Response } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransactionHistoryV2Response'

interface Props {
  productType: DEPOSIT_PRODUCT_TYPE
}

const Index = ({ productType }: Props) => {
  const isSetTrade = productType === DEPOSIT_PRODUCT_TYPE.SetTrade
  const initialState: IGetDepositTransactionsRequest = {
    filters: {
      channel: isSetTrade ? DEPOSIT_PRODUCT_TYPE.SetTrade : 'ALL',
      transactionType: 'Deposit',
      accountType: 'ALL',
      responseCodeId: 'ALL',
      bankCode: 'ALL',
      accountNumber: '',
      customerCode: '',
      accountCode: '',
      transactionNumber: '',
      status: 'PENDING',
      paymentReceivedDateFrom: null,
      paymentReceivedDateTo: null,
      createdAtFrom: dayjs(LAST_7_DAYS).format(DATE_FORMAT),
      createdAtTo: dayjs(TODAY).format(DATE_FORMAT),

      // NOTE: BE treat settrade product type as ThaiEquity, handle settrade by channel filter instead
      productType: isSetTrade ? DEPOSIT_PRODUCT_TYPE.NonGlobalEquity : productType,
    },
    page: 1,
    pageSize: 20,
    orderBy: 'CreatedAt',
    orderDir: 'desc',
  }

  const [total, setTotal] = useState<number>(1)
  const [rows, setRows] = useState<PiBackofficeServiceAPIModelsTransactionHistoryV2Response[]>([])
  const [filter, setFilter] = useState<IGetDepositTransactionsRequest>(initialState)
  const [startCreatedDate, setStartCreatedDate] = useState<Date | null>(LAST_7_DAYS.toDate())
  const [endCreatedDate, setEndCreatedDate] = useState<Date | null>(TODAY)
  const [starEffectiveDate, setStarEffectiveDate] = useState<Date | null>(null)
  const [endEffectiveDate, setEndEffectiveDate] = useState<Date | null>(null)

  const store = useSelector((state: RootState) => state.deposit)
  const dispatch = useDispatch<AppDispatch>()
  const error = useSelector((state: RootState) => state.deposit.errorMessage)
  const dropdownError = useSelector((state: RootState) => state.dropdown.errorMessage)

  useEffect(() => {
    setRows(store.transactions)
    setTotal(store.filter.total ?? 0)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store.transactions])

  useEffect(() => {
    dispatch(updateFilterState(initialState))

    dispatch(fetchDeposits(initialState))

    return () => {
      setFilter(initialState)
      dispatch(resetState(initialState))
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const onPaginate = (currentFilter: IGetDepositTransactionsRequest) => {
    dispatch(updateFilterState(currentFilter))
    dispatch(fetchDeposits(currentFilter))
  }

  const updateFilter = (filterValue: IDepositFilters) => {
    const currentFilter = store.filter.filters
    const newFilter = extend({}, currentFilter, filterValue)

    const storeFilter = extend({}, filter, { filters: newFilter })
    setFilter(storeFilter)

    dispatch(updateFilterState(storeFilter))
  }

  const checkDateRange = () => {
    if ((starEffectiveDate && !endEffectiveDate) || (startCreatedDate && !endCreatedDate)) {
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
    dispatch(fetchDeposits(initialState))
  }

  const onSubmit = () => {
    const currentFilter = { ...store.filter }

    if (!checkDateRange()) {
      currentFilter.filters = omitBy(currentFilter.filters, v => {
        return v === 'ALL' || v === null || v === ''
      })

      dispatch(updateFilterState(currentFilter))

      dispatch(fetchDeposits(currentFilter))
    }
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <DepositFilter
          productType={productType}
          initialState={initialState}
          startCreatedDate={startCreatedDate}
          endCreatedDate={endCreatedDate}
          starEffectiveDate={starEffectiveDate}
          endEffectiveDate={endEffectiveDate}
          store={store}
          error={error}
          dropdownError={dropdownError}
          setFilter={setFilter}
          updateFilter={updateFilter}
          onResetButtonClicked={onResetButtonClicked}
          setStartCreatedDate={setStartCreatedDate}
          setEndCreatedDate={setEndCreatedDate}
          setStarEffectiveDate={setStarEffectiveDate}
          setEndEffectiveDate={setEndEffectiveDate}
          onSubmit={onSubmit}
        />
        <DepositDataTable
          total={total}
          rows={rows}
          store={store}
          onPaginate={onPaginate}
          isLoading={store.isLoading}
          product={productType}
        />
      </Grid>
    </Grid>
  )
}

export default Index
