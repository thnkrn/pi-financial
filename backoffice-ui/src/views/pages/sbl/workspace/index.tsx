import Box from '@mui/material/Box'
import Tab from '@mui/material/Tab'
import Tabs from '@mui/material/Tabs'
import { SyntheticEvent, useState } from 'react'
import WorkSpaceTab from './workspace-tab'
import HistoryTab from './workspace-tab/history'
import PendingTab from './workspace-tab/pending'

const a11yProps = (index: number) => ({
  id: `workspace-tab-${index}`,
  'aria-controls': `workspace-tabpanel-${index}`,
})

interface Props {
  setFetchNotification: (status: boolean) => void
}

const Index = ({ setFetchNotification }: Props) => {
  const [tabValue, setTabValue] = useState(0)

  const handleTabChange = (event: SyntheticEvent, newValue: number) => {
    setTabValue(newValue)
  }

  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Tabs value={tabValue} onChange={handleTabChange} aria-label='workspace tabs'>
          <Tab label='Pending Order' {...a11yProps(0)} />
          <Tab label='History Order' {...a11yProps(1)} />
        </Tabs>
      </Box>
      <WorkSpaceTab value={tabValue} index={0}>
        <PendingTab setFetchNotification={setFetchNotification} />
      </WorkSpaceTab>
      <WorkSpaceTab value={tabValue} index={1}>
        <HistoryTab />
      </WorkSpaceTab>
    </Box>
  )
}

export default Index
