import { getOpenAccounts } from '@/lib/api/clients/backoffice/application-summary'
import Grid from '@mui/material/Grid'
import { GridSortModel } from '@mui/x-data-grid'
import { useState } from 'react'
import ApplicationFilters from './ApplicationFilters'
import ApplicationSummaryDataTable from './ApplicationSummaryDataTable'
import { ApplicationSummaryRowType, Filter, PaginationDataType } from './types'

const initialPaginationData: PaginationDataType = {
  pageSize: 20,
  page: 0,
  total: 0,
}

const initialFilterState: Filter = {
  status: '',
  bpmReceived: true,
  userId: '',
  custCode: '',
  orderBy: 'CreatedAt',
  orderDir: 'desc',
  errorCheck: false,
}

const Index = () => {
  const [filteredData, setFilteredData] = useState<ApplicationSummaryRowType[]>([])
  const [currentFilters, setCurrentFilters] = useState<Filter>(initialFilterState)
  const [paginationData, setPaginationData] = useState<PaginationDataType>(initialPaginationData)
  const [loading, setLoading] = useState<boolean>(false)

  const getDataByFilterValue = async (filter: Filter, { pageSize, page }: PaginationDataType) => {
    try {
      setLoading(true)
      const result = await getOpenAccounts({
        ...filter,
        bpmReceived: !filter.errorCheck,
        pageSize,
        page: page + 1,
      })

      if (typeof result === 'object' && result.accounts) {
        const resultData = result.accounts as ApplicationSummaryRowType[]
        setFilteredData(resultData)
        setLoading(false)

        setPaginationData((prev: PaginationDataType) => ({
          ...prev,
          page: Number(result.page) - 1,
          pageSize: Number(result.pageSize),
          total: Number(result.total),
        }))

        return resultData
      } else {
        setLoading(false)
      }
    } catch (error) {
      setLoading(false)
    }
  }

  const getPageFilterData = (pageFilter: PaginationDataType) => {
    setPaginationData(pageFilter)
    getDataByFilterValue(currentFilters, pageFilter)
  }

  const handleFilter = (filter: Filter) => {
    const { orderBy, orderDir } = currentFilters
    const newFilter = { ...filter, orderBy, orderDir }
    getDataByFilterValue(newFilter, paginationData)
    setCurrentFilters(newFilter)
  }

  const handleSync = (filter: Filter) => {
    handleFilter(filter)
  }

  const handleSortModelChange = (sortModel: GridSortModel) => {
    if (sortModel?.length > 0) {
      const { field, sort } = sortModel[0]

      setCurrentFilters((prev: Filter) => ({
        ...prev,
        orderBy: field,
        orderDir: sort as string,
      }))

      const newPaginationData = {
        ...paginationData,
        page: 0,
      }

      getDataByFilterValue(
        {
          ...currentFilters,
          orderBy: field,
          orderDir: sort as string,
        },
        newPaginationData
      )
    }
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <ApplicationFilters onFilter={handleFilter} onSync={handleSync} />
        <ApplicationSummaryDataTable
          onSortModelChange={handleSortModelChange}
          pagination={paginationData}
          onPageFilter={getPageFilterData}
          rows={filteredData}
          loading={loading}
        />
      </Grid>
    </Grid>
  )
}

export default Index
