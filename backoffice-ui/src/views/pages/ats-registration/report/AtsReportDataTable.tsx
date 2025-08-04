import { downloadAtsReport } from '@/lib/api/clients/backoffice/ats-registration'
import { Chip, IconButton } from '@mui/material'
import { Box } from '@mui/system'
import { DataGrid, GridRenderCellParams } from '@mui/x-data-grid'
import Icon from 'src/@core/components/icon'
import { AtsRegistrationReportDataTableProps } from './types'

const AtsReportDataTable = ({ rows, pagination, onPageFilter, loading }: AtsRegistrationReportDataTableProps) => {
  const customLocaleText = {
    noRowsLabel: 'No results found',
  }

  const handleDownloadReport = async (reportId: string) => {
    const res = await downloadAtsReport(reportId)
    const url = window.URL.createObjectURL(res.blobData)
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', res.fileName)
    document.body.appendChild(link)
    link.click()
    link.parentNode?.removeChild(link)
  }

  return (
    <Box display={'flex'} width={'100%'}>
      <DataGrid
        disableColumnFilter
        disableColumnMenu
        paginationModel={pagination}
        rows={rows}
        rowCount={pagination.total}
        loading={loading}
        autoHeight
        columns={[
          {
            headerName: 'UPLOAD CREATED',
            field: 'requestDate',
            sortable: false,
            flex: 1,
          },
          {
            headerName: 'STATUS',
            field: 'status',
            sortable: false,
            flex: 1,
            renderCell: (params: GridRenderCellParams) => {
              return (
                <Chip
                  label={params.value}
                  size='small'
                  variant={'outlined'}
                  color={params.value === 'COMPLETED' ? 'success' : 'warning'}
                />
              )
            },
          },
          {
            headerName: 'UPLOAD TYPE',
            field: 'atsUploadType',
            sortable: false,
            flex: 1,
          },
          {
            headerName: 'REPORT NAME',
            field: 'reportName',
            sortable: false,
            flex: 1,
          },
          {
            headerName: 'USERNAME',
            field: 'userName',
            sortable: false,
            flex: 1,
          },
          {
            headerName: '',
            field: 'id',
            sortable: false,
            renderCell: (params: GridRenderCellParams) => {
              return (
                <IconButton onClick={() => handleDownloadReport(params.value)}>
                  <Icon icon='mdi:download' fontSize={20} />
                </IconButton>
              )
            },
          },
        ]}
        getRowId={row => `${row.id}`}
        pageSizeOptions={[5, 10, 20]}
        paginationMode='server'
        localeText={customLocaleText}
        onPaginationModelChange={onPageFilter}
        disableRowSelectionOnClick
        sortingMode='server'
        sx={{
          '& .MuiDataGrid-cell:hover, .MuiChip-root:hover': {
            cursor: 'pointer',
          },
        }}
      />
    </Box>
  )
}

export default AtsReportDataTable
