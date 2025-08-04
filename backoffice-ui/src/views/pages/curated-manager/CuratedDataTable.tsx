import { PaginationParams } from '@/types/apps/paginationTypes'
import { CuratedListState, CuratedListTable } from '@/types/curated-manager/list'
import { DataTable } from '@/views/components/DataTable'
import { logicalColumns } from './list/LogicalColumns'
import { manualColumns } from './list/ManualColumns'

interface Props {
  rows: CuratedListTable[]
  total: number
  store: CuratedListState
  onPaginate: (currentFilter: PaginationParams) => void
  isLoading: boolean
  isLogicalTab: boolean
}

const CuratedDataTable = ({ rows, total, store, onPaginate, isLoading, isLogicalTab }: Props) => {
  const columns = isLogicalTab ? logicalColumns : manualColumns

  return (
    <DataTable
      rows={rows}
      total={total}
      columns={columns}
      store={store}
      onPaginate={onPaginate}
      isDisabledFilter
      exportFileName={isLogicalTab ? 'LogicalCuratedList' : 'ManualCuratedList'}
      isLoading={isLoading}
      dateFormatFields={{ field: ['createdAt', 'updatedAt'], format: 'DD/MM/YYYY HH:mm:ss' }}
      csvTransform={{
        relevantTo: 'name',
        instrumentType: 'name',
        ordering: 'name',
      }}
      initialState={{
        sorting: {
          sortModel: [
            {
              field: 'listName',
              sort: 'asc',
            },
          ],
        },
      }}
    />
  )
}

export default CuratedDataTable
