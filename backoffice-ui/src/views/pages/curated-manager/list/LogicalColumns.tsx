import { ICuratedListItem } from '@/constants/curated-list/types'
import { GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import InlineEdit from '../InlineEdit'

interface RowWithEdit extends ICuratedListItem {
  onSave: (id: string, field: string, newValue: string) => Promise<void>
}

export const logicalColumns: GridColDef<RowWithEdit>[] = [
  {
    field: 'relevantTo',
    headerName: 'Relevant to',
    flex: 1,
    minWidth: 100,
  },
  {
    field: 'name',
    headerName: 'List Name',
    flex: 1,
    minWidth: 300,
    renderCell: (params: GridRenderCellParams<RowWithEdit, string>) => {
      return <InlineEdit value={params.value || ''} field='name' rowId={params.row.id} onSave={params.row.onSave} />
    },
  },
  {
    field: 'hashtag',
    headerName: 'List Hashtag',
    flex: 1,
    minWidth: 150,
    renderCell: (params: GridRenderCellParams<RowWithEdit, string>) => {
      return <InlineEdit value={params.value || ''} field='hashtag' rowId={params.row.id} onSave={params.row.onSave} />
    },
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
]
