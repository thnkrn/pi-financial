import { hexToRGBA } from '@/@core/utils/hex-to-rgba'
import { useSessionContext } from '@/context/SessionContext'
import { uploadAtsRequest } from '@/lib/api/clients/backoffice/ats-registration'
import { bytesToKilobytes, mbToBytes } from '@/utils/convert'
import {
  Article as ArticleIcon,
  Cancel as CancelIcon,
  FolderOpenOutlined as FolderOpenOutlinedIcon,
  Photo as PhotoIcon,
  Publish as PublishIcon,
} from '@mui/icons-material'
import { Box, Button, CircularProgress, Divider, Grid, IconButton, Stack, Typography, styled } from '@mui/material'
import { isAxiosError } from 'axios'
import { useRouter } from 'next/router'
import { useState } from 'react'
import { FileRejection, useDropzone } from 'react-dropzone'
import UploadAtsResultDialog from './UploadAtsResultDialog'
import { OVERRIDE_BANK_INFO_UPLOAD_FILE_TYPE, UPDATE_EFFECTIVE_DATE_UPLOAD_FILE_TYPE } from './constants'
import { AcknowledgeDialogProps, AtsUploadType, FileObjectType } from './types'

const DropZoneWrapper = styled('div')(({ theme }) => ({
  '& .dropzone': {
    border: `1px dashed ${theme.palette.grey[400]}`,
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    flexDirection: 'column',
    textAlign: 'center',
    marginTop: '30px',
    padding: '32px',
    transition: 'all 0.15s ease-in-out',
    '&.active': {
      backgroundColor: `${
        theme.palette.mode === 'dark'
          ? hexToRGBA(theme.palette.grey[100], 0.05)
          : hexToRGBA(theme.palette.grey[100], 0.5)
      }`,
    },
    '&.files': {
      padding: 0,
      marginTop: 0,
      border: 'none',
    },
  },
}))

const DocumentListWrapper = styled('ul')(({ theme }) => ({
  margin: '20px 0',
  padding: 0,
  listStyle: 'none',
  display: 'flex',
  flexDirection: 'column',
  gap: '20px',
  '& > li': {
    border: `1px solid ${theme.palette.grey[300]}`,
    display: 'flex',
    justifyContent: 'flex-start',
    alignItems: 'center',
    flexDirection: 'row',
    gap: '10px 20px',
    padding: '12px',
    '&.error, &.validation-error': {
      border: `1px solid ${theme.palette.error.main}`,
    },
    '&.validation-error': {
      flexDirection: 'column',
      gap: 0,
      alignItems: 'flex-start',
    },
    '& .status-icon': {
      display: 'block',
      '& svg': {
        display: 'block',
      },
    },
    '&  ul': {
      margin: 0,
      padding: 0,
      listStyle: 'none',
      '& > li': {
        fontSize: '14px',
        color: `${theme.palette.error.main}`,
      },
    },
  },
}))

