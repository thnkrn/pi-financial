import { Visible } from '@/@core/components/auth/Visible'
import { ACTION_TYPE } from '@/types/sbl/sblTypes'
import { Icon } from '@iconify/react'
import IconButton from '@mui/material/IconButton'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import { gridDateComparator, GridRenderCellParams, gridStringOrNumberComparator } from '@mui/x-data-grid'
import dayjs from 'dayjs'

export const getPendingColumns = (handleOpen: (orderId: string, orderNo: string, type: ACTION_TYPE) => void) => [
  {
    field: 'approve',
    headerName: '',
    width: 60,
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Visible allowedRoles={['sbl-approve']}>
        <Typography variant='body2' sx={{ color: 'text.primary' }}>
          <Tooltip title='Approve'>
            <span>
              <IconButton
                size='medium'
                sx={{ mr: 0.5 }}
                onClick={() => {
                  handleOpen(params.row.id, params.row.orderId, ACTION_TYPE.APPROVED)
                }}
              >
                <Icon icon='mdi:check-circle' color='green' />
              </IconButton>
            </span>
          </Tooltip>
        </Typography>
      </Visible>
    ),
  },
  {
    field: 'reject',
    headerName: '',
    width: 60,
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Visible allowedRoles={['sbl-approve']}>
        <Typography variant='body2' sx={{ color: 'text.primary' }}>
          <Tooltip title='Reject'>
            <span>
              <IconButton
                size='medium'
                sx={{ mr: 0.5 }}
                onClick={() => {
                  handleOpen(params.row.id, params.row.orderId, ACTION_TYPE.REJECTED)
                }}
              >
                <Icon icon='mdi:cross-circle' color='red' />
              </IconButton>
            </span>
          </Tooltip>
        </Typography>
      </Visible>
    ),
  },
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
]
