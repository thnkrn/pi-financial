import { IOddWithdrawDetail } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'
import LocalDateTime from '../LocalDateTime'
import { formatCurrency } from '@/utils/fmt'
import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'

interface Props {
  data: IOddWithdrawDetail
}

const TABLE_HEADER = [
  {
    value: 'STATE',
  },
  {
    value: 'PAYMENT DISBURSED AMOUNT',
  },
  {
    value: 'PAYMENT DISBURSED DATE TIME',
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

const OddWithdrawDetail = ({ data }: Props) => {
  const [rows, setRows] = useState<IOddWithdrawDetail[]>([])

  useEffect(() => {
    setRows([
      {
        state: data?.state,
        paymentDisbursedAmount: data?.paymentDisbursedAmount,
        paymentDisbursedDateTime: data?.paymentDisbursedDateTime,
        fee: data?.fee,
        failedReason: data?.failedReason,
        createdAt: data?.createdAt,
      },
    ])
  }, [data])

  const renderRows = (rows: IOddWithdrawDetail[]) =>
    rows?.map((row: IOddWithdrawDetail) => [
      { value: row?.state ?? '-' },
      {
        value: row?.paymentDisbursedAmount ? (
          <>
            {convertCurrencyPrefixToCurrencySymbol('thb' as string)} {formatCurrency(row?.paymentDisbursedAmount)}
          </>
        ) : (
          '-'
        ),
      },
      { value: row?.paymentDisbursedDateTime ? <LocalDateTime date={row?.paymentDisbursedDateTime} /> : '-' },
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

  const renderTable = (rows: IOddWithdrawDetail[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='ODD Withdraw Detail'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default OddWithdrawDetail
