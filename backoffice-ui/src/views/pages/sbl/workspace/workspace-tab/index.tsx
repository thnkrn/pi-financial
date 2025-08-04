import Box from '@mui/material/Box'
import { ReactNode } from 'react'

interface Props {
  index: number
  value: number
  children?: ReactNode
}

const WorkSpaceTab = (props: Props) => {
  const { children, value, index, ...other } = props

  return (
    <div
      role='tabpanel'
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  )
}

export default WorkSpaceTab
