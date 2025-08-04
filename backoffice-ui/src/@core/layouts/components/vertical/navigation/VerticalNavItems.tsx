import { LayoutProps, NavGroup, NavLink, NavSectionTitle } from '@/@core/layouts/types'
import { useSessionContext } from '@/context/SessionContext'
import { checkRoles } from '@/lib/auth/useUserRole'
import { IfFeatureEnabled } from '@growthbook/growthbook-react'
import VerticalNavGroup from './VerticalNavGroup'
import VerticalNavLink from './VerticalNavLink'
import VerticalNavSectionTitle from './VerticalNavSectionTitle'

interface Props {
  parent?: NavGroup
  navHover?: boolean
  navVisible?: boolean
  groupActive: string[]
  isSubToSub?: NavGroup
  currentActiveGroup: string[]
  navigationBorderWidth: number
  settings: LayoutProps['settings']
  saveSettings: LayoutProps['saveSettings']
  setGroupActive: (value: string[]) => void
  setCurrentActiveGroup: (item: string[]) => void
  verticalNavItems?: LayoutProps['verticalLayoutProps']['navMenu']['navItems']
}

const resolveNavItemComponent = (item: NavGroup | NavLink | NavSectionTitle) => {
  if ((item as NavSectionTitle).sectionTitle) return VerticalNavSectionTitle
  if ((item as NavGroup).children) return VerticalNavGroup

  return VerticalNavLink
}

const VerticalNavItems = (props: Props) => {
  // ** Props
  const { verticalNavItems } = props

  const data = useSessionContext()
  const userRoles = data.roles

  const RenderMenuItems = verticalNavItems?.map((item: NavGroup | NavLink | NavSectionTitle) => {
    if (!checkRoles(item.requiredRoles, userRoles)) return null
    const TagName: any = resolveNavItemComponent(item)

    if (TagName === VerticalNavGroup || TagName === VerticalNavLink) {
      return (
        <IfFeatureEnabled feature={item?.feature as string} key={`${item?.feature}${item?.title}`}>
          <TagName {...props} key={`${item?.feature}`} item={item} />
        </IfFeatureEnabled>
      )
    }

    return <TagName {...props} key={`${item?.feature}`} item={item} />
  })

  return <>{RenderMenuItems}</>
}

export default VerticalNavItems
