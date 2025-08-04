import { formatDate } from '@/utils/fmt'
import RefreshIcon from '@mui/icons-material/Refresh'
import { Grid, IconButton, Typography } from '@mui/material'
import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import Card from '@mui/material/Card'
import { DataGrid } from '@mui/x-data-grid'
import { PiBackofficeServiceAPIModelsReportResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsReportResponse'
import { columns } from './ReportColumns'
import { IPaginationModel } from './index'

interface Props {
  rows: PiBackofficeServiceAPIModelsReportResponse[]
  total: number
  paginationModel: IPaginationModel
  setPaginationModel: (value: IPaginationModel) => void
  isLoading: boolean
  onRefresh: () => void
  currentDateTime: Date
  handleOpenDialog: () => void
}

const ReportHistoryTable = ({
  rows,
  total,
  paginationModel,
  setPaginationModel,
  isLoading,
  currentDateTime,
  onRefresh,
  handleOpenDialog,
}: Props) => (
  <div>
    <Grid container justifyContent='space-between' flexWrap='nowrap'>
      <Grid sx={{ width: '100%' }}>
        <Box sx={{ display: 'flex', flexDirection: 'column', py: 3 }}>
          <Box sx={{ display: 'flex', flexWrap: 'nowrap', alignItems: 'center' }}>
            <Box sx={{ whiteSpace: 'nowrap' }}>
              <i>
                <b>Updated at: </b>
                {formatDate(currentDateTime, { format: 'YYYY-MM-DD HH:mm:ss' })}
              </i>
            </Box>
            <IconButton sx={{ ml: 2 }} size='large' onClick={onRefresh}>
              <RefreshIcon />
              <Typography ml={2} color='textPrimary' variant='body1'>
                Refresh
              </Typography>
            </IconButton>

            <div style={{ marginLeft: 'auto' }}>
              <Button size='medium' sx={{ width: 250 }} variant='outlined' onClick={handleOpenDialog} color='secondary'>
                <span>Upload</span>
              </Button>
            </div>
          </Box>
        </Box>
      </Grid>
    </Grid>
    <Card>
      <DataGrid
        autoHeight
        pagination
        rows={rows}
        rowCount={total}
        columns={columns}
        loading={isLoading}
        getRowId={row => `${row.id}`}
        paginationMode='server'
        pageSizeOptions={[10, 20, 50, 100]}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
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
    </Card>
  </div>
)

export default ReportHistoryTable
