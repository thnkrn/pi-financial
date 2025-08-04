import { toBase64 } from '@/@core/utils/files'
import { hexToRGBA } from '@/@core/utils/hex-to-rgba'
import { OCR_PORTAL_FILE_UPLOAD_ERROR, OCR_PORTAL_TYPE } from '@/constants/ocr-portal/types'
import { getOcrResults } from '@/lib/api/clients/backoffice/ocr'
import { IGetOcrResultsResponse } from '@/lib/api/clients/backoffice/ocr/types'
import { bytesToKilobytes, mbToBytes } from '@/utils/convert'
import {
  Article as ArticleIcon,
  Cancel as CancelIcon,
  Download as DownloadIcon,
  FolderOpenOutlined as FolderOpenOutlinedIcon,
  Photo as PhotoIcon,
  Publish as PublishIcon,
} from '@mui/icons-material'
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Divider,
  Grid,
  IconButton,
  SelectChangeEvent,
  Stack,
  Typography,
  styled,
} from '@mui/material'
import { isAxiosError } from 'axios'
import dayjs from 'dayjs'
import useTranslation from 'next-translate/useTranslation'
import { useCallback, useEffect, useState } from 'react'
import { CSVLink } from 'react-csv'
import { FileRejection, useDropzone } from 'react-dropzone'
import DocumentTypeSelect from './DocumentTypeSelect'
import ErrorInfoAlert from './ErrorInfoAlert'
import OcrStatus from './OcrStatus'
import PasswordDialog from './PasswordDialog'
import { DocumentProcessType, FileObjectType, UploadErrorType } from './types'

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

function splitErrorText(errorText: string) {
  const parts = errorText.split('-').map(part => part.trim())
  const statusCode = parts[0]
  const ocrError = parts.slice(1).join('-')

  return `${statusCode}: ${ocrError}`
}

