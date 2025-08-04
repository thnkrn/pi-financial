import { OCR_PORTAL_TYPE } from '@/constants/ocr-portal/types'
import { Alert, styled } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const OcrStatusWrapper = styled(Alert)(({ theme }) => ({
  '&.MuiAlert-standardSuccess': {
    color: theme.palette.primary.dark,
    backgroundColor: '#ecf7ed',
    '& .MuiAlert-icon': {
      color: theme.palette.primary.dark,
    },
  },

  '&.MuiAlert-standardError': {
    color: theme.palette.error.dark,
    backgroundColor: '#fee5e5',
    '& .MuiAlert-icon': {
      color: theme.palette.error.dark,
    },
  },

  '& .confidence-result': {
    color: theme.palette.common.black,
  },
}))

const OcrStatus = ({ level }: { level: number }) => {
  const { t } = useTranslation('ocr_portal')
  const successLevel = OCR_PORTAL_TYPE.OCR_SUCCESS_LEVEL

  return (
    <OcrStatusWrapper severity={level >= successLevel ? 'success' : 'error'}>
      <span style={{ fontWeight: 'bold' }}>OCR: </span>
      <span className='confidence-result'>
        {t('CONFIDENCE_LEVEL', {}, { default: 'Confidence level' })}:{' '}
        <span style={{ fontWeight: 'bold' }}>{level * 100}%</span>
      </span>
    </OcrStatusWrapper>
  )
}

export default OcrStatus
