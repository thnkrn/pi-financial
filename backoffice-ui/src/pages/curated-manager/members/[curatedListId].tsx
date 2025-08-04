import Layout from '@/layouts/index'
import CuratedMemberPage from '@/views/pages/curated-manager/members'
import { Card, CardContent, CardHeader, Grid, Typography } from '@mui/material'
import { useRouter } from 'next/router'

const CuratedMember = () => {
  const router = useRouter()
  const curatedListId = Number(router.query.curatedListId)
  const isValidId = !isNaN(curatedListId) && curatedListId > 0

  return (
    <Layout title='Curated Member'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title='Curated Members' />
            <CardContent>
              {isValidId ? (
                <CuratedMemberPage curatedListId={curatedListId} curatedListName={router.query.name as string} />
              ) : (
                <Typography variant='body1' align='center'>
                  Invalid Curated List ID.
                </Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default CuratedMember
