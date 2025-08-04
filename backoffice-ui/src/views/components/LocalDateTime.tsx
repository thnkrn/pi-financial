import { Tooltip, Typography } from '@mui/material'
import dayjs from 'dayjs'

interface Props {
  date: Date | null
  fontSize?: number
}

export const LocalDateTime = ({ date, fontSize = 14 }: Props) => {
  const day = dayjs(date)
  const localDateTime = day.format('DD/MM/YYYY HH:mm:ss')
  const utcDateTime = day.utc().format('DD/MM/YYYY HH:mm:ss UTC')

  if (!date) return null

  return (
    <Tooltip title={utcDateTime}>
      <Typography fontSize={fontSize} component='span'>
        {localDateTime}
      </Typography>
    </Tooltip>
  )
}
export default LocalDateTime
