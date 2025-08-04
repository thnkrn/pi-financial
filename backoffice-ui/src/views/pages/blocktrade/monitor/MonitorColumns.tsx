// ** React Imports
import { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import toast from 'react-hot-toast'

// ** MUI Imports
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import Checkbox from '@mui/material/Checkbox'
import UndoIcon from '@mui/icons-material/Undo'
import IconButton from '@mui/material/IconButton'
import CloseIcon from '@mui/icons-material/Close'

// ** Custom Components Imports
import { colorTextLS } from '@/utils/blocktrade/color'
import { IEquityOrder } from '@/types/blocktrade/orders/types'
import { DecimalNumber } from '@/utils/blocktrade/decimal'
import { addOrder, removeOrder, resetState, updateData } from '@/store/apps/blocktrade/monitor'
import { OrderStatus, OrderType } from '@/constants/blocktrade/GlobalEnums'
import { AppDispatch } from '@/store/index'
import Swal from 'sweetalert2'
import { EquityOrderFilterInitialState } from '@/constants/blocktrade/InitialState'
import { fetchEquityOrder, updateFilterState, updateLastUpdated } from '@/store/apps/blocktrade/equity'
import { orderIdComparator } from '@/utils/blocktrade/orderId'
import { MonitorDataType } from '@/types/blocktrade/monitor/types'
import { handleCancelOrder } from '@/hooks/blocktrade/useCancelOrder'
import FlagIC from '@/views/pages/blocktrade/monitor/FlagIC'
import { splitUndo } from '@/lib/api/clients/blocktrade/monitor'

interface Props {
  row: IEquityOrder
}

const ActionsCell = ({ row }: Props) => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const store = useSelector((state: any) => state.btMonitor)

  const [check, setCheck] = useState<boolean>(false)

  const updatingData = {
    blockId: row.blockId,
    pos: row.blockOrders.openClose,
    side: row.blockOrders.clientSide,
    symbol: row.blockOrders.symbol.symbol,
  }

  const orderData = {
    id: row.id,
    orderId: row.orderId,
    blockId: row.blockId,
    pos: row.blockOrders.openClose,
    side: row.blockOrders.clientSide,
    symbol: row.blockOrders.symbol.symbol,
    qty: row.numOfShare,
    qtyFilled: row.numOfShareFilled,
    executedPrice: row.executedPrice,
    mainId: row.mainId,
  }

  const checkDisabled = () => {
    if (
      row.blockOrders.openClose === store.pos &&
      row.blockOrders.clientSide === store.side &&
      row.blockOrders.symbol.symbol === store.symbol &&
      row.status === OrderStatus.S_FILLED
    ) {
      return false
    } else if (!store.pos && !store.side && !store.symbol) {
      return row.status !== OrderStatus.S_FILLED
    }

    return true
  }

  const isCheckedBox = () => {
    return store.data.filter((item: MonitorDataType) => item.id === row.id).length > 0
  }

  return (
    <Box display='flex' justifyContent='center' alignItems='center' sx={{ width: '100%' }}>
      <Checkbox
        checked={isCheckedBox()}
        onClick={async () => {
          if (store.data.length === 0) {
            dispatch(updateData(updatingData))
          } else if (store.data.length === 1 && isCheckedBox()) {
            dispatch(resetState())
          }
          if (!isCheckedBox()) {
            dispatch(addOrder(orderData))
          } else {
            dispatch(removeOrder(orderData))
          }
          setCheck(!check)
        }}
        disabled={checkDisabled()}
      />
    </Box>
  )
}

const CancelCell = ({ row }: Props) => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  if (row.status === OrderStatus.S_FILLED || row.status === OrderStatus.WORKING) {
    return (
      <IconButton
        onClick={async () => {
          const result = await handleCancelOrder(dispatch, row)
          if (result.success) {
            dispatch(resetState())
            dispatch(updateFilterState(EquityOrderFilterInitialState))
            dispatch(fetchEquityOrder(EquityOrderFilterInitialState))
            dispatch(updateLastUpdated(new Date().toLocaleString()))
          }
        }}
      >
        <CloseIcon />
      </IconButton>
    )
  }

  return null
}

