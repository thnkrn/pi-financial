import { GridColDef } from '@mui/x-data-grid'
import { filter, isEmpty, map, upperCase } from 'lodash'

export interface CsvHeader {
  label: string
  key: string
}

export const getCsvHeaders = (columns: GridColDef[]): CsvHeader[] => {
  const headers = map(columns, c => {
    return { label: upperCase(c.headerName), key: c.field as string } as CsvHeader
  })

  return filter(headers, h => {
    return !isEmpty(h.label)
  })
}
