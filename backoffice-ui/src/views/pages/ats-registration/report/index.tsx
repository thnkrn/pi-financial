import { getAtsReports } from '@/lib/api/clients/backoffice/ats-registration'
import Grid from '@mui/material/Grid'
import format from 'date-fns/format'
import { useEffect, useState } from 'react'
import AtsReportDataTable from './AtsReportDataTable'
import ReportFilters from './ReportFilters'
import { AtsRegistrationReportRowType, Filter, PaginationDataType } from './types'

const initialPaginationData: PaginationDataType = {
  pageSize: 10,
  page: 0,
  total: 0,
}

const initialFilterState: Filter = {
  atsUploadType: null,
  requestDate: null,
}

const Index = () => {
  const [filteredData, setFilteredData] = useState<AtsRegistrationReportRowType[]>([])
  const [currentFilters, setCurrentFilters] = useState<Filter>(initialFilterState)
  const [paginationData, setPaginationData] = useState<PaginationDataType>(initialPaginationData)
  const [loading, setLoading] = useState<boolean>(false)

  const getDataByFilterValue = async (filter: Filter, { page, pageSize }: PaginationDataType) => {
    try {
      setLoading(true)
      const result = await getAtsReports({
        ...filter,
        requestDate: filter.requestDate ? format(new Date(filter.requestDate), 'yyyy/MM/dd') : null,
        pageSize,
        page: page + 1,
      })
      setFilteredData(result.data)
      setPaginationData((prev: PaginationDataType) => ({
        ...prev,
        page: Number(result.page) - 1,
        pageSize: Number(result.pageSize),
        total: Number(result.totalPages! * result.pageSize!),
        hasNextPage: result.hasNextPage,
        hasPreviousPage: result.hasPreviousPage,
      }))

      return result.data
    } finally {
      setLoading(false)
    }
  }

  const handleApplyFilter = () => {
    setPaginationData(initialPaginationData)
    getDataByFilterValue(currentFilters, paginationData)
  }

  useEffect(() => {
    getDataByFilterValue(currentFilters, paginationData)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paginationData.page, paginationData.pageSize])

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <ReportFilters onFilter={setCurrentFilters} filter={currentFilters} onApplyFilter={handleApplyFilter} />
      </Grid>
      <Grid item xs={12}>
        <AtsReportDataTable
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
