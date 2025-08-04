import { IUpBackDetail } from '@/lib/api/clients/backoffice/transactions/types'
import Table from '@/tables/Tables'
import { CardHeader } from '@mui/material'
import Card from '@mui/material/Card'
import { useEffect, useState } from 'react'
import LocalDateTime from '../LocalDateTime'

interface Props {
  data: IUpBackDetail
}

const TABLE_HEADER = [
  {
    value: 'STATE',
  },
  {
    value: 'FAILED REASON',
  },
  {
    value: 'CREATED AT',
  },
]

const UpBackDetail = ({ data }: Props) => {
  const [rows, setRows] = useState<IUpBackDetail[]>([])

  useEffect(() => {
    setRows([
      {
        state: data?.state,
        failedReason: data?.failedReason,
        createdAt: data?.createdAt,
      },
    ])
  }, [data])

  const renderRows = (rows: IUpBackDetail[]) =>
    rows?.map((row: IUpBackDetail) => [
      { value: row?.state ?? '-' },
      { value: row?.failedReason ?? '-' },
      { value: row?.createdAt ? <LocalDateTime date={row?.createdAt} /> : '-' },
    ])

  const renderTable = (rows: IUpBackDetail[]) => {
    return <Table showPagination={false} header={TABLE_HEADER} rows={renderRows(rows)} />
  }

  if (rows?.length <= 0) {
    return null
  }

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <CardHeader title='UpBack Detail'></CardHeader>
        {renderTable(rows)}
      </Card>
    </div>
  )
}

export default UpBackDetail
