import FileDownloadIcon from '@mui/icons-material/FileDownload'
import { Button } from '@mui/material'
import React from 'react'
import { CSVLink } from 'react-csv'

import dayjs from 'dayjs'
import { GridColDef } from '@mui/x-data-grid'
import { getCsvHeaders } from '@/utils/csv'

const ExportCSV: React.FC<{
  data: Array<any>
  fileName: string
  columnsDef: GridColDef[]
}> = ({ data, fileName, columnsDef }) => {
  if (data.length === 0) return null

  const csvHeaders = getCsvHeaders(columnsDef)

  return (
    <CSVLink
      style={{ textDecoration: 'none' }}
      data={data}
      headers={csvHeaders}
      filename={`${fileName}_${dayjs().format('YYYY-MM-DD-HH:mm:ss')}.csv`}
      target='_blank'
      rel='noopener noreferrer'
    >
      <Button variant='contained' color='primary' sx={{ ml: 4 }} startIcon={<FileDownloadIcon />}>
        Download CSV
      </Button>
    </CSVLink>
  )
}

export default ExportCSV
