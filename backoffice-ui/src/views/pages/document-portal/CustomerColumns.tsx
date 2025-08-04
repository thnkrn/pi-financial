import { renderCellWithNA } from '@/@core/utils/text-utilities'
import DownloadIcon from '@mui/icons-material/Download'
import Button from '@mui/material/Button'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import dayjs from 'dayjs'
import { Translate } from 'next-translate'
import { AttachmentData } from './types'

type DownloadAttachmentsHandler = (data: AttachmentData[], custCode: string | null) => Promise<void>

export const columnsConfig = (
  t: Translate,
  handleDownloadAttachments: DownloadAttachmentsHandler,
  isDownloading: boolean
): GridColDef[] => [
  {
    width: 150,
    type: 'date',
    headerName: t('DOCUMENT_PORTAL_TABLE_CREATED_DATE', {}, { default: 'Created date' }),
    field: 'createdAt',
    valueGetter: params => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={dayjs(params.row.createdDate).format('DD/MM/YYYY, HH:mm')} placement='bottom-end'>
        <Typography variant='body2' sx={{ color: 'text.primary' }} data-testid={'customer-created-date'}>
          {dayjs(params.row.createdDate).format('DD/MM/YYYY, HH:mm')}
        </Typography>
      </Tooltip>
    ),
  },
  {
    width: 150,
    field: 'customerCode',
    sortable: false,
    headerName: t('DOCUMENT_PORTAL_TABLE_CUSTOMER_CODE', {}, { default: 'Customer code' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={params.row.custCode} placement='bottom-end'>
        <Typography variant='body2' sx={{ color: 'text.primary' }} data-testid={'customer-code'}>
          {renderCellWithNA(params.row.custCode)}
        </Typography>
      </Tooltip>
    ),
  },
  {
    minWidth: 250,
    flex: 1,
    sortable: false,
    field: 'customerEmail',
    headerName: t('DOCUMENT_PORTAL_TABLE_EMAIL', {}, { default: 'Customer code' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={params.row.email} placement='bottom-end'>
        <Typography variant='body2' sx={{ color: 'text.primary' }} data-testid={'customer-email'}>
          {renderCellWithNA(params.row.email)}
        </Typography>
      </Tooltip>
    ),
  },
  {
    width: 200,
    field: 'customerEnglishName',
    sortable: false,
    headerName: t('DOCUMENT_PORTAL_TABLE_ENGLISH_NAME', {}, { default: 'English name' }),
    renderCell: (params: GridRenderCellParams) => {
      return (
        <Tooltip title={`${params.row.firstNameEn} ${params.row.lastNameEn}`} placement='bottom-end'>
          <Typography variant='body2' sx={{ color: 'text.primary' }} data-testid={'customer-english-name'}>
            {renderCellWithNA(`${params.row.firstNameEn} ${params.row.lastNameEn}`)}
          </Typography>
        </Tooltip>
      )
    },
  },
  {
    width: 200,
    sortable: false,
    field: 'customerThaiName',
    headerName: t('DOCUMENT_PORTAL_TABLE_THAI_NAME', {}, { default: 'Thai name' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={`${params.row.firstNameTh} ${params.row.lastNameTh}`} placement='bottom-end'>
        <Typography variant='body2' sx={{ color: 'text.primary' }} data-testid={'customer-thai-name'}>
          {renderCellWithNA(`${params.row.firstNameTh} ${params.row.lastNameTh}`)}
        </Typography>
      </Tooltip>
    ),
  },
  {
    width: 200,
    field: 'customerCitizenId',
    sortable: false,
    headerName: t('DOCUMENT_PORTAL_TABLE_CITIZEN_ID', {}, { default: 'Citizen Id' }),
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={params.row.citizenId} placement='bottom-end'>
        <Typography variant='body2' sx={{ color: 'text.primary' }} data-testid={'customer-citizen-id'}>
          {renderCellWithNA(params.row.citizenId)}
        </Typography>
      </Tooltip>
    ),
  },
  {
    minWidth: 180,
    sortable: false,
    headerName: '',
    field: 'fileDownloadButton',
    renderCell: (params: GridRenderCellParams) => (
      <Button
        variant='contained'
        disabled={isDownloading}
        onClick={() => handleDownloadAttachments(params.row.documents, params.row.custCode)}
        data-testid={'customer-file-download-button'}
      >
        <DownloadIcon sx={{ fontSize: '1.25rem', marginRight: '10px', verticalAlign: 'middle' }} />
        {t('DOCUMENT_PORTAL_TABLE_DOWNLOAD', {}, { default: 'Download' })}
      </Button>
    ),
  },
]
