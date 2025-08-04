import { ACTION_AFFINITY, RESPONSE_ACTIONS } from '@/constants/ResponseActions'
import {
  IPayloadResponse,
  ITicket,
  UpdateBillPaymentReferencePayload,
} from '@/lib/api/clients/backoffice/transactions/types'
import { Chip, IconButton, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material'
import { makeStyles } from '@mui/styles'
import LocalDateTime from '../LocalDateTime'
import NameMismatchAlertTable from '@/views/components/transaction/alert-table/NameMismatchAlertTable'
import SearchIcon from '@mui/icons-material/Search'
import withReactContent from 'sweetalert2-react-content'
import Swal from 'sweetalert2'
import { getTicketPayload } from '@/lib/api/clients/backoffice/transactions'
import { useEffect, useState } from 'react'

const useMakerTableHeaderStyles = makeStyles({
  tableHeadRow: {
    fontWeight: 'bold',
    backgroundColor: '#888888',
    color: '#fff',
    textTransform: 'uppercase',
    border: '1px solid rgba(247, 247, 247, 0.3)',
  },
  tableBodyRoll: {
    position: 'relative',
    border: '1px solid rgba(76, 78, 100, 0.22)',
  },
})

const TABLE_HEADER: Array<{ value: string }> = [
  {
    value: 'ACTION BY',
  },
  {
    value: 'TIMESTAMP',
  },
  {
    value: 'OPERATE NAME',
  },
  {
    value: 'ACTION STATUS',
  },
  {
    value: 'REMARK',
  },
]

interface Props {
  tickets: ITicket[]
}

const getStatusColor = (status: string | null | undefined) => {
  const responseAction = RESPONSE_ACTIONS.find(v => v.alias === status)

  return responseAction?.affinity === ACTION_AFFINITY.POSITIVE ? '#72E128 !important' : '#FF4D49 !important'
}

const ALERT_TABLE_HEADER = ['OLD ACCOUNT NO.', 'NEW ACCOUNT NO.']

const HistoryActionTable = ({ tickets }: Props) => {
  const classes = useMakerTableHeaderStyles()

  const [payloadResponses, setPayloadResponses] = useState<Record<string, IPayloadResponse | null>>({})

  useEffect(() => {
    const fetchPayloadResponses = async () => {
      const responses: Record<string, IPayloadResponse | null> = {}

      await Promise.all(
        tickets.map(async ticket => {
          if (ticket.ticketId) {
            try {
              responses[ticket.ticketId] = await getTicketPayload(ticket.ticketId)
            } catch (error) {
              responses[ticket.ticketId] = null
            }
          }
        })
      )

      setPayloadResponses(responses)
    }

    fetchPayloadResponses()
  }, [tickets])

  const MySwal = withReactContent(Swal)
  const displayAlert = ({ title, element }: { title: string; element: JSX.Element }) => {
    MySwal.fire({
      title: `<small style="text-align: left; display:flex;">${title}</small>`,
      html: element,
      showCancelButton: true,
      cancelButtonText: 'Close',
      showConfirmButton: false,
      cancelButtonColor: '#3dd884',
    })
  }

  return (
    <TableContainer
      sx={{ boxShadow: '0px 5px 8px 0px rgba(0, 0, 0, 0.42) !important', borderRadius: '10px !important' }}
    >
      <Table aria-label='Table'>
        <TableHead>
          <TableRow sx={{ border: 1 }}>
            {TABLE_HEADER.map(head => (
              <TableCell
                key={head.value}
                className={classes.tableHeadRow}
                sx={{ border: '1px solid rgba(247, 247, 247, 0.3)' }}
              >
                {head.value}
              </TableCell>
            ))}
          </TableRow>
        </TableHead>
        <TableBody>
          {tickets.map(ticket => {
            const ticketPayload = payloadResponses[ticket.ticketId ?? '']

            return (
              <TableRow hover key={`${ticket?.actionBy}-${ticket?.ticketId}`}>
                <TableCell className={classes.tableBodyRoll}>
                  <Chip
                    key={ticket?.actionBy}
                    variant='outlined'
                    sx={{
                      typography: 'body1',
                      fontSize: '0.875rem',
                      color: '#FFF',
                      backgroundColor: ticket?.actionBy === 'Maker' ? '#1AA57A !important' : '#666CFF !important',
                    }}
                    label={ticket?.actionBy}
                  />
                </TableCell>
                <TableCell className={classes.tableBodyRoll}>
                  {ticket?.timestamp ? <LocalDateTime date={ticket?.timestamp} /> : '-'}
                </TableCell>
                <TableCell className={classes.tableBodyRoll}>{ticket?.name}</TableCell>
                <TableCell className={classes.tableBodyRoll}>
                  <Chip
                    key={ticket?.status}
                    variant='outlined'
                    sx={{
                      typography: 'body1',
                      fontSize: '0.875rem',
                      color: getStatusColor(ticket?.status),
                      borderColor: getStatusColor(ticket?.status),
                    }}
                    label={ticket?.status}
                  />
                </TableCell>
                <TableCell className={classes.tableBodyRoll}>
                  {ticket?.remark}
                  {ticketPayload?.action === 'UpdateBillPaymentReference' && (
                    <IconButton
                      sx={{ ml: 'auto' }}
                      size='small'
                      onClick={() => {
                        const payload = JSON.parse(ticketPayload?.payload ?? '{}') as UpdateBillPaymentReferencePayload
                        displayAlert({
                          title: 'Account No.',
                          element: (
                            <NameMismatchAlertTable
                              header={ALERT_TABLE_HEADER}
                              values={[payload.oldReference as string, payload.newReference as string]}
                            />
                          ),
                        })
                      }}
                    >
                      <SearchIcon />
                    </IconButton>
                  )}
                </TableCell>
              </TableRow>
            )
          })}
        </TableBody>
      </Table>
    </TableContainer>
  )
}

export default HistoryActionTable
