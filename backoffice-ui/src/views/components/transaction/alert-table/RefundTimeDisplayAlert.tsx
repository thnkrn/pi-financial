import { Paper, Table, TableCell, TableRow } from '@mui/material'
import LocalDateTime from '../../LocalDateTime'

type AlertTableDialogProps = {
  header: string
  value: Date
}

const RefundTimeAlertTable = ({ header, value }: AlertTableDialogProps) => {
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
            {header}
          </TableCell>
          <TableCell sx={{ borderBottom: '1px solid rgba(224, 224, 224, 1)' }}>
            <LocalDateTime date={value} />
          </TableCell>
        </TableRow>
      </Table>
    </Paper>
  )
}

export default RefundTimeAlertTable
