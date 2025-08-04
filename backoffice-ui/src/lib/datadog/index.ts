import { datadogLogs } from '@datadog/browser-logs'
import { datadogRum } from '@datadog/browser-rum'
import getConfig from 'next/config'

export const initDatadogRum = () => {
  const { publicRuntimeConfig } = getConfig()
  datadogRum.init({
    applicationId: process.env.NEXT_PUBLIC_DATADOG_APPLICATION_ID ?? '',
    clientToken: process.env.NEXT_PUBLIC_DATADOG_CLIENT_TOKEN ?? '',
    site: 'datadoghq.com',
    service: 'backoffice-ui',
    env: process.env.NEXT_PUBLIC_DATADOG_ENVIRONMENT ?? 'staging',
    version: publicRuntimeConfig?.version ?? '',
    sessionSampleRate: 100,
    sessionReplaySampleRate: 20,
    trackUserInteractions: true,
    trackResources: true,
    trackLongTasks: true,
    defaultPrivacyLevel: 'mask-user-input',
  })
}

export const initDatadogLogs = () => {
  const { publicRuntimeConfig } = getConfig()
  datadogLogs.init({
    clientToken: process.env.NEXT_PUBLIC_DATADOG_CLIENT_TOKEN ?? '',
    site: 'datadoghq.com',
    service: 'pi.backoffice.ui',
    env: process.env.NEXT_PUBLIC_DATADOG_ENVIRONMENT ?? 'staging',
    version: publicRuntimeConfig?.version ?? '',
    forwardErrorsToLogs: true,
    sessionSampleRate: 100,
  })
}
