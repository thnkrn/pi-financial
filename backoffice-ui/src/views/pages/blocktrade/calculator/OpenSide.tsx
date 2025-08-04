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

const OpenSide = ({ calculationResult }: OpenSideProps) => {
  const data = [
    {
      id: 1,
      label: 'SSF Price Open',
      value: calculationResult
        ? Number(calculationResult.openPosition.futuresPriceOpen).toLocaleString('en-US', {
            minimumFractionDigits: 4,
            maximumFractionDigits: 4,
          })
        : '-',
    },
    {
      id: 2,
      label: 'IM',
      value: calculationResult
        ? Number(calculationResult.openPosition.im).toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
          })
        : '-',
    },
    {
      id: 3,
      label: 'Time to Maturity',
      value: calculationResult ? Number(calculationResult.openPosition.timeToMaturity).toLocaleString() : '-',
    },
    {
      id: 4,
      label: 'Comm and Fees',
      value: calculationResult
        ? Number(calculationResult.openPosition.commissionFeeOpen).toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
          })
        : '-',
    },
    {
      id: 5,
      label: 'Cash Usage',
      value: calculationResult
        ? Number(calculationResult.openPosition.cashUsage).toLocaleString('en-US', {
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
              Open Position
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

export default OpenSide
