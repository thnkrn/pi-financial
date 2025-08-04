import NotificationsIcon from '@mui/icons-material/Notifications'
import Badge from '@mui/material/Badge'
import IconButton from '@mui/material/IconButton'
import Popover from '@mui/material/Popover'
import Typography from '@mui/material/Typography'
import { CSSProperties, MouseEvent, ReactNode, useEffect, useState } from 'react'
import useSound from 'use-sound'
import NotificationSound from './notification-sound.mp3'

interface Props {
  message?: ReactNode | string
  amount?: number
  style?: CSSProperties
  playSound?: boolean
}

const NotificationBadge = ({ style, amount, message, playSound = true }: Props) => {
  const [anchorEl, setAnchorEl] = useState<HTMLButtonElement | null>(null)
  const [play] = useSound(NotificationSound)

  const handleClick = (event: MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget)
  }

  const handleClose = () => {
    setAnchorEl(null)
  }

  useEffect(() => {
    if (playSound) {
      // NOTE: Define a one-time click handler to unlock audio
      const handleUserInteraction = () => {
        if (amount && amount > 0) {
          play() // Play sound to unlock audio context
        }

        // NOTE: Remove this event listener after the first interaction
        document.removeEventListener('click', handleUserInteraction)
      }

      // NOTE: Add the event listener to the document for any click
      document.addEventListener('click', handleUserInteraction)

      // NOTE: Clean up event listener if component unmounts
      return () => document.removeEventListener('click', handleUserInteraction)
    }
  }, [amount, play, playSound])

  useEffect(() => {
    if (playSound) {
      if (amount && amount > 0) {
        const interval = setInterval(() => {
          play()
        }, 2000)

        return () => clearInterval(interval)
      }
    }
  }, [amount, play, playSound])

  return (
    <>
      <div style={style}>
        <IconButton color='inherit' onClick={handleClick}>
          <Badge badgeContent={amount} color='primary'>
            <NotificationsIcon color='action' />
          </Badge>
        </IconButton>
      </div>
      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'left',
        }}
      >
        <span style={{ display: 'block' }}>
          <Typography sx={{ p: 2 }}>
            {!amount ? 'No notification found' : message ?? 'You have receive notification'}
          </Typography>
        </span>
      </Popover>
    </>
  )
}

export default NotificationBadge
