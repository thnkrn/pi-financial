import { Box } from '@mui/material'
import CardHeader from '@mui/material/CardHeader'

interface PageHeaderProp {
  cardTitle: string | null
}

const PageHeader = ({ cardTitle }: PageHeaderProp) => {
  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        paddingRight: 5,
      }}
    >
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'flex-start',
          alignItems: 'center',
        }}
      >
        <CardHeader title={cardTitle} />
      </Box>
    </Box>
  )
}

export default PageHeader