const Index = () => {
  const router = useRouter()
  const sessionData = useSessionContext()
  const [acceptedFiles, setAcceptedFiles] = useState<FileObjectType | null>()
  const [rejectedFiles, setRejectedFiles] = useState<FileRejection[]>([])
  const [dragEnter, setDragEnter] = useState<boolean>(false)
  const [documentProcessing, setDocumentProcessing] = useState<boolean>(false)
  const [dialogProps, setDialogProps] = useState<AcknowledgeDialogProps>({
    content: '',
    isSuccess: false,
    open: false,
  })

  const { getRootProps, getInputProps, open } = useDropzone({
    maxSize: mbToBytes(10),
    accept: {
      'txt/csv': ['.csv'],
      'application/vnd.ms-excel': [],
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': [],
    },
    onDrop: (newFiles, fileRejections) => {
      const [file] = newFiles
      if (file) {
        const updatedFiles = { file, status: 'pending' }
        if (file.type === 'text/csv' || file.type === 'text/plain') {
          const reader = new FileReader()
          reader.readAsText(file)
        }
        setAcceptedFiles(updatedFiles)
      }
      setRejectedFiles(fileRejections)
      setDragEnter(false)
    },
    onDragEnter: () => setDragEnter(true),
    onDragLeave: () => setDragEnter(false),
    noClick: true,
    noKeyboard: true,
    maxFiles: 1,
  })

  const handleDeleteFile = () => {
    setAcceptedFiles(null)
  }

  const uploadFile = async (uploadType: AtsUploadType) => {
    if (acceptedFiles) {
      setDocumentProcessing(true)
      try {
        const result = await uploadAtsRequest({
          uploadFile: acceptedFiles?.file,
          uploadType,
          userName: sessionData.user.name ?? '',
        })

        let dialogContent = `Your upload ${uploadType} file is processing. You can check status from result file.`
        if (result.error) {
          if (result.error === 'Missing some values') {
            dialogContent = `Your upload ${uploadType} file is missing some value(s). Please check your file again.`
          } else {
            dialogContent = `Error processing document`
          }
        }
        setDialogProps({
          content: dialogContent,
          isSuccess: !result.error,
          open: true,
        })
      } catch (error) {
        const message: string = isAxiosError(error) ? error?.response?.data?.title : 'Error processing document(s)'
        setDialogProps({
          content: message,
          isSuccess: false,
          open: true,
        })
      } finally {
        handleDeleteFile()
        setDocumentProcessing(false)
      }
    }
  }

  const getErrorMessage = (code: string) => {
    const errorMessages: Record<string, string> = {
      'file-invalid-type': 'The file type is invalid. The supported formats are CSV, XLS, and XLSX.',
      'file-too-large': 'File exceeds the size limit',
    }

    return errorMessages[code] ?? ''
  }

  const renderFiles = () => {
    return acceptedFiles ? (
      <li key={acceptedFiles.file.name}>
        {acceptedFiles.file.type === 'application/pdf' && <ArticleIcon fontSize='large' sx={{ color: '#999' }} />}
        {acceptedFiles.file.type === 'image/jpeg' || acceptedFiles.file.type === 'image/png' ? (
          <PhotoIcon fontSize='large' sx={{ color: '#999' }} />
        ) : null}
        <Box sx={{ maxWidth: '80%', overflow: 'hidden' }}>
          <Typography noWrap>{acceptedFiles.file.name}</Typography>
          <Typography sx={{ color: '#999', fontSize: '14px' }}>{bytesToKilobytes(acceptedFiles.file.size)}</Typography>
        </Box>
        <Box sx={{ display: 'flex', gap: '10px', alignItems: 'center', marginLeft: 'auto' }}>
          <IconButton aria-label='delete' size='medium' onClick={() => handleDeleteFile()}>
            <CancelIcon fontSize='inherit' />
          </IconButton>
        </Box>
      </li>
    ) : null
  }

  const renderFileRejectionItems = () => {
    return rejectedFiles.map(({ file, errors }) => (
      <li key={file.name} className='validation-error'>
        {file.name} - {bytesToKilobytes(file.size)}
        <ul>
          {errors.map(e => (
            <li key={e.code}>{getErrorMessage(e.code)}</li>
          ))}
        </ul>
      </li>
    ))
  }

  const onAcknowledge = () => {
    if (dialogProps.isSuccess) {
      router.push('/ats-registration/report')
    }
    setDialogProps({
      ...dialogProps,
      open: false,
    })
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Divider />
        <Grid item xs={12}>
          <DropZoneWrapper>
            <div
              {...getRootProps({
                className: `dropzone ${dragEnter ? 'active' : ''} ${acceptedFiles ? 'files' : ''}`,
              })}
            >
              <input id='ocrFileInput' {...getInputProps()} />

              {!acceptedFiles && (
                <>
                  <FolderOpenOutlinedIcon sx={{ fontSize: '4rem', color: '#999' }} />
                  <p style={{ fontWeight: 'bold', fontSize: 16 }}>{'Drag a file here to upload'}</p>
                  <text style={{ fontSize: '14px' }}>{'Supported formats: CSV, XLS, and XLSX'}</text>
                  <text style={{ fontSize: '14px', marginBottom: 20 }}>{'Maximum file size: 10MB'}</text>
                  <Button variant='contained' onClick={open} startIcon={<PublishIcon />}>
                    {'Upload documents'}
                  </Button>
                </>
              )}
            </div>
          </DropZoneWrapper>

          <Box>
            {acceptedFiles && <h4>Documents</h4>}

            <DocumentListWrapper>
              {renderFiles()} {renderFileRejectionItems()}
            </DocumentListWrapper>

            {renderFileRejectionItems()?.length > 0 && !acceptedFiles && (
              <Button
                variant='contained'
                onClick={() => {
                  setRejectedFiles([])
                }}
              >
                {'Reset uploads'}
              </Button>
            )}
          </Box>

          {acceptedFiles && (
            <Stack direction='row' spacing={4}>
              <Button
                sx={{ display: 'flex', gap: '0 10px' }}
                disabled={documentProcessing}
                variant='contained'
                onClick={() => {
                  uploadFile(UPDATE_EFFECTIVE_DATE_UPLOAD_FILE_TYPE)
                  setRejectedFiles([])
                }}
              >
                {documentProcessing ? (
                  <>
                    <CircularProgress size={20} variant='indeterminate' />
                    {'Processing'}
                  </>
                ) : (
                  'UPDATE EFFECTIVE DATE'
                )}
              </Button>
              <Button
                sx={{ display: 'flex', gap: '0 10px' }}
                disabled={documentProcessing}
                variant={'outlined'}
                onClick={() => {
                  uploadFile(OVERRIDE_BANK_INFO_UPLOAD_FILE_TYPE)
                  setRejectedFiles([])
                }}
              >
                {documentProcessing ? (
                  <>
                    <CircularProgress size={20} variant='indeterminate' />
                    {'Processing'}
                  </>
                ) : (
                  'OVERRIDE BANK INFO'
                )}
              </Button>

              {renderFileRejectionItems()?.length > 0 && (
                <Button
                  variant='outlined'
                  onClick={() => {
                    setRejectedFiles([])
                  }}
                >
                  {'Reset uploads'}
                </Button>
              )}
            </Stack>
          )}

          <UploadAtsResultDialog
            open={dialogProps.open}
            handleDialogClose={() => {
              setDialogProps({
                ...dialogProps,
                open: false,
              })
            }}
            isSuccess={dialogProps.isSuccess}
            dialogContent={dialogProps.content}
            handleDoAction={onAcknowledge}
          />
        </Grid>
      </Grid>
    </Grid>
  )
}

export default Index
