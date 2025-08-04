import { styled } from '@mui/system'
import { Box } from '@mui/material'

export const RadioBox = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'row',
  gap: theme.spacing(6),
  alignItems: 'start',
  paddingTop: theme.spacing(1),
  gridColumn: 'span 1 / auto',
  [theme.breakpoints.up('lg')]: {
    flexDirection: 'column',
    gap: theme.spacing(4),
  },
}))
