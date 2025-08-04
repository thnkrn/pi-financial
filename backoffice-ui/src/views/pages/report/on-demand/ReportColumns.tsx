import { Visible } from '@/@core/components/auth/Visible'
import { downloadReport } from '@/lib/api/clients/backoffice/report'
import { renderStatus } from '@/views/components/DataTableUtil'
import { Icon } from '@iconify/react'
import IconButton from '@mui/material/IconButton'
import Tooltip from '@mui/material/Tooltip'
import Typography from '@mui/material/Typography'
import { GridColDef, GridRenderCellParams, gridDateComparator } from '@mui/x-data-grid'
import dayjs from 'dayjs'

const onClickDownlaod = async (reportId: string, reportName: string) => {
  try {
    const response = await downloadReport(reportId)

    // Create a temporary anchor element
    const link = document.createElement('a')
    link.style.display = 'none'
    link.href = response?.URL
    link.download = `${reportName}.csv`

    // Trigger a click on the anchor element
    document.body.appendChild(link)

    link.target = '_blank'
    link.click()

    // Clean up
    window.URL.revokeObjectURL(response?.URL)
    document.body.removeChild(link)
  } catch (e) {
    const displayAlert = (await import('@/views/components/DisplayAlert')).default
    displayAlert((e as Error)?.message)
  }
}

export const columns: GridColDef[] = [
  {
    field: 'col1',
    headerName: '',
    width: 50,
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Visible allowedRoles={['report-export']}>
        <Typography variant='body2' sx={{ color: 'text.primary' }}>
          <Tooltip title='Download'>
            <span>
              <IconButton
                size='small'
                sx={{ mr: 0.5 }}
                disabled={params?.row?.status !== 'Success' || !params?.row?.generatedAt}
                onClick={() => onClickDownlaod(params?.row?.id, params?.row?.name)}
              >
                <Icon icon='mdi:download' />
              </IconButton>
            </span>
          </Tooltip>
        </Typography>
      </Visible>
    ),
  },
  {
    flex: 0.1,
    type: 'date',
    minWidth: 180,
    headerName: 'REPORT GENERATED',
    field: 'createdAt',
    sortComparator: gridDateComparator,
    valueGetter: params => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {dayjs(params.row.createdAt).format('DD/MM/YYYY HH:mm:ss')}
      </Typography>
    ),
  },
  {
    flex: 0.1,
    minWidth: 150,
    headerName: 'STATUS',
    field: 'status',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => renderStatus(params.row.status),
  },
  {
    flex: 0.25,
    minWidth: 400,
    headerName: 'REPORT NAME',
    field: 'name',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.name}
      </Typography>
    ),
  },
  {
    flex: 0.15,
    type: 'date',
    minWidth: 200,
    headerName: 'DATE',
    field: 'dateRange',
    sortable: false,
    valueGetter: params => new Date(params.value),
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.dateFrom && params.row.dateTo
          ? `${dayjs(params.row.dateFrom).format('DD/MM/YYYY')} - ${dayjs(params.row.dateTo).format('DD/MM/YYYY')}`
          : '-'}
      </Typography>
    ),
  },
  {
    flex: 0.15,
    minWidth: 250,
    headerName: 'USERNAME',
    field: 'userName',
    sortable: false,
    renderCell: (params: GridRenderCellParams) => (
      <Typography variant='body2' sx={{ color: 'text.primary' }}>
        {params.row.userName}
      </Typography>
    ),
  },
]
