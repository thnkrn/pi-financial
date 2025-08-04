import { checkRoles } from '@/lib/auth/useUserRole'
import { HorizontalNavItemsType, NavGroup, NavLink } from 'src/@core/layouts/types'
import { useSessionContext } from 'src/context/SessionContext'
import HorizontalNavGroup from './HorizontalNavGroup'
import HorizontalNavLink from './HorizontalNavLink'

interface Props {
  hasParent?: boolean
  horizontalNavItems?: HorizontalNavItemsType
}
const resolveComponent = (item: NavGroup | NavLink) => {
  if ((item as NavGroup).children) return HorizontalNavGroup

  return HorizontalNavLink
}

const HorizontalNavItems = (props: Props) => {
  const data = useSessionContext()
  const userRoles = data.roles

  const RenderMenuItems = props.horizontalNavItems?.map((item: NavGroup | NavLink) => {
    if (!checkRoles(item.requiredRoles, userRoles)) return null
    const TagName: any = resolveComponent(item)

    return <TagName {...props} key={item?.feature} item={item} />
  })

  return <>{RenderMenuItems}</>
}

export default HorizontalNavItems
