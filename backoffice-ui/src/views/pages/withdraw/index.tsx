import { WITHDRAW_PRODUCT_TYPE } from '@/constants/withdraw/type'
import { IGetWithdrawTransactionsRequest, IWithdrawFilters } from '@/lib/api/clients/backoffice/withdraw/types'
import { fetchWithdraws, resetState, updateFilterState } from '@/store/apps/withdraw'
import { AppDispatch, RootState } from '@/store/index'
import Grid from '@mui/material/Grid'
import dayjs from 'dayjs'
import extend from 'lodash/extend'
import omitBy from 'lodash/omitBy'
import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import Swal from 'sweetalert2'
import WithdrawDataTable from './WithdrawDataTable'
import WithdrawFilter from './WithdrawFilter'
import { DATE_FORMAT, LAST_7_DAYS, TODAY } from './constants'
import { PiBackofficeServiceAPIModelsTransactionHistoryV2Response } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransactionHistoryV2Response'

interface Props {
  productType: WITHDRAW_PRODUCT_TYPE
}

const Index = ({ productType }: Props) => {
  const initialState: IGetWithdrawTransactionsRequest = {
    filters: {
      channel: 'ALL',
      transactionType: 'Withdraw',
      accountType: 'ALL',
      responseCodeId: 'ALL',
      bankCode: 'ALL',
      accountNumber: '',
      customerCode: '',
      accountCode: '',
      transactionNumber: '',
      status: 'PENDING',
      effectiveDateFrom: null,
      effectiveDateTo: null,
      createdAtFrom: dayjs(LAST_7_DAYS).format(DATE_FORMAT),
      createdAtTo: dayjs(TODAY).format(DATE_FORMAT),
      productType,
    },
    page: 1,
    pageSize: 20,
    orderBy: 'CreatedAt',
    orderDir: 'desc',
  }

  const [total, setTotal] = useState<number>(1)
  const [rows, setRows] = useState<PiBackofficeServiceAPIModelsTransactionHistoryV2Response[]>([])
  const [filter, setFilter] = useState<IGetWithdrawTransactionsRequest>(initialState)
  const [startCreatedDate, setStartCreatedDate] = useState<Date | null>(LAST_7_DAYS.toDate())
  const [endCreatedDate, setEndCreatedDate] = useState<Date | null>(TODAY)
  const [starEffectiveDate, setStarEffectiveDate] = useState<Date | null>(null)
  const [endEffectiveDate, setEndEffectiveDate] = useState<Date | null>(null)

  const store = useSelector((state: RootState) => state.withdraw)
  const error = useSelector((state: RootState) => state.withdraw.errorMessage)
  const dropdownError = useSelector((state: RootState) => state.dropdown.errorMessage)

  const dispatch = useDispatch<AppDispatch>()

  useEffect(() => {
    setRows(store.transactions)
    setTotal(store.filter.total ?? 0)

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store.transactions])

  useEffect(() => {
    dispatch(updateFilterState(initialState))

    dispatch(fetchWithdraws(initialState))

    return () => {
      setFilter(initialState)
      dispatch(resetState(initialState))
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const updateFilter = (filterValue: IWithdrawFilters) => {
    const currentFilter = store.filter.filters
    const newFilter = extend({}, currentFilter, filterValue)

    const storeFilter = extend({}, filter, { filters: newFilter })
    setFilter(storeFilter)

    dispatch(updateFilterState(storeFilter))
  }

  const onResetButtonClicked = () => {
    dispatch(resetState(initialState))

    dispatch(fetchWithdraws(initialState))
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

  const onSubmit = () => {
    const currentFilter = { ...store.filter }

    if (!checkDateRange() && currentFilter?.filters) {
      currentFilter.filters = omitBy(currentFilter.filters, v => {
        return v === 'ALL' || v === null || v === ''
      })

      dispatch(updateFilterState(currentFilter))

      dispatch(fetchWithdraws(currentFilter))
    }
  }

  const onPaginate = (currentFilter: IGetWithdrawTransactionsRequest) => {
    dispatch(updateFilterState(currentFilter))

    dispatch(fetchWithdraws(currentFilter))
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <WithdrawFilter
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
        <WithdrawDataTable
          rows={rows}
          total={total}
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
