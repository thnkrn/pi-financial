import Grid from '@mui/material/Grid'
import { isAxiosError } from 'axios'
import dayjs from 'dayjs'
import { useRef, useState } from 'react'
import { SubmitHandler } from 'react-hook-form'
import ReportFilter from './ReportFilter'
import ReportHistoryTable from './ReportHistoryTable'
import UploadDialog from './UploadDialog'
import { FILE_TYPE_CONFIG, FileTypeKey } from './const'

export interface IFormInput {
  type: string
}

export interface IPaginationModel {
  page: number
  pageSize: number
}

const Index = () => {
  const [openDialog, setOpenDialog] = useState(false)
  const [file, setFile] = useState<File | null>(null)
  const [error, setError] = useState<string>('')
  const [isUploading, setIsUploading] = useState(false)
  const [selectedFileType, setSelectedFileType] = useState<FileTypeKey | ''>('')

  const fileInputRef = useRef<HTMLInputElement | null>(null)

  const onSubmit: SubmitHandler<IFormInput> = () => {}

  const onDateRangeChange = () => {}

  const updateFilter = () => {}

  const onRefresh = () => {}

  const handleOpenDialog = () => {
    setOpenDialog(true)
  }

  const handleDiscardFile = () => {
    setFile(null)
    setError('')
    if (fileInputRef.current) {
      fileInputRef.current.value = ''
    }
  }

  const handleUploadClick = () => {
    fileInputRef.current?.click()
  }

  const handleFileTypeChange = (fileType: FileTypeKey | '') => {
    setSelectedFileType(fileType)

    // Clear existing file when file type changes
    setFile(null)
    setError('')
    if (fileInputRef.current) {
      fileInputRef.current.value = ''
    }
  }

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = event.target.files?.[0]

    if (selectedFile && selectedFileType) {
      const config = FILE_TYPE_CONFIG[selectedFileType]

      // Check MIME type
      if (!config.allowedTypes.includes(selectedFile.type)) {
        const allowedText = config.allowedExtensions.join(' or ')
        setError(`Only ${allowedText} files are allowed for ${config.label}`)
        setFile(null)

        return
      }

      // Additional extension check as fallback
      const fileExtension = '.' + selectedFile.name.split('.').pop()?.toLowerCase()
      if (!config.allowedExtensions.includes(fileExtension)) {
        const allowedText = config.allowedExtensions.join(' or ')
        setError(`Only ${allowedText} files are allowed for ${config.label}`)
        setFile(null)

        return
      }

      setFile(selectedFile)
      setError('')
    }
  }

  const handleUploadFile = async (file: File | null, fileType: FileTypeKey | '') => {
    if (!file || !fileType) return

    setIsUploading(true)

    try {
      // TODO: calling upload here with file type information
      // await uploadCMEFile(file, fileType, config.prefix)
    } catch (error) {
      const displayAlert = (await import('@/views/components/DisplayAlert')).default

      if (isAxiosError(error)) {
        displayAlert(error?.response?.data?.detail ?? 'Unexpected error from upload CME file')
      } else {
        displayAlert('Unexpected error from upload CME file')
      }
    } finally {
      setIsUploading(false)
      setOpenDialog(false)
      handleDiscardFile()
      setSelectedFileType('')

      // TODO: Fetching new report here
      // await fetchCMEReport(filter)
      // setCurrentDateTime(new Date())
    }
  }

  const handleCloseDialog = () => {
    setOpenDialog(false)
    setSelectedFileType('')
    handleDiscardFile()
  }

  return (
    <>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <ReportFilter
            onSubmit={onSubmit}
            dateFrom={dayjs().subtract(7, 'days').toDate()}
            dateTo={new Date()}
            onDateRangeChange={onDateRangeChange}
            updateFilter={updateFilter}
          />
          <ReportHistoryTable
            rows={[]}
            total={0}
            paginationModel={{ page: 0, pageSize: 10 }}
            setPaginationModel={() => {}}
            isLoading={false}
            onRefresh={onRefresh}
            currentDateTime={new Date()}
            handleOpenDialog={handleOpenDialog}
          />
        </Grid>
      </Grid>

      <UploadDialog
        open={openDialog}
        fileInputRef={fileInputRef}
        file={file}
        error={error}
        isUploading={isUploading}
        selectedFileType={selectedFileType}
        handleClose={handleCloseDialog}
        handleUploadClick={handleUploadClick}
        handleFileChange={handleFileChange}
        handleDiscardFile={handleDiscardFile}
        handleUploadFile={handleUploadFile}
        handleFileTypeChange={handleFileTypeChange}
      />
    </>
  )
}

export default Index
