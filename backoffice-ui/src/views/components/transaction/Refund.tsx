import { IRefundDetail } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'
import LocalDateTime from '../LocalDateTime'
import { formatCurrency } from '@/utils/fmt'
import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'

interface Props {
  data: IRefundDetail
}

const TABLE_HEADER = [
  {
    value: 'REFUND ID',
  },
  {
    value: 'AMOUNT',
  },
  {
    value: 'FEE',
  },
  {
    value: 'TRANSFER TO ACCOUNT NO',
  },
  {
    value: 'TRANSFER TO ACCOUNT NAME',
  },
  {
    value: 'CREATED AT',
  },
]

const RefundDetail = ({ data }: Props) => {
  const [rows, setRows] = useState<IRefundDetail[]>([])

  useEffect(() => {
    setRows([
      {
        id: data?.id,
        amount: data?.amount,
        fee: data?.fee,
        transferToAccountNo: data?.transferToAccountNo,
        transferToAccountName: data?.transferToAccountName,
        createdAt: data?.createdAt,
      },
    ])
  }, [data])

  const renderRows = (rows: IRefundDetail[]) =>
    rows?.map((row: IRefundDetail) => [
      { value: row?.id ?? '-' },
      {
        value: row?.amount ? (
          <>
            {convertCurrencyPrefixToCurrencySymbol('thb' as string)} {formatCurrency(row?.amount)}
          </>
        ) : (
          '-'
        ),
      },
      {
        value: row?.fee ? (
          <>
            {convertCurrencyPrefixToCurrencySymbol('thb' as string)} {formatCurrency(row?.fee)}
          </>
        ) : (
          '-'
        ),
      },
      { value: row?.transferToAccountNo ?? '-' },
      { value: row?.transferToAccountName ?? '-' },
      { value: row?.createdAt ? <LocalDateTime date={row?.createdAt} /> : '-' },
    ])

  const renderTable = (rows: IRefundDetail[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Refund Detail'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default RefundDetail
