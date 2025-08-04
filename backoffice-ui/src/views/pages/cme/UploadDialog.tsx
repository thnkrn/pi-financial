import DeleteIcon from '@mui/icons-material/Delete'
import { FormControl, IconButton, InputLabel, MenuItem, Select, Typography } from '@mui/material'
import Button from '@mui/material/Button'
import CircularProgress from '@mui/material/CircularProgress'
import Dialog from '@mui/material/Dialog'
import DialogActions from '@mui/material/DialogActions'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogTitle from '@mui/material/DialogTitle'
import { MutableRefObject } from 'react'
import { FILE_TYPE_CONFIG, FileTypeKey } from './const'

interface Props {
  open: boolean
  fileInputRef: MutableRefObject<HTMLInputElement | null>
  file: File | null
  error: string
  isUploading: boolean
  selectedFileType: FileTypeKey | ''
  handleClose: () => void
  handleUploadClick: () => void
  handleFileChange: (event: React.ChangeEvent<HTMLInputElement>) => void
  handleDiscardFile: () => void
  handleUploadFile: (file: File | null, fileType: FileTypeKey | '') => void
  handleFileTypeChange: (fileType: FileTypeKey | '') => void
}

const UploadDialog = ({
  open,
  fileInputRef,
  file,
  error,
  isUploading,
  selectedFileType,
  handleClose,
  handleUploadClick,
  handleFileChange,
  handleDiscardFile,
  handleUploadFile,
  handleFileTypeChange,
}: Props) => {
  const currentConfig = selectedFileType ? FILE_TYPE_CONFIG[selectedFileType] : null
  const allowedFileText = currentConfig?.allowedExtensions.join(' or ') || '.txt or .json'

  return (
    <Dialog open={open} onClose={handleClose}>
      <DialogTitle>Add to Dashboard by importing file</DialogTitle>
      <DialogContent>
        <DialogContentText component='div'>
          <Typography variant='subtitle1'>Upload a file for CME</Typography>
          <Typography variant='caption'>
            Select file type first, then upload the corresponding {allowedFileText} file
          </Typography>
        </DialogContentText>

        <div style={{ marginTop: '15px' }}>
          <FormControl fullWidth style={{ marginBottom: '15px' }}>
            <InputLabel>File Type</InputLabel>
            <Select
              value={selectedFileType}
              label='File Type'
              onChange={e => handleFileTypeChange(e.target.value as FileTypeKey | '')}
            >
              <MenuItem value=''>
                <em>Select file type</em>
              </MenuItem>
              {Object.entries(FILE_TYPE_CONFIG).map(([key, config]) => (
                <MenuItem key={key} value={key}>
                  {config.label} ({config.allowedExtensions.join(', ')})
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <div style={{ display: 'flex' }}>
            <Button onClick={handleUploadClick} color='secondary' variant='contained' disabled={!selectedFileType}>
              Upload {currentConfig?.allowedExtensions.join(' or ') || 'File'}
            </Button>
            <input
              type='file'
              accept={currentConfig?.accept || '.txt,.json,text/plain,application/json'}
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
          onClick={() => handleUploadFile(file, selectedFileType)}
          autoFocus
          variant='contained'
          disabled={!!error || !file || !selectedFileType || isUploading}
          startIcon={isUploading ? <CircularProgress size={20} color='inherit' /> : null}
        >
          {isUploading ? 'Uploading' : 'Confirm'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default UploadDialog
