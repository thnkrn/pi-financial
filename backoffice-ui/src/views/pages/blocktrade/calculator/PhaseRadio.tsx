import { Radio, RadioGroup, FormControlLabel, FormControl, Box } from '@mui/material'
import { Side } from 'src/constants/blocktrade/GlobalEnums'
import { useDispatch, useSelector } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'
import { updateSide } from '@/store/apps/blocktrade/calculator'

const PhaseRadio = () => {
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const calculatorStore = useSelector((state: any) => state.btCalculator)

  const onValueChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.value === Side.LONG || event.target.value === Side.SHORT) {
      dispatch(updateSide(event.target.value))
    }
  }

  return (
    <Box sx={{ marginX: 'auto' }}>
      <FormControl>
        <RadioGroup row name='phase' value={calculatorStore.side} onChange={onValueChange}>
          <FormControlLabel value={Side.LONG} control={<Radio color='primary' />} label='LONG' labelPlacement='end' />
          <FormControlLabel value={Side.SHORT} control={<Radio color='primary' />} label='SHORT' labelPlacement='end' />
        </RadioGroup>
      </FormControl>
    </Box>
  )
}

export default PhaseRadio
