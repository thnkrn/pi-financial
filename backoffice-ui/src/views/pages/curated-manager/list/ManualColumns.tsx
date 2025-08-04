import { ICuratedListItem } from '@/constants/curated-list/types'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import ActionCell from '../ActionCell'

interface RowWithDelete extends ICuratedListItem {
  onDelete?: (id: string) => void
}

export const manualColumns: GridColDef<RowWithDelete>[] = [
  {
    field: 'relevantTo',
    headerName: 'Relevant to',
    flex: 1,
    minWidth: 150,
  },
  {
    field: 'name',
    headerName: 'List Name',
    flex: 1,
    minWidth: 200,
  },
  {
    field: 'hashtag',
    headerName: 'List Hashtag',
    flex: 1,
    minWidth: 150,
  },
  {
    field: 'instrumentType',
    headerName: 'Instrument Type',
    flex: 1,
    minWidth: 150,
    valueGetter: params => params.row.instrumentType || '-',
  },
  {
    field: 'ordering',
    headerName: 'Ordering',
    flex: 1,
    minWidth: 50,
    valueGetter: params => params.row.ordering || '-',
  },
  {
    field: 'curatedListSource',
    headerName: 'Data Source',
    flex: 1,
    minWidth: 100,
    sortable: true,
  },
  {
    field: 'actions',
    headerName: 'Actions',
    width: 100,
    sortable: false,
    renderCell: (params: GridRenderCellParams<RowWithDelete>) => (
      <ActionCell
        id={params.row.id}
        name={params.row.name || ''}
        curatedListId={params.row.curatedListId}
        onDelete={params.row.onDelete!}
      />
    ),
  },
]
