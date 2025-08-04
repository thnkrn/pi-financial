import Link from '@mui/material/Link'
import Stack from '@mui/material/Stack'
import dayjs from 'dayjs'

const DashboardFooterBar = () => {
  return (
    <footer>
      <Stack
        direction={{ xs: 'column', sm: 'row' }}
        justifyContent={'center'}
        alignItems={'center'}
        spacing={{ xs: 2, sm: 4 }}
        sx={{ padding: '10px 0 20px' }}
      >
        <Link href='#'>Terms & Conditions</Link>
        <Link href='#'>Privacy</Link>
        <Link href='#'>&copy; Pi Financial {dayjs().format('YYYY')}</Link>
      </Stack>
    </footer>
  )
}

export default DashboardFooterBar
