import Button from '@mui/material/Button'
import { GridToolbar } from '@mui/x-data-grid'
import { UserRoles } from '@/constants/blocktrade/GlobalEnums'

type CustomToolbarProps = {
  userRole: string
  onSetRollover: () => void
}

export const CustomToolbar = ({ userRole, onSetRollover }: CustomToolbarProps) => {
  if (userRole !== UserRoles.ADMIN) {
    return <GridToolbar showQuickFilter={true} />
  } else {
    return (
      <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
        <div>
          <Button
            variant='outlined'
            color='primary'
            size='small'
            style={{ marginLeft: 10, marginTop: 4, marginBottom: 4, padding: 3 }}
            onClick={onSetRollover}
          >
            Rollover
          </Button>
        </div>
        <GridToolbar showQuickFilter={true} />
      </div>
    )
  }
}
