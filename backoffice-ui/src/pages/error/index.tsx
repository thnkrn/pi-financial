import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import ArrowBackIosNewIcon from '@mui/icons-material/ArrowBackIosNew'
import RestartAltIcon from '@mui/icons-material/RestartAlt'
import { Box, Button, Container, Stack, Typography } from '@mui/material'

import { signOut } from 'next-auth/react'
import { useRouter } from 'next/router'
import { ReactNode } from 'react'
import BlankLayout from 'src/@core/layouts/BlankLayout'

export const ErrorFallback = () => {
  const router = useRouter()
  const { reason } = router.query

  let errorTitle: string
  let errorDescription: string
  let errorLinkType: 'unauthorized' | 'retry' | 'logout'

  if (!reason) return null

  if (reason === 'unauthorized') {
    errorTitle = '401 Unauthorized'
    errorDescription = 'You session has been expired. Please relogin.'
    errorLinkType = 'unauthorized'
  } else if (reason === 'forbidden') {
    errorTitle = '403 Forbidden'
    errorDescription = `You do not have permission for this, please check with admin then relogin`
    errorLinkType = 'logout'
  } else {
    errorTitle = 'Unexpected Error: ' + reason
    errorDescription = JSON.stringify(reason)
    errorLinkType = 'retry'
  }

  const handleClick = () => {
    if (errorLinkType === 'unauthorized') {
      router.replace('/signin')
    }
  }

  return (
    <Container
      disableGutters
      maxWidth={false}
      sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}
    >
      <Box
        component='main'
        sx={{
          alignItems: 'center',
          display: 'flex',
          flexGrow: 1,
          minHeight: '100%',
        }}
      >
        <Container maxWidth='md'>
          <Box
            sx={{
              alignItems: 'center',
              display: 'flex',
              flexDirection: 'column',
            }}
          >
            <Typography align='center' color='textPrimary' variant='h1'>
              {errorTitle}
            </Typography>
            <Typography align='center' color='textPrimary' variant='subtitle2'>
              {errorDescription}
            </Typography>
            {errorLinkType !== 'logout' && (
              <Button
                startIcon={
                  errorLinkType === 'unauthorized' ? (
                    <ArrowBackIcon fontSize='small' />
                  ) : (
                    <RestartAltIcon fontSize='small' />
                  )
                }
                sx={{ mt: 3 }}
                variant='contained'
                onClick={handleClick}
              >
                {errorLinkType === 'unauthorized' ? 'Login' : 'Retry'}
              </Button>
            )}
            {errorLinkType === 'logout' && (
              <Stack direction='row' spacing={2} sx={{ mt: 3 }} alignItems='flex-start'>
                <Button
                  onClick={() => router.back()}
                  startIcon={<ArrowBackIosNewIcon fontSize='small' />}
                  color='primary'
                  variant='contained'
                >
                  Back
                </Button>
                <Button onClick={() => signOut({ callbackUrl: '/signin' })} color='primary' variant='contained'>
                  Log out
                </Button>
              </Stack>
            )}
          </Box>
        </Container>
      </Box>
    </Container>
  )
}

ErrorFallback.getLayout = (page: ReactNode) => <BlankLayout>{page}</BlankLayout>

ErrorFallback.guestGuard = true

export default ErrorFallback
