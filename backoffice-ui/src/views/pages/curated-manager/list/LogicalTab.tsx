import { updateCuratedList } from '@/store/apps/curated-manager/list'
import { AppDispatch, RootState } from '@/store/index'
import { PaginationParams } from '@/types/apps/paginationTypes'
import { UpdateCuratedList } from '@/types/curated-manager/list'
import { memo, useCallback, useMemo, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import CuratedDataTable from '../CuratedDataTable'

const LogicalTabContent = () => {
  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)
  const dispatch = useDispatch<AppDispatch>()
  const store = useSelector((state: RootState) => state.curatedList)
  const { isLoading, logicalList } = store

  const onPaginate = useCallback((currentFilter: PaginationParams) => {
    setCurrentPage(currentFilter.page)
    setPageSize(currentFilter.pageSize)
  }, [])

  const handleSave = useCallback(
    (updateCuratedListValue: UpdateCuratedList) => {
      return dispatch(updateCuratedList(updateCuratedListValue))
    },
    [dispatch]
  )

  const enhancedLogicalList = useMemo(() => {
    return logicalList.map(item => ({
      ...item,
      onSave: (id: string, field: string, newValue: string) =>
        handleSave({
          id,
          dataSource: item.curatedListSource,
          [field]: newValue,
        }),
    }))
  }, [logicalList, handleSave])

  const paginatedRows = useMemo(() => {
    const startIndex = (currentPage - 1) * pageSize
    const endIndex = startIndex + pageSize

    return enhancedLogicalList.slice(startIndex, endIndex)
  }, [enhancedLogicalList, currentPage, pageSize])

  return (
    <CuratedDataTable
      rows={paginatedRows}
      total={logicalList.length}
      store={store}
      onPaginate={onPaginate}
      isLoading={isLoading}
      isLogicalTab={true}
    />
  )
}

export default memo(LogicalTabContent)
