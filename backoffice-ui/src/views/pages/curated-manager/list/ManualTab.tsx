import { deleteCuratedList, uploadCuratedList } from '@/store/apps/curated-manager/list'
import { AppDispatch, RootState } from '@/store/index'
import { PaginationParams } from '@/types/apps/paginationTypes'
import { DataSource } from '@/types/curated-manager'
import { CuratedListTable, ExtendedCuratedListTable } from '@/types/curated-manager/list'
import { AddOutlined } from '@mui/icons-material'
import { Button, FormControl, FormControlLabel, Grid, Radio, RadioGroup, Typography } from '@mui/material'
import { useCallback, useMemo, useRef, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import CuratedDataTable from '../CuratedDataTable'
import SearchField from '../SearchField'
import { VisuallyHiddenInput } from '../VisuallyHiddenInput'

const ManualTabContent = () => {
  const dispatch = useDispatch<AppDispatch>()
  const store = useSelector((state: RootState) => state.curatedList)
  const { isLoading, manualList } = useSelector((state: RootState) => state.curatedList)
  const [searchQuery, setSearchQuery] = useState<string>('')
  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
  const [dataSource, setDataSource] = useState<DataSource>(DataSource.SET)
  const fileInputRef = useRef<HTMLInputElement>(null)

  const handleSearch = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value)
  }, [])

  const onPaginate = useCallback((currentFilter: PaginationParams) => {
    setCurrentPage(currentFilter.page)
    setPageSize(currentFilter.pageSize)
  }, [])

  const handleFileChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    const files = event?.target?.files
    if (files && files.length > 0) {
      const file = files[0]

      if (file.type !== 'text/csv' && !file.name.endsWith('.csv')) {
        alert('Please upload a CSV file')
        setSelectedFile(null)
        if (fileInputRef.current) fileInputRef.current.value = ''
      } else {
        setSelectedFile(file)
      }
    }
  }, [])

  const handleDataSourceChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    setDataSource(event.target.value as DataSource)
  }, [])

  const handleUpload = useCallback(() => {
    if (!selectedFile) {
      alert('Please select a CSV file')
    } else {
      const formData = new FormData()
      formData.append('file', selectedFile)

      dispatch(uploadCuratedList({ formData, dataSource }))
    }
  }, [selectedFile, dispatch, dataSource])

  const handleDelete = useCallback(
    (curatedList: CuratedListTable) => {
      dispatch(deleteCuratedList(curatedList))
    },
    [dispatch]
  )

  const filterSearchList = useMemo(() => {
    const searchLower = searchQuery?.toLowerCase()

    return manualList
      .filter(item => item.name.toLowerCase().includes(searchLower))
      .map(
        item =>
          ({
            ...item,
            onDelete: () => handleDelete(item),
          }) as ExtendedCuratedListTable
      )
  }, [manualList, searchQuery, handleDelete])

  const paginatedRows = useMemo(() => {
    return filterSearchList.slice((currentPage - 1) * pageSize, currentPage * pageSize)
  }, [filterSearchList, currentPage, pageSize])

  return (
    <>
      <Typography variant='body2' color='text.secondary' sx={{ mb: 3 }}>
        For Manual curated list, you have to upload a file which used to inserting list and edit.
      </Typography>

      <Grid container sx={{ mb: 3 }}>
        <Grid item xs={12} sx={{ mb: 2 }}>
          <Grid container spacing={2} alignItems='center'>
            <Grid item>
              <Button
                component='label'
                variant='contained'
                startIcon={<AddOutlined />}
                disabled={isLoading}
                size='medium'
                sx={{
                  bgcolor: 'grey.700',
                  '&:hover': { bgcolor: 'grey.800' },
                  whiteSpace: 'nowrap',
                  mr: 0,
                }}
              >
                Manual List
                <VisuallyHiddenInput type='file' accept='.csv' ref={fileInputRef} onChange={handleFileChange} />
              </Button>
            </Grid>
            <Grid item>
              <FormControl component='fieldset'>
                <RadioGroup
                  row
                  aria-label='data-source'
                  name='data-source'
                  value={dataSource}
                  onChange={handleDataSourceChange}
                >
                  <FormControlLabel value='SET' control={<Radio size='small' />} label='SET' />
                  <FormControlLabel value='GE' control={<Radio size='small' />} label='GE' />
                </RadioGroup>
              </FormControl>
            </Grid>
            <Grid item>
              <Button
                variant='contained'
                disabled={isLoading || !selectedFile}
                onClick={handleUpload}
                size='medium'
                sx={{
                  bgcolor: 'primary.main',
                  '&:hover': { bgcolor: 'primary.dark' },
                }}
              >
                Upload
              </Button>
            </Grid>
            <Grid item xs={12} sm={4} md={3} lg={3} sx={{ marginLeft: 'auto' }}>
              <SearchField
                value={searchQuery}
                onChange={handleSearch}
                placeholder='Search list name'
                disabled={isLoading}
              />
            </Grid>
          </Grid>
        </Grid>
      </Grid>

      <CuratedDataTable
        rows={paginatedRows}
        total={filterSearchList.length}
        store={store}
        onPaginate={onPaginate}
        isLoading={isLoading}
        isLogicalTab={false}
      />
    </>
  )
}
export default ManualTabContent
