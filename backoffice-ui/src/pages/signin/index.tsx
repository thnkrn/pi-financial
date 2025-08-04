import Layout from '@/layouts/index'
import { Box, Button, Container, Typography } from '@mui/material'
import { signIn } from 'next-auth/react'
import { ReactNode } from 'react'
import BlankLayout from 'src/@core/layouts/BlankLayout'

export const SigninLayout = () => {
  return (
    <Layout>
      <Container
        disableGutters
        maxWidth={false}
        sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}
      >
        <Box
          sx={{
            alignItems: 'center',
            display: 'flex',
            flex: 1,
            minHeight: '100%',
          }}
        >
          <Container maxWidth='xs'>
            <Box>
              <Typography color='text.primary' variant='h4'>
                Welcome to Back Office
              </Typography>
              <Typography color='text.secondary' variant='body2' gutterBottom>
                Please login using Pi Securities SSO
              </Typography>
            </Box>

            <Box sx={{ mt: 4 }}>
              <Button
                onClick={e => {
                  e.preventDefault()
                  signIn('keycloak', { callbackUrl: '/dashboard', redirect: true })
                }}
                variant='contained'
                size='large'
                color='primary'
                fullWidth
              >
                Login via SSO
              </Button>
            </Box>
          </Container>
        </Box>
      </Container>
    </Layout>
  )
}

SigninLayout.getLayout = (page: ReactNode) => <BlankLayout>{page}</BlankLayout>

SigninLayout.guestGuard = true

export default SigninLayout
