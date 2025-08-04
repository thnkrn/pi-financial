import { ITransferCashCustomerProfile } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'

interface Props {
  customerProfile: ITransferCashCustomerProfile
}

const TABLE_HEADER = [
  {
    value: 'CUSTOMER ACCOUNT NAME',
  },
]

const TransferCashCustomerProfile = ({ customerProfile }: Props) => {
  const [rows, setRows] = useState<ITransferCashCustomerProfile[]>([])

  useEffect(() => {
    setRows([
      {
        customerName: customerProfile?.customerName ?? '',
      },
    ])
  }, [customerProfile])

  const renderTable = (rows: ITransferCashCustomerProfile[]) => {
    return (
      <Table
        showPagination={false}
        header={TABLE_HEADER}
        rows={rows.map((row: ITransferCashCustomerProfile) => [
          {
            value: row?.customerName ?? '-',
          },
        ])}
      />
    )
  }

  if (rows?.length === 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='Customer Profile' />
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default TransferCashCustomerProfile
