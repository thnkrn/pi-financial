import { Visible } from '@/@core/components/auth/Visible'
import { downloadPiAppDwDailyReport } from '@/lib/api/clients/backoffice/report'
import { RootState } from '@/store/index'
import ErrorModal from '@/views/components/ErrorModal'
import { datadogLogs } from '@datadog/browser-logs'
import Grid from '@mui/material/Grid'
import dayjs from 'dayjs'
import isEmpty from 'lodash/isEmpty'
import { useState } from 'react'
import { SubmitHandler } from 'react-hook-form'
import { useSelector } from 'react-redux'
import Swal from 'sweetalert2'
import { DATE_FORMAT } from '../constants'
import ReportGeneration from './ReportGeneration'

export interface IFormInput {
  type: string
}

export interface IPaginationModel {
  page: number
  pageSize: number
}

const Index = () => {
  const [dateFrom, setDateFrom] = useState<Date | null>(null)
  const [dateTo, setDateTo] = useState<Date | null>(null)
  const [isDownloading, setIsDownloading] = useState(false)
  const error = useSelector((state: RootState) => state.report.errorMessage)

  const isDateRangeError = () => {
    if (!dateFrom) {
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
    setIsDownloading(true)
    try {
      if (!isDateRangeError()) {
        await downloadPiAppDwDailyReport(
          dayjs(dateFrom).format(DATE_FORMAT),
          dateTo ? dayjs(dateTo).format(DATE_FORMAT) : dayjs(dateFrom).format(DATE_FORMAT),
          data?.type
        )
        datadogLogs.logger.info('downloadPiAppDwDailyReport', {
          action: 'downloadPiAppDwDailyReport',
          action_status: 'request',
        })
      }
    } catch (e) {
      const displayAlert = (await import('@/views/components/DisplayAlert')).default
      if (typeof e === 'string') {
        displayAlert(e)
      } else if (e instanceof Error) {
        displayAlert(e.message)
      } else {
        displayAlert('Unexpected error from download report')
      }
    }

    setTimeout(() => {
      setIsDownloading(false)
    }, 1000)
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
            isDownloading={isDownloading}
            dateFrom={dateFrom}
            dateTo={dateTo}
            onDateRangeChange={onDateRangeChange}
          />
        </Visible>
      </Grid>
      <ErrorModal isError={!isEmpty(error)} errorMessage={error} dependencies={[error]} />
    </Grid>
  )
}

export default Index