const handleUndoClick = async (dispatch: AppDispatch, row: IEquityOrder) => {
  const result = await handleUndo(dispatch, row)

  if (result.success) {
    dispatch(resetState())
    dispatch(updateFilterState(EquityOrderFilterInitialState))
    dispatch(fetchEquityOrder(EquityOrderFilterInitialState))
    dispatch(updateLastUpdated(new Date().toLocaleString()))
  }
}

const UndoCell = ({ row }: Props) => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  if (row.mainId && row.status === OrderStatus.S_FILLED && row.canUndo) {
    return (
      <IconButton onClick={() => handleUndoClick(dispatch, row)}>
        <UndoIcon />
      </IconButton>
    )
  }

  return null
}

const handleUndo = async (dispatch: AppDispatch, row: IEquityOrder) => {
  try {
    const result = await Swal.fire({
      title: 'Are you sure?',
      text: 'Do you confirm to UNDO the split order?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, undo it!',
    })

    if (result.isConfirmed && row.mainId) {
      const data = {
        mainId: Number(row.mainId),
      }
      const response = await splitUndo(data)

      if (response) {
        const message = 'The undo of the split order was successful.'
        toast.success(message)

        return { success: true, message }
      } else {
        const message = 'The undo of the split order was failed.'
        toast.error(message)

        return { success: false, message }
      }
    } else {
      const message = 'Canceled undo the split order'
      toast.error(message)

      return { success: false, message }
    }
  } catch (error) {
    const message = 'Undo was error'
    toast.error(message)

    return { success: false, message }
  }
}

export const columns: GridColDef[] = [
  {
    flex: 0.1,
    minWidth: 30,
    field: '',
    headerName: '',
    sortable: false,
    filterable: false,
    disableColumnMenu: true,
    renderCell: params => <ActionsCell row={params.row} />,
  },
  {
    flex: 0.175,
    minWidth: 90,
    field: 'employeeId',
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
    headerName: 'Ref ID',
    sortComparator: orderIdComparator,
    renderCell: (params: GridRenderCellParams) => {
      const orderId = params.row.orderId ?? 'N/A'

      //NOTE: If it is JPM Orders, we shorten the reference order id for display
      //NOTE: JPM Order ID is Y97K4NVFTFVP and we only display Y..FTFVP, while IFis Order ID is 12345 and we display 12345
      const displayOrderId = orderId.length > 7 ? orderId.slice(0, 1) + '..' + orderId.slice(-5) : orderId

      return (
        <Typography variant='body2' sx={{ color: 'text.primary' }}>
          {displayOrderId}
        </Typography>
      )
    },
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'blockId',
    headerName: 'Block ID',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.blockId}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 60,
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
    minWidth: 70,
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
    minWidth: 80,
    field: 'symbol',
    headerName: 'Symbol',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.blockOrders.symbol.symbol + (params.row.blockOrders.series?.series ?? '')}
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
          {params.row.blockOrders.numOfContract ? DecimalNumber(params.row.blockOrders.numOfContract, 0) : 'N/A'}
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
          {params.row.orderType === OrderType.LIMIT ? DecimalNumber(params.row.orderPrice, 2) : params.row.orderType}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'executedPrice',
    headerName: 'Exc.Price',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {params.row.executedPrice ? DecimalNumber(params.row.executedPrice, 4) : 'N/A'}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 100,
    field: 'customerAccount',
    headerName: 'Cus Acc',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.blockOrders.customerAccount}
      </Typography>
    ),
  },
  {
    flex: 0.125,
    minWidth: 100,
    field: 'status',
    headerName: 'Status',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.status}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'icName',
    headerName: 'IC Name',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.icName}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 100,
    field: 'undo',
    headerName: '',
    sortable: false,
    filterable: false,
    disableColumnMenu: true,
    renderCell: params => (
      <>
        <FlagIC row={params.row} />
        <UndoCell row={params.row} />
        <CancelCell row={params.row} />
      </>
    ),
  },
]
