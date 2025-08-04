import { default as MUIBreadcrumbs } from '@mui/material/Breadcrumbs'
import Link from '@mui/material/Link'
import { default as NextLink } from 'next/link'

export interface IContent {
  name: string
  href?: string
}

interface Props {
  contents: IContent[]
}

const Breadcrumbs = ({ contents }: Props) => {
  if (!contents.length) {
    return null
  }

  return (
    <MUIBreadcrumbs aria-label='breadcrumb'>
      {contents?.map((content: IContent) => (
        <Link
          sx={{ typography: 'body1' }}
          underline={content?.href ? 'hover' : 'none'}
          color='inherit'
          href={content?.href}
          key={content?.name}
          {...(content?.href && ({ component:  NextLink }))}
        >
          {content?.name}
        </Link>
      ))}
    </MUIBreadcrumbs>
  )
}

export default Breadcrumbs
