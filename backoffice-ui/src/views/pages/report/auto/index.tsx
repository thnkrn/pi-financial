import { IGetReportsRequest } from '@/lib/api/clients/backoffice/report/types'
import { fetchReport, resetState, updateFilterState } from '@/store/apps/report'
import ErrorModal from '@/views/components/ErrorModal'
import Grid from '@mui/material/Grid'
import { PiBackofficeServiceAPIModelsReportResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsReportResponse'
import { ThunkDispatch } from '@reduxjs/toolkit'
import dayjs from 'dayjs'
import extend from 'lodash/extend'
import isEmpty from 'lodash/isEmpty'
import { useEffect, useRef, useState } from 'react'
import { SubmitHandler } from 'react-hook-form'
import { useDispatch, useSelector } from 'react-redux'
import { RootState } from 'src/store'
import Swal from 'sweetalert2'
import { DATE_FORMAT, LAST_7_DAYS, TODAY } from '../constants'
import ReportFilter from './ReportFilter'
import ReportHistoryTable from './ReportHistoryTable'

const INITIAL_FILTER: IGetReportsRequest = {
  page: 1,
  pageSize: 10,
  orderBy: 'createdAt',
  orderDir: 'desc',
  generatedType: 'Auto',
  dateFrom: dayjs(LAST_7_DAYS).format(DATE_FORMAT),
  dateTo: dayjs(TODAY).format(DATE_FORMAT),
}

export interface IFormInput {
  type: string
}

export interface IPaginationModel {
  page: number
  pageSize: number
}

const Index = () => {
  const [total, setTotal] = useState<number>(1)
  const [rows, setRows] = useState<PiBackofficeServiceAPIModelsReportResponse[]>([])
  const [paginationModel, setPaginationModel] = useState<IPaginationModel>({ page: 0, pageSize: 10 })
  const [filter, setFilter] = useState<IGetReportsRequest>(INITIAL_FILTER)
  const [currentDateTime, setCurrentDateTime] = useState(new Date())
  const [dateFrom, setDateFrom] = useState<Date | null>(LAST_7_DAYS.toDate())
  const [dateTo, setDateTo] = useState<Date | null>(TODAY)

  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const initialRender = useRef(true)
  const error = useSelector((state: RootState) => state.report.errorMessage)
  const store = useSelector((state: any) => state.report)

  useEffect(() => {
    dispatch(updateFilterState(filter))
    dispatch(fetchReport(filter))

    return () => {
      setFilter(INITIAL_FILTER)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  useEffect(() => {
    if (initialRender.current) {
      initialRender.current = false
    } else {
      const currentFilter = extend(
        {},
        { ...store.filter },
        { page: paginationModel.page + 1, pageSize: paginationModel.pageSize }
      )
      setFilter(currentFilter)
      dispatch(updateFilterState(currentFilter))
      setCurrentDateTime(new Date())
      dispatch(fetchReport(currentFilter))
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paginationModel])

  useEffect(() => {
    setRows(store?.reports)
    setTotal(store?.filter?.total || 0)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store?.reports])

  const isDateRangeError = () => {
    if (!dateFrom || !dateTo) {
      Swal.fire({
        title: 'Error!',
        text: 'Please input a valid date range',
        icon: 'error',
        confirmButtonText: 'OK',
      })

      return true
    }

    return false
  }

  const onSubmit: SubmitHandler<IFormInput> = () => {
    const currentFilter = store.filter

    if (!isDateRangeError()) {
      setCurrentDateTime(new Date())
      dispatch(fetchReport(currentFilter))
    }
  }

  const onRefresh = () => {
    setCurrentDateTime(new Date())

    if (dateFrom && dateTo) {
      const currentFilter = store.filter
      dispatch(fetchReport(currentFilter))
    } else {
      dispatch(resetState(INITIAL_FILTER))
      dispatch(fetchReport(INITIAL_FILTER))
    }
  }

  const updateFilter = (filterValue: any) => {
    const currentFilter = store.filter
    const newFilter = extend({}, currentFilter, filterValue)

    setFilter(newFilter)
    dispatch(updateFilterState(newFilter))
  }

  const onDateRangeChange = (start: Date, end: Date) => {
    updateFilter({
      dateFrom: start ? dayjs(start).format(DATE_FORMAT) : null,
      dateTo: end ? dayjs(end).format(DATE_FORMAT) : null,
    })

    setDateFrom(start ? dayjs(start).toDate() : null)
    setDateTo(end ? dayjs(end).toDate() : null)
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <ReportFilter
          onSubmit={onSubmit}
          dateFrom={dateFrom}
          dateTo={dateTo}
          onDateRangeChange={onDateRangeChange}
          updateFilter={updateFilter}
        />
        <ReportHistoryTable
          rows={rows}
          total={total}
          paginationModel={paginationModel}
          setPaginationModel={setPaginationModel}
          isLoading={store.isLoading}
          onRefresh={onRefresh}
          currentDateTime={currentDateTime}
        />
      </Grid>
      <ErrorModal isError={!isEmpty(error)} errorMessage={error} dependencies={[error]} />
    </Grid>
  )
}

export default Index
