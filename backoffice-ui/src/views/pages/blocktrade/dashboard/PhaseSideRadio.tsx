// ** React Imports
import React from 'react'
import { useDispatch, useSelector } from 'react-redux'

// ** MUI Imports
import { Box, FormControl, FormControlLabel, Radio, RadioGroup } from '@mui/material'

// ** Custom Components
import { sideMap } from '@/constants/blocktrade/SideMap'
import { updateData } from 'src/store/apps/blocktrade/order'

const PhaseSideRadio = () => {
  const dispatch = useDispatch()
  const orderStore = useSelector((state: any) => state.btOrder)

  const selectedOption = orderStore
    ? Object.entries(sideMap).find(entry => entry[1].oc === orderStore.oc && entry[1].side === orderStore.side)?.[0] ??
      null
    : null

  const onValueChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value
    dispatch(
      updateData({
        oc: sideMap[value].oc,
        side: sideMap[value].side,
      })
    )
  }

  return (
    <Box sx={{ marginX: 'auto', width: '100%' }}>
      <FormControl disabled={orderStore.isAmend} sx={{ width: '100%', display: 'flex', alignItems: 'center' }}>
        <RadioGroup
          row
          name='phase'
          value={!orderStore.isCancel && selectedOption}
          onChange={onValueChange}
          sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}
        >
          <Box sx={{ display: 'flex', flexDirection: 'column', justifyContent: 'flex-end' }}>
            <FormControlLabel value='LONG' control={<Radio color='primary' />} label='BUY' labelPlacement='end' />
            <FormControlLabel
              value='COVERBUY'
              control={<Radio color='primary' />}
              label='COVER BUY'
              labelPlacement='end'
              disabled
            />
          </Box>
          <Box sx={{ display: 'flex', flexDirection: 'column', justifyContent: 'space-between' }}>
            <FormControlLabel value='SHORT' control={<Radio color='primary' />} label='SELL' labelPlacement='end' />
            <FormControlLabel
              value='SHORTSELL'
              control={<Radio color='primary' />}
              label='SHORT SELL'
              labelPlacement='end'
              disabled
            />
          </Box>
        </RadioGroup>
      </FormControl>
    </Box>
  )
}

export default PhaseSideRadio
