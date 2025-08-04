import { IRecoveryDetail } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'
import LocalDateTime from '../LocalDateTime'
import { formatCurrency } from '@/utils/fmt'
import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'

interface Props {
  data: IRecoveryDetail
}

const TABLE_HEADER = [
  {
    value: 'STATE',
  },
  {
    value: 'GLOBAL ACCOUNT',
  },
  {
    value: 'TRANSFER AMOUNT',
  },
  {
    value: 'TRANSFER FROM ACCOUNT',
  },
  {
    value: 'TRANSFER TO ACCOUNT',
  },
  {
    value: 'TRANSFER COMPLETE TIME',
  },
  {
    value: 'FAILED REASON',
  },
  {
    value: 'CREATED AT',
  },
]

const RecoveryDetail = ({ data }: Props) => {
  const [rows, setRows] = useState<IRecoveryDetail[]>([])

  useEffect(() => {
    setRows([
      {
        state: data?.state,
        globalAccount: data?.globalAccount,
        transferAmount: data?.transferAmount,
        transferCurrency: data?.transferCurrency,
        transferFromAccount: data?.transferFromAccount,
        transferToAccount: data?.transferToAccount,
        transferCompleteTime: data?.transferCompleteTime,
        failedReason: data?.failedReason,
        createdAt: data?.createdAt,
      },
    ])
  }, [data])

  const renderRows = (rows: IRecoveryDetail[]) =>
    rows?.map((row: IRecoveryDetail) => [
      { value: row?.state ?? '-' },
      { value: row?.globalAccount ?? '-' },
      {
        value: row?.transferAmount ? (
          <>
            {row?.transferCurrency ? convertCurrencyPrefixToCurrencySymbol(row?.transferCurrency) : ''}{' '}
            {formatCurrency(row?.transferAmount)}
          </>
        ) : (
          '-'
        ),
      },
      { value: row?.transferFromAccount ?? '-' },
      { value: row?.transferToAccount ?? '-' },
      { value: row?.transferCompleteTime ? <LocalDateTime date={row?.transferCompleteTime} /> : '-' },
      { value: row?.failedReason ?? '-' },
      { value: row?.createdAt ? <LocalDateTime date={row?.createdAt} /> : '-' },
    ])

  const renderTable = (rows: IRecoveryDetail[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Recovery Detail'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default RecoveryDetail
