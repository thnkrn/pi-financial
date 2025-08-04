import { formatCurrency } from '@/utils/fmt'
import { renderEffectiveDate, renderStatus } from '@/views/components/DataTableUtil'
import { Icon } from '@iconify/react'
import IconButton from '@mui/material/IconButton'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams, gridDateComparator } from '@mui/x-data-grid'
import dayjs from 'dayjs'
import Link from 'next/link'

export const geColumns: GridColDef[] = [
  {
    field: 'col1',
    headerName: '',
    width: 50,
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        <Tooltip title='View'>
          <IconButton
            size='small'
            component={Link}
            target='_blank'
            href={`/transaction/${params?.row?.transactionNo}`}
            sx={{ mr: 0.5 }}
            disabled={!params?.row?.transactionNo}
          >
            <Icon icon='mdi:eye-outline' />
          </IconButton>
        </Tooltip>
      </Typography>
    ),
  },
  {
    flex: 0.175,
    type: 'date',
    minWidth: 200,
    field: 'createdAt',
    headerName: 'TRANSACTION CREATED',
    sortComparator: gridDateComparator,
    valueGetter: params => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {dayjs(params.row.createdAt).format('DD/MM/YYYY HH:mm:ss')}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 180,
    field: 'transactionNo',
    headerName: 'TRANSACTION NO',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.transactionNo ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 150,
    field: 'accountNo',
    headerName: 'ACCOUNT NUMBER',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.accountCode ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 200,
    field: 'globalAccount',
    headerName: 'GLOBAL ACCOUNT',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.globalAccount ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 200,
    field: 'accountType',
    headerName: 'ACCOUNT TYPE',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.product?.name ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 250,
    field: 'customerName',
    headerName: 'CUSTOMER ACCOUNT NAME',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Tooltip title={params.row.customerName} placement='bottom-end'>
        <Typography variant='body2' sx={{ color: 'text.primary' }}>
          {params.row.customerName ?? '-'}
        </Typography>
      </Tooltip>
    ),
  },
  {
    flex: 0.1,
    minWidth: 125,
    field: 'amountThb',
    headerName: 'AMOUNT THB',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant={'body2'} sx={{ color: 'text.primary', display: 'inline' }}>
        {`฿ ${formatCurrency(params.row.requestedAmount)}`}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 125,
    field: 'amountCcy',
    headerName: 'AMOUNT CCY',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.requestedAmount ? (
          <Typography variant={'body2'} sx={{ color: 'text.primary', display: 'inline' }}>
            {`${params.row.toCurrency === 'usd' ? '$' : ''} ${formatCurrency(params.row.transferAmount)}`}
          </Typography>
        ) : (
          '-'
        )}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 100,
    field: 'channel',
    headerName: 'CHANNEL',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.channel?.name ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'status',
    headerName: 'STATUS',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => {
      return renderStatus(params.row.status)
    },
  },
  {
    flex: 0.15,
    minWidth: 280,
    field: 'responseCode',
    headerName: 'RESPONSE DESCRIPTION',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.responseCode?.description ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    type: 'date',
    minWidth: 180,
    field: 'paymentReceivedDateTime',
    headerName: 'PAYMENT RECEIVED',
    sortable: false,
    valueGetter: params => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => {
      return renderEffectiveDate(params)
    },
  },
  {
    flex: 0.125,
    minWidth: 200,
    field: 'bankAccountNo',
    headerName: 'BANK ACCOUNT NO',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.bankAccountNo ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 200,
    field: 'bankName',
    headerName: 'SENDER BANK NAME',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.bankName ?? '-'}
      </Typography>
    ),
  },
]
