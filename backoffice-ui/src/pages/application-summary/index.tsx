import Layout from '@/layouts/index'
import ApplicationSummaryPage from '@/views/pages/application-summary'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const ApplicationSummary = () => {
  const { t } = useTranslation('application_summary')

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader
              title={t('APPLICATION_SUMMARY_HEADER', {}, { default: 'Application Summary' })}
              data-testid={'application-summary-header'}
            />
            <CardContent>
              <ApplicationSummaryPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default ApplicationSummary
