import { Theme } from '@mui/material'

export const getBorderHover = () => ({
  '&:hover': { borderColor: (theme: Theme) => `rgba(${theme.palette.customColors.main}, 0.25)` },
})
