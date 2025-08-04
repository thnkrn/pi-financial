import { IGetTransferCashTransactionResponse } from '@/lib/api/clients/backoffice/transactions/types'
import HistoryAction from '@/views/components/transaction/HistoryAction'
import TransferCashAccountInfo from '@/views/components/transaction/TransferCashAccountInfo'
import TransferCashCustomerProfile from '@/views/components/transaction/TransferCashCustomerProfile'
import TransferCashStatusAction from '@/views/components/transaction/TransferCashStatusAction'
import TransferCashTransactionDetail from '@/views/components/transaction/TransferCashTransactionDetail'
import Divider from '@mui/material/Divider'
import Grid from '@mui/material/Grid'

interface Props {
  transaction: IGetTransferCashTransactionResponse
}

const TransactionTable = ({ transaction }: Props) => {
  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Divider sx={{ m: '0 !important' }} />
        <TransferCashStatusAction statusAction={transaction?.statusAction ?? ''} />
        <TransferCashCustomerProfile customerProfile={transaction?.customerProfile} />
        <TransferCashAccountInfo accountInfo={transaction?.accountInfo} />
        <TransferCashTransactionDetail transactionDetail={transaction?.transactionDetail} />
        <HistoryAction makerTickets={transaction?.makerType} checkerTickets={transaction?.checkerType} />
      </Grid>
    </Grid>
  )
}

export default TransactionTable
