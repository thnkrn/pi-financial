import Grid from '@mui/material/Grid'
import LoadingOverlay from 'react-loading-overlay-ts'
import { useSelector } from 'react-redux'
import { RootState } from 'src/store'
import TicketDataTable from './TicketDataTable'
import TicketFilter from './TicketFilter'

const Index = () => {
  const store = useSelector((state: RootState) => state.centralWorkspace)

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <LoadingOverlay active={store.isLoading} spinner text='Loading ...'>
          <TicketFilter />
          <TicketDataTable />
        </LoadingOverlay>
      </Grid>
    </Grid>
  )
}

export default Index
