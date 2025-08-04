import { renderOtpConfirmedDateTime, renderStatus } from '@/views/components/DataTableUtil'
import { Icon } from '@iconify/react'
import IconButton from '@mui/material/IconButton'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams, gridDateComparator } from '@mui/x-data-grid'
import dayjs from 'dayjs'
import Link from 'next/link'
import { NumericFormat } from 'react-number-format'

export const transferCashColumns: GridColDef[] = [
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
            href={`/transfer-cash/${params?.row?.transactionNo}`}
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
    field: 'transferFromAccountCode',
    headerName: 'FROM ACCOUNT NUMBER',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.transferFromAccountCode ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 150,
    field: 'transferToAccountCode',
    headerName: 'TO ACCOUNT NUMBER',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.transferToAccountCode ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 200,
    field: 'transferFromExchangeMarket',
    headerName: 'FROM ACCOUNT',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.transferFromExchangeMarket?.name ?? '-'}
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 200,
    field: 'transferToExchangeMarket',
    headerName: 'TO ACCOUNT',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.transferToExchangeMarket?.name ?? '-'}
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
    field: 'amount',
    headerName: 'AMOUNT',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.amount ? (
          <NumericFormat
            value={+params.row.amount}
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
    field: 'otpConfirmedDateTime',
    headerName: 'PAYMENT RECEIVED',
    sortable: false,
    valueGetter: params => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => {
      return renderOtpConfirmedDateTime(params)
    },
  },
]
