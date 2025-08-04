import AppBar from '@/@core/layouts/components/dashboard-app-bar'
import DashboardFooterBar from '@/@core/layouts/components/dashboard-footer-bar'
import { SessionProvider } from '@/context/SessionContext'
import { Grid } from '@mui/material'
import Box, { BoxProps } from '@mui/material/Box'
import { styled } from '@mui/material/styles'
import { useSession } from 'next-auth/react'

type DashboardBlankLayoutProps = {
  children: React.ReactNode
}

// Styled component for Blank Layout with AppBar component
const DashboardBlankLayoutWrapper = styled(Box)<BoxProps>(({ theme }) => ({
  height: '100vh',

  // For V1 Blank layout pages
  '& .content-center': {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: theme.spacing(5),
    minHeight: `calc(100vh - ${theme.spacing((theme.mixins.toolbar.minHeight as number) / 4)})`,
  },

  // For V2 Blank layout pages
  '& .content-right': {
    display: 'flex',
    overflowX: 'hidden',
    position: 'relative',
    minHeight: `calc(100vh - ${theme.spacing((theme.mixins.toolbar.minHeight as number) / 4)})`,
  },
}))

const DashboardBlankLayout = ({ children }: DashboardBlankLayoutProps) => {
  const { data } = useSession()

  if (!data) return null

  return (
    <DashboardBlankLayoutWrapper>
      <Grid container direction='column' style={{ minHeight: '100vh' }}>
        <Grid item>
          <AppBar />
        </Grid>
        <Grid item xs style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', textAlign: 'center' }}>
          <SessionProvider session={data}>{children}</SessionProvider>
        </Grid>
        <Grid item>
          <DashboardFooterBar />
        </Grid>
      </Grid>
    </DashboardBlankLayoutWrapper>
  )
}

export default DashboardBlankLayout
