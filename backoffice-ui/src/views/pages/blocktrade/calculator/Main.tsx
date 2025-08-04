// ** MUI Imports
import Grid from '@mui/material/Grid'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Hidden from '@mui/material/Hidden'
import { useState } from 'react'
import FillData from 'src/views/pages/blocktrade/calculator/FillData'
import Summary from 'src/views/pages/blocktrade/calculator/Summary'
import OpenSide from 'src/views/pages/blocktrade/calculator/OpenSide'
import CloseSide from 'src/views/pages/blocktrade/calculator/CloseSide'
import Disclaimer from 'src/views/pages/blocktrade/calculator/Disclaimer'
import Projection from 'src/views/pages/blocktrade/calculator/Projection'
import { CalculationResult } from 'src/types/blocktrade/calculator/result'

const BlocktradeCalculator = () => {
  const [calculationResult, setCalculationResult] = useState<CalculationResult | null>(null)

  return (
    <div>
      <CardHeader title='Blocktrade Calculator' />
      <CardContent sx={{ paddingTop: 0, paddingBottom: 2 }}>
        <FillData onCalculation={setCalculationResult} />
      </CardContent>
      <CardContent>
        <Grid container spacing={2}>
          <Grid item xs={12} lg={8}>
            <Summary calculationResult={calculationResult} />
            <Grid container spacing={2}>
              <Grid item xs={12} lg={6}>
                <OpenSide calculationResult={calculationResult} />
              </Grid>
              <Grid item xs={12} lg={6}>
                <CloseSide calculationResult={calculationResult} />
              </Grid>
            </Grid>
            <Hidden lgDown>
              <Disclaimer />
            </Hidden>
          </Grid>
          <Grid item xs={12} lg={4}>
            <Projection calculationResult={calculationResult} />
            <Hidden lgUp>
              <Disclaimer />
            </Hidden>
          </Grid>
        </Grid>
      </CardContent>
    </div>
  )
}

export default BlocktradeCalculator
