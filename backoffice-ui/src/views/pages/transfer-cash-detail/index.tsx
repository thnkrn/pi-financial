import { Visible } from '@/@core/components/auth/Visible'
import { IGetTransferCashTransactionResponse } from '@/lib/api/clients/backoffice/transactions/types'
import CardContent from '@mui/material/CardContent'
import AddTransactionStatus from './AddTransactionStatus'
import TransactionTable from './TransactionTable'

interface Props {
  transaction: IGetTransferCashTransactionResponse
  handleStatusChange: ({ remark, action }: { remark: string; action: string }) => Promise<void>
  refreshTransactionDetails: () => Promise<void>
}

const TransferCashTransactionPage = ({ transaction, handleStatusChange, refreshTransactionDetails }: Props) => (
  <CardContent>
    <TransactionTable transaction={transaction} />
    {transaction?.actions && transaction?.actions?.length > 0 && (
      <Visible allowedRoles={['ticket-manage']}>
        <AddTransactionStatus
          isLoading={false}
          onStatusChange={handleStatusChange}
          refreshTransactionDetails={refreshTransactionDetails}
          actions={transaction?.actions}
        />
      </Visible>
    )}
  </CardContent>
)

export default TransferCashTransactionPage
