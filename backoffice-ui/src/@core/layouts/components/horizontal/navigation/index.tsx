import { LayoutProps } from '@/@core/layouts/types'
import themeConfig from '@/configs/themeConfig'
import Box from '@mui/material/Box'
import HorizontalNavItems from './HorizontalNavItems'

interface Props {
  settings: LayoutProps['settings']
  horizontalNavItems: NonNullable<NonNullable<LayoutProps['horizontalLayoutProps']>['navMenu']>['navItems']
}

const Navigation = (props: Props) => {
  return (
    <Box
      className='menu-content'
      sx={{
        display: 'flex',
        flexWrap: 'wrap',
        alignItems: 'center',
        '& > *': {
          '&:not(:last-child)': { mr: 2 },
          ...(themeConfig.menuTextTruncate && { maxWidth: 220 }),
        },
      }}
    >
      <HorizontalNavItems {...props} />
    </Box>
  )
}

export default Navigation
