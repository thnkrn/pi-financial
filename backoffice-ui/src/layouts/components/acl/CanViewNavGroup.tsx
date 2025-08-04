import { ReactNode } from 'react'

interface Props {
  children: ReactNode
}

const CanViewNavGroup = (props: Props) => {
  const { children } = props

  return <>{children}</>
}

export default CanViewNavGroup
