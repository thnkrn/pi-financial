import { renderEffectiveDate, renderStatus } from '@/views/components/DataTableUtil'
import { Icon } from '@iconify/react'
import IconButton from '@mui/material/IconButton'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams, gridDateComparator } from '@mui/x-data-grid'
import dayjs from 'dayjs'
import Link from 'next/link'
import { NumericFormat } from 'react-number-format'

export const nonGeColumns: GridColDef[] = [
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
    headerName: 'TRANSACTION CREATED',
    field: 'createdAt',
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
    flex: 0.1,
    minWidth: 125,
    field: 'amount',
    headerName: 'AMOUNT',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.requestedAmount ? (
          <NumericFormat
            value={params.row.requestedAmount}
            thousandSeparator=','
            displayType='text'
            decimalScale={2}
            fixedDecimalScale={true}
          />
        ) : (
          '-'
        )}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
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
    headerName: 'RESPONSE DESCRIPTION',
    field: 'responseCode',
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
    minWidth: 200,
    headerName: 'PAYMENT SENT',
    field: 'effectiveDateTime',
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
    headerName: 'RECEIVER BANK NAME',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.bankName ?? '-'}
      </Typography>
    ),
  },
]
