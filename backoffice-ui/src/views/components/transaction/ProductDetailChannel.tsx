import { IProductDetailChannel } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'

interface Props {
  productDetailChannel: IProductDetailChannel
}

const TABLE_HEADER = [
  {
    value: 'CHANNEL',
  },
  {
    value: 'CUSTOMER ACCOUNT NUMBER',
  },
  {
    value: 'PRODUCT',
  },
]

const ProductDetailChannel = ({ productDetailChannel }: Props) => {
  const [rows, setRows] = useState<IProductDetailChannel[]>([])

  useEffect(() => {
    setRows([
      {
        channelName: productDetailChannel?.channelName ?? '',
        customerAccountNumber: productDetailChannel?.customerAccountNumber ?? '',
        accountType: productDetailChannel?.accountType ?? '',
      },
    ])
  }, [productDetailChannel])

  const renderRows = (rows: IProductDetailChannel[]) =>
    rows?.map((row: IProductDetailChannel) => [
      { value: row?.channelName ?? '-' },
      { value: row?.customerAccountNumber ?? '-' },
      { value: row?.accountType ?? '-' },
    ])

  const renderTable = (rows: IProductDetailChannel[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Product - Channel'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default ProductDetailChannel
