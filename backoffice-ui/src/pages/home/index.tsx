import Layout from '@/layouts/index'
import EmailIcon from '@mui/icons-material/Email'
import FaceIcon from '@mui/icons-material/Face'
import PersonIcon from '@mui/icons-material/Person'
import { Card, CardContent, CardHeader, Chip, CircularProgress, Container, Grid, Typography } from '@mui/material'
import { useSession } from 'next-auth/react'
import useTranslation from 'next-translate/useTranslation'

const Home = () => {
  const { data } = useSession()
  const { t } = useTranslation('homepage')

  if (!data) return <CircularProgress />

  return (
    <Layout title='Back Office - Home'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title='Dashboard' />
            <CardContent>
              <Container sx={{ px: 6, py: 3 }} disableGutters maxWidth={false}>
                <Grid container spacing={2}>
                  <Grid item xs={2} textAlign='right'>
                    <Typography variant='h6'>{t('WELCOME_LINE_1', {}, { default: 'Welcome' })}:</Typography>
                  </Grid>
                  <Grid item xs={10}>
                    <Chip
                      key={data.user.name}
                      icon={<PersonIcon fontSize='medium' />}
                      label={
                        <Typography color='text.primary' fontSize='medium' fontWeight='bold'>
                          {data.user.name}
                        </Typography>
                      }
                      size='medium'
                    />
                  </Grid>
                  <Grid item xs={2} textAlign='right'>
                    <Typography variant='h6'>{t('WELCOME_LINE_2', {}, { default: 'Your Email ID' })}:</Typography>
                  </Grid>
                  <Grid item xs={10}>
                    <Chip
                      key={data.user.email}
                      icon={<EmailIcon fontSize='medium' />}
                      label={
                        <Typography color='text.primary' fontSize='medium' fontWeight='bold'>
                          {data.user.email}
                        </Typography>
                      }
                      size='medium'
                    />
                  </Grid>
                  <Grid item xs={2} textAlign='right'>
                    <Typography variant='h6'>{t('WELCOME_LINE_3', {}, { default: 'Your Role Groups' })}:</Typography>
                  </Grid>
                  <Grid item xs={10}>
                    {data.rolesGroup && data.rolesGroup.length > 0 ? (
                      data.rolesGroup.sort().map(role => (
                        <Chip
                          key={role}
                          sx={{ m: 0.5 }}
                          icon={<FaceIcon />}
                          size='medium'
                          label={
                            <Typography color='secondary' fontSize='medium' fontWeight='bold'>
                              {role}
                            </Typography>
                          }
                          color='primary'
                          variant='outlined'
                        />
                      ))
                    ) : (
                      <Chip
                        size='medium'
                        label={
                          <Typography color='error' fontSize='medium' fontWeight='bold' variant='subtitle1'>
                            Sorry.. You don't have any role groups assigned
                          </Typography>
                        }
                        color='secondary'
                        variant='outlined'
                      />
                    )}
                  </Grid>
                  <Grid item xs={2} textAlign='right'>
                    <Typography variant='h6'>{t('WELCOME_LINE_4', {}, { default: 'Your Roles' })}:</Typography>
                  </Grid>
                  <Grid item xs={10}>
                    {data.roles && data.roles.length > 0 ? (
                      data.roles.sort().map(role => (
                        <Chip
                          key={role}
                          sx={{ m: 0.5 }}
                          icon={<FaceIcon />}
                          size='medium'
                          label={
                            <Typography color='secondary' fontSize='medium' fontWeight='bold'>
                              {role}
                            </Typography>
                          }
                          color='primary'
                          variant='outlined'
                        />
                      ))
                    ) : (
                      <Chip
                        size='medium'
                        label={
                          <Typography color='error' fontSize='medium' fontWeight='bold' variant='subtitle1'>
                            Sorry.. You don't have any roles assigned
                          </Typography>
                        }
                        color='secondary'
                        variant='outlined'
                      />
                    )}
                  </Grid>
                </Grid>
              </Container>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default Home
