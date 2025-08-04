import { Typography, Box } from '@mui/material'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import { styled } from '@mui/material/styles'
import Table from '@mui/material/Table'
import TableBody from '@mui/material/TableBody'
import TableCell, { tableCellClasses } from '@mui/material/TableCell'
import TableContainer from '@mui/material/TableContainer'
import TableHead from '@mui/material/TableHead'
import TableRow from '@mui/material/TableRow'
import Paper from '@mui/material/Paper'
import { CalculationResult } from 'src/types/blocktrade/calculator/result'
import { DecimalNumber } from 'src/utils/blocktrade/decimal'
import { colorTextPnL } from 'src/utils/blocktrade/color'

interface ProjectionProps {
  calculationResult: CalculationResult | null
}

const StyledTableCell = styled(TableCell)(() => ({
  [`&.${tableCellClasses.body}`]: {
    fontSize: 14,
    fontWeight: 600,
  },
}))

const StyledTableRow = styled(TableRow)(({ theme }) => ({
  '&:nth-of-type(odd)': {
    backgroundColor: theme.palette.action.hover,
  },
  '&:last-child td, &:last-child th': {
    border: 0,
  },
}))

const Projection = ({ calculationResult }: ProjectionProps) => {
  const upperRows = []
  const middleRows = []
  const lowerRows = []
  if (calculationResult) {
    for (let i = 1; i <= 7; i++) {
      const dataObj = calculationResult.projUpper.data[i.toString()]
      upperRows.push({
        id: dataObj.id,
        closeStockPrice: DecimalNumber(dataObj.closeStockPrice, 2),
        openSSFPrice: DecimalNumber(dataObj.openSsfPrice, 4),
        closeSSFPrice: DecimalNumber(dataObj.closeSsfPrice, 4),
        pnL: dataObj.pnl,
      })
    }
    for (let i = 1; i <= 1; i++) {
      const dataObj = calculationResult.projMiddle.data[i.toString()]
      middleRows.push({
        id: dataObj.id,
        closeStockPrice: DecimalNumber(dataObj.closeStockPrice, 2),
        openSSFPrice: DecimalNumber(dataObj.openSsfPrice, 4),
        closeSSFPrice: DecimalNumber(dataObj.closeSsfPrice, 4),
        pnL: DecimalNumber(dataObj.pnl, 2),
      })
    }
    for (let i = 1; i <= 7; i++) {
      const dataObj = calculationResult.projLower.data[i.toString()]
      lowerRows.push({
        id: dataObj.id,
        closeStockPrice: DecimalNumber(dataObj.closeStockPrice, 2),
        openSSFPrice: DecimalNumber(dataObj.openSsfPrice, 4),
        closeSSFPrice: DecimalNumber(dataObj.closeSsfPrice, 4),
        pnL: dataObj.pnl,
      })
    }
  }

  return (
    <Box>
      <Card sx={{ textAlign: 'center', marginLeft: 0, marginTop: { xs: 3, lg: 0 }, height: '100%' }}>
        <CardHeader
          sx={{ backgroundColor: 'primary.main', paddingY: 2 }}
          title={
            <Typography variant='h6' sx={{ color: 'white', fontWeight: 700 }}>
              Projection PnL
            </Typography>
          }
        />
        <CardContent sx={{ padding: 0, marginBottom: -5 }}>
          <TableContainer component={Paper}>
            <Table size='small'>
              <TableHead>
                <TableRow>
                  <StyledTableCell align='center'>C.Stk Prc</StyledTableCell>
                  <StyledTableCell align='center'>O.SSF Prc</StyledTableCell>
                  <StyledTableCell align='center'>C.SSF Prc</StyledTableCell>
                  <StyledTableCell align='right'>Net PnL</StyledTableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {upperRows.map(row => (
                  <StyledTableRow key={row.id}>
                    <StyledTableCell align='center'>{row.closeStockPrice}</StyledTableCell>
                    <StyledTableCell align='center'>{row.openSSFPrice}</StyledTableCell>
                    <StyledTableCell align='center'>{row.closeSSFPrice}</StyledTableCell>
                    <StyledTableCell align='right'>{colorTextPnL(row.pnL)}</StyledTableCell>
                  </StyledTableRow>
                ))}
                {middleRows.map(row => (
                  <StyledTableRow
                    key={row.id}
                    sx={{ backgroundColor: 'primary.main', color: 'white', marginTop: 0, paddingTop: 0 }}
                  >
                    <StyledTableCell align='center' sx={{ color: 'white' }}>
                      {row.closeStockPrice}
                    </StyledTableCell>
                    <StyledTableCell align='center' sx={{ color: 'white' }}>
                      {row.openSSFPrice}
                    </StyledTableCell>
                    <StyledTableCell align='center' sx={{ color: 'white' }}>
                      {row.closeSSFPrice}
                    </StyledTableCell>
                    <StyledTableCell align='right' sx={{ color: 'white' }}>
                      {row.pnL}
                    </StyledTableCell>
                  </StyledTableRow>
                ))}
                {lowerRows.map(row => (
                  <StyledTableRow key={row.id}>
                    <StyledTableCell align='center'>{row.closeStockPrice}</StyledTableCell>
                    <StyledTableCell align='center'>{row.openSSFPrice}</StyledTableCell>
                    <StyledTableCell align='center'>{row.closeSSFPrice}</StyledTableCell>
                    <StyledTableCell align='right'>{colorTextPnL(row.pnL)}</StyledTableCell>
                  </StyledTableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </CardContent>
      </Card>
    </Box>
  )
}

export default Projection
