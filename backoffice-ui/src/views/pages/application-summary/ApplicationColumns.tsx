import { renderCellWithNA } from '@/@core/utils/text-utilities'
import ErrorIcon from '@mui/icons-material/Error'
import Chip, { ChipProps } from '@mui/material/Chip'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import dayjs from 'dayjs'
import { Translate } from 'next-translate'

function getStatusColor(status: string): ChipProps['color'] {
  const statusColors: Record<string, ChipProps['color']> = {
    created: 'info',
    pending: 'secondary',
    completed: 'success',
    failed: 'warning',
    cancelled: 'error',
  }

  return statusColors[status] ?? 'default'
}

function getStatusVariant(status: string): 'outlined' | 'filled' {
  return status === 'created' ? 'outlined' : 'filled'
}

export const columnsConfig = (t: Translate): GridColDef[] => [
  {
    width: 150,
    type: 'date',
    headerName: t('APPLICATION_SUMMARY_TABLE_CREATED_DATE', {}, { default: 'Created date' }),
    field: 'CreatedAt',
    valueGetter: params => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={dayjs(params.row.createdDate).format('DD/MM/YYYY, HH:mm')} placement='bottom-end'>
        <Typography
          variant='body2'
          sx={{ color: 'text.primary', textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}
          data-testid={'application-summary-created-date'}
        >
          {dayjs(params.row.createdDate).format('DD/MM/YYYY, HH:mm')}
        </Typography>
      </Tooltip>
    ),
  },
  {
    width: 150,
    field: 'customerCode',
    sortable: false,
    headerName: t('APPLICATION_SUMMARY_TABLE_CUSTOMER_CODE', {}, { default: 'Customer code' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={params.row.custCode} placement='bottom-end'>
        <Typography
          variant='body2'
          sx={{ color: 'text.primary', textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}
          data-testid={'application-summary-customer-code'}
        >
          {renderCellWithNA(params.row.custCode)}
        </Typography>
      </Tooltip>
    ),
  },
  {
    width: 150,
    field: 'englishName',
    sortable: false,
    headerName: t('APPLICATION_SUMMARY_TABLE_ENGLISH_NAME', {}, { default: 'English name' }),
    renderCell: (params: GridRenderCellParams) => {
      return (
        <Tooltip title={`${params.row.firstNameEn} ${params.row.lastNameEn}`} placement='bottom-end'>
          <Typography
            variant='body2'
            sx={{ color: 'text.primary', textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}
            data-testid={'application-summary-english-name'}
          >
            {params.row.firstNameEn && params.row.lastNameEn
              ? `${params.row.firstNameEn} ${params.row.lastNameEn}`
              : `N/A`}
          </Typography>
        </Tooltip>
      )
    },
  },
  {
    width: 150,
    sortable: false,
    field: 'customerThaiName',
    headerName: t('APPLICATION_SUMMARY_TABLE_THAI_NAME', {}, { default: 'Thai name' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={`${params.row.firstNameTh} ${params.row.lastNameTh}`} placement='bottom-end'>
        <Typography
          variant='body2'
          sx={{ color: 'text.primary', textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}
          data-testid={'application-summary-thai-name'}
        >
          {params.row.firstNameTh && params.row.lastNameTh
            ? `${params.row.firstNameTh} ${params.row.lastNameTh}`
            : `N/A`}
        </Typography>
      </Tooltip>
    ),
  },
  {
    minWidth: 150,
    flex: 1,
    field: 'userId',
    sortable: false,
    headerName: t('APPLICATION_SUMMARY_TABLE_USER_ID', {}, { default: 'User ID' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={params.row.userId} placement='bottom-end'>
        <Typography
          variant='body2'
          sx={{ color: 'text.primary', textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}
          data-testid={'application-summary-user-id'}
        >
          {renderCellWithNA(params.row.userId)}
        </Typography>
      </Tooltip>
    ),
  },
  {
    minWidth: 150,
    flex: 1,
    field: 'accountOpeningRequestId',
    sortable: false,
    headerName: t('APPLICATION_SUMMARY_TABLE_AOR_ID', {}, { default: 'AOR Id' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={params.row.id} placement='bottom-end'>
        <Typography
          variant='body2'
          sx={{ color: 'text.primary', textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}
          data-testid={'application-summary-account-opening-request-id'}
        >
          {renderCellWithNA(params.row.id)}
        </Typography>
      </Tooltip>
    ),
  },
  {
    minWidth: 150,
    flex: 1,
    sortable: false,
    field: 'citizenId',
    headerName: t('APPLICATION_SUMMARY_TABLE_CITIZEN_ID', {}, { default: 'Citizen Id' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={params.row.citizenId} placement='bottom-end'>
        <Typography
          variant='body2'
          sx={{ color: 'text.primary', textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}
          data-testid={'application-summary-citizen-id'}
        >
          {renderCellWithNA(params.row.citizenId)}
        </Typography>
      </Tooltip>
    ),
  },
  {
    width: 150,
    sortable: false,
    field: 'applicationStatus',
    headerName: t('APPLICATION_SUMMARY_TABLE_STATUS', {}, { default: 'Status' }),
    renderCell: (params: GridRenderCellParams) => (
      <Chip
        sx={{ textTransform: 'uppercase' }}
        label={renderCellWithNA(params.row.status)}
        color={getStatusColor(params.row.status.toLowerCase())}
        variant={getStatusVariant(params.row.status)}
        data-testid={'application-summary-application-status'}
      />
    ),
  },
  {
    width: 70,
    field: 'bpmReceived',
    headerName: '',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.warning' }}>
        {!params.row.bpmReceived && (
          <Tooltip title='Error' placement='bottom-start'>
            <ErrorIcon />
          </Tooltip>
        )}
      </Typography>
    ),
  },
]
