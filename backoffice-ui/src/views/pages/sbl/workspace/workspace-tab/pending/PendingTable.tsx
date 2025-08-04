import { ACTION_TYPE, IPaginationModel } from '@/types/sbl/sblTypes'
import { formatDate } from '@/utils/fmt'
import RefreshIcon from '@mui/icons-material/Refresh'
import { Grid, IconButton, Typography } from '@mui/material'
import Box from '@mui/material/Box'
import Card from '@mui/material/Card'
import { DataGrid } from '@mui/x-data-grid'
import { PiBackofficeServiceApplicationModelsSblSblOrder } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsSblSblOrder'
import { getPendingColumns } from './PendingColumn'

interface Props {
  rows: PiBackofficeServiceApplicationModelsSblSblOrder[]
  total: number
  paginationModel: IPaginationModel
  setPaginationModel: (value: IPaginationModel) => void
  isLoading: boolean
  onRefresh: () => void
  currentDateTime: Date
  handleOpen: (orderId: string, orderNo: string, type: ACTION_TYPE) => void
}

const PendingTable = ({
  rows,
  total,
  paginationModel,
  setPaginationModel,
  isLoading,
  currentDateTime,
  onRefresh,
  handleOpen,
}: Props) => (
  <div>
    <Grid container justifyContent='space-between' flexWrap='nowrap'>
      <Grid item xs={12} sm={9} md={6} lg={3}>
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
        columns={getPendingColumns(handleOpen)}
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

export default PendingTable
