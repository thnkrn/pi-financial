import { Card, CardContent, CardHeader, Grid } from '@mui/material'

import Layout from '@/layouts/index'
import CuratedListPage from '@/views/pages/curated-manager/list'

const CuratedManagerList = () => (
  <Layout title='Curated List Manager'>
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Card>
          <CardHeader title='Curated List' />
          <CardContent>
            <CuratedListPage />
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  </Layout>
)

export default CuratedManagerList
