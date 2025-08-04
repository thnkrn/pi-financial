import Layout from '@/layouts/index'
import AutoReportPage from '@/views/pages/report/auto'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'

const ReportAuto = () => {
  return (
    <Layout title='Auto Report'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title='Auto Report' />
            <CardContent>
              <AutoReportPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default ReportAuto
