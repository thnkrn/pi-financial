import { Button, Input } from '@mui/material'
import { IFilter } from './types'

interface Props {
  filters: IFilter
  setFilters: (filter: IFilter) => void
  onApply: () => void
}

const ReportFilters = ({ filters, setFilters, onApply }: Props) => {
  return (
    <div className='flex space-x-4 items-end'>
      <div>
        <Input
          value={filters.custcode || ''}
          onChange={e => setFilters({ ...filters, custcode: e.target.value })}
          placeholder='Enter custcode'
        />
        <Button onClick={onApply}>Apply Filter</Button>
      </div>
    </div>
  )
}

export default ReportFilters
