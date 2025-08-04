import { IGetTransactionResponse } from '@/lib/api/clients/backoffice/transactions/types'
import AtsDepositDetail from '@/views/components/transaction/AtsDeposit'
import AtsWithdrawDetail from '@/views/components/transaction/AtsWithdraw'
import CustomerProfile from '@/views/components/transaction/CustomerProfile'
import GlobalTransferDetail from '@/views/components/transaction/GlobalTransfer'
import HistoryAction from '@/views/components/transaction/HistoryAction'
import OddDepositDetail from '@/views/components/transaction/OddDeposit'
import OddWithdrawDetail from '@/views/components/transaction/OddWithdraw'
import ProductDetailChannel from '@/views/components/transaction/ProductDetailChannel'
import QrDepositDetail from '@/views/components/transaction/QrDeposit'
import BillPaymentDepositDetail from '@/views/components/transaction/BillPaymentDeposit'
import RecoveryDetail from '@/views/components/transaction/Recovery'
import RefundDetail from '@/views/components/transaction/Refund'
import TransactionDetail from '@/views/components/transaction/TransactionDetail'
import TransactionStatusAction from '@/views/components/transaction/TransactionStatusAction'
import UpBackDetail from '@/views/components/transaction/UpBack'
import Divider from '@mui/material/Divider'
import Grid from '@mui/material/Grid'

interface Props {
  transaction: IGetTransactionResponse
}

const TransactionTable = ({ transaction }: Props) => {
  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Divider sx={{ m: '0 !important' }} />
        <TransactionStatusAction
          currency={transaction.transactionDetail?.requestedCurrency ?? ''}
          statusAction={transaction?.statusAction ?? ''}
          customerAccountName={transaction?.customerProfile?.customerAccountName ?? ''}
          customerBankAccountName={transaction?.transactionDetail?.senderBankAccountName ?? ''}
        />
        <CustomerProfile customerProfile={transaction?.customerProfile} />
        <ProductDetailChannel productDetailChannel={transaction?.productDetail} />
        <TransactionDetail transactionDetail={transaction?.transactionDetail} />
        {transaction?.qrDeposit && <QrDepositDetail data={transaction?.qrDeposit} />}
        {transaction?.oddDeposit && <OddDepositDetail data={transaction?.oddDeposit} />}
        {transaction?.atsDeposit && <AtsDepositDetail data={transaction?.atsDeposit} />}
        {transaction?.billPaymentDeposit && <BillPaymentDepositDetail data={transaction?.billPaymentDeposit} />}
        {transaction?.oddWithdraw && <OddWithdrawDetail data={transaction?.oddWithdraw} />}
        {transaction?.atsWithdraw && <AtsWithdrawDetail data={transaction?.atsWithdraw} />}
        {transaction?.upback && <UpBackDetail data={transaction?.upback} />}
        {transaction?.globalTransfer && <GlobalTransferDetail data={transaction?.globalTransfer} />}
        {transaction?.recovery && <RecoveryDetail data={transaction?.recovery} />}
        {transaction?.refundInfo && <RefundDetail data={transaction?.refundInfo} />}
        <HistoryAction makerTickets={transaction?.makerType} checkerTickets={transaction?.checkerType} />
      </Grid>
    </Grid>
  )
}

export default TransactionTable
