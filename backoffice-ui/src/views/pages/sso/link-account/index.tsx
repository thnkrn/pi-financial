const validateFilter = (filter: IFilter): string | null => {
  if (!filter.custcode) return 'Please provide custcode'

  return null
}

const extractSSOError = (err: unknown): string => {
  return typeof err === 'object' && err !== null && 'detail' in err
    ? (err as { detail: string }).detail
    : 'Unexpected error'
}

import { getLinkAccountInfo } from '@/lib/api/clients/sso/link-account'
import { ISendLinkAccountInfo } from '@/lib/api/clients/sso/link-account/types'
import Grid from '@mui/material/Grid'
import { useState } from 'react'
import DataTable from './DataTable'
import ReportFilters from './ReportFilters'
import { IFilter } from './types'

const initialFilterState: IFilter = {
  custcode: null,
}

const Index = () => {
  const [filteredData, setFilteredData] = useState<ISendLinkAccountInfo[] | null>([])
  const [currentFilters, setCurrentFilters] = useState<IFilter>(initialFilterState)
  const [loading, setLoading] = useState<boolean>(false)
  const [error, setError] = useState<string | null>(null)

  const loadLinkAccountData = async () => {
    try {
      setFilteredData([])
      setError(null)
      const errorMessage = validateFilter(currentFilters)
      if (errorMessage) {
        setError(errorMessage)

        return
      }
      setLoading(true)
      const linkAccountInfoResponse = await getLinkAccountInfo(currentFilters?.custcode as string)
      setFilteredData(linkAccountInfoResponse.data)
    } catch (err) {
      setError(extractSSOError(err))
    } finally {
      setLoading(false)
    }
  }

  return (
    <Grid container spacing={3}>
      <Grid item xs={12}>
        <ReportFilters filters={currentFilters} setFilters={setCurrentFilters} onApply={loadLinkAccountData} />
      </Grid>
      <Grid item xs={12}>
        <DataTable
          data={filteredData || []}
          paginationData={{
            page: 0,
            pageSize: 1,
            total: filteredData?.length || 1,
          }}
          setPaginationData={() => {}}
          loading={loading}
          fetchData={loadLinkAccountData}
        />
        {error && <p style={{ color: 'red' }}>{error}</p>}
      </Grid>
    </Grid>
  )
}

export default Index
