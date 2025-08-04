import { Grid, TextField } from '@mui/material'
import Stack from '@mui/material/Stack'
import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import Typography from '@mui/material/Typography'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { updateOpenPrice, updateClosePrice, updateMinDay } from '@/store/apps/blocktrade/calculator'

interface TopForthColProps {
  onCalculate: () => void
}

const TopForthCol = ({ onCalculate }: TopForthColProps) => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const calculatorStore = useSelector((state: any) => state.btCalculator)

  const openDateAtMidnight = new Date(
    calculatorStore.openDate.getFullYear(),
    calculatorStore.openDate.getMonth(),
    calculatorStore.openDate.getDate()
  )
  const closeDateAtMidnight = new Date(
    calculatorStore.closeDate.getFullYear(),
    calculatorStore.closeDate.getMonth(),
    calculatorStore.closeDate.getDate()
  )
  const checkCalDisable =
    calculatorStore.symbol !== null &&
    calculatorStore.contractAmount >= (calculatorStore.futuresProperty?.blocksize || 0) &&
    closeDateAtMidnight >= openDateAtMidnight &&
    (calculatorStore.openPrice ?? 0) > 0 &&
    (calculatorStore.closePrice ?? 0) > 0 &&
    (calculatorStore.side === 'LONG' || calculatorStore.side === 'SHORT') &&
    calculatorStore.xd >= 0 &&
    calculatorStore.intRate >= 0 &&
    calculatorStore.minDay >= 0

  return (
    <Grid item xs={12} md={6} lg={3}>
      <Box sx={{ paddingTop: 0, paddingX: 0 }}>
        <Stack spacing={2}>
          <Box sx={{ paddingTop: 0 }}>
            <Grid container spacing={2}>
              <Grid item xs={5} lg={6}>
                <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                  >
                    Open Stk Price
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={7} lg={6}>
                <TextField
                  size={'small'}
                  fullWidth
                  label=''
                  placeholder=''
                  value={calculatorStore.openPrice}
                  type='number'
                  InputProps={{ inputProps: { min: 0, step: 0.01 } }}
                  onChange={e => {
                    dispatch(updateOpenPrice(parseFloat(e.target?.value ?? '')))
                  }}
                />
              </Grid>
            </Grid>
          </Box>

          <Box sx={{ paddingTop: 0 }}>
            <Grid container spacing={2}>
              <Grid item xs={5} lg={6}>
                <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                  <Typography
                    variant='body1'
                    gutterBottom
                    noWrap
                    sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                  >
                    Close Stk Price
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={7} lg={6}>
                <TextField
                  size={'small'}
                  fullWidth
                  label=''
                  placeholder=''
                  value={calculatorStore.closePrice}
                  type='number'
                  InputProps={{ inputProps: { min: 0, step: 0.01 } }}
                  onChange={e => dispatch(updateClosePrice(parseFloat(e.target.value)))}
                />
              </Grid>
            </Grid>
          </Box>

          <Grid container rowGap={2}>
            <Grid item xs={12} md={12} lg={6}>
              <Box sx={{ paddingTop: 0 }}>
                <Grid container spacing={2}>
                  <Grid item xs={5} lg={7}>
                    <Box display='flex' sx={{ marginLeft: { lg: 2 }, height: '100%' }}>
                      <Typography
                        variant='body1'
                        gutterBottom
                        noWrap
                        sx={{ marginY: 'auto', fontSize: '16px', color: 'secondary.main' }}
                      >
                        Min Day
                      </Typography>
                    </Box>
                  </Grid>
                  <Grid item xs={7} lg={5}>
                    <TextField
                      sx={{ paddingRight: { lg: 1 } }}
                      size={'small'}
                      fullWidth
                      label=''
                      defaultValue={calculatorStore.minDay}
                      onChange={e => dispatch(updateMinDay(Number(e.target.value)))}
                    />
                  </Grid>
                </Grid>
              </Box>
            </Grid>
            <Grid item xs={12} md={12} lg={6}>
              <Button
                size='medium'
                type='submit'
                sx={{ width: '100%', fontWeight: 700, marginTop: 0, marginLeft: { lg: 1 } }}
                variant='contained'
                disabled={!checkCalDisable}
                onClick={onCalculate}
              >
                Calculate
              </Button>
            </Grid>
          </Grid>
        </Stack>
      </Box>
    </Grid>
  )
}

export default TopForthCol
