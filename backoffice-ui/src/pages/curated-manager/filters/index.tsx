import { Card, CardContent, CardHeader, Grid } from '@mui/material'

import Layout from '@/layouts/index'
import CuratedFiltersPage from '@/views/pages/curated-manager/filters'

const CuratedManagerFilters = () => (
  <Layout title='Curated Filters'>
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Card>
          <CardHeader title='Curated Filters' />
          <CardContent>
            <CuratedFiltersPage />
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  </Layout>
)

export default CuratedManagerFilters
