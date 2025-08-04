import Box from '@mui/material/Box'
import { Grid, Typography } from '@mui/material'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import { CalculationResult } from 'src/types/blocktrade/calculator/result'
import { DecimalNumber } from 'src/utils/blocktrade/decimal'

interface SummaryDataProps {
  calculationResult: CalculationResult | null
}

const Summary = ({ calculationResult }: SummaryDataProps) => {
  const data = [
    {
      id: 1,
      title: 'Profit/Loss',
      value: calculationResult ? DecimalNumber(calculationResult.PnLAfterInt, 2) : '-',
    },
    {
      id: 2,
      title: 'Total Comm and Fees',
      value: calculationResult ? DecimalNumber(calculationResult.totalCommissionFee, 2) : '-',
    },
    {
      id: 3,
      title: 'Net Profit/Loss',
      value: calculationResult ? DecimalNumber(calculationResult.netPnL, 2) : '-',
    },
  ]

  return (
    <Box sx={{ marginRight: { xs: 0, lg: 2 }, marginTop: -2 }}>
      <Grid container spacing={2} sx={{ marginTop: 0 }} rowGap={2}>
        {data.map(data => (
          <Grid item xs={12} md={4} key={data.id}>
            <Card
              key={data.id}
              sx={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'space-between',
                textAlign: 'center',
                marginLeft: 0,
                height: '100%',
                backgroundColor: 'primary.main',
              }}
            >
              <CardHeader
                title={
                  <Typography variant='h6' sx={{ color: 'white', fontWeight: 700 }}>
                    {data.title}
                  </Typography>
                }
              ></CardHeader>
              <CardContent sx={{ fontSize: '20px', color: 'white', fontWeight: 700 }}>{data.value}</CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  )
}

export default Summary
