// ** React Imports
import { ReactNode } from 'react'

// ** MUI Imports
import { Theme } from '@mui/material/styles'
import useMediaQuery from '@mui/material/useMediaQuery'

// ** Layout Imports
// !Do not remove this Layout import
import Layout from 'src/@core/layouts/Layout'

// ** Navigation Imports
import HorizontalNavItems from 'src/navigation/horizontal'
import VerticalNavItems from 'src/navigation/vertical'

// ** Component Import
// Uncomment the below line (according to the layout type) when using server-side menu
// import ServerSideVerticalNavItems from './components/vertical/ServerSideNavItems'
// import ServerSideHorizontalNavItems from './components/horizontal/ServerSideNavItems'

import HorizontalAppBarContent from './components/horizontal/AppBarContent'
import VerticalAppBarContent from './components/vertical/AppBarContent'

// ** Hook Import
import { useSession } from 'next-auth/react'
import { useSettings } from 'src/@core/hooks/useSettings'
import { SessionProvider } from 'src/context/SessionContext'

interface Props {
  children: ReactNode
  contentHeightFixed?: boolean
  toggleNavVisibility?: any
}

interface IRenderVerticalAppBarContent {
  toggleNavVisibility?: any
}

const UserLayout = ({ children, contentHeightFixed }: Props) => {
  // ** Hooks
  const { settings, saveSettings } = useSettings()

  const { data } = useSession()

  // ** Vars for server side navigation
  // const { menuItems: verticalMenuItems } = ServerSideVerticalNavItems()
  // const { menuItems: horizontalMenuItems } = ServerSideHorizontalNavItems()

  /**
   *  The below variable will hide the current layout menu at given screen size.
   *  The menu will be accessible from the Hamburger icon only (Vertical Overlay Menu).
   *  You can change the screen size from which you want to hide the current layout menu.
   *  Please refer useMediaQuery() hook: https://mui.com/material-ui/react-use-media-query/,
   *  to know more about what values can be passed to this hook.
   *  ! Do not change this value unless you know what you are doing. It can break the template.
   */
  const hidden = useMediaQuery((theme: Theme) => theme.breakpoints.down('lg'))

  const renderHorizontalAppBarContent = () => {
    return <HorizontalAppBarContent settings={settings} saveSettings={saveSettings} />
  }

  const renderVerticalAppBarContent = (props: IRenderVerticalAppBarContent) => {
    return (
      <VerticalAppBarContent
        hidden={hidden}
        settings={settings}
        saveSettings={saveSettings}
        toggleNavVisibility={props.toggleNavVisibility}
      />
    )
  }

  if (!data) return null
  if (hidden && settings.layout === 'horizontal') {
    settings.layout = 'vertical'
  }

  return (
    <SessionProvider session={data}>
      <Layout
        hidden={hidden}
        settings={settings}
        saveSettings={saveSettings}
        contentHeightFixed={contentHeightFixed}
        verticalLayoutProps={{
          navMenu: {
            navItems: VerticalNavItems(),

            // Uncomment the below line when using server-side menu in vertical layout and comment the above line
            // navItems: verticalMenuItems
          },
          appBar: {
            content: props => renderVerticalAppBarContent(props),
          },
        }}
        {...(settings.layout === 'horizontal' && {
          horizontalLayoutProps: {
            navMenu: {
              navItems: HorizontalNavItems(),

              // Uncomment the below line when using server-side menu in horizontal layout and comment the above line
              // navItems: horizontalMenuItems
            },
            appBar: {
              content: () => renderHorizontalAppBarContent(),
            },
          },
        })}
      >
        {children}
      </Layout>
    </SessionProvider>
  )
}

export default UserLayout
