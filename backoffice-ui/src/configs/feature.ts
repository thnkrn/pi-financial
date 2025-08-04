import { filter, includes, isEmpty, map } from 'lodash'

export type FeatureConfig = {
  [name: string]: {
    name: string
    path: string
  }
}

const featureConfig: FeatureConfig = {
  HOME_MENU: {
    name: 'home-menu',
    path: '/home',
  },
  SSO_MENU: {
    name: 'sso-menu',
    path: '',
  },
  SSO_ACCOUNT_MANAGEMENT_MENU: {
    name: 'sso-account-management',
    path: '/sso/account-management',
  },
  SSO_LINK_ACCOUNT_MENU: {
    name: 'sso-link-account',
    path: '/sso/link-account',
  },
  DEPOSIT_MENU: {
    name: 'deposit-menu',
    path: '',
  },
  DEPOSIT_GE_MENU: {
    name: 'deposit-ge-menu',
    path: '/deposit/ge',
  },
  DEPOSIT_NON_GE_MENU: {
    name: 'deposit-non-ge-menu',
    path: '/deposit/non-ge',
  },
  DEPOSIT_SETTRADE_MENU: {
    name: 'deposit-settrade-menu',
    path: '/deposit/settrade',
  },
  WITHDRAW_MENU: {
    name: 'withdraw-menu',
    path: '',
  },
  WITHDRAW_GE_MENU: {
    name: 'withdraw-ge-menu',
    path: '/withdraw/ge',
  },
  WITHDRAW_NON_GE_MENU: {
    name: 'withdraw-non-ge-menu',
    path: '/withdraw/non-ge',
  },
  TRANSFER_MENU: {
    name: 'transfer-menu',
    path: '/transfer-cash/non-ge',
  },
  TRANSFER_CASH_MENU: {
    name: 'transfer-cash-menu',
    path: '/transfer-cash/non-ge',
  },
  CENTRAL_WORKSPACE_MENU: {
    name: 'central-workspace-menu',
    path: '/central-workspace',
  },
  REPORT_MENU: {
    name: 'report-menu',
    path: '',
  },
  REPORT_AUTO_MENU: {
    name: 'report-auto-menu',
    path: '/report/auto',
  },
  REPORT_ON_DEMAND_MENU: {
    name: 'report-on-demand-menu',
    path: '/report/on-demand',
  },
  REPORT_DEPOSIT_WITHDRAW_MENU: {
    name: 'report-pi-app-dw-daily-menu',
    path: '/report/deposit-withdraw',
  },
  BLOCKTRADE_MENU: {
    name: 'blocktrade-menu',
    path: '',
  },
  BLOCKTRADE_DASHBOARD_MENU: {
    name: 'blocktrade-dashboard-menu',
    path: '/blocktrade/dashboard',
  },
  BLOCKTRADE_ALLOCATION_MENU: {
    name: 'blocktrade-allocation-menu',
    path: '/blocktrade/allocation',
  },
  BLOCKTRADE_PORTFOLIO_MENU: {
    name: 'blocktrade-portfolio-menu',
    path: '/blocktrade/portfolio',
  },
  BLOCKTRADE_ACTIVITY_LOGS_MENU: {
    name: 'blocktrade-activity-logs-menu',
    path: '/blocktrade/activity-logs',
  },
  BLOCKTRADE_CALCULATOR_MENU: {
    name: 'blocktrade-calculator-menu',
    path: '/blocktrade/calculator',
  },
  BLOCKTRADE_MONITOR_MENU: {
    name: 'blocktrade-monitor-menu',
    path: '/blocktrade/monitor',
  },
  BLOCKTRADE_MARGIN_MENU: {
    name: 'blocktrade-margin-report-menu',
    path: '/blocktrade/margin-report',
  },
  DOCUMENT_PORTAL_MENU: {
    name: 'document-portal-menu',
    path: '/document-portal',
  },
  APPLICATION_SUMMARY_MENU: {
    name: 'application-summary-menu',
    path: '/application-summary',
  },
  OCR_PORTAL_MENU: {
    name: 'ocr-portal-menu',
    path: '/ocr-portal',
  },
  ATS_REGISTRATION_MENU: {
    name: 'ats-registration-menu',
    path: '',
  },
  ATS_REGISTRATION_PORTAL_MENU: {
    name: 'ats-registration-portal-menu',
    path: '/ats-registration/portal',
  },
  ATS_REGISTRATION_REPORT_MENU: {
    name: 'ats-registration-report-menu',
    path: '/ats-registration/report',
  },
  SBL_MENU: {
    name: 'sbl-menu',
    path: '',
  },
  SBL_DASHBOARD_MENU: {
    name: 'sbl-dashboard-menu',
    path: '/sbl/dashboard',
  },
  SBL_WORKSPACE_MENU: {
    name: 'sbl-workspace-menu',
    path: '/sbl/workspace',
  },
  CURATED_LIST_MANAGER_MENU: {
    name: 'curated-list-manager-menu',
    path: '',
  },
  CURATED_LIST_MANAGER_LIST_MENU: {
    name: 'curated-list-manager-list-menu',
    path: '/curated-manager/list',
  },
  CURATED_LIST_MANAGER_FILTERS_MENU: {
    name: 'curated-list-manager-filters-menu',
    path: '/curated-manager/filters',
  },
  CURATED_LIST_MANAGER_MEMBER_MENU: {
    name: 'curated-list-manager-member-menu',
    path: '/curated-manager/members',
  },
  CUSTOMER_INFORMATION_MENU: {
    name: 'customer-information-menu',
    path: '',
  },
  CUSTOMER_INFORMATION_INFORMATION_UPDATE_MENU: {
    name: 'customer-information-information-update-menu',
    path: '/customer-information/information-update',
  },
  CUSTOMER_INFORMATION_REQUEST_MANAGEMENT_MENU: {
    name: 'customer-information-request-management-menu',
    path: '/customer-information/request-management',
  },
  CME_MENU: {
    name: 'cme-menu',
    path: '/cme-management',
  },
}

export default featureConfig

export function getFeatureName(path: string) {
  const filtered = filter(featureConfig, config => {
    return config.path === path
  })

  return filtered[0]?.name
}

export function getAllFeaturePath() {
  const paths = map(featureConfig, config => {
    return config.path
  })

  return filter(paths, p => !isEmpty(p))
}

export function isFeaturePath(path: string): boolean {
  return includes(getAllFeaturePath(), path)
}
