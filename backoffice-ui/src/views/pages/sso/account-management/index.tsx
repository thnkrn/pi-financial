import { IAccountInfo } from '@/lib/api/clients/sso/account-management/types'
import { datadogLogs } from '@datadog/browser-logs'
import Grid from '@mui/material/Grid'
import { useEffect, useState } from 'react'
import DataTable from './DataTable'
import ReportFilters from './ReportFilters'
import { Filter, PaginationDataType } from './types'

const initialPaginationData: PaginationDataType = {
  pageSize: 10,
  page: 0,
  total: 0,
}

const initialFilterState: Filter = {
  username: null,
}

const Index = () => {
  const [filteredData, setFilteredData] = useState<IAccountInfo[] | null>(null)
  const [currentFilters, setCurrentFilters] = useState<Filter>(initialFilterState)
  const [paginationData, setPaginationData] = useState<PaginationDataType>(initialPaginationData)
  const [loading, setLoading] = useState<boolean>(false)
  const [error, setError] = useState<string | null>(null) // สำหรับจัดการ error message

  const fetchAccountInfo = async () => {
    try {
      setLoading(true)
      const filter = currentFilters.username !== null ? `&username=${currentFilters.username}` : ''
      const response = await fetch(
        `/api/sso/account-management/getAccountInfos?page=${paginationData.page + 1}&pageSize=${
          paginationData.pageSize
        }${filter}`
      )
      const json = await response.json()

      setFilteredData(json.data)
      setPaginationData((prev: PaginationDataType) => ({
        ...prev,
        page: Number(json?.currentPage) - 1,
        total: Number(json?.totalPages * json?.pageSize),
        hasNextPage: json?.hasNextPage,
        hasPreviousPage: json?.hasPreviousPage,
      }))
    } catch (err: any) {
      setError('Failed to fetch account information ' + err)
      datadogLogs.logger.error(err)
    } finally {
      setLoading(false)
    }
  }

  const handleApplyFilter = () => {
    setPaginationData(initialPaginationData) // รีเซ็ต pagination
    fetchAccountInfo()
  }

  useEffect(() => {
    fetchAccountInfo()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentFilters, paginationData.page, paginationData.pageSize])

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <ReportFilters
          fetchAccountData={async () => await fetchAccountInfo()}
          onFilter={setCurrentFilters}
          filter={currentFilters}
          onApplyFilter={handleApplyFilter}
        />
      </Grid>
      <Grid item xs={12}>
        {error && <div style={{ color: 'red' }}>{error}</div>} {/* แสดงข้อความ error */}
        <DataTable
          fetchAccountData={fetchAccountInfo}
          pagination={paginationData}
          onPageFilter={setPaginationData}
          rows={filteredData}
          loading={loading}
        />
      </Grid>
    </Grid>
  )
}

export default Index
