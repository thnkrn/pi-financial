import { IState } from '@/store/apps/transfer-cash'
import { DataTable } from '@/views/components/DataTable'
import { transferCashColumns } from '@/views/pages/transfer-cash/TransferCashColumns'
import { IGetTransferCashTransactionsRequest } from '@/lib/api/clients/backoffice/transactions/types'
import { PiBackofficeServiceAPIModelsTransferCashResponse } from '@pi-financial/backoffice-srv'

interface Props {
  rows: PiBackofficeServiceAPIModelsTransferCashResponse[]
  total: number
  store: IState
  onPaginate: (value: IGetTransferCashTransactionsRequest) => void
  isLoading: boolean
}

const TransferCashDataTable = ({ rows, total, store, onPaginate, isLoading }: Props) => {
  const rowsWithId = rows.map((row, index) => ({ ...row, id: index }))

  return (
    <DataTable
      rows={rowsWithId}
      total={total}
      columns={transferCashColumns}
      store={store}
      onPaginate={onPaginate}
      isDisabledFilter
      exportFileName={'TransferCashTransaction'}
      isLoading={isLoading}
      dateFormatFields={{ field: ['createdAt', 'paymentReceivedDateTime'], format: 'DD/MM/YYYY HH:mm:ss' }}
      csvTransform={{
        accountType: 'name',
        channel: 'name',
        responseCode: 'description',
      }}
      initialState={{
        sorting: {
          sortModel: [
            {
              field: 'createdAt',
              sort: 'desc',
            },
          ],
        },
      }}
    />
  )
}

export default TransferCashDataTable
