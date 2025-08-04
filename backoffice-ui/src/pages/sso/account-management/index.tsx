import Layout from '@/layouts/index'
import SSOAccountManagementPage from '@/views/pages/sso/account-management'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const SSOAccountManagement = () => {
  const { t } = useTranslation('application_summary')

  // ใช้ fallback translation
  const pageTitle = t('SSO_ADMIN_ACCOUNT_MANAGEMENT', {}, { fallback: 'Account Management' })

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title={pageTitle} data-testid='sso-admin-account-management-header' />
            <CardContent>
              <SSOAccountManagementPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default SSOAccountManagement
