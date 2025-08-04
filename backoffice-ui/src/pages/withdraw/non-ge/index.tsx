import { WITHDRAW_PRODUCT_TYPE } from '@/constants/withdraw/type'
import Layout from '@/layouts/index'
import WithdrawPage from '@/views/pages/withdraw'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import Grid from '@mui/material/Grid'

const WithdrawNonGE = () => {
  return (
    <Layout title='Withdraw - Thai Equity & TFEX'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader
              title='Withdraw'
              sx={{ typography: 'h6' }}
              subheader='Thai Equity & TFEX'
              subheaderTypographyProps={{
                typography: 'h4',
              }}
            ></CardHeader>
            <CardContent>
              <WithdrawPage productType={WITHDRAW_PRODUCT_TYPE.NonGlobalEquity} />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default WithdrawNonGE
