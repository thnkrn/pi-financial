import { useSettings } from '@/@core/hooks/useSettings'
import { renderCellWithNA } from '@/@core/utils/text-utilities'
import { formatDate } from '@/utils/fmt'
import DownloadIcon from '@mui/icons-material/Download'
import { Box, Button, Grid, Tab, Tabs, Typography } from '@mui/material'
import dayjs from 'dayjs'
import useTranslation from 'next-translate/useTranslation'
import React from 'react'
import AttachmentsDataGrid from './AttachmentDataTable'
import { AttachmentData, CustomerDetailProps, TabPanelProps, TranslationValueViewModel } from './types'

const CustomerDetailTabPanel = ({ children, value, index }: TabPanelProps) => {
  return (
    <div
      role='tabpanel'
      hidden={value !== index}
      id={`quick-view-tabpanel-${index}`}
      aria-labelledby={`quick-view-tab-${index}`}
    >
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  )
}

const a11yProps = (index: number) => {
  return {
    id: `quick-view-tab-${index}`,
    'aria-controls': `quick-view-tabpanel-${index}`,
  }
}

const CustomerDetail = ({ data, action, downloading }: CustomerDetailProps) => {
  const { settings } = useSettings()
  const { t } = useTranslation('document_portal')

  const [value, setValue] = React.useState(0)

  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue)
  }

  const personalInfoDataView: Array<TranslationValueViewModel> = [
    {
      key: 'title',
      value: data.title ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_TITLE', {}, { default: 'Title' }),
    },
    {
      key: 'firstNameEn',
      value: data.firstNameEn ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_FIRST_NAME_ENGLISH', {}, { default: 'First name (English)' }),
    },
    {
      key: 'lastNameEn',
      value: data.lastNameEn ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_LAST_NAME_ENGLISH', {}, { default: 'Last name (English)' }),
    },
    {
      key: 'firstNameTh',
      value: data.firstNameTh ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_FIRST_NAME_THAI', {}, { default: 'First name (Thai)' }),
    },
    {
      key: 'lastNameTh',
      value: data.lastNameTh ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_LAST_NAME_THAI', {}, { default: 'Last name (Thai)' }),
    },
    {
      key: 'dateOfBirth',
      value: dayjs(data.dateOfBirth).format('YYYY-MM-DD') ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_DATE_OF_BIRTH', {}, { default: 'Date of birth' }),
    },
    {
      key: 'nationality',
      value: data.nationality ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_NATIONALITY', {}, { default: 'Nationality' }),
    },
    {
      key: 'email',
      value: data.email ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_EMAIL', {}, { default: 'Email' }),
    },
    {
      key: 'phone',
      value: data.phone ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_PHONE', {}, { default: 'Phone' }),
    },
  ]

  const identificationDataView: Array<TranslationValueViewModel> = [
    {
      key: 'custCode',
      value: data.custCode ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_CUSTOMER_CODE', {}, { default: 'Customer code' }),
    },
    {
      key: 'userId',
      value: data.userId ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_USER_ID', {}, { default: 'User ID' }),
    },
    {
      key: 'accountOpeningRequestId',
      value: data.id ?? null,
      translation: t(
        'DOCUMENT_PORTAL_DATA_VIEW_ACCOUNT_OPENING_REQUEST_ID',
        {},
        { default: 'Account Opening Request ID (AOR ID)' }
      ),
    },
    {
      key: 'citizenId',
      value: data.citizenId ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_CITIZEN_ID', {}, { default: 'Citizen Id' }),
    },
    {
      key: 'laserCode',
      value: data.laserCode ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_LASER_CODE', {}, { default: 'Laser code' }),
    },
    {
      key: 'idCardExpiryDate',
      value: data.idCardExpiryDate ? formatDate(new Date(data.idCardExpiryDate), { format: 'YYYY-MM-DD' }) : null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_ID_EXPIRY_DATE', {}, { default: 'ID expiry date' }),
    },
    {
      key: 'createdDate',
      value: dayjs(data.createdDate).format('YYYY-MM-DD, HH:mm:ss') ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_CREATED_DATE', {}, { default: 'Created date' }),
    },
    {
      key: 'updatedDate',
      value: dayjs(data.updatedDate).format('YYYY-MM-DD, HH:mm:ss') ?? null,
      translation: t('DOCUMENT_PORTAL_DATA_VIEW_UPDATED_DATE', {}, { default: 'Updated date' }),
    },
  ]

  return (
    <>
      <Tabs value={value} onChange={handleChange}>
        <Tab
          label={t('DOCUMENT_PORTAL_DATA_VIEW_CUSTOMER_INFO', {}, { default: 'Customer info' })}
          {...a11yProps(0)}
          data-testid={'customer-info-tab'}
        />
        <Tab
          label={t('DOCUMENT_PORTAL_DATA_VIEW_ATTACHMENTS', {}, { default: 'Attachments' })}
          {...a11yProps(1)}
          data-testid={'attachments-tab'}
        />
      </Tabs>
      <CustomerDetailTabPanel value={value} index={0}>
        <Box
          sx={{
            backgroundColor: settings.mode === 'light' ? 'grey.50' : 'customColors.darkBg',
            padding: '16px',
            marginBottom: '16px',
          }}
        >
          <Typography variant='h6' gutterBottom data-testid={'quick-view-personal-info-title'}>
            {t('DOCUMENT_PORTAL_DATA_VIEW_PERSONAL_INFORMATION', {}, { default: 'Personal information' })}
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

        <Box
          sx={{
            backgroundColor: settings.mode === 'light' ? 'grey.50' : 'customColors.darkBg',
            padding: '16px',
          }}
        >
          <Typography variant='h6' gutterBottom data-testid={'quick-view-identification'}>
            {t('DOCUMENT_PORTAL_DATA_VIEW_IDENTIFICATION', {}, { default: 'Identification' })}
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
      </CustomerDetailTabPanel>

      <CustomerDetailTabPanel value={value} index={1}>
        <AttachmentsDataGrid documents={data.documents} />
      </CustomerDetailTabPanel>

      <Box sx={{ display: 'flex', width: '100%', justifyContent: 'flex-end' }}>
        <Button
          sx={{ marginTop: '20px' }}
          autoFocus
          disabled={downloading}
          variant='contained'
          onClick={() => action.handleDownloadAttachments(data.documents as AttachmentData[], data.custCode ?? null)}
        >
          <DownloadIcon sx={{ fontSize: '1.25rem', marginRight: '10px', verticalAlign: 'middle' }} />
          {t('c', {}, { default: 'Download attachments' })}
        </Button>
      </Box>
    </>
  )
}

export default CustomerDetail
