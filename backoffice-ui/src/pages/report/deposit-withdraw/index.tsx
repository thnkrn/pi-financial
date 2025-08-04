import Layout from '@/layouts/index'
import PiAppDailyReportPage from 'src/views/pages/report/deposit-withdraw'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'

const PiAppDailyReport = () => (
  <Layout title='Deposit Withdraw Report'>
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Card>
          <CardHeader title='Deposit Withdraw Report' />
          <CardContent>
            <PiAppDailyReportPage />
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  </Layout>
)

export default PiAppDailyReport
