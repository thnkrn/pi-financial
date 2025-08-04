import DeleteIcon from '@mui/icons-material/Delete'
import { IconButton, Typography } from '@mui/material'
import Button from '@mui/material/Button'
import CircularProgress from '@mui/material/CircularProgress'
import Dialog from '@mui/material/Dialog'
import DialogActions from '@mui/material/DialogActions'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogTitle from '@mui/material/DialogTitle'
import { MutableRefObject } from 'react'

interface Props {
  open: boolean
  fileInputRef: MutableRefObject<HTMLInputElement | null>
  file: File | null
  error: string
  isUploading: boolean
  handleClose: () => void
  handleUploadClick: () => void
  handleFileChange: (event: React.ChangeEvent<HTMLInputElement>) => void
  handleDiscardFile: () => void
  handleUploadFile: (file: File | null) => void
}

const DashboardDialog = ({
  open,
  fileInputRef,
  file,
  error,
  isUploading,
  handleClose,
  handleUploadClick,
  handleFileChange,
  handleDiscardFile,
  handleUploadFile,
}: Props) => {
  return (
    <Dialog open={open} onClose={handleClose}>
      <DialogTitle>Add to Dashboard by importing .csv file</DialogTitle>
      <DialogContent>
        <DialogContentText component='div'>
          <Typography variant='subtitle1'>Upload a .csv file for SBL</Typography>
          <Typography variant='caption'>
            The file format has to be in .csv and the maximum size in less than 10MB
          </Typography>
        </DialogContentText>
        <div style={{ marginTop: '15px' }}>
          <div style={{ display: 'flex' }}>
            <Button onClick={handleUploadClick} color='secondary' variant='contained'>
              Upload
            </Button>
            <input
              type='file'
              accept='.csv'
              ref={fileInputRef}
              onChange={handleFileChange}
              style={{ display: 'none' }}
            />
            {!!file && (
              <div style={{ display: 'flex', alignItems: 'center', marginLeft: '15px' }}>
                <Typography variant='caption'>File selected: {file.name}</Typography>
                <IconButton color='error' onClick={handleDiscardFile}>
                  <DeleteIcon />
                </IconButton>
              </div>
            )}
          </div>
          {!!error && (
            <Typography color='error' variant='caption' style={{ marginTop: '10px' }}>
              {error}
            </Typography>
          )}
        </div>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} color='inherit' variant='contained'>
          Cancel
        </Button>
        <Button
          onClick={() => handleUploadFile(file)}
          autoFocus
          variant='contained'
          disabled={!!error || !file || file === null || isUploading}
          startIcon={isUploading ? <CircularProgress size={20} color='inherit' /> : null}
        >
          {isUploading ? 'Uploading' : 'Confirm'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default DashboardDialog
