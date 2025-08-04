import { Visible } from '@/@core/components/auth/Visible'
import { generateReport } from '@/lib/api/clients/backoffice/report'
import { IGenerateReportRequest, IGetReportsRequest } from '@/lib/api/clients/backoffice/report/types'
import { fetchReport, updateFilterState } from '@/store/apps/report'
import ErrorModal from '@/views/components/ErrorModal'
import { datadogLogs } from '@datadog/browser-logs'
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
import { DATE_FORMAT } from '../constants'
import ReportGeneration from './ReportGeneration'
import ReportHistoryTable from './ReportHistoryTable'

const INITIAL_FILTER: IGetReportsRequest = {
  page: 1,
  pageSize: 10,
  orderBy: 'createdAt',
  orderDir: 'desc',
  generatedType: 'onDemand',
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
  const [dateFrom, setDateFrom] = useState<Date | null>(null)
  const [dateTo, setDateTo] = useState<Date | null>(null)
  const [isGenerating, setIsGenerating] = useState(false)

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

  const onSubmit: SubmitHandler<IFormInput> = async data => {
    setIsGenerating(true)
    try {
      if (!isDateRangeError()) {
        const payload: IGenerateReportRequest = {
          type: data?.type,
          dateFrom: dayjs(dateFrom).format(DATE_FORMAT),
          dateTo: dayjs(dateTo).format(DATE_FORMAT),
        }

        datadogLogs.logger.info('generateReport', { action: 'generateReport', payload, action_status: 'request' })

        const response = await generateReport(payload)

        if (response.status !== 201) {
          throw new Error(`Unexpected error from generate report: ${response.status}`)
        }
      }
    } catch (e) {
      const displayAlert = (await import('@/views/components/DisplayAlert')).default
      if (typeof e === 'string') {
        displayAlert(e)
      } else if (e instanceof Error) {
        displayAlert(e.message)
      } else {
        displayAlert('Unexpected error from generate report')
      }
    }

    setTimeout(() => {
      setIsGenerating(false)
      setCurrentDateTime(new Date())
      dispatch(fetchReport(INITIAL_FILTER))
    }, 1000)
  }

  const onRefresh = () => {
    setCurrentDateTime(new Date())
    dispatch(fetchReport(INITIAL_FILTER))
  }

  const onDateRangeChange = (start: Date, end: Date) => {
    setDateFrom(start ? dayjs(start).toDate() : null)
    setDateTo(end ? dayjs(end).toDate() : null)
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Visible allowedRoles={['report-export']}>
          <ReportGeneration
            onSubmit={onSubmit}
            isGenerating={isGenerating}
            dateFrom={dateFrom}
            dateTo={dateTo}
            onDateRangeChange={onDateRangeChange}
          />
        </Visible>
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
