import Zoom from '@mui/material/Zoom'
import { styled } from '@mui/material/styles'
import useScrollTrigger from '@mui/material/useScrollTrigger'
import { ReactNode } from 'react'

interface ScrollToTopProps {
  className?: string
  children: ReactNode
}

const ScrollToTopStyled = styled('div')(({ theme }) => ({
  zIndex: 11,
  position: 'fixed',
  right: theme.spacing(6),
  bottom: theme.spacing(10),
}))

const ScrollToTop = ({ children, className }: ScrollToTopProps) => {
  const trigger = useScrollTrigger({
    threshold: 400,
    disableHysteresis: true,
  })

  const handleClick = () => {
    const anchor = document.querySelector('body')
    if (anchor) {
      anchor.scrollIntoView({ behavior: 'smooth' })
    }
  }

  return (
    <Zoom in={trigger}>
      <ScrollToTopStyled className={className} onClick={handleClick}>
        {children}
      </ScrollToTopStyled>
    </Zoom>
  )
}

export default ScrollToTop
