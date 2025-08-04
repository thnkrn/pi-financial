// ** React Imports

// ** Next Imports
import type { NextPage } from 'next'
import { SessionProvider } from 'next-auth/react'
import type { AppProps } from 'next/app'
import { Router, useRouter } from 'next/router'

// ** Store Imports
import { Provider } from 'react-redux'
import { store } from 'src/store'

// ** Loader Import
import NProgress from 'nprogress'

// ** Emotion Imports
import type { EmotionCache } from '@emotion/cache'
import { CacheProvider } from '@emotion/react'

// ** Config Imports

import themeConfig from 'src/configs/themeConfig'

// ** Third Party Import
import { Toaster } from 'react-hot-toast'

// ** Component Imports
import ThemeComponent from 'src/@core/theme/ThemeComponent'
import UserLayout from 'src/layouts/UserLayout'

// ** Contexts
import { SettingsConsumer, SettingsProvider } from 'src/@core/context/settingsContext'

// ** Styled Components
import ReactHotToast from 'src/@core/styles/libs/react-hot-toast'

// ** Utils Imports
import { createEmotionCache } from 'src/@core/utils/create-emotion-cache'

// ** Prismjs Styles
import 'prismjs'
import 'prismjs/components/prism-jsx'
import 'prismjs/components/prism-tsx'
import 'prismjs/themes/prism-tomorrow.css'

// ** React Perfect Scrollbar Style
import 'react-perfect-scrollbar/dist/css/styles.css'

import 'src/iconify-bundle/icons-bundle-react'

// ** Global css styles
import { SnackbarProvider } from 'notistack'
import '../../styles/globals.css'

// ** Extend App Props with Emotion
type ExtendedAppProps = AppProps & {
  Component: NextPage
  emotionCache: EmotionCache
}

const clientSideEmotionCache = createEmotionCache()

// ** Pace Loader
if (themeConfig.routingLoader) {
  Router.events.on('routeChangeStart', () => {
    NProgress.start()
  })
  Router.events.on('routeChangeError', () => {
    NProgress.done()
  })
  Router.events.on('routeChangeComplete', () => {
    NProgress.done()
  })
}

import defaultFeatureJson from '@/configs/default_feature.json'
import { getFeatureName, isFeaturePath } from '@/configs/feature'
import { initDatadogLogs, initDatadogRum } from '@/lib/datadog'
import { GrowthBook, GrowthBookProvider } from '@growthbook/growthbook-react'
import isEmpty from 'lodash/isEmpty'
import * as process from 'process'
import { useEffect } from 'react'

const gb = new GrowthBook({
  apiHost: process.env.NEXT_PUBLIC_FEATURE_MANAGEMENT_SERVICE_BASE_URL,
  clientKey: `?project=${process.env.NEXT_PUBLIC_GROWTHBOOK_PROJECT_ID}`,
  enableDevMode: process.env.NODE_ENV !== 'production',
})

// ** Configure JSS & ClassName
const App = (props: ExtendedAppProps) => {
  const { Component, emotionCache = clientSideEmotionCache, pageProps } = props

  // Variables
  const contentHeightFixed = Component.contentHeightFixed ?? false
  const getLayout =
    Component.getLayout ?? (page => <UserLayout contentHeightFixed={contentHeightFixed}>{page}</UserLayout>)

  const setConfig = Component.setConfig ?? undefined
  const router = useRouter()

  useEffect(() => {
    ;(async () => {
      await gb.loadFeatures({ autoRefresh: true, timeout: 2000 })
      if (!isEmpty(gb.getFeatures())) {
        if (isFeaturePath(router.pathname) && gb.isOff(getFeatureName(router.pathname))) {
          await router.push('/dashboard')
        }
      } else {
        gb.setFeatures(defaultFeatureJson)
      }

      if (process.env.NEXT_PUBLIC_DATADOG_ENVIRONMENT === 'production' || gb.isOn('datadog-rum')) {
        initDatadogRum()
      }

      initDatadogLogs()
    })()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  return (
    <Provider store={store}>
      <CacheProvider value={emotionCache}>
        <SnackbarProvider anchorOrigin={{ vertical: 'top', horizontal: 'center' }} autoHideDuration={2000}>
          <SessionProvider session={pageProps.session}>
            <GrowthBookProvider growthbook={gb}>
              <SettingsProvider {...(setConfig ? { pageSettings: setConfig() } : {})}>
                <SettingsConsumer>
                  {({ settings }) => {
                    return (
                      <ThemeComponent settings={settings}>
                        {getLayout(<Component {...pageProps} />)}
                        <ReactHotToast>
                          <Toaster position={settings.toastPosition} toastOptions={{ className: 'react-hot-toast' }} />
                        </ReactHotToast>
                      </ThemeComponent>
                    )
                  }}
                </SettingsConsumer>
              </SettingsProvider>
            </GrowthBookProvider>
          </SessionProvider>
        </SnackbarProvider>
      </CacheProvider>
    </Provider>
  )
}

export default App
