import Layout from '@/layouts/index'
import { Card, CardContent, Grid } from '@mui/material'
import { useSession } from 'next-auth/react'
import useTranslation from 'next-translate/useTranslation'

const InformationUpdate = () => {
  const session = useSession()
  const permission = session.data?.roles.includes('customer-info-edit') ? 'edit' : 'read'
  const { lang } = useTranslation()
  const url = `${process.env.NEXT_PUBLIC_BACKOFFICE_CUSTOMER_INFORMATION_UPDATE_URL}&user_id=${session.data?.user.id}&name=${session.data?.user.name}&permission=${permission}&language=${lang}`

  return (
    <Layout>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardContent style={{ padding: 0 }}>
              <iframe
                title='Backoffice Customer Information'
                src={url}
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

export default InformationUpdate
