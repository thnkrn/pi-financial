import MenuItem from '@mui/material/MenuItem'
import Select, { SelectChangeEvent } from '@mui/material/Select'
import { useRouter } from 'next/router'

const LanguageTranslator = () => {
  const router = useRouter()

  const handleLanguageChange = (event: SelectChangeEvent) => {
    router.push(
      {
        pathname: router.pathname,
        query: router.query,
      },
      undefined,
      { locale: event.target.value }
    )
  }

  return (
    <Select sx={{ width: '100px' }} size='small' value={router?.locale} onChange={handleLanguageChange}>
      <MenuItem value={'en-US'}>ENG</MenuItem>
      <MenuItem value={'th-TH'}>ไทย</MenuItem>
    </Select>
  )
}

export default LanguageTranslator
