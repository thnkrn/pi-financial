import { Grid, TextField, Stack } from '@mui/material'
import Box from '@mui/material/Box'
import SelectOption from '../SelectOption'
import { RadioBox } from './styled/TopBox'
import PhaseRadio from './PhaseRadio'
import Typography from '@mui/material/Typography'
import InputAdornment from '@mui/material/InputAdornment'
import Hidden from '@mui/material/Hidden'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { updateCommFee } from '@/store/apps/blocktrade/calculator'

const customerAccount = [
  { key: 'acc1', value: 'Account 1' },
  { key: 'acc2', value: 'Account 2' },
  { key: 'acc3', value: 'Account 3' },
]

const TopFirstCol = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const calculatorStore = useSelector((state: any) => state.btCalculator)

  return (
    <Grid item xs={12} md={6} lg={3}>
      <Box sx={{ paddingTop: 0, paddingX: 0 }}>
        <Grid container spacing={2} columnGap={4}>
          <Hidden xlUp>
            <Grid item xs={12} xl={3}>
              <Box sx={{ paddingTop: 0, marginTop: { xs: 0, lg: -1 }, paddingX: 0 }}>
                <RadioBox>
                  <PhaseRadio />
                </RadioBox>
              </Box>
            </Grid>
          </Hidden>
          <Grid item xs={12} xl={8}>
            <Stack spacing={2}>
              <SelectOption
                id={'customer-account'}
                label={'Customer Account'}
                labelId={'customer-account'}
                options={customerAccount}
                disabled={true}
                onChange={() => {}}
              />
              <Box sx={{ paddingTop: 0 }}>
                <Grid container spacing={2}>
                  <Grid item xs={5}>
                    <Box display='flex' sx={{ marginLeft: 0, height: '100%' }}>
                      <Typography
                        variant='body1'
                        gutterBottom
                        noWrap
                        sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                      >
                        Comm Fee
                      </Typography>
                    </Box>
                  </Grid>
                  <Grid item xs={7}>
                    <TextField
                      sx={{}}
                      size={'small'}
                      fullWidth
                      label=''
                      defaultValue={calculatorStore.commFee || 0.1}
                      onChange={event => {
                        dispatch(updateCommFee(Number(event.target.value)))
                      }}
                      InputProps={{
                        endAdornment: <InputAdornment position='end'>%</InputAdornment>,
                      }}
                    />
                  </Grid>
                </Grid>
              </Box>
            </Stack>
          </Grid>
          <Hidden xlDown>
            <Grid item xs={12} xl={3}>
              <Box sx={{ paddingTop: 0, marginTop: { xs: 0, lg: -1 }, paddingX: 0 }}>
                <RadioBox>
                  <PhaseRadio />
                </RadioBox>
              </Box>
            </Grid>
          </Hidden>
        </Grid>
      </Box>
    </Grid>
  )
}

export default TopFirstCol
