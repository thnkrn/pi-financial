import Layout from '@/layouts/index'
import CMEReportPage from '@/views/pages/cme'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'

const CMEManagement = () => {
  return (
    <Layout title='CME Report'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title='CME Report' />
            <CardContent>
              <CMEReportPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default CMEManagement