const Index = () => {
  const { t } = useTranslation('ocr_portal')
  const { t: commonTranslation } = useTranslation('common')
  const [documentType, setDocumentType] = useState<string>('bankStatement')
  const [acceptedFiles, setAcceptedFiles] = useState<FileObjectType[]>([])
  const [rejectedFiles, setRejectedFiles] = useState<FileRejection[]>([])
  const [processResult, setProcessResult] = useState<IGetOcrResultsResponse | null | string>(null)
  const [openPasswordDialog, setOpenPasswordDialog] = useState<boolean>(false)
  const [dragEnter, setDragEnter] = useState<boolean>(false)
  const [openUploadLimitAlertDialog, setOpenUploadLimitAlertDialog] = useState<boolean>(false)
  const [documentPassword, setDocumentPassword] = useState<string>('')
  const [uploadError, setUploadError] = useState<UploadErrorType>({
    error: false,
    message: '',
  })
  const [documentProcess, setDocumentProcess] = useState<DocumentProcessType>({
    processDocumentClick: false,
    processingStatus: false,
    processComplete: false,
  })

  const { getRootProps, getInputProps, open } = useDropzone({
    maxSize: mbToBytes(10),
    accept: {
      'image/jpeg': [],
      'image/png': [],
      'application/pdf': [],
    },
    onDrop: (newFiles, fileRejections) => {
      setDragEnter(false)

      const uniqueNewFiles = newFiles.filter(
        newFile => !acceptedFiles.some(fileObj => fileObj.file.name === newFile.name)
      )

      uniqueNewFiles.forEach(file => {
        const reader = new FileReader()

        reader.onload = function (e) {
          const result = e?.target?.result as string
          const encryptIndex = result?.indexOf('/Encrypt')

          if (encryptIndex !== -1) {
            setOpenPasswordDialog(true)
          }
        }

        reader.readAsText(file)
      })

      const updatedFiles = uniqueNewFiles.map(file => ({
        file,
        status: 'pending',
      }))

      setRejectedFiles(fileRejections)
      setAcceptedFiles(prevFiles => [...prevFiles, ...updatedFiles])
    },
    maxFiles: OCR_PORTAL_TYPE.MAX_UPLOAD_LIMIT_COUNT,

    onDragEnter: () => {
      setDragEnter(true)
    },
    onDragLeave: () => {
      setDragEnter(false)
    },
    noClick: true,
    noKeyboard: true,
  })

  const handleDocumentTypeChange = (event: SelectChangeEvent) => {
    setDocumentType(event.target.value)
  }

  const handleDeleteFile = (fileName: string) => {
    setAcceptedFiles(prevFiles => prevFiles.filter(fileObj => fileObj.file.name !== fileName))
  }

  const uploadFile = useCallback(
    async (fileArray: FileObjectType[]) => {
      try {
        const base64Array: string[] = []
        setDocumentProcess(prevState => ({
          ...prevState,
          processingStatus: true,
        }))

        await Promise.all(
          fileArray.map(async (fileObject: FileObjectType) => {
            base64Array.push(await toBase64(fileObject.file))
          })
        )

        const result = await getOcrResults({
          files: base64Array,
          documentType: documentType,
          output: 'csv',
          password: documentPassword,
        })

        setProcessResult(result)
        setDocumentProcess({
          processDocumentClick: false,
          processingStatus: false,
          processComplete: true,
        })

        setDocumentPassword('')
      } catch (error) {
        const message: string = isAxiosError(error) ? error?.response?.data?.title : 'Error processing document(s)'
        setDocumentProcess(prevState => ({
          ...prevState,
          processComplete: true,
        }))
        setDocumentPassword('')
        setUploadError(prevState => ({
          ...prevState,
          error: true,
          message,
        }))
      }
    },
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [documentPassword, documentType]
  )

  useEffect(() => {
    if (documentProcess.processDocumentClick) {
      uploadFile(acceptedFiles)
    }
  }, [acceptedFiles, uploadFile, documentProcess.processDocumentClick])

  useEffect(() => {
    acceptedFiles.length >= OCR_PORTAL_TYPE.MAX_UPLOAD_LIMIT_COUNT
      ? setOpenUploadLimitAlertDialog(true)
      : setOpenUploadLimitAlertDialog(false)
    if (documentProcess.processComplete && !acceptedFiles?.length) {
      documentProcessingReset()
    }
  }, [acceptedFiles, documentProcess.processComplete])

  const documentTypeSelectOptions = {
    bankStatement: `${t('DOCUMENT_TYPE_BANK_STATEMENTS', {}, { default: '90 Day Bank Statements' })}`,
  }

  const getErrorMessage = (code: string) => {
    switch (code) {
      case OCR_PORTAL_FILE_UPLOAD_ERROR.FILE_INVALID_TYPE:
        return `${t(
          'ERROR_FILE_TYPE_INVALID_WITH_SUPPORTED_FORMATS',
          {},
          { default: 'The file type is invalid. The supported formats are PDF, JPG, and PNG.' }
        )}`
      case OCR_PORTAL_FILE_UPLOAD_ERROR.FILE_TOO_LARGE:
        return `${t('ERROR_FILE_EXCEEDS_SIZE_LIMIT', {}, { default: 'File exceeds the size limit' })}`
      case OCR_PORTAL_FILE_UPLOAD_ERROR.TOO_MANY_FILES:
        return `${t('ERROR_MAX_DOCUMENTS_SELECTED', {}, { default: 'A maximum of only 10 documents can be selected' })}`
      default:
        return ''
    }
  }

  const handlePasswordDialogClose = () => {
    setOpenPasswordDialog(false)
  }

  const files = acceptedFiles.map(fileObj => (
    <li key={fileObj.file.name}>
      {fileObj.file.type === 'application/pdf' && <ArticleIcon fontSize='large' sx={{ color: '#999' }} />}
      {fileObj.file.type === 'image/jpeg' || fileObj.file.type === 'image/png' ? (
        <PhotoIcon fontSize='large' sx={{ color: '#999' }} />
      ) : null}
      <Box sx={{ maxWidth: '80%', overflow: 'hidden' }}>
        <Typography noWrap>{fileObj.file.name}</Typography>
        <Typography sx={{ color: '#999', fontSize: '14px' }}>{bytesToKilobytes(fileObj.file.size)}</Typography>
      </Box>
      <Box sx={{ display: 'flex', gap: '10px', alignItems: 'center', marginLeft: 'auto' }}>
        <IconButton aria-label='delete' size='medium' onClick={() => handleDeleteFile(fileObj.file.name)}>
          <CancelIcon fontSize='inherit' />
        </IconButton>
      </Box>
    </li>
  ))

  const fileRejectionItems = rejectedFiles.map(({ file, errors }) => (
    <li key={file.name} className='validation-error'>
      {file.name} - {bytesToKilobytes(file.size)}
      <ul>
        {errors.map(e => (
          <li key={e.code}>{getErrorMessage(e.code)}</li>
        ))}
      </ul>
    </li>
  ))

  const documentProcessingReset = () => {
    setAcceptedFiles([])
    setDocumentProcess({
      processDocumentClick: false,
      processingStatus: false,
      processComplete: false,
    })
    setProcessResult(null)
    setDocumentPassword('')
    setUploadError(prevState => ({
      ...prevState,
      error: false,
      message: '',
    }))
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <DocumentTypeSelect
          documentType={documentType}
          handleDocumentTypeChange={handleDocumentTypeChange}
          documentTypeSelectOptions={documentTypeSelectOptions}
        />
        <Divider />
        <Grid item xs={12}>
          <DropZoneWrapper>
            <div
              {...getRootProps({
                className: `dropzone ${dragEnter ? 'active' : ''} ${acceptedFiles?.length > 0 ? 'files' : ''}`,
              })}
            >
              <input id='ocrFileInput' {...getInputProps()} />

              {!acceptedFiles?.length && (
                <>
                  <FolderOpenOutlinedIcon sx={{ fontSize: '4rem', color: '#999' }} />
                  <p>
                    <strong>{t('FILE_UPLOAD_INSTRUCTION', {}, { default: 'Drag a file here to upload' })}</strong>
                  </p>
                  <p style={{ fontSize: '14px' }}>
                    {t('FILE_UPLOAD_INSTRUCTION_MAX_FILE_SIZE', {}, { default: 'Maximum file size: 10MB' })}
                    <br />
                    {t(
                      'FILE_UPLOAD_INSTRUCTION_SUPPORTED_FORMATS',
                      {},
                      { default: 'Supported formats: PDF, JPG, PNG' }
                    )}
                    <br />
                    {t(
                      'FILE_UPLOAD_INSTRUCTION_MAX_UPLOAD_LIMIT',
                      {},
                      { default: 'Maximum upload limit: 10 documents' }
                    )}
                  </p>
                  <Button variant='contained' onClick={open} startIcon={<PublishIcon />}>
                    {t('UPLOAD_DOCUMENTS', {}, { default: 'Upload documents' })}
                  </Button>
                </>
              )}
            </div>
          </DropZoneWrapper>

          <ErrorInfoAlert
            openUploadLimitAlertDialog={openUploadLimitAlertDialog}
            acceptedFiles={acceptedFiles}
            setOpenUploadLimitAlertDialog={setOpenUploadLimitAlertDialog}
          />

          {acceptedFiles?.length > 0 &&
            acceptedFiles?.length < OCR_PORTAL_TYPE.MAX_UPLOAD_LIMIT_COUNT &&
            !documentProcess.processComplete && (
              <Button
                disabled={documentProcess.processingStatus}
                sx={{ marginTop: '30px' }}
                variant='outlined'
                onClick={open}
                startIcon={<PublishIcon />}
              >
                {t('UPLOAD_MORE', {}, { default: 'Upload more' })}
              </Button>
            )}

          <Box>
            {acceptedFiles?.length > 0 && <h4>Documents</h4>}

            <DocumentListWrapper>
              {files} {fileRejectionItems}
            </DocumentListWrapper>

            {fileRejectionItems?.length > 0 && acceptedFiles?.length < 1 && (
              <Button
                variant='contained'
                onClick={() => {
                  setRejectedFiles([])
                }}
              >
                {commonTranslation('RESET_UPLOADS', {}, { default: 'Reset uploads' })}
              </Button>
            )}
          </Box>

          {acceptedFiles?.length > 0 && !documentProcess.processComplete && (
            <Stack direction='row' spacing={4}>
              <Button
                sx={{ display: 'flex', gap: '0 10px' }}
                disabled={
                  documentProcess?.processingStatus || acceptedFiles?.length > OCR_PORTAL_TYPE.MAX_UPLOAD_LIMIT_COUNT
                }
                variant='contained'
                onClick={() => {
                  setDocumentProcess(prevState => ({
                    ...prevState,
                    processDocumentClick: true,
                  }))
                  setRejectedFiles([])
                  setOpenUploadLimitAlertDialog(false)
                }}
              >
                {documentProcess?.processingStatus ? (
                  <>
                    <CircularProgress size={20} variant='indeterminate' />
                    {commonTranslation('PROCESSING', {}, { default: 'Processing' })}
                  </>
                ) : (
                  `${t('PROCESS_DOCUMENT', {}, { default: 'Process document' })}`
                )}
              </Button>

              {fileRejectionItems?.length > 0 && (
                <Button
                  variant='outlined'
                  onClick={() => {
                    setRejectedFiles([])
                  }}
                >
                  {commonTranslation('RESET_UPLOADS', {}, { default: 'Reset uploads' })}
                </Button>
              )}
            </Stack>
          )}

          {documentProcess?.processComplete && acceptedFiles?.length > 0 && (
            <Box sx={{ marginTop: '40px' }}>
              <Box sx={{ marginBottom: '20px' }}>
                {typeof processResult !== 'string' && processResult?.ocrConfidence && (
                  <OcrStatus level={processResult.ocrConfidence} />
                )}

                {typeof processResult === 'string' && <Alert severity='error'>{splitErrorText(processResult)}</Alert>}

                {uploadError.error && <Alert severity='error'>{uploadError.message}</Alert>}
              </Box>
              <Stack spacing={4} direction='row'>
                {typeof processResult !== 'string' && processResult?.ocrConfidence && (
                  <CSVLink
                    style={{ textDecoration: 'none' }}
                    data={processResult?.data as string}
                    filename={`download-${dayjs().format('YYYY-MM-DD-HH:mm:ss')}.csv`}
                    target='_blank'
                    rel='noopener noreferrer'
                  >
                    <Button variant='contained' color='primary' startIcon={<DownloadIcon />}>
                      {commonTranslation('DOWNLOAD', {}, { default: 'Download' })} CSV
                    </Button>
                  </CSVLink>
                )}
                <Button
                  variant='outlined'
                  onClick={() => {
                    documentProcessingReset()
                  }}
                >
                  {commonTranslation('RESET_UPLOADS', {}, { default: 'Reset uploads' })}
                </Button>
              </Stack>
            </Box>
          )}

          <PasswordDialog
            openPasswordDialog={openPasswordDialog}
            handlePasswordDialogClose={handlePasswordDialogClose}
            setOpenPasswordDialog={setOpenPasswordDialog}
            setDocumentPassword={setDocumentPassword}
            documentPassword={documentPassword}
          />
        </Grid>
      </Grid>
    </Grid>
  )
}

export default Index
