import { useTableHeaderStyles } from '@/utils/styles'
import {
  Paper,
  SxProps,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
} from '@mui/material'
import React from 'react'

export type ParamType = {
  value: string | JSX.Element | null
  align?: 'inherit' | 'left' | 'center' | 'right' | 'justify'
  sx?: SxProps
}

const PAGE_PER_SIZE = 10

const DisplayTable = ({
  header,
  rows,
  showPagination,
}: {
  header: ParamType[]
  rows: Array<ParamType[]> | undefined
  showPagination: boolean
}) => {
  const classes = useTableHeaderStyles()

  const [page, setPage] = React.useState(0)
  const handleChangePage = (_event: any, newPage: React.SetStateAction<number>) => {
    setPage(newPage)
  }

  return (
    <TableContainer component={Paper}>
      <Table sx={{ border: '1px solid rgba(224, 224, 224, 1)' }} aria-label='Table'>
        <TableHead>
          <TableRow>
            {header.map(head => (
              <TableCell key={`TableHead-${head.value}`} className={classes.tableHead} align={head.align} sx={head.sx}>
                {head.value}
              </TableCell>
            ))}
          </TableRow>
        </TableHead>
        <TableBody>
          {!rows || rows?.length === 0 ? (
            <TableRow>
              <TableCell colSpan={header.length}>No data</TableCell>
            </TableRow>
          ) : (
            (showPagination ? rows.slice(page * PAGE_PER_SIZE, page * PAGE_PER_SIZE + PAGE_PER_SIZE) : rows).map(
              (row, index) => (
                <TableRow key={`TableBody-${index}`}>
                  {row.map((row, rowIndex) => (
                    <TableCell
                      align={row?.align}
                      key={`TableCell-${index}-${rowIndex}`}
                      className={classes.tableBody}
                      sx={row?.sx}
                    >
                      {row?.value ? row.value : '-'}
                    </TableCell>
                  ))}
                </TableRow>
              )
            )
          )}
        </TableBody>
      </Table>

      {showPagination && rows && rows?.length > PAGE_PER_SIZE && (
        <TablePagination
          rowsPerPageOptions={[]}
          component='div'
          count={rows.length}
          rowsPerPage={PAGE_PER_SIZE}
          page={page}
          onPageChange={handleChangePage}
        />
      )}
    </TableContainer>
  )
}

export default DisplayTable
