// ** Import MUI
import { styled } from '@mui/system'
import Card from '@mui/material/Card'
import { DataGrid, GridColDef, GridFilterModel, GridSortModel } from '@mui/x-data-grid'
import { useEffect, useRef, useState } from 'react'
import { LinearProgress } from '@mui/material'
import extend from 'lodash/extend'
import DefaultToolbar from './DefaultToolbar'

type StyledDataGridProps = {
  height?: number | string
}

const StyledDataGrid = styled(DataGrid)<StyledDataGridProps>(({ height = '250px' }) => ({
  '.MuiDataGrid-columnHeaders': {
    backgroundColor: '#20CE99',
  },
  '& .MuiDataGrid-columnHeader': {
    backgroundColor: '#20CE99',
    color: 'white',
  },
  '& .MuiDataGrid-footerContainer': {
    height: '20px !important',
    marginTop: 0,
    marginBottom: 0,
  },
  height: height,
}))

type DataTableProps = {
  rows: any[]
  total: number
  columns: GridColDef[]
  store: any
  onPaginate: (currentFilter: any) => void
  customToolbar?: () => JSX.Element
  filterModel?: GridFilterModel
  onFilterModelChange?: (model: GridFilterModel) => void
  height?: number | string
  rowHeight?: number
}

export const DataTable = (props: DataTableProps) => {
  const [paginationModel, setPaginationModel] = useState({ page: 1, pageSize: 20 })
  const [sortModel, setSortModel] = useState<GridSortModel>([])
  const initialRender = useRef(true)

  useEffect(() => {
    if (initialRender.current) {
      initialRender.current = false
    } else {
      const currentFilter = extend(
        {},
        { ...props.store.filter },
        { page: paginationModel.page + 1, pageSize: paginationModel.pageSize }
      )

      props.onPaginate(currentFilter)
    }
  }, [paginationModel, props])

  const handleSortModelChange = (model: GridSortModel) => {
    setSortModel(model)
  }

  // NOTE: sortingMode='server' Temporarily disable server side sorting
  return (
    <Card sx={{ marginTop: '0' }}>
      <StyledDataGrid
        height={props.height}
        pagination
        rowHeight={props.rowHeight ?? 30}
        rows={props.rows}
        rowCount={props.total}
        columns={props.columns}
        sortModel={sortModel}
        onSortModelChange={handleSortModelChange}
        paginationMode='server'
        pageSizeOptions={[20, 50, 100]}
        loading={props.store.isLoading}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
        hideFooter
        filterModel={props.filterModel}
        onFilterModelChange={props.onFilterModelChange}
        components={{
          Toolbar: props.customToolbar || DefaultToolbar,
          LoadingOverlay: LinearProgress,
        }}
      />
    </Card>
  )
}
