import { IGlobalTransferDetail } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { convertCurrencyPrefixToCurrencySymbol } from '@/utils/convert'
import { formatCurrency } from '@/utils/fmt'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'
import LocalDateTime from '../LocalDateTime'

interface Props {
  data: IGlobalTransferDetail
}

const TABLE_HEADER = [
  {
    value: 'STATE',
  },
  {
    value: 'GLOBAL ACCOUNT',
  },
  {
    value: 'FX RATE',
  },
  {
    value: 'ACTUAL FX RATE',
  },
  {
    value: 'FX CONFIRMED DATE TIME',
  },
  {
    value: 'TRANSFER AMOUNT',
  },
  {
    value: 'TRANSFER FEE',
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

const GlobalTransferDetail = ({ data }: Props) => {
  const [rows, setRows] = useState<IGlobalTransferDetail[]>([])

  useEffect(() => {
    setRows([
      {
        state: data?.state,
        globalAccount: data?.globalAccount,
        requestedFxRate: data?.requestedFxRate,
        requestedFxCurrency: data?.requestedFxCurrency,
        fxConfirmedExchangeRate: data?.fxConfirmedExchangeRate,
        fxConfirmedDateTime: data?.fxConfirmedDateTime,
        transferAmount: data?.transferAmount,
        transferFee: data?.transferFee,
        transferCurrency: data?.transferCurrency,
        transferCompleteTime: data?.transferCompleteTime,
        failedReason: data?.failedReason,
        createdAt: data?.createdAt,
      },
    ])
  }, [data])

  const renderRows = (rows: IGlobalTransferDetail[]) =>
    rows?.map((row: IGlobalTransferDetail) => [
      { value: row?.state ?? '-' },
      { value: row?.globalAccount ?? '-' },
      { value: row?.requestedFxRate ? formatCurrency(row?.requestedFxRate) : '-' },
      { value: row?.fxConfirmedExchangeRate ? formatCurrency(row?.fxConfirmedExchangeRate) : '-' },
      { value: row?.fxConfirmedDateTime ? <LocalDateTime date={row?.fxConfirmedDateTime} /> : '-' },
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
      {
        value: row?.transferFee ? (
          <>
            {convertCurrencyPrefixToCurrencySymbol('usd' as string)} {formatCurrency(row?.transferFee)}
          </>
        ) : (
          '-'
        ),
      },
      { value: row?.transferCompleteTime ? <LocalDateTime date={row?.transferCompleteTime} /> : '-' },
      { value: row?.failedReason ?? '-' },
      { value: row?.createdAt ? <LocalDateTime date={row?.createdAt} /> : '-' },
    ])

  const renderTable = (rows: IGlobalTransferDetail[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Global Transfer Detail'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default GlobalTransferDetail
