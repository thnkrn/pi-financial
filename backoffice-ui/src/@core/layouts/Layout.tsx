import { useEffect, useRef } from 'react'
import { LayoutProps } from 'src/@core/layouts/types'
import HorizontalLayout from './HorizontalLayout'
import VerticalLayout from './VerticalLayout'

const Layout = (props: LayoutProps) => {
  // ** Props
  const { hidden, children, settings, saveSettings } = props

  // ** Ref
  const isCollapsed = useRef(settings.navCollapsed)

  useEffect(() => {
    if (hidden) {
      if (settings.navCollapsed) {
        saveSettings({ ...settings, navCollapsed: false, layout: 'vertical' })
        isCollapsed.current = true
      }
    } else if (isCollapsed.current) {
      saveSettings({ ...settings, navCollapsed: true, layout: settings.lastLayout })
      isCollapsed.current = false
    } else if (settings.lastLayout !== settings.layout) {
      saveSettings({ ...settings, layout: settings.lastLayout })
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hidden])

  if (settings.layout === 'horizontal') {
    return <HorizontalLayout {...props}>{children}</HorizontalLayout>
  }

  return <VerticalLayout {...props}>{children}</VerticalLayout>
}

export default Layout
