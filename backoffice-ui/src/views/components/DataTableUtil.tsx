import { Chip } from '@mui/material'
import Typography from '@mui/material/Typography'
import { GridRenderCellParams } from '@mui/x-data-grid'
import dayjs from 'dayjs'
import isEmpty from 'lodash/isEmpty'

export const renderOtpConfirmedDateTime = (params: GridRenderCellParams) => {
  if (isEmpty(params?.row?.otpConfirmedDateTime)) {
    return '-'
  }

  return (
    <Typography variant='body2' sx={{ color: 'text.primary' }}>
      {dayjs(params?.row?.otpConfirmedDateTime).format('DD/MM/YYYY HH:mm:ss')}
    </Typography>
  )
}

export const renderEffectiveDate = (params: GridRenderCellParams) => {
  if (isEmpty(params?.row?.effectiveDateTime)) {
    return '-'
  }

  return (
    <Typography variant='body2' sx={{ color: 'text.primary' }}>
      {dayjs(params?.row?.effectiveDateTime).format('DD/MM/YYYY HH:mm:ss')}
    </Typography>
  )
}

export const renderStatus = (status: string) => {
  let color: 'default' | 'secondary' | 'success' | 'warning' | 'error' | 'primary' | 'info' = 'secondary'
  switch (status?.toLowerCase()) {
    case 'success':
      color = 'success'
      break
    case 'pending':
    case 'processing':
      color = 'warning'
      break
    case 'fail':
    case 'rejected':
      color = 'error'
      break
    case 'approved':
      color = 'primary'
  }

  return <Chip key={status} variant='outlined' color={color} label={status} size='medium' />
}
