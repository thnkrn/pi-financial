import createCache from '@emotion/cache'
import { CacheProvider } from '@emotion/react'
import { Direction as DirectionType } from '@mui/material'
import { ReactNode, useEffect } from 'react'
import stylisRTLPlugin from 'stylis-plugin-rtl'

interface DirectionProps {
  children: ReactNode
  direction: DirectionType
}

const styleCache = () =>
  createCache({
    key: 'rtl',
    prepend: true,
    stylisPlugins: [stylisRTLPlugin]
  })

const Direction = ({ children, direction }: DirectionProps) => {

  useEffect(() => {
    document.dir = direction
  }, [direction])

  if (direction === 'rtl') {
    return <CacheProvider value={styleCache()}>{children}</CacheProvider>
  }

  return <>{children}</>
}

export default Direction
