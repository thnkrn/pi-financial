import { Typography } from '@mui/material'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import List from '@mui/material/List'
import ListItem from '@mui/material/ListItem'
import ListItemText from '@mui/material/ListItemText'
import { CalculationResult } from 'src/types/blocktrade/calculator/result'

interface OpenSideProps {
  calculationResult: CalculationResult | null
}

const CloseSide = ({ calculationResult }: OpenSideProps) => {
  const data = [
    {
      id: 1,
      label: 'SSF Price Close',
      value: calculationResult
        ? Number(calculationResult.closePosition.futuresPriceClose).toLocaleString('en-US', {
            minimumFractionDigits: 4,
            maximumFractionDigits: 4,
          })
        : '-',
    },
    {
      id: 2,
      label: 'Interest Per Share',
      value: calculationResult
        ? Number(calculationResult.closePosition.interestPerShare).toLocaleString('en-US', {
            minimumFractionDigits: 4,
            maximumFractionDigits: 4,
          })
        : '-',
    },
    {
      id: 3,
      label: 'Holding Period',
      value: calculationResult ? Number(calculationResult.closePosition.holdingPeriod).toLocaleString() : '-',
    },
    {
      id: 4,
      label: 'Total Interest Amount',
      value: calculationResult
        ? Number(calculationResult.closePosition.totalInterestAmount).toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
          })
        : '-',
    },
    {
      id: 5,
      label: 'Comm and Fees',
      value: calculationResult
        ? Number(calculationResult.closePosition.commissionFeeClose).toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
          })
        : '-',
    },
  ]

  return (
    <div>
      <Card sx={{ textAlign: 'center', marginLeft: 0, marginRight: { xs: 0, lg: 2 }, marginTop: 3, height: '100%' }}>
        <CardHeader
          sx={{ backgroundColor: 'primary.main', paddingY: 2 }}
          title={
            <Typography variant='h6' sx={{ color: 'white', fontWeight: 700 }}>
              Close Position
            </Typography>
          }
        />
        <CardContent sx={{ fontSize: '20px' }}>
          <List>
            {data.map(data => (
              <ListItem
                key={data.id}
                secondaryAction={
                  <Typography variant='body1' sx={{ fontWeight: 500 }}>
                    {data.value}
                  </Typography>
                }
              >
                <ListItemText primary={data.label} />
              </ListItem>
            ))}
          </List>
        </CardContent>
      </Card>
    </div>
  )
}

export default CloseSide
