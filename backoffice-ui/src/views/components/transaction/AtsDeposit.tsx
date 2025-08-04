import { IAtsDepositDetail } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'
import LocalDateTime from '../LocalDateTime'
import { formatCurrency } from '@/utils/fmt'
import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'

interface Props {
  data: IAtsDepositDetail
}

const TABLE_HEADER = [
  {
    value: 'STATE',
  },
  {
    value: 'PAYMENT RECEIVED AMOUNT',
  },
  {
    value: 'PAYMENT RECEIVED DATE TIME',
  },
  {
    value: 'FEE',
  },
  {
    value: 'FAILED REASON',
  },
  {
    value: 'CREATED AT',
  },
]

const AtsDepositDetail = ({ data }: Props) => {
  const [rows, setRows] = useState<IAtsDepositDetail[]>([])

  useEffect(() => {
    setRows([
      {
        state: data?.state,
        paymentReceivedAmount: data?.paymentReceivedAmount,
        paymentReceivedDateTime: data?.paymentReceivedDateTime,
        fee: data?.fee,
        failedReason: data?.failedReason,
        createdAt: data?.createdAt,
      },
    ])
  }, [data])

  const renderRows = (rows: IAtsDepositDetail[]) =>
    rows?.map((row: IAtsDepositDetail) => [
      { value: row?.state ?? '-' },
      {
        value: row?.paymentReceivedAmount ? (
          <>
            {convertCurrencyPrefixToCurrencySymbol('thb' as string)} {formatCurrency(row?.paymentReceivedAmount)}
          </>
        ) : (
          '-'
        ),
      },
      { value: row?.paymentReceivedDateTime ? <LocalDateTime date={row?.paymentReceivedDateTime} /> : '-' },
      {
        value: row?.fee ? (
          <>
            {convertCurrencyPrefixToCurrencySymbol('thb' as string)} {formatCurrency(row?.fee)}
          </>
        ) : (
          '-'
        ),
      },
      { value: row?.failedReason ?? '-' },
      { value: row?.createdAt ? <LocalDateTime date={row?.createdAt} /> : '-' },
    ])

  const renderTable = (rows: IAtsDepositDetail[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='ATS Deposit Detail'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default AtsDepositDetail
