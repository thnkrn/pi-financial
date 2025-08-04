// ** React Imports
import { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

// ** Next Import
import Head from 'next/head'

// ** MUI Imports
import TabContext from '@mui/lab/TabContext'
import Box from '@mui/material/Box'
import TabPanel from '@mui/lab/TabPanel'
import Grid from '@mui/material/Grid'
import Card from '@mui/material/Card'
import CircularProgress from '@mui/material/CircularProgress'
import Typography from '@mui/material/Typography'

// ** Components Imports
import { useSessionContext } from '@/context/SessionContext'
import BlocktradeDashboard from './dashboard/Main'
import BlocktradeCalculator from './calculator/Main'
import RegisterModal from '@/views/components/blocktrade/RegisterModal'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { fetchUserData } from '@/store/apps/blocktrade/user'
import BlocktradeAllocation from '@/views/pages/blocktrade/allocation/Main'
import BlocktradePortfolio from '@/views/pages/blocktrade/portfolio/Main'
import BlocktradeActivityLogs from '@/views/pages/blocktrade/activity-logs/Main'
import BlocktradeMonitor from '@/views/pages/blocktrade/monitor/Main'
import BlocktradeMarginReport from '@/views/pages/blocktrade/margin-report/Main'

type Props = {
  tab: string
}

const BlocktradePage = ({ tab }: Props) => {
  const [activeTab, setActiveTab] = useState<string>('dashboard')
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const userStore = useSelector((state: any) => state.btUser)
  const data = useSessionContext()

  useEffect(() => {
    if (tab && tab !== activeTab) {
      setActiveTab(tab)
    }
  }, [tab, activeTab])

  useEffect(() => {
    setIsLoading(false)
    dispatch(fetchUserData())
  }, [dispatch])

  return (
    <Box sx={{ marginTop: -8 }}>
      {userStore.errorMessage && <RegisterModal name={data?.user?.name} />}
      <Head>
        <title>Blocktrade {tab.charAt(0).toUpperCase() + tab.slice(1)} - Pi Securities</title>
      </Head>
      <TabContext value={activeTab}>
        <Grid container spacing={6} sx={{ mt: 0 }}>
          <Grid item xs={12}>
            {isLoading ? (
              <Box sx={{ mt: 6, display: 'flex', alignItems: 'center', flexDirection: 'column' }}>
                <CircularProgress sx={{ mb: 4 }} />
                <Typography>Loading...</Typography>
              </Box>
            ) : (
              <Card>
                <TabPanel sx={{ p: 0 }} value='dashboard'>
                  <BlocktradeDashboard />
                </TabPanel>
                <TabPanel sx={{ p: 0 }} value='allocation'>
                  <BlocktradeAllocation />
                </TabPanel>
                <TabPanel sx={{ p: 0 }} value='portfolio'>
                  <BlocktradePortfolio />
                </TabPanel>
                <TabPanel sx={{ p: 0 }} value='activity-logs'>
                  <BlocktradeActivityLogs />
                </TabPanel>
                <TabPanel sx={{ p: 0 }} value='calculator'>
                  <BlocktradeCalculator />
                </TabPanel>
                <TabPanel sx={{ p: 0 }} value='monitor'>
                  <BlocktradeMonitor />
                </TabPanel>
                <TabPanel sx={{ p: 0 }} value='margin-report'>
                  <BlocktradeMarginReport />
                </TabPanel>
              </Card>
            )}
          </Grid>
        </Grid>
      </TabContext>
    </Box>
  )
}

export default BlocktradePage
