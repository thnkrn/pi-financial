import RestartAltIcon from '@mui/icons-material/RestartAlt'
import { IconButton, Tooltip } from '@mui/material'
import { GridColDef } from '@mui/x-data-grid'
import { ISendLinkAccountInfo } from './types'

const formatDate = (value: string) => {
  const date = new Date(value)
  if (isNaN(date.getTime())) return 'Invalid date'

  const pad = (n: number) => n.toString().padStart(2, '0')

  return `${pad(date.getUTCDate())}/${pad(date.getUTCMonth() + 1)}/${date.getUTCFullYear()} ${pad(
    date.getUTCHours()
  )}:${pad(date.getUTCMinutes())}:${pad(date.getUTCSeconds())}`
}

export const buildColumns = (onReset: (row: ISendLinkAccountInfo) => void): GridColDef[] => [
  { field: 'email', headerName: 'Email', flex: 1 },
  { field: 'custcode', headerName: 'Custcode', flex: 1 },
  {
    field: 'createdAt',
    headerName: 'Created At',
    flex: 1,
    valueFormatter: params => formatDate(params.value),
  },
  {
    field: 'isUsed',
    headerName: 'Used',
    flex: 1,
    valueGetter: params => (params.row.isUsed ? `Yes (${formatDate(params.row.usedAt)})` : 'No'),
  },
  {
    field: 'action',
    headerName: 'Action',
    flex: 1,
    sortable: false,
    filterable: false,
    renderCell: params => (
      <Tooltip title='Resend Link Email'>
        <IconButton onClick={() => onReset(params.row)}>
          <RestartAltIcon />
        </IconButton>
      </Tooltip>
    ),
  },
]
