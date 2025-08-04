import Typography from '@mui/material/Typography'
import { ReactNode } from 'react'

interface Props {
  content: ReactNode
}

const Content = ({ content }: Props) => {
  if (typeof content === 'string') {
    return (
      <Typography variant='body2' sx={{ my: 'auto', textAlign: 'center' }}>
        {content}
      </Typography>
    )
  }

  return <>{content}</>
}

export default Content
