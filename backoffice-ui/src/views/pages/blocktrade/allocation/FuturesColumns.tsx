// ** React Imports
import { useDispatch, useSelector } from 'react-redux'

// ** MUI Imports
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'
import Checkbox from '@mui/material/Checkbox'
import IconButton from '@mui/material/IconButton'
import UndoIcon from '@mui/icons-material/Undo'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'

// ** Custom Components Imports
import { colorTextLS } from 'src/utils/blocktrade/color'
import { fetchFuturesOrder, updateFilterState, updateLastUpdated } from 'src/store/apps/blocktrade/futures'
import { DecimalNumber } from 'src/utils/blocktrade/decimal'
import { FuturesOrderFilterInitialState } from 'src/constants/blocktrade/InitialState'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { Side, UserRoles } from 'src/constants/blocktrade/GlobalEnums'
import { IFuturesOrder } from '@/types/blocktrade/futures/types'
import { handleSubmitFutures } from 'src/hooks/blocktrade/useSubmitFutures'
import { handleUndo } from '@/hooks/blocktrade/useUndoFutures'

const ActionSubmit = ({ row, isIC }: { row: IFuturesOrder; isIC: boolean }) => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const userStore = useSelector((state: any) => state.btUser)

  return (
    <Typography variant='body2' sx={{ color: 'text.primary' }}>
      <Checkbox
        checked={isIC ? row.saleSubmitted : row.dealerSubmitted}
        onClick={async () => {
          const result = await handleSubmitFutures(dispatch, row.id, isIC, userStore.data.role === UserRoles.ADMIN)
          if (result.success) {
            dispatch(fetchFuturesOrder(FuturesOrderFilterInitialState))
          }
        }}
        disabled={!isIC && userStore.data.role !== UserRoles.ADMIN}
      />
    </Typography>
  )
}

const UndoCell = ({ row }: { row: IFuturesOrder }) => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const userStore = useSelector((state: any) => state.btUser)

  if (userStore.data.role === UserRoles.ADMIN) {
    return (
      <IconButton
        onClick={async () => {
          const result = await handleUndo(dispatch, row)
          if (result.success) {
            dispatch(updateFilterState(FuturesOrderFilterInitialState))
            dispatch(fetchFuturesOrder(FuturesOrderFilterInitialState))
            dispatch(updateLastUpdated(new Date().toLocaleString()))
          }
        }}
      >
        <UndoIcon />
      </IconButton>
    )
  }

  return <></>
}

export const columns: GridColDef[] = [
  {
    flex: 0.175,
    minWidth: 80,
    field: 'dealerSubmitted',
    headerName: 'Dealer',
    renderCell: (params: GridRenderCellParams) => <ActionSubmit row={params.row} isIC={false} />,
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'salesSubmitted',
    headerName: 'Sales',
    renderCell: (params: GridRenderCellParams) => <ActionSubmit row={params.row} isIC={true} />,
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'orderId',
    headerName: 'Order ID',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='center' sx={{ color: 'text.primary' }}>
          {params.row.blockId}
        </Typography>
      </Box>
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
    field: 'piSide',
    headerName: 'Pi Side',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {colorTextLS(params.row.blockOrders.clientSide === Side.LONG ? Side.SHORT : Side.LONG)}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 50,
    field: 'clientSide',
    headerName: 'Client Side',
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
        {params.row.blockOrders.symbol.symbol + params.row.blockOrders.series.series}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'qty',
    headerName: 'Qty',
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
    minWidth: 120,
    field: 'futuresPrice',
    headerName: 'Futures Price',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary', fontWeight: 600 }}>
          {DecimalNumber(params.row.futuresPrice, 4)}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'stockPrice',
    headerName: 'Stock Price',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {DecimalNumber(params.row.equityPrice, 2)}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 60,
    field: 'xd',
    headerName: 'XD',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {DecimalNumber(params.row.xd, 2)}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'customerAccount',
    headerName: 'Cust Acc',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {params.row.blockOrders.customerAccount || 'N/A'}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'salesId',
    headerName: 'Sales ID',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='center' sx={{ color: 'text.primary' }}>
          {params.row.salesId}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'status',
    headerName: 'Status',
    renderCell: (params: GridRenderCellParams) => {
      const status =
        params.row.blockOrders.futuresSettrade.length > 0 ? params.row.blockOrders.futuresSettrade[0].status : 'N/A'

      return (
        <Box sx={{ width: '100%' }}>
          <Typography variant='body2' align='center' sx={{ color: 'text.primary' }}>
            {status}
          </Typography>
        </Box>
      )
    },
  },
  {
    flex: 0.1,
    minWidth: 60,
    field: '',
    headerName: '',
    sortable: false,
    filterable: false,
    disableColumnMenu: true,
    renderCell: params => <UndoCell row={params.row} />,
  },
]
