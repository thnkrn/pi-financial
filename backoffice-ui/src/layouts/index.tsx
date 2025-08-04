import themeConfig from '@/configs/themeConfig'
import Head from 'next/head'
import { ReactNode } from 'react'

interface Props {
  children: ReactNode
  title?: string
  desc?: string
}


const Layout = ({
  children,
  title = `${themeConfig.templateName} - ${themeConfig.title}`,
  desc = 'Pi Securities',
}: Props) => (
  <>
    <Head>
      <title>{title}</title>
      <meta charSet="utf-8" />
      <meta name="viewport" content="initial-scale=1.0, width=device-width" />
      <meta name="description" content={desc} />
    </Head>
    {children}
  </>
)

export default Layout
