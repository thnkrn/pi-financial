import { CuratedFilterItem, CuratedFilterState } from '@/types/curated-manager/filter'
import { DataTable } from '@/views/components/DataTable'
import { Checkbox, Typography } from '@mui/material'
import { GridColDef } from '@mui/x-data-grid'

const curatedFilterColumns: GridColDef[] = [
  {
    field: 'filterName',
    headerName: 'Filter Name',
    width: 250,
    sortable: true,
  },
  {
    field: 'filterCategory',
    headerName: 'Filter Category',
    width: 200,
    sortable: true,
  },
  {
    field: 'filterType',
    headerName: 'Filter Type',
    width: 200,
    sortable: true,
  },
  {
    field: 'listSource',
    headerName: 'List Source',
    width: 200,
    sortable: true,
  },
  {
    field: 'listName',
    headerName: 'List Name',
    width: 200,
    sortable: true,
  },
  {
    field: 'isHighLight',
    headerName: 'Highlight',
    width: 100,
    type: 'boolean',
    sortable: true,
    editable: true,
    renderCell: params => (params?.value ? <Checkbox disabled checked /> : <Checkbox disabled />),
  },
  {
    field: 'isDefault',
    headerName: 'Default',
    width: 100,
    type: 'boolean',
    sortable: true,
    editable: true,
    renderCell: params => (params?.value ? <Checkbox disabled checked /> : <Checkbox disabled />),
  },
]

interface CuratedFilterTableProps {
  name: string
  rows: CuratedFilterItem[]
  total: number
  store: CuratedFilterState
  isLoading: boolean
  onPaginate: (data: { page: number; pageSize: number }) => void
  page?: number
  pageSize?: number
}

const CuratedFilterTable = ({
  name,
  isLoading,
  onPaginate,
  store,
  total,
  rows,
  page,
  pageSize = 20,
}: CuratedFilterTableProps) => {
  const pageIndex = page ? page - 1 : 0

  const initialState = {
    pagination: {
      paginationModel: {
        page: pageIndex,
        pageSize: pageSize,
      },
    },
  }

  return (
    <div className='w-full'>
      <Typography variant='h5' sx={{ mt: 6 }}>
        {name}
      </Typography>
      <DataTable
        rows={rows}
        total={total}
        columns={curatedFilterColumns}
        store={store}
        onPaginate={onPaginate}
        isDisabledFilter
        exportFileName='FiltersList'
        isLoading={isLoading}
        initialState={initialState}
      />
    </div>
  )
}

export default CuratedFilterTable
