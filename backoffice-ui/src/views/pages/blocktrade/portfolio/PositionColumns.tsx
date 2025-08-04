// ** React Imports
import { useRouter } from 'next/router'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import toast from 'react-hot-toast'

// ** MUI Imports
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import IconButton from '@mui/material/IconButton'
import CalculateIcon from '@mui/icons-material/Calculate'

// ** Custom Components Imports
import { colorTextLS, colorTextPnL } from 'src/utils/blocktrade/color'
import { DecimalNumber } from 'src/utils/blocktrade/decimal'
import { DateTimeToDate } from '@/utils/blocktrade/date'
import { PositionRowType } from '@/views/pages/blocktrade/portfolio/types'
import { updateData } from '@/store/apps/blocktrade/calculator'
import { OC, UserRoles } from '@/constants/blocktrade/GlobalEnums'
import RolloverModal from '@/views/pages/blocktrade/portfolio/Rollover'

const Action = ({ row }: { row: PositionRowType }) => {
  const router = useRouter()
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const userStore = useSelector((state: any) => state.btUser)
  const store = useSelector((state: any) => state.btPosition)

  const handleCalculate = async (data: PositionRowType) => {
    const { symbol, series, futuresOrders, futuresClose } = data
    if (symbol.symbol && series.series) {
      try {
        dispatch(
          updateData({
            id: data.id,
            oc: OC.OPEN,
            side: data.clientSide,
            symbol: symbol.symbol + series.series,
            contractAmount: data.numOfContract,
            openPrice: futuresOrders.equityPrice,
            closePrice: DecimalNumber(futuresClose?.mktPrice ?? 0, 2),
            xd: DecimalNumber(futuresClose?.xd ?? 0, 2),
            intRate: DecimalNumber(data?.interestRate ?? 0, 2),
            minDay: data.minDay,
            openDate: data.createdAt ? new Date(data.createdAt) : new Date(),
            closeDate: new Date(),
            commFee: 0.1,
            futuresProperty: null,
          })
        )
        await router.push('/blocktrade/calculator')
      } catch (error) {
        const message = 'Calculate was error'
        toast.error(message)

        return { success: false, message }
      }
    }
  }

  return (
    <>
      <IconButton
        sx={{ mx: -1 }}
        onClick={() => {
          handleCalculate(row)
        }}
      >
        <CalculateIcon />
      </IconButton>
      {userStore.data.role === UserRoles.ADMIN && store.rollover && (
        <RolloverModal row={row} mktPrice={row.futuresClose?.mktPrice ?? 0} />
      )}
    </>
  )
}

export const columns: GridColDef[] = [
  {
    flex: 0.175,
    minWidth: 50,
    field: 'clientSide',
    headerName: 'Side',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {colorTextLS(params.row.clientSide)}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'symbolLabel',
    headerName: 'Symbol',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.symbol.symbol + params.row.series.series}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 100,
    field: 'numOfContract',
    headerName: 'Begin Qty',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {DecimalNumber(params.row.numOfContract, 0)}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 100,
    field: 'availContract',
    headerName: 'Avail Qty',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {DecimalNumber(params.row.availContract, 0)}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'equityPrice',
    headerName: 'Open Price',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align={'right'} sx={{ color: 'text.primary' }}>
          {DecimalNumber(params.row.futuresOrders.equityPrice, 4)}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'equityMKTPrice',
    headerName: 'MKT Price',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align={'right'} sx={{ color: 'text.primary' }}>
          {params.row.futuresClose?.mktPrice || null ? DecimalNumber(params.row.futuresClose.mktPrice, 2) : 'N/A'}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'xd',
    headerName: 'XD',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {params.row.futuresClose?.xd || null ? DecimalNumber(params.row.futuresClose.xd, 2) : 0}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'futureProjPrice',
    headerName: 'Futures Proj Price',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='right' sx={{ color: 'text.primary' }}>
          {params.row.futuresClose?.mktPrice || null ? DecimalNumber(params.row.futuresClose.projFutures, 4) : 'N/A'}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'unrealizedPL',
    headerName: 'Unrealized P/L',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align={'right'} sx={{ color: 'text.primary', fontWeight: 600 }}>
          {params.row.futuresClose?.mktPrice || null
            ? colorTextPnL(params.row.futuresClose.unrealizedPnlPerCont)
            : 'N/A'}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'customerAccount',
    headerName: 'Cus Acc',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.customerAccount}
      </Typography>
    ),
  },
  {
    flex: 0.175,
    minWidth: 80,
    field: 'employeeId',
    headerName: 'Sales ID',
    renderCell: (params: GridRenderCellParams) => (
      <Box sx={{ width: '100%' }}>
        <Typography variant='body2' align='center' sx={{ color: 'text.primary' }}>
          {params.row.sales?.employeeId || ''}
        </Typography>
      </Box>
    ),
  },
  {
    flex: 0.175,
    minWidth: 120,
    field: 'createdAt',
    headerName: 'Open Date',
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {DateTimeToDate(params.row.createdAt)}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 80,
    field: 'action',
    headerName: 'Action',
    renderCell: (params: GridRenderCellParams) => <Action row={params.row} />,
  },
]
