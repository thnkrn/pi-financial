import { ITicket } from '@/lib/api/clients/backoffice/transactions/types'
import ExpandMoreIcon from '@mui/icons-material/ExpandMore'
import { Typography } from '@mui/material'
import MuiAccordion from '@mui/material/Accordion'
import AccordionDetails from '@mui/material/AccordionDetails'
import AccordionSummary from '@mui/material/AccordionSummary'
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import { styled } from '@mui/material/styles'
import groupBy from 'lodash/groupBy'
import { useMemo } from 'react'
import HistoryActionTable from './HistoryActionTable'

const Accordion = styled(MuiAccordion)(() => ({
  boxShadow: 'none',
}))

interface ITickets {
  [ticketId: string]: ITicket[]
}

interface Props {
  makerTickets: ITicket[]
  checkerTickets: ITicket[]
}

const HistoryAction = ({ makerTickets, checkerTickets }: Props) => {
  const groupedTickets: ITickets = useMemo(() => {
    return groupBy(
      [
        ...(makerTickets.length > 0 ? [...makerTickets] : []),
        ...(checkerTickets.length > 0 ? [...checkerTickets] : []),
      ],
      (ticket: ITicket) => ticket.ticketId
    )
  }, [makerTickets, checkerTickets])

  if (Object.keys(groupedTickets).length <= 0) {
    return null
  }

  return (
    <Card sx={{ marginTop: '40px' }}>
      <CardHeader title='History Action' sx={{ typography: 'h6' }} />
      {Object.entries(groupedTickets).map(([ticketId, tickets], index) => (
        <Accordion key={ticketId} defaultExpanded={index === Object.keys(groupedTickets).length - 1}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />} aria-controls='panel1a-content' id='panel1a-header'>
            <Typography variant='body1'>
              {`• Ticket : ${ticketId}${
                tickets[0]?.ticketDescription || tickets[1]?.ticketDescription
                  ? ` - ${tickets[0]?.ticketDescription || tickets[1]?.ticketDescription}`
                  : ''
              }${
                tickets[0]?.ticketStatus || tickets[1]?.ticketStatus
                  ? ` - ${tickets[0]?.ticketStatus || tickets[1]?.ticketStatus}`
                  : ''
              }`}
            </Typography>
          </AccordionSummary>
          <AccordionDetails>
            <HistoryActionTable tickets={tickets} />
          </AccordionDetails>
        </Accordion>
      ))}
    </Card>
  )
}

export default HistoryAction
