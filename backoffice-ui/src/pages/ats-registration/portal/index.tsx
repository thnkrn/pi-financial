import Layout from '@/layouts/index'
import AtsRegistrationPortalPage from '@/views/pages/ats-registration/portal'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const AtsRegistrationPortal = () => {
  const { t } = useTranslation('application_summary')

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader
              title={t('ATS_PORTAL_HEADER', {}, { default: 'ATS Portal' })}
              data-testid={'ats-portal-header'}
            />
            <CardContent>
              <AtsRegistrationPortalPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default AtsRegistrationPortal
