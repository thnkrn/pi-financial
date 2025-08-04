import { sendLinkAccount } from '@/lib/api/clients/sso/link-account'
import { ISendLinkAccountInfo, ISSOError } from '@/lib/api/clients/sso/link-account/types'
import AddIcon from '@mui/icons-material/Add'
import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@mui/material'
import { DataGrid, GridPaginationModel } from '@mui/x-data-grid'
import { useState } from 'react'
import { buildColumns } from './columns'
import { IPaginationDataType } from './types'

interface Props {
  data: ISendLinkAccountInfo[]
  paginationData: IPaginationDataType
  setPaginationData: (pagination: IPaginationDataType) => void
  loading: boolean
  fetchData: () => void
}

const DataTable = ({ data, paginationData, setPaginationData, loading, fetchData }: Props) => {
  const [openDialog, setOpenDialog] = useState(false)
  const [newCustcode, setNewCustcode] = useState('')
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false)
  const [selectedRow, setSelectedRow] = useState<ISendLinkAccountInfo | { custcode: string } | null>(null)
  const [confirmLoading, setConfirmLoading] = useState(false)
  const handlePaginationModelChange = (model: GridPaginationModel) => {
    setPaginationData({ ...paginationData, page: model.page })
    fetchData()
  }

  const handleResetClick = (row: ISendLinkAccountInfo | { custcode: string }) => {
    setSelectedRow(row)
    setConfirmDialogOpen(true)
  }

  const handleConfirmResetMail = async () => {
    if (!selectedRow?.custcode) return
    setConfirmLoading(true)
    try {
      await sendLinkAccount(selectedRow.custcode)
      alert('Link email sent successfully.')
    } catch (error) {
      const message = (error as ISSOError)?.detail || 'Unexpected error'
      alert(`Failed to send: ${message}`)
    } finally {
      setConfirmLoading(false)
      setConfirmDialogOpen(false)
      setSelectedRow(null)
      fetchData()
    }
  }

  const handleAdd = () => {
    handleResetClick({ custcode: newCustcode })
    setOpenDialog(false)
    setNewCustcode('')
  }

  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button variant='contained' startIcon={<AddIcon />} onClick={() => setOpenDialog(true)}>
          Add Send Link
        </Button>
      </Box>
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)}>
        <DialogTitle>Add Send Link</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin='dense'
            label='Custcode'
            fullWidth
            value={newCustcode}
            onChange={e => setNewCustcode(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
          <Button onClick={handleAdd} disabled={!newCustcode.trim()}>
            Submit
          </Button>
        </DialogActions>
      </Dialog>
      <Dialog open={confirmDialogOpen} onClose={() => setConfirmDialogOpen(false)}>
        <DialogTitle>Confirm</DialogTitle>
        <DialogContent>
          Are you sure you want to resend the link email for custcode {selectedRow?.custcode}?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleConfirmResetMail} variant='contained' autoFocus disabled={confirmLoading}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
      <Box sx={{ height: 500, width: '100%' }}>
        <DataGrid
          rows={data ?? []}
          columns={buildColumns(handleResetClick)}
          pagination
          paginationModel={{ page: paginationData.page, pageSize: paginationData.pageSize }}
          rowCount={paginationData.total}
          paginationMode='server'
          onPaginationModelChange={handlePaginationModelChange}
          loading={loading}
          getRowId={row => row.id}
        />
      </Box>
    </Box>
  )
}
export default DataTable
