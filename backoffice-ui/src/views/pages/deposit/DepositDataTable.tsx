import { IGetDepositTransactionsRequest } from '@/lib/api/clients/backoffice/deposit/types'
import { IState } from '@/store/apps/deposit'
import { DataTable } from '@/views/components/DataTable'
import { nonGeColumns } from './NonGeDepositColumns'
import { geColumns } from './GeDepositColumns'
import { PiBackofficeServiceAPIModelsTransactionHistoryV2Response } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTransactionHistoryV2Response'
import { DEPOSIT_PRODUCT_TYPE } from '@/constants/deposit/type'

interface Props {
  rows: PiBackofficeServiceAPIModelsTransactionHistoryV2Response[]
  total: number
  store: IState
  onPaginate: (value: IGetDepositTransactionsRequest) => void
  isLoading: boolean
  product: DEPOSIT_PRODUCT_TYPE
}

const DepositDataTable = ({ rows, total, store, onPaginate, isLoading, product }: Props) => {
  const rowsWithId = rows.map((row, index) => ({ ...row, id: index }))
  const columns = product === DEPOSIT_PRODUCT_TYPE.GlobalEquity ? geColumns : nonGeColumns

  return (
    <DataTable
      rows={rowsWithId}
      total={total}
      columns={columns}
      store={store}
      onPaginate={onPaginate}
      isDisabledFilter
      exportFileName={'DepositTransaction'}
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

export default DepositDataTable
