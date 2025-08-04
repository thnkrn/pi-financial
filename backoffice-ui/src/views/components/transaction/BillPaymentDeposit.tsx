import { IBillPaymentDepositDetail } from '@/lib/api/clients/backoffice/transactions/types'
import { useEffect, useState } from 'react'
import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'
import { formatCurrency } from '@/utils/fmt'
import LocalDateTime from '@/views/components/LocalDateTime'
import Table from '@/tables/Tables'
import Card from '@mui/material/Card'
import { CardHeader } from '@mui/material'

interface Props {
  data: IBillPaymentDepositDetail
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
    value: 'REFERENCE1',
  },
  {
    value: 'REFERENCE2',
  },
  {
    value: 'TRANSACTION REF',
  },
  {
    value: 'FAILED REASON',
  },
]

const BillPaymentDepositDetail = ({ data }: Props) => {
  const [rows, setRows] = useState<IBillPaymentDepositDetail[]>([])

  useEffect(() => {
    setRows([
      {
        state: data?.state,
        paymentReceivedAmount: data?.paymentReceivedAmount,
        paymentReceivedDateTime: data?.paymentReceivedDateTime,
        fee: data?.fee,
        reference1: data?.reference1,
        reference2: data?.reference2,
        billPaymentTransactionRef: data?.billPaymentTransactionRef,
        failedReason: data?.failedReason,
      },
    ])
  }, [data])

  const renderRows = (rows: IBillPaymentDepositDetail[]) =>
    rows?.map((row: IBillPaymentDepositDetail) => [
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
      { value: row?.reference1 ?? '-' },
      { value: row?.reference2 ?? '-' },
      { value: row?.billPaymentTransactionRef ?? '-' },
      { value: row?.failedReason ?? '-' },
    ])

  const renderTable = (rows: IBillPaymentDepositDetail[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Bill Payment Deposit Detail'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default BillPaymentDepositDetail
