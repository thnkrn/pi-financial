import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'
import { formatCurrency } from '@/utils/fmt'
import { Paper, Table, TableCell, TableRow, Typography } from '@mui/material'
import Decimal from 'decimal.js'

const AmountMismatchAlertTable = ({
  currency,
  receivedAmount,
  requestedAmount,
}: {
  currency: string
  receivedAmount: Decimal
  requestedAmount: Decimal
}) => {
  const difference = Math.abs(Number(receivedAmount) - Number(requestedAmount))

  return (
    <Paper sx={{ maxWidth: 900, margin: '0 auto' }}>
      <Table
        sx={{
          border: '1px solid rgba(224, 224, 224, 1)',
          tableLayout: 'auto',
        }}
        aria-label='alert table'
      >
        <TableRow>
          <TableCell variant='head' sx={{ fontWeight: 'bold', borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            QR GENERATED AMOUNT
          </TableCell>
          <TableCell sx={{ borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            <Typography variant='subtitle1'>
              {convertCurrencyPrefixToCurrencySymbol(currency)} {formatCurrency(requestedAmount)}
            </Typography>
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell variant='head' sx={{ fontWeight: 'bold', borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            PAYMENT RECEIVED AMOUNT
          </TableCell>
          <TableCell sx={{ borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            <Typography variant='subtitle1'>
              {convertCurrencyPrefixToCurrencySymbol(currency)} {formatCurrency(receivedAmount)}
            </Typography>
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell variant='head' sx={{ fontWeight: 'bold', borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            DIFFERENCE
          </TableCell>
          <TableCell sx={{ borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            {difference < 0 ? (
              <Typography variant='subtitle1' color='red'>
                {convertCurrencyPrefixToCurrencySymbol(currency)} -{formatCurrency(difference)}
              </Typography>
            ) : (
              <Typography variant='subtitle1' color='green'>
                {convertCurrencyPrefixToCurrencySymbol(currency)} +{formatCurrency(difference)}
              </Typography>
            )}
          </TableCell>
        </TableRow>
      </Table>
    </Paper>
  )
}

export default AmountMismatchAlertTable
