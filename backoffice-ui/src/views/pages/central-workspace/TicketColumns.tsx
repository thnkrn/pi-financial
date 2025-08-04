import { Icon } from '@iconify/react'
import IconButton from '@mui/material/IconButton'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams, gridDateComparator } from '@mui/x-data-grid'
import dayjs from 'dayjs'
import Link from 'next/link'

export const columns: GridColDef[] = [
  {
    field: 'col1',
    headerName: '',
    width: 50,
    sortable: false,
    renderCell: (params: GridRenderCellParams) => {
      const href =
        params?.row?.transactionType === 'transferCash'
          ? `transfer-cash/${params.row.transactionNo}`
          : `transaction/${params.row.transactionNo}`

      return (
        <Typography variant='body2' sx={{ color: 'text.primary' }}>
          <Tooltip title='View'>
            <IconButton
              size='small'
              component={Link}
              target='_blank'
              href={href}
              sx={{ mr: 0.5 }}
              disabled={!params?.row?.transactionNo} // Optional: disable the button if no href
            >
              <Icon icon='mdi:eye-outline' />
            </IconButton>
          </Tooltip>
        </Typography>
      )
    },
  },
  {
    flex: 0.175,
    minWidth: 120,
    headerName: 'TICKET ID',
    field: 'ticketNo',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.ticketNo}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    headerName: 'TRANSACTION ID',
    field: 'transactionNo',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.transactionNo}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 180,
    type: 'date',
    headerName: 'CREATE DATE',
    field: 'createdAt',
    sortComparator: gridDateComparator,
    valueGetter: params => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {dayjs(params.row.createdAt).format('DD/MM/YYYY')}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 150,
    headerName: 'MAKER NAME',
    field: 'makerName',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        <Tooltip title={`${params.row.maker?.firstName} ${params.row.maker?.lastName}`} placement='bottom-end'>
          <span>{params.row.maker?.email.split('@')[0]}</span>
        </Tooltip>
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 150,
    headerName: 'RESPONSE CODE',
    field: 'responseCode',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.responseCode?.description}
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 150,
    field: 'customerCode',
    headerName: 'CUSTOMER CODE',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.customerCode}
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 200,
    field: 'customerName',
    headerName: 'CUSTOMER ACCOUNT NAME',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.customerName}
      </Typography>
    ),
  },
]
