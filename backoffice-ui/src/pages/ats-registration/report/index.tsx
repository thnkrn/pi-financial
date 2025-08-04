import Layout from '@/layouts/index'
import AtsRegistrationReportPage from '@/views/pages/ats-registration/report'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const AtsRegistrationReport = () => {
  const { t } = useTranslation('application_summary')

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader
              title={t('ATS_REPORT_HEADER', {}, { default: 'ATS Report' })}
              data-testid={'ats-report-header'}
            />
            <CardContent>
              <AtsRegistrationReportPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default AtsRegistrationReport
