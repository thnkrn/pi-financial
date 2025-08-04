import { DEPOSIT_PRODUCT_TYPE } from '@/constants/deposit/type'
import Layout from '@/layouts/index'
import DepositPage from '@/views/pages/deposit'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import Grid from '@mui/material/Grid'

const DepositSetTrade = () => (
  <Layout title='Deposit - Settrade E-Payment'>
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Card>
          <CardHeader
            title='Deposit'
            sx={{ typography: 'h6' }}
            subheader='SetTrade E-Payment (Streaming)'
            subheaderTypographyProps={{
              typography: 'h4',
            }}
          />
          <CardContent>
            <DepositPage productType={DEPOSIT_PRODUCT_TYPE.SetTrade} />
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  </Layout>
)

export default DepositSetTrade
