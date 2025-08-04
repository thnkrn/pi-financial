// ** Type import
import { HorizontalNavItemsType } from 'src/@core/layouts/types'

const navigation = (): HorizontalNavItemsType => [
  {
    title: 'Home',
    path: '/home',
    requiredRoles: [],
    icon: 'mdi:home-outline',
  },
  {
    title: 'Deposit',
    path: '/deposit',
    requiredRoles: ['transaction-read'],
    icon: 'mdi:instant-deposit',
  },
  {
    title: 'Withdraw',
    path: '/withdraw',
    requiredRoles: ['transaction-read'],
    icon: 'bx:money-withdraw',
  },
  {
    title: 'Central Workspace',
    path: '/central-workspace',
    requiredRoles: ['ticket-workspace-view'],
    icon: 'fluent-mdl2:work-item',
  },
  {
    title: 'Report',
    path: '/report',
    icon: 'mdi:file-download-outline',
    requiredRoles: ['report-read'],
  },
  {
    title: 'Blocktrade Calculator',
    path: '/blocktrade/calculator',
    icon: 'ph:calculator',
    requiredRoles: [],
  },
]

export default navigation
