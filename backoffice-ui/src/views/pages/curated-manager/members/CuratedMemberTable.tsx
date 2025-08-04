import { PaginationParams } from '@/types/apps/paginationTypes'
import { CuratedMemberItem, CuratedMemberState } from '@/types/curated-manager/member'
import { DataTable } from '@/views/components/DataTable'
import { Avatar } from '@mui/material'
import { GridColDef } from '@mui/x-data-grid'

const curatedMemberColumns: GridColDef[] = [
  {
    field: 'logo',
    headerName: 'Logo',
    sortable: false,
    width: 100,
    renderCell: params => <Avatar src={params.value} alt={params.row.name} style={{ width: 40, height: 40 }} />,
  },
  {
    field: 'symbol',
    headerName: 'Symbol',
    sortable: true,
    width: 100,
  },
  {
    field: 'friendlyName',
    headerName: 'Friendly Name',
    sortable: true,
    width: 250,
  },
  {
    field: 'figi',
    headerName: 'FIGI',
    sortable: true,
    width: 150,
  },
  {
    field: 'units',
    headerName: 'Units',
    sortable: true,
    width: 100,
  },
  {
    field: 'exchange',
    headerName: 'Exchange',
    sortable: true,
    width: 100,
  },
  {
    field: 'dataVenderCode',
    headerName: 'Data Vendor Code',
    sortable: true,
    width: 200,
  },
  {
    field: 'dataVenderCode2',
    headerName: 'Data Vendor Code 2',
    sortable: true,
    width: 200,
  },
  {
    field: 'source',
    headerName: 'Data Source',
    sortable: true,
    width: 150,
  },
]

interface CuratedMemberTableProps {
  row: CuratedMemberItem[]
  total: number
  store: CuratedMemberState
  onPaginate: (currentFilter: PaginationParams) => void
  isLoading: boolean
}

const CuratedMemberTable = ({ row, total, store, onPaginate, isLoading }: CuratedMemberTableProps) => {
  return (
    <div className='w-full'>
      <DataTable
        rows={row}
        total={total}
        columns={curatedMemberColumns}
        store={store}
        onPaginate={onPaginate}
        isLoading={isLoading}
        exportFileName='CuratedMembers'
      />
    </div>
  )
}

export default CuratedMemberTable
