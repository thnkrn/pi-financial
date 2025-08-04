import { Role } from './role';

export const ALL_PATHS_ROLES: Array<{ path: string; roles: Array<Role> }> = [
  {
    path: '/deposit/ge',
    roles: ['transaction-read']
  },
  {
    path: '/deposit/non-ge',
    roles: ['transaction-read']
  },
  {
    path: '/deposit/settrade',
    roles: ['transaction-read']
  },
  {
    path: '/withdraw/ge',
    roles: ['transaction-read']
  },
  {
    path: '/withdraw/non-ge',
    roles: ['transaction-read']
  },
  {
    path: '/transactions',
    roles: ['transaction-read']
  },
  { path: '/central-workspace', roles: ['ticket-workspace-view'] },
  { path: '/dashboard', roles: [] },
  { path: '/blocktrade/dashboard', roles: ['blocktrade-dashboard'] },
  { path: '/blocktrade/allocation', roles: ['blocktrade-allocation']},
  { path: '/blocktrade/portfolio', roles: ['blocktrade-portfolio'] },
  { path: '/blocktrade/activity-logs', roles: ['blocktrade-activitylog'] },
  { path: '/blocktrade/calculator', roles: ['blocktrade-calculator'] },
  { path: '/blocktrade/monitor', roles: ['blocktrade-monitor'] },
  { path: '/document-portal', roles: ['document-portal'] },
  { path: '/application-summary', roles: ['application-summary'] },
  { path: '/ocr-portal', roles: ['ocr-portal'] },
  { path: '/report/auto', roles: ['report-read'] },
  { path: '/report/on-demand', roles: ['report-read'] },
  { path: '/sbl/dashboard', roles: ['sbl-read'] },
  { path: '/sbl/workspace', roles: ['sbl-read']},
  { path: '/sso/account-management', roles: ['sso-read', 'sso-edit']},
  { path: '/api/sso/account-management/changePassword', roles: ['sso-edit']},
  { path: '/api/sso/account-management/changePin', roles: ['sso-edit']},
  { path: '/api/sso/account-management/getAccountInfos', roles: ['sso-read', 'sso-edit']},
  { path: '/api/sso/account-management/sendResetPin', roles: ['sso-edit']},
  { path: '/api/sso/account-management/sendResetPwd', roles: ['sso-edit']},
  { path: '/api/sso/account-management/unlockPin', roles: ['sso-edit']},
  { path: '/api/sso/account-management/unlockUser', roles: ['sso-edit']},
  { path: '/sso/link-account', roles: ['sso-read', 'sso-edit']},
  { path: '/curated-manager', roles: ['curated-manager']},
  // TODO: Update role here if needed
  { path: '/cme-management', roles: ['report-read'] },
]
