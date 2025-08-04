// ** MUI Imports
import Typography from '@mui/material/Typography'
import { Box } from '@mui/material'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'

// ** Custom Components Imports
import { colorTextLS } from 'src/utils/blocktrade/color'
import { DecimalNumber } from 'src/utils/blocktrade/decimal'

export const columns: GridColDef[] = [
  {
    flex: 0.175,
    minWidth: 90,
    field: 'salesId',
    headerName: 'Sales ID',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.blockOrders.sales.employeeId}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'orderId',
    headerName: 'Order ID',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.blockId}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 50,
    field: 'openClose',
    headerName: 'O/C',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.blockOrders.openClose}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 50,
    field: 'side',
    headerName: 'Side',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {colorTextLS(params.row.blockOrders.clientSide)}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 100,
    field: 'symbol',
    headerName: 'Symbol',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.blockOrders.symbol.symbol + (params.row.blockOrders.series?.series || '')}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'numOfContract',
    headerName: '#Contract',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {DecimalNumber(params.row.blockOrders.numOfContract, 0)}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 100,
    field: 'numOfShare',
    headerName: '#Share',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {DecimalNumber(params.row.numOfShare, 0)}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 100,
    field: 'numOfShareFilled',
    headerName: '#Filled',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {DecimalNumber(params.row.numOfShareFilled, 0)}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 110,
    field: 'orderPrice',
    headerName: 'Order Price',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {DecimalNumber(params.row.orderPrice, 2)}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'execPrice',
    headerName: 'Exc.Price',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {params.row.executedPrice ? DecimalNumber(params.row.executedPrice, 2) : 'N/A'}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 100,
    field: 'cusAcc',
    headerName: 'Cus Acc',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.blockOrders.customerAccount}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'im',
    headerName: 'IM',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
        {DecimalNumber(params.row.im, 0)}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'status',
    headerName: 'Status',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.status}
      </Typography>
    ),
  },
]
