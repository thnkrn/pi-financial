import { GridToolbar } from '@mui/x-data-grid'

type DefaultToolbarProps = {
  showQuickFilter?: boolean
}

const DefaultToolbar = ({ showQuickFilter = true }: DefaultToolbarProps) => (
  <GridToolbar showQuickFilter={showQuickFilter} />
)

export default DefaultToolbar
