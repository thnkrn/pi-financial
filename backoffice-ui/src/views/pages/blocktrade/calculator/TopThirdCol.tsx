import { Grid, TextField } from '@mui/material'
import Stack from '@mui/material/Stack'
import Box from '@mui/material/Box'
import InputAdornment from '@mui/material/InputAdornment'
import Typography from '@mui/material/Typography'
import Datepicker from '@/views/components/blocktrade/DatePicker'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { updateOpenDate, updateCloseDate, updateXD, updateIntRate } from '@/store/apps/blocktrade/calculator'

const TopThirdCol = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const calculatorStore = useSelector((state: any) => state.btCalculator)

  const handleSetOpenDate = (date: Date) => {
    dispatch(updateOpenDate(date))
  }

  const handleSetCloseDate = (date: Date) => {
    dispatch(updateCloseDate(date))
  }

  return (
    <Grid item sm={12} md={6} lg={3}>
      <Box sx={{ paddingTop: 0, paddingX: 0 }}>
        <Stack spacing={2}>
          <Box sx={{ paddingTop: 0 }}>
            <Grid container spacing={2}>
              <Grid item xs={5}>
                <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                  >
                    Open Date
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={7}>
                <Datepicker defaultDate={calculatorStore.openDate} onChange={handleSetOpenDate} />
              </Grid>
            </Grid>
          </Box>

          <Box sx={{ paddingTop: 0 }}>
            <Grid container spacing={2}>
              <Grid item xs={5}>
                <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                  >
                    Close Date
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={7}>
                <Datepicker defaultDate={calculatorStore.closeDate} onChange={handleSetCloseDate} />
              </Grid>
            </Grid>
          </Box>

          <Grid container rowGap={2}>
            <Grid item xs={12} md={12} lg={6}>
              <Box sx={{ paddingTop: 0 }}>
                <Grid container spacing={2}>
                  <Grid item xs={5}>
                    <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                      <Typography
                        variant='body1'
                        gutterBottom
                        noWrap
                        sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                      >
                        XD
                      </Typography>
                    </Box>
                  </Grid>
                  <Grid item xs={7}>
                    <TextField
                      sx={{ paddingRight: { lg: 1 } }}
                      size={'small'}
                      fullWidth
                      label=''
                      defaultValue={calculatorStore.xd}
                      onChange={e => {
                        dispatch(updateXD(Number(e.target.value ?? 0)))
                      }}
                    />
                  </Grid>
                </Grid>
              </Box>
            </Grid>
            <Grid item xs={12} md={12} lg={6}>
              <Box sx={{ paddingTop: 0 }}>
                <Grid container spacing={2}>
                  <Grid item xs={5}>
                    <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                      <Typography
                        variant='body1'
                        gutterBottom
                        noWrap
                        sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                      >
                        Int. Fee
                      </Typography>
                    </Box>
                  </Grid>
                  <Grid item xs={7}>
                    <TextField
                      sx={{}}
                      size={'small'}
                      fullWidth
                      label=''
                      defaultValue={calculatorStore.intRate}
                      onChange={e => {
                        dispatch(updateIntRate(Number(e.target.value ?? 0)))
                      }}
                      InputProps={{
                        endAdornment: <InputAdornment position='end'>%</InputAdornment>,
                      }}
                    />
                  </Grid>
                </Grid>
              </Box>
            </Grid>
          </Grid>
        </Stack>
      </Box>
    </Grid>
  )
}

export default TopThirdCol
