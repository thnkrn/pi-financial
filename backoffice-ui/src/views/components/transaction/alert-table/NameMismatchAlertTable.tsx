import { Paper, Table, TableCell, TableRow } from '@mui/material'

interface AlertTableDialogProps {
  header: Array<string>
  values: Array<string>
}

const NameMismatchAlertTable = ({ header, values }: AlertTableDialogProps) => {
  return (
    <Paper sx={{ maxWidth: 900, margin: '0 auto' }}>
      <Table
        sx={{
          border: '1px solid rgba(224, 224, 224, 1)',
          tableLayout: 'auto',
        }}
        aria-label='alert table'
      >
        <TableRow>
          <TableCell variant='head' sx={{ fontWeight: 'bold', borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            {header[0]}
          </TableCell>
          <TableCell sx={{ borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>{values[0]}</TableCell>
        </TableRow>
        <TableRow>
          <TableCell variant='head' sx={{ fontWeight: 'bold', borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            {header[1]}
          </TableCell>
          <TableCell sx={{ borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>{values[1]}</TableCell>
        </TableRow>
      </Table>
    </Paper>
  )
}

export default NameMismatchAlertTable
