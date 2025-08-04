import { ITransferCashTransactionDetail } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'
import { formatCurrency } from '@/utils/fmt'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'
import LocalDateTime from '../LocalDateTime'
import dayjs from 'dayjs'

interface Props {
  transactionDetail: ITransferCashTransactionDetail
}

const TABLE_HEADER = [
  {
    value: 'TRANSACTION NO',
  },
  {
    value: 'TRANSACTION TYPE',
  },
  {
    value: 'REQUESTED AMOUNT',
  },
  {
    value: 'EFFECTIVE DATE',
  },
  {
    value: 'CREATED AT',
  },
]

const TransferCashTransactionDetail = ({ transactionDetail }: Props) => {
  const [rows, setRows] = useState<ITransferCashTransactionDetail[]>([])

  useEffect(() => {
    setRows([
      {
        transactionNo: transactionDetail?.transactionNo,
        transactionType: transactionDetail?.transactionType,
        requestedAmount: transactionDetail?.requestedAmount,
        effectiveDate: transactionDetail?.effectiveDate,
        createdAt: transactionDetail?.createdAt,
      },
    ])
  }, [transactionDetail])

  const renderRows = (rows: ITransferCashTransactionDetail[]) =>
    rows?.map((row: ITransferCashTransactionDetail) => [
      {
        value: row?.transactionNo ?? '-',
      },
      {
        value: row?.transactionType ?? '-',
      },
      {
        value: row?.requestedAmount ? (
          <>
            {convertCurrencyPrefixToCurrencySymbol('thb')} {formatCurrency(row?.requestedAmount)}
          </>
        ) : (
          '-'
        ),
      },
      { value: row?.effectiveDate ? dayjs(row?.effectiveDate).format('DD/MM/YYYY') : '-' },
      { value: row?.createdAt ? <LocalDateTime date={row?.createdAt} /> : '-' },
    ])

  const renderTable = (rows: ITransferCashTransactionDetail[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Transaction Summary'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default TransferCashTransactionDetail
