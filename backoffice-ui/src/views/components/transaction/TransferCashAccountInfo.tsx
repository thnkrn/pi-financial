import { ITransferCashAccountInfo } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'

interface Props {
  accountInfo: ITransferCashAccountInfo
}

const TABLE_HEADER = [
  {
    value: 'TRANSFER FROM ACCOUNT NUMBER',
  },
  {
    value: 'TRANSFER FROM ACCOUNT',
  },
  {
    value: 'TRANSFER TO ACCOUNT NUMBER',
  },
  {
    value: 'TRANSFER TO ACCOUNT',
  },
]

const ProductDetailChannel = ({ accountInfo }: Props) => {
  const [rows, setRows] = useState<ITransferCashAccountInfo[]>([])

  useEffect(() => {
    setRows([
      {
        transferFromAccountCode: accountInfo?.transferFromAccountCode ?? '',
        transferFromExchangeMarket: accountInfo?.transferFromExchangeMarket ?? '',
        transferToAccountCode: accountInfo?.transferToAccountCode ?? '',
        transferToExchangeMarket: accountInfo?.transferToExchangeMarket ?? '',
      },
    ])
  }, [accountInfo])

  const renderRows = (rows: ITransferCashAccountInfo[]) =>
    rows?.map((row: ITransferCashAccountInfo) => [
      { value: row?.transferFromAccountCode ?? '-' },
      { value: row?.transferFromExchangeMarket ?? '-' },
      { value: row?.transferToAccountCode ?? '-' },
      { value: row?.transferToExchangeMarket ?? '-' },
    ])

  const renderTable = (rows: ITransferCashAccountInfo[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Transfer Account Info'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default ProductDetailChannel
