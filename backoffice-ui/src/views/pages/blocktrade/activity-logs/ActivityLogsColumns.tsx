// ** MUI Imports
import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'

// ** Custom Components Imports
import { FormatDateTime } from '@/utils/blocktrade/date'

export const columns: GridColDef[] = [
  {
    flex: 0.15,
    minWidth: 50,
    field: 'dateTime',
    headerName: 'Date/Time',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {FormatDateTime(params.row.createdAt)}
      </Typography>
    ),
  },
  {
    flex: 0.15,
    minWidth: 150,
    field: 'action',
    headerName: 'Action',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.action.replace(/_/g, ' ')}
      </Typography>
    ),
  },
  {
    flex: 0.63,
    minWidth: 100,
    field: 'details',
    headerName: 'Details',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.detail}
      </Typography>
    ),
  },
  {
    flex: 0.07,
    minWidth: 50,
    field: 'salesId',
    headerName: 'Sales ID',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.saleId}
      </Typography>
    ),
  },
]
