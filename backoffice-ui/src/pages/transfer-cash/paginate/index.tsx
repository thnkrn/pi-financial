import Layout from '@/layouts/index'
import TransferCashPage from '@/views/pages/transfer-cash'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import Grid from '@mui/material/Grid'

const TransferCash = () => (
  <Layout title='Transfer Cash'>
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Card>
          <CardHeader
            title='Transfer'
            sx={{ typography: 'h6' }}
            subheader='Transfer Cash'
            subheaderTypographyProps={{
              typography: 'h4',
            }}
          />
          <CardContent>
            <TransferCashPage />
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  </Layout>
)

export default TransferCash
