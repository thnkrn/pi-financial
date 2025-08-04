import Typography from '@mui/material/Typography'
import { ReactNode } from 'react'

interface Props {
  title: ReactNode
  content: ReactNode
}

const Title = ({ title, content }: Props) => {
  if (typeof title === 'string') {
    return <Typography sx={{ fontWeight: 500, ...(content ? { mb: 1 } : { my: 'auto' }) }}>{title}</Typography>
  }

  return <>{title}</>
}

export default Title
