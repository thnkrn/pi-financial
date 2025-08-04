import { fetchCuratedMembers, resetState, updateCuratedMemberState } from '@/store/apps/curated-manager/member'
import { AppDispatch, RootState } from '@/store/index'
import { PaginationParams } from '@/types/apps/paginationTypes'
import { CuratedMemberState } from '@/types/curated-manager/member'
import { Grid, Typography } from '@mui/material'
import { useEffect, useMemo, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import CuratedMemberTable from './CuratedMemberTable'

interface CuratedMemberProps {
  curatedListId: number
  curatedListName: string
}

const Index = ({ curatedListId, curatedListName }: CuratedMemberProps) => {
  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)
  const dispatch = useDispatch<AppDispatch>()
  const store = useSelector((state: RootState) => state.curatedMember)

  const onPaginate = (currentFilter: PaginationParams) => {
    setCurrentPage(currentFilter.page)
    setPageSize(currentFilter.pageSize)
  }

  useEffect(() => {
    const initialState: CuratedMemberState = {
      isLoading: false,
      errorMessage: '',
      members: [],
    }

    dispatch(updateCuratedMemberState(initialState))

    dispatch(fetchCuratedMembers(curatedListId))

    return () => {
      dispatch(updateCuratedMemberState(initialState))
      dispatch(resetState())
    }
  }, [dispatch, curatedListId])

  const paginatedRows = useMemo(() => {
    return store.members.slice((currentPage - 1) * pageSize, currentPage * pageSize)
  }, [store.members, currentPage, pageSize])

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Typography variant='body2' color='text.secondary' sx={{ mb: 3 }}>
          {curatedListName}
        </Typography>

        <CuratedMemberTable
          row={paginatedRows}
          onPaginate={onPaginate}
          isLoading={store.isLoading}
          store={store}
          total={store.members.length}
        />
      </Grid>
    </Grid>
  )
}

export default Index
