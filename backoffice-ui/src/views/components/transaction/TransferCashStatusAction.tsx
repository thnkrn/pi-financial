import { IBaseStatusAction } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader, Chip, Grid, Typography } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'

interface Props {
  statusAction: IBaseStatusAction
}

const TABLE_HEADER = [
  {
    value: 'STATUS',
  },
  {
    value: 'RESPONSE DESCRIPTION',
  },
  {
    value: 'FAIL REASON',
  },
  {
    value: 'CALL TO ACTION',
  },
]

const TransferCashTransactionStatusAction = ({ statusAction }: Props) => {
  const [rows, setRows] = useState<IBaseStatusAction[]>([])

  useEffect(() => {
    setRows([
      {
        status: statusAction?.status ?? '',
        errorCode: statusAction?.errorCode ?? '',
        callToAction: statusAction.callToAction ?? '',
        failedReason: statusAction?.failedReason ?? '',
      },
    ])
  }, [statusAction])

  const renderStatus = ({ status }: { status: string }) => {
    let color: 'error' | 'secondary' | 'success' | 'warning'

    switch (status) {
      case 'Success':
        color = 'success'
        break
      case 'Pending':
        color = 'warning'
        break
      case 'Fail':
        color = 'error'
        break
      default:
        color = 'secondary'
        break
    }

    return <Chip key={status} variant='outlined' color={color} label={status} size='medium' />
  }

  const renderRows = (rows: IBaseStatusAction[]) =>
    rows.map((row: IBaseStatusAction) => [
      {
        value: renderStatus({ status: row?.status ?? '' }),
      },
      {
        value: (
          <Grid container direction='row' alignItems='center'>
            <Grid item>
              <Typography mr={2} variant='body1' color='text.primary' fontWeight='bold'>
                {row?.errorCode}
              </Typography>
            </Grid>
          </Grid>
        ),
      },
      { value: row?.failedReason ?? '-' },
      { value: row?.callToAction ?? '-' },
    ])

  const renderTable = (rows: IBaseStatusAction[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Overall Status and Required Action'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default TransferCashTransactionStatusAction
