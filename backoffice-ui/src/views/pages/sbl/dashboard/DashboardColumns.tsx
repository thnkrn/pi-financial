import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams, gridStringOrNumberComparator } from '@mui/x-data-grid'

export const columns: GridColDef[] = [
  {
    flex: 0.075,
    minWidth: 100,
    headerName: 'SYMBOL',
    field: 'symbol',
    sortComparator: gridStringOrNumberComparator,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row?.symbol}
      </Typography>
    ),
  },
  {
    flex: 0.2,
    minWidth: 200,
    headerName: 'AVAILABLE FOR LENDING',
    field: 'availableLending',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row?.availableLending?.toLocaleString()}
      </Typography>
    ),
  },
  {
    flex: 0.15,
    minWidth: 150,
    headerName: 'RETAIL LENDER',
    field: 'retailLender',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row?.retailLender?.toLocaleString()}
      </Typography>
    ),
  },
  {
    flex: 0.2,
    minWidth: 200,
    headerName: 'BORROW OUTSTANDING',
    field: 'borrowOutstanding',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row?.borrowOutstanding?.toLocaleString()}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 100,
    headerName: 'RATE',
    field: 'interestRate',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row?.interestRate?.toLocaleString()}
      </Typography>
    ),
  },
]
