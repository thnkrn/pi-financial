import { IGetWithdrawTransactionsRequest } from '@/lib/api/clients/backoffice/withdraw/types'
import { IState } from '@/store/apps/withdraw'
import { DataTable } from '@/views/components/DataTable'
import { geColumns } from '@/views/pages/withdraw/GeWithdrawColumns'
import { nonGeColumns } from '@/views/pages/withdraw/NonGeWithdrawColumns'
import { PiBackofficeServiceAPIModelsTransactionHistoryV2Response } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransactionHistoryV2Response'
import { WITHDRAW_PRODUCT_TYPE } from '@/constants/withdraw/type'

interface Props {
  rows: PiBackofficeServiceAPIModelsTransactionHistoryV2Response[]
  total: number
  store: IState
  onPaginate: (value: IGetWithdrawTransactionsRequest) => void
  isLoading: boolean
  product: WITHDRAW_PRODUCT_TYPE
}

const WithdrawDataTable = ({ rows, total, store, onPaginate, isLoading, product }: Props) => {
  const rowsWithId = rows.map((row, index) => ({ ...row, id: index }))
  const columns = product === WITHDRAW_PRODUCT_TYPE.GlobalEquity ? geColumns : nonGeColumns

  return (
    <DataTable
      rows={rowsWithId}
      total={total}
      columns={columns}
      store={store}
      onPaginate={onPaginate}
      isDisabledFilter
      exportFileName={'WithdrawTransaction'}
      isLoading={isLoading}
      dateFormatFields={{ field: ['createdAt', 'effectiveDateTime'], format: 'DD/MM/YYYY HH:mm:ss' }}
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

export default WithdrawDataTable
