import Layout from '@/layouts/index'
import SBLDashboardPage from '@/views/pages/sbl/dashboard'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'

const SBLDashboard = () => {
  return (
    <Layout title='SBL Dashboard'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title='SBL Dashboard' />
            <CardContent>
              <SBLDashboardPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default SBLDashboard
