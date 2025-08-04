import Layout from '@/layouts/index'
import { Card, CardContent, CardHeader, Grid } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

const ApplicationMetaTraderMT5 = () => {
  const { t } = useTranslation('application_meta_trader')

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader
              title={t('APPLICATION_META_TRADER_HEADER', {}, { default: 'Application MT5' })}
              data-testid={'application-meta-trader-header'}
            />
            <CardContent>
              <iframe
                title='Backoffice Metatrader 5'
                src={`${process.env.NEXT_PUBLIC_BACKOFFICE_ONBOARDING_BASE_URL}/prod/pages/MT5`}
                style={{ flex: 1, border: 0, width: '100%', height: '1000px' }}
                allowFullScreen
                loading='lazy'
              />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default ApplicationMetaTraderMT5
