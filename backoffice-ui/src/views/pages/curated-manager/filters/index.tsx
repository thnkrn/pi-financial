import {
  fetchFilterGroups,
  resetState,
  setActiveTab,
  uploadCuratedFiltersThunk,
} from '@/store/apps/curated-manager/filter'
import { AppDispatch, RootState } from '@/store/index'
import { DataSource } from '@/types/curated-manager'
import { CuratedFilterItem, FilterTabType, TAB_CONFIG } from '@/types/curated-manager/filter'
import ErrorModal from '@/views/components/ErrorModal'
import { AddOutlined } from '@mui/icons-material'
import { Button, FormControl, FormControlLabel, Grid, Radio, RadioGroup, Tabs, Typography } from '@mui/material'
import isEmpty from 'lodash/isEmpty'
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import SearchField from '../SearchField'
import { StyledTab } from '../StyledTab'
import SuccessAlert from '../SuccessAlert'
import { VisuallyHiddenInput } from '../VisuallyHiddenInput'
import CuratedFilterTable from './CuratedFilterTable'

const MemoizedCuratedFilterTable = memo(CuratedFilterTable)

const Index = () => {
  const [searchQuery, setSearchQuery] = useState<string>('')
  const [pagination, setPagination] = useState<Record<string, { page: number; pageSize: number }>>({})
  const defaultPageSize = 20
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
  const [dataSource, setDataSource] = useState<DataSource>(DataSource.SET)
  const [openUploadAlert, setOpenUploadAlert] = useState(false)
  const dispatch = useDispatch<AppDispatch>()
  const store = useSelector((state: RootState) => state.curatedFilter)
  const filterGroups = useSelector((state: RootState) => state.curatedFilter.filterGroups)
  const isLoading = useSelector((state: RootState) => state.curatedFilter.isLoading)
  const activeTab = useSelector((state: RootState) => state.curatedFilter.activeTab)
  const errorMessage = useSelector((state: RootState) => state.curatedFilter.errorMessage)
  const isUploadSuccess = useSelector((state: RootState) => state.curatedFilter.isUploadSuccess)
  const fileInputRef = useRef<HTMLInputElement>(null)

  const getGroupNameFromTab = useCallback((tab: string) => {
    return TAB_CONFIG[tab as FilterTabType]
  }, [])

  useEffect(() => {
    const groupName = getGroupNameFromTab(activeTab)
    dispatch(fetchFilterGroups(groupName))
  }, [activeTab, dispatch, getGroupNameFromTab])

  useEffect(() => {
    return () => {
      dispatch(resetState())
    }
  }, [dispatch])

  useEffect(() => {
    if (isUploadSuccess) {
      setOpenUploadAlert(true)
    }
  }, [isUploadSuccess])

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

      const groupName = getGroupNameFromTab(activeTab)

      dispatch(uploadCuratedFiltersThunk({ formData, dataSource, groupName }))
    }
  }, [selectedFile, getGroupNameFromTab, activeTab, dispatch, dataSource])

  const handleTabChange = useCallback(
    (event: React.SyntheticEvent, newValue: FilterTabType) => {
      dispatch(setActiveTab(newValue))
    },
    [dispatch]
  )

  const handleSearch = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value)
  }, [])

  const handlePaginate = useCallback((groupName: string, newPage: number, newPageSize: number) => {
    setPagination(prevPagination => ({
      ...prevPagination,
      [groupName]: { page: newPage, pageSize: newPageSize },
    }))
  }, [])

  const getPaginatedRows = useCallback(
    (groupName: string, data: CuratedFilterItem[]) => {
      const currentPagination = pagination[groupName] || { page: 1, pageSize: defaultPageSize }
      const { page, pageSize } = currentPagination

      return data.slice((page - 1) * pageSize, page * pageSize)
    },
    [pagination, defaultPageSize]
  )

  const handleUploadAlertClose = useCallback(() => {
    setOpenUploadAlert(false)
  }, [])

  const filteredGroups = useMemo(() => {
    if (!searchQuery.trim()) return filterGroups

    const query = searchQuery.toLowerCase()

    return filterGroups
      .map(group => ({
        ...group,
        data: group.data.filter(item => item.filterName.toLowerCase().includes(query)),
      }))
      .filter(group => group.data.length > 0)
  }, [filterGroups, searchQuery])

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Typography variant='body2' color='text.secondary' sx={{ mb: 3 }}>
          The curated filter list have to upload file which is contained data table.
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
                  Filter List
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
                  placeholder='Search filter name'
                  disabled={isLoading}
                />
              </Grid>
            </Grid>
          </Grid>
        </Grid>

        <Tabs
          value={store.activeTab}
          onChange={handleTabChange}
          sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}
        >
          {Object.entries(TAB_CONFIG).map(([value, label]) => (
            <StyledTab key={value} label={label} value={value} />
          ))}
        </Tabs>

        {filteredGroups.length > 0 &&
          filteredGroups.map((filterGroup, index) => {
            const paginationData = pagination[filterGroup.name] || { page: 1, pageSize: defaultPageSize }
            const paginatedRows = getPaginatedRows(filterGroup.name, filterGroup.data)

            return (
              <MemoizedCuratedFilterTable
                key={`${filterGroup.name}-${index}`}
                name={filterGroup.name}
                rows={paginatedRows}
                total={filterGroup.data.length}
                store={store}
                isLoading={isLoading}
                onPaginate={currentFilter =>
                  handlePaginate(filterGroup.name, currentFilter.page, currentFilter.pageSize)
                }
                page={paginationData.page}
                pageSize={paginationData.pageSize}
              />
            )
          })}

        {filteredGroups.length === 0 && !isLoading && (
          <Typography variant='body1' sx={{ mt: 4, textAlign: 'center' }}>
            No filter groups available for this category. Please select another tab or upload filter data.
          </Typography>
        )}
      </Grid>

      <ErrorModal isError={!isEmpty(errorMessage)} errorMessage={errorMessage} dependencies={[errorMessage]} />

      <SuccessAlert
        open={openUploadAlert}
        onClose={handleUploadAlertClose}
        message='Curated list uploaded successfully'
      />
    </Grid>
  )
}

export default Index
