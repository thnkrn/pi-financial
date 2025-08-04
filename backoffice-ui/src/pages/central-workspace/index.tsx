import Layout from '@/layouts/index'
import CentralWorkspacePage from '@/views/pages/central-workspace'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const CentralWorkspace = () => {
  const { t } = useTranslation('central_workspace')

  return (
    <Layout title='Central Workspace'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title={t('CENTRAL_WORKSPACE_HEADER', {}, { default: 'Central Workspace' })} />
            <CardContent>
              <CentralWorkspacePage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default CentralWorkspace
