import { ITransactionDetail } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'
import { formatCurrency } from '@/utils/fmt'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'
import LocalDateTime from '../LocalDateTime'
import dayjs from 'dayjs'

interface Props {
  transactionDetail: ITransactionDetail
}

const TABLE_HEADER = [
  {
    value: 'TRANSACTION NO',
  },
  {
    value: 'TRANSACTION TYPE',
  },
  {
    value: 'SENDER BANK / ACCOUNT NO',
  },
  {
    value: 'SENDER BANK ACCOUNT NAME',
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

const TransactionDetail = ({ transactionDetail }: Props) => {
  const [rows, setRows] = useState<ITransactionDetail[]>([])

  useEffect(() => {
    setRows([
      {
        transactionNo: transactionDetail?.transactionNo,
        transactionType: transactionDetail?.transactionType,
        requestedAmount: transactionDetail?.requestedAmount,
        requestedCurrency: transactionDetail?.requestedCurrency,
        senderBankName: transactionDetail?.senderBankName,
        senderBankAccountNumber: transactionDetail?.senderBankAccountNumber,
        senderBankAccountName: transactionDetail?.senderBankAccountName,
        effectiveDate: transactionDetail?.effectiveDate,
        createdAt: transactionDetail?.createdAt,
      },
    ])
  }, [transactionDetail])

  const renderRows = (rows: ITransactionDetail[]) =>
    rows?.map((row: ITransactionDetail) => [
      {
        value: row?.transactionNo ?? '-',
      },
      {
        value: row?.transactionType ?? '-',
      },
      {
        value: (
          <>
            {row?.senderBankName ?? '-'} / {row?.senderBankAccountNumber ?? '-'}
          </>
        ),
      },
      { value: row?.senderBankAccountName ?? '-' },
      {
        value: row?.requestedAmount ? (
          <>
            {convertCurrencyPrefixToCurrencySymbol(row?.requestedCurrency ?? '')} {formatCurrency(row?.requestedAmount)}
          </>
        ) : (
          '-'
        ),
      },
      { value: row?.effectiveDate ? dayjs(row?.effectiveDate).format('DD/MM/YYYY') : '-' },
      { value: row?.createdAt ? <LocalDateTime date={row?.createdAt} /> : '-' },
    ])

  const renderTable = (rows: ITransactionDetail[]) => {
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

export default TransactionDetail
