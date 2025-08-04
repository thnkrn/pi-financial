import Layout from '@/layouts/index'
import OcrPortalPage from '@/views/pages/ocr-portal'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const OcrPortal = () => {
  const { t } = useTranslation('ocr_portal')

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title={t('OCR_PORTAL_TITLE', {}, { default: 'OCR Portal' })} />
            <CardContent>
              <OcrPortalPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default OcrPortal
