import { ICustomerProfile } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'

interface Props {
  customerProfile: ICustomerProfile
}

const TABLE_HEADER = [
  {
    value: 'CUSTOMER CODE',
  },
  {
    value: 'CUSTOMER ACCOUNT NAME',
  },
]

const CustomerProfile = ({ customerProfile }: Props) => {
  const [rows, setRows] = useState<ICustomerProfile[]>([])

  useEffect(() => {
    setRows([
      {
        customerCode: customerProfile?.customerCode ?? '',
        customerAccountName: customerProfile?.customerAccountName ?? '',
      },
    ])
  }, [customerProfile])

  const renderTable = (rows: ICustomerProfile[]) => {
    return (
      <Table
        showPagination={false}
        header={TABLE_HEADER}
        rows={rows.map((row: ICustomerProfile) => [
          { value: row?.customerCode ?? '-' },
          {
            value: row?.customerAccountName ?? '-',
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

export default CustomerProfile
