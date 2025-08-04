import Layout from '@/layouts/index'
import SSOLinkAccountPage from '@/views/pages/sso/link-account'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'

const SSOAccountManagement = () => {
  // ใช้ fallback translation
  const pageTitle = 'Link Account'

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title={pageTitle} />
            <CardContent>
              <SSOLinkAccountPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default SSOAccountManagement
