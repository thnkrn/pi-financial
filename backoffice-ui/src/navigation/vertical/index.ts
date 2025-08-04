// ** Type import
import featureConfig from '@/configs/feature'
import { NavLink, VerticalNavItemsType } from 'src/@core/layouts/types'

const navigation = (): VerticalNavItemsType => {
  return [
    {
      title: 'Home',
      path: '/home',
      icon: 'mdi:home-outline',
      requiredRoles: [],
      feature: featureConfig.HOME_MENU.name,
      testId: 'hamburger-menu-home',
    },
    {
      title: 'Deposit',
      icon: 'mdi:instant-deposit',
      testId: 'hamburger-menu-deposit',
      children: [
        {
          title: 'Thai Equity & TFEX',
          path: '/deposit/non-ge',
          requiredRoles: ['transaction-read'],
          feature: featureConfig.DEPOSIT_NON_GE_MENU.name,
          testId: 'hamburger-menu-deposit-non-ge',
        },
        {
          title: 'Global Equity',
          path: '/deposit/ge',
          requiredRoles: ['transaction-read'],
          feature: featureConfig.DEPOSIT_GE_MENU.name,
          testId: 'hamburger-menu-deposit-ge',
        },
        {
          title: 'SetTrade E-Payment',
          path: '/deposit/settrade',
          requiredRoles: ['transaction-read'],
          feature: featureConfig.DEPOSIT_SETTRADE_MENU.name,
          testId: 'hamburger-menu-deposit-set-trade',
        },
      ] as NavLink[],
      requiredRoles: ['transaction-read'],
      feature: featureConfig.DEPOSIT_MENU.name,
    },
    {
      title: 'Withdraw',
      icon: 'bx:money-withdraw',
      testId: 'hamburger-menu-withdraw',
      children: [
        {
          title: 'Thai Equity & TFEX',
          path: '/withdraw/non-ge',
          requiredRoles: ['transaction-read'],
          feature: featureConfig.WITHDRAW_NON_GE_MENU.name,
          testId: 'hamburger-menu-withdraw-non-ge',
        },
        {
          title: 'Global Equity',
          path: '/withdraw/ge',
          requiredRoles: ['transaction-read'],
          feature: featureConfig.WITHDRAW_GE_MENU.name,
          testId: 'hamburger-menu-withdraw-ge',
        },
      ] as NavLink[],
      requiredRoles: ['transaction-read'],
      feature: featureConfig.WITHDRAW_MENU.name,
    },
    {
      title: 'Transfer',
      icon: 'mdi:instant-deposit',
      testId: 'hamburger-menu-withdraw',
      children: [
        {
          title: 'Transfer Cash',
          path: '/transfer-cash/paginate',
          requiredRoles: ['transaction-read'],
          feature: featureConfig.TRANSFER_CASH_MENU.name,
          testId: 'hamburger-menu-withdraw-non-ge',
        },
      ] as NavLink[],
      requiredRoles: ['transaction-read'],
      feature: featureConfig.TRANSFER_MENU.name,
    },
    {
      title: 'Report',
      icon: 'mdi:file-download-outline',
      children: [
        {
          title: 'On-Demand report',
          path: '/report/on-demand',
          requiredRoles: ['report-read'],
          feature: featureConfig.REPORT_ON_DEMAND_MENU.name,
        },
        {
          title: 'Auto Report',
          path: '/report/auto',
          requiredRoles: ['report-read'],
          feature: featureConfig.REPORT_AUTO_MENU.name,
        },
        {
          title: 'Deposit Withdraw report',
          path: '/report/deposit-withdraw',
          requiredRoles: ['report-read'],
          feature: featureConfig.REPORT_DEPOSIT_WITHDRAW_MENU.name,
        },
      ] as NavLink[],
      requiredRoles: ['report-read'],
      feature: featureConfig.REPORT_MENU.name,
    },
    {
      title: 'Central Workspace',
      path: '/central-workspace',
      icon: 'fluent-mdl2:work-item',
      requiredRoles: ['ticket-workspace-view'],
      feature: featureConfig.CENTRAL_WORKSPACE_MENU.name,
      testId: 'hamburger-menu-central-workspace',
    },
    {
      title: 'SBL Management',
      icon: 'mdi:finance',
      children: [
        {
          title: 'Dashboard',
          path: '/sbl/dashboard',
          requiredRoles: ['sbl-read'],
          feature: featureConfig.SBL_DASHBOARD_MENU.name,
        },
        {
          title: 'Workspace',
          path: '/sbl/workspace',
          requiredRoles: ['sbl-read'],
          feature: featureConfig.SBL_WORKSPACE_MENU.name,
        },
      ] as NavLink[],
      requiredRoles: ['sbl-read'],
      feature: featureConfig.REPORT_MENU.name,
    },
    {
      title: 'CME',
      path: '/cme-management',
      icon: 'mdi:file-chart',

      // TODO: Update role here if needed
      requiredRoles: ['report-read'],
      feature: featureConfig.CME_MENU.name,
    },
    {
      title: 'Blocktrade Dashboard',
      path: '/blocktrade/dashboard',
      icon: 'ic:outline-dashboard',
      requiredRoles: ['blocktrade-dashboard'],
      feature: featureConfig.BLOCKTRADE_DASHBOARD_MENU.name,
      testId: 'hamburger-menu-blocktrade-dashboard',
    },
    {
      title: 'Blocktrade Allocation',
      path: '/blocktrade/allocation',
      icon: 'icon-park-outline:order',
      requiredRoles: ['blocktrade-allocation'],
      feature: featureConfig.BLOCKTRADE_ALLOCATION_MENU.name,
      testId: 'hamburger-menu-blocktrade-allocation',
    },
    {
      title: 'Blocktrade Portfolio',
      path: '/blocktrade/portfolio',
      icon: 'material-symbols:folder-shared-outline',
      requiredRoles: ['blocktrade-portfolio'],
      feature: featureConfig.BLOCKTRADE_PORTFOLIO_MENU.name,
      testId: 'hamburger-menu-blocktrade-portfolio',
    },
    {
      title: 'Blocktrade Activity Logs',
      path: '/blocktrade/activity-logs',
      icon: 'eva:activity-outline',
      requiredRoles: ['blocktrade-activitylog'],
      feature: featureConfig.BLOCKTRADE_ACTIVITY_LOGS_MENU.name,
      testId: 'hamburger-menu-blocktrade-activity-logs',
    },
    {
      title: 'Blocktrade Calculator',
      path: '/blocktrade/calculator',
      icon: 'ph:calculator',
      requiredRoles: ['blocktrade-calculator'],
      feature: featureConfig.BLOCKTRADE_CALCULATOR_MENU.name,
      testId: 'hamburger-menu-blocktrade-calculator',
    },
    {
      title: 'Blocktrade Monitor',
      path: '/blocktrade/monitor',
      icon: 'pajamas:monitor',
      requiredRoles: ['blocktrade-monitor'],
      feature: featureConfig.BLOCKTRADE_MONITOR_MENU.name,
      testId: 'hamburger-menu-blocktrade-monitor',
    },
    {
      title: 'Blocktrade Margin Report',
      path: '/blocktrade/margin-report',
      icon: 'mdi:file-chart',
      requiredRoles: ['blocktrade-marginreport'],
      feature: featureConfig.BLOCKTRADE_MARGIN_MENU.name,
      testId: 'hamburger-menu-blocktrade-margin-report',
    },
    {
      title: 'Document Portal',
      path: '/document-portal',
      icon: 'mdi:text-box-outline',
      requiredRoles: ['document-portal'],
      feature: featureConfig.DOCUMENT_PORTAL_MENU.name,
      testId: 'hamburger-menu-document-portal',
    },
    {
      title: 'Application Summary',
      path: '/application-summary',
      icon: 'mdi:sync',
      requiredRoles: ['application-summary'],
      feature: featureConfig.APPLICATION_SUMMARY_MENU.name,
      testId: 'hamburger-menu-application-summary',
    },
    {
      title: 'Onboarding',
      icon: 'mdi:file-download-outline',
      children: [
        {
          title: 'Application MT4',
          path: '/application-meta-trader-4',
          requiredRoles: ['application-summary'],
          feature: featureConfig.APPLICATION_SUMMARY_MENU.name,
        },
        {
          title: 'Application MT5',
          path: '/application-meta-trader-5',
          requiredRoles: ['application-summary'],
          feature: featureConfig.APPLICATION_SUMMARY_MENU.name,
        },
      ] as NavLink[],
      requiredRoles: ['application-summary'],
      feature: featureConfig.APPLICATION_SUMMARY_MENU.name,
      testId: 'hamburger-menu-application-meta-trader-4',
    },
    {
      title: 'OCR Portal',
      path: '/ocr-portal',
      icon: 'mdi:text-box-outline',
      requiredRoles: ['ocr-portal'],
      feature: featureConfig.OCR_PORTAL_MENU.name,
    },
    {
      title: 'ATS Registration',
      icon: 'mdi:text-box-outline',
      children: [
        {
          title: 'ATS Portal',
          path: '/ats-registration/portal',
          requiredRoles: ['application-summary'],
          feature: featureConfig.ATS_REGISTRATION_PORTAL_MENU.name,
          testId: 'hamburger-menu-ats-registration-portal',
        },
        {
          title: 'ATS Report',
          path: '/ats-registration/report',
          requiredRoles: ['application-summary'],
          feature: featureConfig.ATS_REGISTRATION_REPORT_MENU.name,
          testId: 'hamburger-menu-ats-registration-report',
        },
      ] as NavLink[],
      requiredRoles: ['application-summary'],
      feature: featureConfig.ATS_REGISTRATION_MENU.name,
    },
    {
      title: 'SSO Admin',
      icon: 'mdi:insert-emoticon',
      testId: 'hamburger-menu-ssoadmin-account-management',
      children: [
        {
          title: 'Account Management',
          path: '/sso/account-management',
          requiredRoles: ['sso-read'],
          feature: featureConfig.SSO_ACCOUNT_MANAGEMENT_MENU.name,
        },
        {
          title: 'Link Account',
          path: '/sso/link-account',
          requiredRoles: ['sso-read'],
          feature: featureConfig.SSO_LINK_ACCOUNT_MENU.name,
        },
      ] as NavLink[],
      requiredRoles: ['sso-read'],
      feature: featureConfig.SSO_MENU.name,
    },
    {
      title: 'Curated Manager',
      icon: 'mdi:file-eye-outline',
      children: [
        {
          title: 'Curated List',
          path: featureConfig.CURATED_LIST_MANAGER_LIST_MENU.path,
          requiredRoles: ['curated-manager'],
          feature: featureConfig.CURATED_LIST_MANAGER_LIST_MENU.name,
        },
        {
          title: 'Curated Filters',
          path: featureConfig.CURATED_LIST_MANAGER_FILTERS_MENU.path,
          requiredRoles: ['curated-manager'],
          feature: featureConfig.CURATED_LIST_MANAGER_FILTERS_MENU.name,
        },
      ] as NavLink[],
      requiredRoles: ['curated-manager'],
      feature: featureConfig.CURATED_LIST_MANAGER_MENU.name,
    },
    {
      title: 'Customer Information',
      icon: 'garden:customer-lists-fill-26',
      children: [
        {
          title: 'Information Update',
          path: '/customer-information/information-update',
          requiredRoles: ['customer-info-read', 'customer-info-edit'],
          feature: featureConfig.CUSTOMER_INFORMATION_INFORMATION_UPDATE_MENU.name,
        },
        {
          title: 'Request Management',
          path: '/customer-information/request-management',
          requiredRoles: ['customer-info-edit'],
          feature: featureConfig.CUSTOMER_INFORMATION_REQUEST_MANAGEMENT_MENU.name,
        },
      ] as NavLink[],
      requiredRoles: ['customer-info-read', 'customer-info-edit'],
      feature: featureConfig.CUSTOMER_INFORMATION_MENU.name,
    },
  ]
}

export default navigation
