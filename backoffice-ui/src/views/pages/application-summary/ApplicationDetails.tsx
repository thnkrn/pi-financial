import { useSettings } from '@/@core/hooks/useSettings'
import { renderCellWithNA } from '@/@core/utils/text-utilities'
import { Box, Grid, Typography } from '@mui/material'
import dayjs from 'dayjs'
import useTranslation from 'next-translate/useTranslation'
import { CustomerDetailProps, TranslationValueViewModel } from './types'

const CustomerDetail = ({ data }: CustomerDetailProps) => {
  const { settings } = useSettings()
  const { t } = useTranslation('application_summary')

  const personalInfoDataView: Array<TranslationValueViewModel> = [
    {
      key: 'title',
      value: data.title ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_TITLE', {}, { default: 'Title' }),
    },
    {
      key: 'firstNameEn',
      value: data.firstNameEn ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_FIRST_NAME_ENGLISH', {}, { default: 'First name (English)' }),
    },
    {
      key: 'lastNameEn',
      value: data.lastNameEn ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_LAST_NAME_ENGLISH', {}, { default: 'Last name (English)' }),
    },
    {
      key: 'firstNameTh',
      value: data.firstNameTh ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_FIRST_NAME_THAI', {}, { default: 'First name (Thai)' }),
    },
    {
      key: 'lastNameTh',
      value: data.lastNameTh ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_LAST_NAME_THAI', {}, { default: 'Last name (Thai)' }),
    },
    {
      key: 'dateOfBirth',
      value: dayjs(data.dateOfBirth).format('YYYY-MM-DD') ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_DATE_OF_BIRTH', {}, { default: 'Date of birth' }),
    },
    {
      key: 'nationality',
      value: data.nationality ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_NATIONALITY', {}, { default: 'Nationality' }),
    },
    {
      key: 'email',
      value: data.email ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_EMAIL', {}, { default: 'Email' }),
    },
    {
      key: 'phone',
      value: data.phone ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_PHONE', {}, { default: 'Phone' }),
    },
  ]

  const identificationDataView: Array<TranslationValueViewModel> = [
    {
      key: 'custCode',
      value: data.custCode ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_CUSTOMER_CODE', {}, { default: 'Customer code' }),
    },
    {
      key: 'userId',
      value: data.userId ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_USER_ID', {}, { default: 'User ID' }),
    },
    {
      key: 'accountOpeningRequestId',
      value: data.id ?? null,
      translation: t(
        'APPLICATION_SUMMARY_DATA_VIEW_ACCOUNT_OPENING_REQUEST_ID',
        {},
        { default: 'Account Opening Request ID (AOR ID)' }
      ),
    },
    {
      key: 'citizenId',
      value: data.citizenId ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_CITIZEN_ID', {}, { default: 'Citizen Id' }),
    },
    {
      key: 'laserCode',
      value: data.laserCode ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_LASER_CODE', {}, { default: 'Laser code' }),
    },
    {
      key: 'referId',
      value: data.referId ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_REFER_ID', {}, { default: 'Refer ID' }),
    },
    {
      key: 'transId',
      value: data.transId ?? null,
      translation: t('APPLICATION_SUMMARY_DATA_VIEW_TRANS_ID', {}, { default: 'Trans ID' }),
    },
  ]

  return (
    <>
      <Box
        sx={{
          backgroundColor: settings.mode === 'light' ? 'grey.50' : 'customColors.darkBg',
          padding: '16px',
          marginBottom: '16px',
        }}
      >
        <Typography variant='h6' gutterBottom data-testid={'quick-view-personal-info-title'}>
          {t('APPLICATION_SUMMARY_DATA_VIEW_PERSONAL_INFORMATION_HEADER', {}, { default: 'Personal Information' })}
        </Typography>

        <Grid container rowSpacing={5} columnSpacing={{ xs: 1, sm: 2, md: 4 }}>
          {personalInfoDataView.map(item => (
            <Grid item xs={6} md={4} lg={3} key={item.key}>
              <Typography
                variant='subtitle2'
                gutterBottom
                data-testid={`quick-view-personal-info-subtitle-${item.key}`}
              >
                {item.translation}
              </Typography>
              <Typography sx={{ wordBreak: 'break-all' }} data-testid={`quick-view-personal-info-value-${item.key}`}>
                {renderCellWithNA(item.value)}
              </Typography>
            </Grid>
          ))}
        </Grid>
      </Box>

      <Box sx={{ backgroundColor: settings.mode === 'light' ? 'grey.50' : 'customColors.darkBg', padding: '16px' }}>
        <Typography variant='h6' gutterBottom data-testid={'quick-view-identification'}>
          {t('APPLICATION_SUMMARY_DATA_VIEW_IDENTIFICATION_HEADER', {}, { default: 'Identification' })}
        </Typography>

        <Grid container rowSpacing={5} columnSpacing={{ xs: 1, sm: 2, md: 4 }}>
          {identificationDataView.map(item => (
            <Grid item xs={6} md={4} lg={3} key={item.key}>
              <Typography
                variant='subtitle2'
                gutterBottom
                data-testid={`quick-view-identification-subtitle-${item.key}`}
              >
                {item.translation}
              </Typography>
              <Typography sx={{ wordBreak: 'break-all' }} data-testid={`quick-view-identification-value-${item.key}`}>
                {renderCellWithNA(item.value)}
              </Typography>
            </Grid>
          ))}
        </Grid>
      </Box>
    </>
  )
}

export default CustomerDetail
