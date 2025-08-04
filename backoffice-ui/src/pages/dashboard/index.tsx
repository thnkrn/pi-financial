import Layout from '@/layouts/index'
import { useUserRole } from '@/lib/auth/useUserRole'
import AccountBoxIcon from '@mui/icons-material/AccountBox'
import GridViewIcon from '@mui/icons-material/GridView'
import ManageAccountsIcon from '@mui/icons-material/ManageAccounts'
import SettingsIcon from '@mui/icons-material/Settings'
import ShowChartIcon from '@mui/icons-material/ShowChart'
import { CircularProgress, Grid, Typography } from '@mui/material'
import Box from '@mui/material/Box'
import Link from '@mui/material/Link'
import Stack from '@mui/material/Stack'
import useTranslation from 'next-translate/useTranslation'
import { default as NextLink } from 'next/link'
import { ReactNode } from 'react'
import { Visible } from 'src/@core/components/auth/Visible'
import DashboardBlankLayout from 'src/@core/layouts/DashboardBlankLayout'
import { useSessionContext } from 'src/context/SessionContext'

const Dashboard = () => {
  const data = useSessionContext()
  const setAccountOpeningUrl = useUserRole('document-portal') ? '/document-portal' : '/application-summary'
  const { t } = useTranslation('dashboard')

  if (!data) return <CircularProgress />

  return (
    <Layout title={'Back Office - Dashboard'}>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Box sx={{ textAlign: 'center' }}>
            <Typography variant='h5'>
              {t('DASHBOARD_GREETING', {}, { default: 'Hi' })} {data.user.name},{' '}
              {t('DASHBOARD_WELCOME', {}, { default: 'welcome back' })}!
            </Typography>
          </Box>
          <Stack
            direction={'row'}
            justifyContent='center'
            spacing={2}
            sx={{ maxWidth: '280px', margin: '20px auto 0', gap: '2rem' }}
          >
            <Visible allowedRoles={['application-summary', 'document-portal']}>
              <Link
                href={setAccountOpeningUrl}
                underline={'none'}
                color='inherit'
                sx={{
                  textAlign: 'center',
                  '&:hover': {
                    color: 'text.secondary',
                  },
                }}
                component={NextLink}
              >
                <AccountBoxIcon sx={{ fontSize: '4rem' }} />
                <Typography> {t('DASHBOARD_ACCOUNT_OPENING', {}, { default: 'Account Opening' })}</Typography>
              </Link>
            </Visible>
            <Visible allowedRoles={['transaction-read', 'ticket-workspace-view']}>
              <Link
                href='/home'
                underline={'none'}
                color='inherit'
                sx={{
                  textAlign: 'center',
                  '&:hover': {
                    color: 'text.secondary',
                  },
                }}
                component={NextLink}
              >
                <SettingsIcon sx={{ fontSize: '4rem' }} />
                <Typography>
                  {t('DASHBOARD_SETTLEMENT_OPERATIONS', {}, { default: 'Settlement Operations' })}
                </Typography>
              </Link>
            </Visible>
            <Visible
              allowedRoles={[
                'blocktrade-dashboard',
                'blocktrade-allocation',
                'blocktrade-portfolio',
                'blocktrade-activitylog',
                'blocktrade-calculator',
              ]}
            >
              <Link
                href='/blocktrade/dashboard'
                underline={'none'}
                color='inherit'
                sx={{
                  textAlign: 'center',
                  '&:hover': {
                    color: 'text.secondary',
                  },
                }}
                component={NextLink}
              >
                <GridViewIcon sx={{ fontSize: '4rem' }} />
                <Typography>{t('DASHBOARD__BLOCKTRADE', {}, { default: 'Blocktrade System' })}</Typography>
              </Link>
            </Visible>
            <Visible allowedRoles={['sbl-read', 'sso-edit']}>
              <Link
                href='/home'
                underline={'none'}
                color='inherit'
                sx={{
                  textAlign: 'center',
                  '&:hover': {
                    color: 'text.secondary',
                  },
                }}
                component={NextLink}
              >
                <ShowChartIcon sx={{ fontSize: '4rem' }} />
                <Typography>{t('DASHBOARD_SBL_MANAGEMENT', {}, { default: 'SBL Management' })}</Typography>
              </Link>
            </Visible>
            <Visible allowedRoles={['sso-read', 'sso-edit']}>
              <Link
                href='/sso/account-management'
                underline={'none'}
                color='inherit'
                sx={{
                  textAlign: 'center',
                  '&:hover': {
                    color: 'text.secondary',
                  },
                }}
                component={NextLink}
              >
                <ManageAccountsIcon sx={{ fontSize: '4rem' }} />
                <Typography>{t('DASHBOARD_SSO_ADMIN', {}, { default: 'SSO ADMIN' })}</Typography>
              </Link>
            </Visible>
            <Visible allowedRoles={['curated-manager']}>
              <Link
                href='/home'
                underline={'none'}
                color='inherit'
                sx={{
                  textAlign: 'center',
                  '&:hover': {
                    color: 'text.secondary',
                  },
                }}
                component={NextLink}
              >
                <ShowChartIcon sx={{ fontSize: '4rem' }} />
                <Typography>
                  {t('DASHBOARD_MARKET_DATA_MANAGEMENT', {}, { default: 'Market Data Management' })}
                </Typography>
              </Link>
            </Visible>
          </Stack>
        </Grid>
      </Grid>
    </Layout>
  )
}

Dashboard.getLayout = (page: ReactNode) => <DashboardBlankLayout>{page}</DashboardBlankLayout>

export default Dashboard
