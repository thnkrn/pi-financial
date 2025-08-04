import Layout from '@/layouts/index'
import DocumentPortalPage from '@/views/pages/document-portal'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const DocumentPortal = () => {
  const { t } = useTranslation('document_portal')

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader
              title={t('DOCUMENT_PORTAL_HEADER', {}, { default: 'Document Portal' })}
              data-testid={'document-portal-header'}
            />
            <CardContent>
              <DocumentPortalPage />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default DocumentPortal
