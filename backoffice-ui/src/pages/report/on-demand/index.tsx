import Layout from '@/layouts/index'
import OnDemandReportPage from '@/views/pages/report/on-demand'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'

const ReportOnDemand = () => {
  return (
    <Layout title='On-Demand Report'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title='On-Demand Report'></CardHeader>
            <CardContent>
              <OnDemandReportPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default ReportOnDemand
