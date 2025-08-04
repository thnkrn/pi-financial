import type { NextComponentType, NextPageContext } from 'next/dist/shared/lib/utils'
import type { ReactElement, ReactNode } from 'react'

declare module 'next' {
  export declare type NextPage<P = {}, IP = P> = NextComponentType<NextPageContext, IP, P> & {
    setConfig?: () => void
    contentHeightFixed?: boolean
    getLayout?: (page: ReactElement) => ReactNode
  }
}
