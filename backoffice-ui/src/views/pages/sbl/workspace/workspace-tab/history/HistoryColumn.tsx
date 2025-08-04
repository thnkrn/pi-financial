import { renderStatus } from '@/views/components/DataTableUtil'
import Typography from '@mui/material/Typography'
import { gridDateComparator, GridRenderCellParams, gridStringOrNumberComparator } from '@mui/x-data-grid'
import dayjs from 'dayjs'

export const getHistoryColumns = () => [
  {
    flex: 0.1,
    minWidth: 100,
    headerName: 'ORDER NO',
    field: 'orderId',
    sortComparator: gridStringOrNumberComparator,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.orderId}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 100,
    headerName: 'ACCOUNT',
    field: 'tradingAccountNo',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.tradingAccountNo}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 100,
    headerName: 'TYPE',
    field: 'type',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.type}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 100,
    headerName: 'SYMBOL',
    field: 'symbol',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.symbol}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 100,
    headerName: 'Volume',
    field: 'volume',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.volume}
      </Typography>
    ),
  },
  {
    flex: 0.15,
    type: 'date',
    minWidth: 150,
    headerName: 'ORDER CREATED',
    field: 'createdAt',
    sortComparator: gridDateComparator,
    valueGetter: (params: { value: string | number | Date }) => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {dayjs(params.row.createdAt).format('DD/MM/YYYY HH:mm:ss')}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 100,
    headerName: 'STATUS',
    field: 'orderStatus',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => renderStatus(params.row.orderStatus),
  },
  {
    flex: 0.2,
    minWidth: 200,
    headerName: 'REASON',
    field: 'rejectedReason',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.rejectedReason}
      </Typography>
    ),
  },
]
