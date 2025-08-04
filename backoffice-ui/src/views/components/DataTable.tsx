import { formatDate } from '@/utils/fmt'
import Card from '@mui/material/Card'
import { DataGrid, GridColDef, GridFooter, GridFooterContainer } from '@mui/x-data-grid'
import { forOwn, includes, isEmpty, keys, map } from 'lodash'
import extend from 'lodash/extend'
import { useEffect, useRef, useState } from 'react'
import ExportCSV from './ExportCSV'

interface Props {
  rows: any[]
  total: number
  columns: GridColDef[]
  dateFormatFields?: DateFormat
  store: any
  onPaginate: (currentFilter: any) => void
  isDisabledFilter?: boolean
  exportFileName: string
  isLoading: boolean
  csvTransform?: CsvTransform
  initialState?: any
}

interface DateFormat {
  field: Array<string>
  format: string
}
interface CsvTransform {
  [key: string]: string
}

export const DataTable = ({
  rows,
  total,
  columns,
  dateFormatFields,
  store,
  onPaginate,
  isDisabledFilter = false,
  csvTransform,
  exportFileName,
  isLoading = false,
  initialState,
}: Props) => {
  const [paginationModel, setPaginationModel] = useState({ page: 0, pageSize: 20 })
  const initialRender = useRef(true)
  const [csvRows, setCsvRows] = useState<any[]>(rows)

  useEffect(() => {
    if (initialRender.current) {
      initialRender.current = false
    } else {
      const currentFilter = extend(
        {},
        { ...store.filter },
        { page: paginationModel.page + 1, pageSize: paginationModel.pageSize }
      )

      onPaginate(currentFilter)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paginationModel])

  useEffect(() => {
    const csvRows = map(rows, r => {
      const csvRow = {
        ...r,
      }

      forOwn(r, (value: any, key) => {
        if (value && key) {
          if (dateFormatFields && includes(dateFormatFields.field, key)) {
            csvRow[key] = formatDate(value, { format: dateFormatFields.format })
          }
          if (!isEmpty(csvTransform) && includes(keys(csvTransform), key)) {
            csvRow[key] = value[csvTransform[key]]
          }
        }
      })

      return csvRow
    })
    setCsvRows(csvRows)
  }, [dateFormatFields, csvTransform, rows])

  const renderFooter = () => (
    <GridFooterContainer>
      <ExportCSV data={csvRows} fileName={exportFileName} columnsDef={columns} />
      <GridFooter />
    </GridFooterContainer>
  )

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <DataGrid
          autoHeight
          pagination
          rows={rows}
          rowCount={total}
          columns={columns}
          paginationMode='server'
          pageSizeOptions={[20, 50, 100]}
          paginationModel={paginationModel}
          onPaginationModelChange={setPaginationModel}
          disableColumnFilter={isDisabledFilter}
          loading={isLoading}
          components={{
            Footer: () => renderFooter(),
          }}
          initialState={initialState}
        />
      </Card>
    </div>
  )
}
