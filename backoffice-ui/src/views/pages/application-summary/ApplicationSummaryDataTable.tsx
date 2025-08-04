import QuickViewModal from '@/views/components/QuickViewModal'
import { DataGrid, GridCellParams } from '@mui/x-data-grid'
import useTranslation from 'next-translate/useTranslation'
import { useEffect, useRef, useState } from 'react'
import { columnsConfig } from './ApplicationColumns'
import ApplicationDetails from './ApplicationDetails'
import { ApplicationSummaryTableProps, CustomerDetail } from './types'

const initialState = {
  open: false,
  data: {},
}

const ApplicationSummaryDataTable = ({
  rows,
  pagination,
  onPageFilter,
  onSortModelChange,
  loading,
}: ApplicationSummaryTableProps) => {
  const [paginationModel, setPaginationModel] = useState({
    page: 0,
    pageSize: 20,
  })
  const [total, setTotal] = useState(pagination?.total ?? 0)
  const [{ open, data }, setApplicationModal] = useState<CustomerDetail>(initialState)
  const onPageFilterRef = useRef(onPageFilter)

  const { t } = useTranslation('application_summary')

  useEffect(() => {
    onPageFilterRef.current = onPageFilter
  }, [onPageFilter])

  useEffect(() => {
    onPageFilterRef.current(paginationModel)
  }, [paginationModel])

  const columns = columnsConfig(t)

  const handleClose = () => setApplicationModal(initialState)

  const onViewCustomerDetail = (params: GridCellParams) => {
    setApplicationModal({
      open: true,
      data: params.row,
    })
  }

  useEffect(() => {
    setTotal(prev => (pagination?.total !== undefined ? pagination?.total : prev))
  }, [pagination?.total, setTotal])

  const customLocaleText = {
    noRowsLabel: `${t('APPLICATION_SUMMARY_NO_RESULTS_FOUND', {}, { default: 'No results found' })}.`,
  }

  return (
    <>
      <DataGrid
        disableColumnFilter
        paginationModel={pagination}
        rows={rows}
        rowCount={total}
        loading={loading}
        autoHeight
        columns={columns}
        getRowId={row => `${row.id}`}
        pageSizeOptions={[5, 10, 20]}
        paginationMode='server'
        localeText={customLocaleText}
        onPaginationModelChange={setPaginationModel}
        disableRowSelectionOnClick
        onCellClick={onViewCustomerDetail}
        onSortModelChange={onSortModelChange}
        sortingMode='server'
        sx={{
          '& .MuiDataGrid-cell:hover, .MuiChip-root:hover': {
            cursor: 'pointer',
          },
        }}
      />
      {open && (
        <QuickViewModal
          maxWidth='md'
          title={t('APPLICATION_SUMMARY_QUICK_VIEW', {}, { default: 'Quick view' })}
          open={open}
          onClose={handleClose}
        >
          <ApplicationDetails data={data} />
        </QuickViewModal>
      )}
    </>
  )
}

export default ApplicationSummaryDataTable
