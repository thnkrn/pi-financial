import { IStatusAction } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import NameMismatchAlertTable from '@/views/components/transaction/alert-table/NameMismatchAlertTable'
import SearchIcon from '@mui/icons-material/Search'
import { CardHeader, Chip, Grid, IconButton, Typography } from '@mui/material'
import Card from '@mui/material/Card'
import Decimal from 'decimal.js'
import { useEffect, useState } from 'react'
import Swal from 'sweetalert2'
import withReactContent from 'sweetalert2-react-content'
import AmountMismatchAlertTable from './alert-table/AmountMismatchAlertTable'
import RefundTimeAlertTable from './alert-table/RefundTimeDisplayAlert'

interface Props {
  currency: string
  statusAction: IStatusAction
  customerAccountName: string
  customerBankAccountName: string
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

const ALERT_TABLE_HEADER = ['PI ACCOUNT NAME', 'BANK ACCOUNT NAME']
const REFUND_TIME_TABLE_HEADER = 'REFUND TIME'

const TransactionStatusAction = ({ currency, statusAction, customerAccountName, customerBankAccountName }: Props) => {
  const [rows, setRows] = useState<IStatusAction[]>([])
  const MySwal = withReactContent(Swal)

  useEffect(() => {
    setRows([
      {
        status: statusAction?.status ?? '',
        errorCode: statusAction?.errorCode ?? '',
        callToAction: statusAction.callToAction ?? '',
        refundSuccessfulAt: statusAction?.refundSuccessfulAt,
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

  const displayAlert = ({ title, element }: { title: string; element: JSX.Element }) => {
    MySwal.fire({
      title: `<small style="text-align: left; display:flex;">${title}</small>`,
      html: element,
      showCancelButton: true,
      cancelButtonText: 'Close',
      showConfirmButton: false,
      cancelButtonColor: '#3dd884',
    })
  }

  const renderRows = (rows: IStatusAction[]) =>
    rows.map((row: IStatusAction) => [
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
            {customerAccountName && customerBankAccountName && row?.errorCode?.startsWith('Name Mismatch') && (
              <Grid item>
                <IconButton
                  sx={{ ml: 'auto' }}
                  size='small'
                  onClick={() =>
                    displayAlert({
                      title: 'Name Mismatch',
                      element: (
                        <NameMismatchAlertTable
                          header={ALERT_TABLE_HEADER}
                          values={[customerAccountName, customerBankAccountName]}
                        />
                      ),
                    })
                  }
                >
                  <SearchIcon />
                </IconButton>
              </Grid>
            )}

            {row?.refundSuccessfulAt && (
              <Grid item>
                <IconButton
                  sx={{ ml: 'auto' }}
                  size='small'
                  onClick={() =>
                    displayAlert({
                      title: 'Refund Detail',
                      element: (
                        <RefundTimeAlertTable
                          header={REFUND_TIME_TABLE_HEADER}
                          value={statusAction.refundSuccessfulAt!}
                        />
                      ),
                    })
                  }
                >
                  <SearchIcon />
                </IconButton>
              </Grid>
            )}
            {row?.errorCode === 'Amount Mismatch' && (
              <Grid item>
                <IconButton
                  sx={{ ml: 'auto' }}
                  size='small'
                  onClick={() =>
                    displayAlert({
                      title: 'Amount Mismatch',
                      element: (
                        <AmountMismatchAlertTable
                          currency={currency}
                          receivedAmount={statusAction.paymentReceivedAmount as Decimal}
                          requestedAmount={statusAction.requestedAmount as Decimal}
                        />
                      ),
                    })
                  }
                >
                  <SearchIcon />
                </IconButton>
              </Grid>
            )}
          </Grid>
        ),
      },
      { value: row?.failedReason ?? '-' },
      { value: row?.callToAction ?? '-' },
    ])

  const renderTable = (rows: IStatusAction[]) => {
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

export default TransactionStatusAction
