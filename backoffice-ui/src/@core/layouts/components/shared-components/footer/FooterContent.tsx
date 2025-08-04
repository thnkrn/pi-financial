import themeConfig from '@/configs/themeConfig'
import Box from '@mui/material/Box'
import Typography from '@mui/material/Typography'

const FooterContent = () => (
  <Box sx={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', justifyContent: 'space-between' }}>
    <Typography sx={{ mr: 2 }}>{`© ${new Date().getFullYear()}, ${themeConfig.title} `}</Typography>
  </Box>
)

export default FooterContent
