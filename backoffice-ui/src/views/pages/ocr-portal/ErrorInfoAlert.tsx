import { OCR_PORTAL_TYPE } from '@/constants/ocr-portal/types'
import { Close as CloseIcon } from '@mui/icons-material'
import { Alert, Collapse, IconButton } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

interface ErrorInfoAlertProps {
  openUploadLimitAlertDialog: boolean
  acceptedFiles: object[]
  setOpenUploadLimitAlertDialog: (arg1: boolean) => void
}
const ErrorInfoAlert = ({
  openUploadLimitAlertDialog,
  acceptedFiles,
  setOpenUploadLimitAlertDialog,
}: ErrorInfoAlertProps) => {
  const { t } = useTranslation('ocr_portal')

  return (
    <Collapse in={openUploadLimitAlertDialog}>
      <Alert
        severity={acceptedFiles.length <= OCR_PORTAL_TYPE.MAX_UPLOAD_LIMIT_COUNT ? 'info' : 'error'}
        action={
          <IconButton
            aria-label='close'
            color='inherit'
            size='small'
            onClick={() => {
              setOpenUploadLimitAlertDialog(false)
            }}
          >
            <CloseIcon fontSize='inherit' />
          </IconButton>
        }
        sx={{ mt: '20px' }}
      >
        <span>
          {t('UPLOAD_LIMIT_REACHED', {}, { default: 'Upload limit of 10 documents reached.' })}{' '}
          {acceptedFiles.length > OCR_PORTAL_TYPE.MAX_UPLOAD_LIMIT_COUNT &&
            `${t('THERE_ARE', {}, { default: 'There are' })} ${acceptedFiles.length} ${t(
              'ITEMS_IN_QUEUE',
              {},
              { default: 'items in queue.' }
            )}`}
        </span>
      </Alert>
    </Collapse>
  )
}

export default ErrorInfoAlert
