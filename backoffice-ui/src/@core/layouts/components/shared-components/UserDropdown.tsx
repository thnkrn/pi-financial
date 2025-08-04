import LogoutIcon from '@mui/icons-material/Logout'
import { Button } from '@mui/material'
import { signOut } from 'next-auth/react'
import useTranslation from 'next-translate/useTranslation'

const UserDropdown = () => {
  const { t } = useTranslation('common')

  return (
    <Button
      onClick={() => signOut()}
      sx={{ typography: 'button' }}
      startIcon={<LogoutIcon fontSize='small' />}
      size='medium'
      variant='contained'
    >
      {t('NAV_BAR_SIGNOUT', {}, { default: 'Sign Out' })}
    </Button>
  )
}

export default UserDropdown
