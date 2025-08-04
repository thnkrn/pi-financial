import useFetchSblInstruments from '@/hooks/backoffice/useFetchSblInstruments'
import { uploadSblFile } from '@/lib/api/clients/backoffice/sbl'
import { IPaginationModel } from '@/types/sbl/sblTypes'
import Grid from '@mui/material/Grid'
import { PiBackofficeServiceApplicationModelsSblSblInstrument } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceApplicationModelsSblSblInstrument'
import { isAxiosError } from 'axios'
import extend from 'lodash/extend'
import { useEffect, useRef, useState } from 'react'
import { SubmitHandler } from 'react-hook-form'
import DashboardAction from './DashboardAction'
import DashboardTable from './DashboardDataTable'
import DashboardDialog from './DashboardDialog'

export interface IFormInput {
  symbol: string
}

const INITIAL_FILTER = {
  page: 1,
  pageSize: 10,
  orderBy: 'createdAt',
  orderDir: 'desc',
}

const Index = () => {
  const [total, setTotal] = useState<number>(0)
  const [rows, setRows] = useState<PiBackofficeServiceApplicationModelsSblSblInstrument[]>([])
  const [currentDateTime, setCurrentDateTime] = useState(new Date())
  const [paginationModel, setPaginationModel] = useState<IPaginationModel>({ page: 0, pageSize: 10 })
  const [filter, setFilter] = useState<any>(INITIAL_FILTER)
  const [search, setSearch] = useState<string>('')
  const [openDialog, setOpenDialog] = useState(false)
  const [file, setFile] = useState<File | null>(null)
  const [error, setError] = useState<string>('')
  const [isUploading, setIsUploading] = useState(false)
  const [isFirstRender, setIsFirstRender] = useState<boolean>(true)

  const fileInputRef = useRef<HTMLInputElement | null>(null)

  const { instrumentResponse, loading, error: fetchInstrumentError, fetchSblInstruments } = useFetchSblInstruments()

  useEffect(() => {
    const fetchData = async () => {
      await fetchSblInstruments(filter)
    }

    if (isFirstRender) {
      fetchData()
      setCurrentDateTime(new Date())
    }

    setIsFirstRender(false)

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isFirstRender])

  useEffect(() => {
    const updatedFilter = extend(
      {},
      { ...filter },
      {
        page: paginationModel.page + 1,
        pageSize: paginationModel.pageSize,
      }
    )
    setFilter(updatedFilter)

    const fetchData = async () => {
      await fetchSblInstruments(updatedFilter)
    }

    if (!isFirstRender) {
      fetchData()
      setCurrentDateTime(new Date())
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paginationModel])

  useEffect(() => {
    if (instrumentResponse?.instruments) {
      setRows(instrumentResponse.instruments)
      setTotal(instrumentResponse.total)
    }
  }, [instrumentResponse])

  const onRefresh = async () => {
    await fetchSblInstruments(filter)
    setCurrentDateTime(new Date())
  }

  const onSearchChange = (searchValue: { symbol: string }) => {
    setSearch(searchValue.symbol)
  }

  const onSubmit: SubmitHandler<IFormInput> = async () => {
    const updatedFilter = extend(
      {},
      { ...filter },
      {
        symbol: search,
      }
    )
    setFilter(updatedFilter)

    await fetchSblInstruments(updatedFilter)
    setCurrentDateTime(new Date())
  }

  const handleOpenDialog = () => {
    setOpenDialog(true)
  }

  const handleUploadClick = () => {
    fileInputRef.current?.click()
  }

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = event.target.files?.[0]

    if (selectedFile) {
      if (selectedFile.type !== 'text/csv') {
        setError('Only .csv files are allowed')
        setFile(null)

        return
      }

      if (selectedFile.size > 10 * 1024 * 1024) {
        setError('File size should not exceed 10MB')
        setFile(null)

        return
      }

      setFile(selectedFile)
      setError('')
    }
  }

  const handleDiscardFile = () => {
    setFile(null)
    setError('')
    if (fileInputRef.current) {
      fileInputRef.current.value = ''
    }
  }

  const handleUploadFile = async (file: File | null) => {
    setIsUploading(true)

    try {
      if (!file) {
        throw new Error('File not found')
      }

      await uploadSblFile(file)
    } catch (error) {
      const displayAlert = (await import('@/views/components/DisplayAlert')).default

      if (isAxiosError(error)) {
        displayAlert(error?.response?.data?.detail ?? 'Unexpected error from upload SBL file')
      } else {
        displayAlert('Unexpected error from upload SBL file')
      }
    } finally {
      setIsUploading(false)
      setOpenDialog(false)
      handleDiscardFile()
      await fetchSblInstruments(filter)
      setCurrentDateTime(new Date())
    }
  }

  if (fetchInstrumentError) {
    ;(async () => {
      const displayAlert = (await import('@/views/components/DisplayAlert')).default
      displayAlert(fetchInstrumentError.message)
    })()
  }

  return (
    <>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <DashboardAction onSubmit={onSubmit} onSearchChange={onSearchChange} />
          <DashboardTable
            rows={rows}
            total={total}
            paginationModel={paginationModel}
            setPaginationModel={setPaginationModel}
            isLoading={loading}
            onRefresh={onRefresh}
            currentDateTime={currentDateTime}
            handleOpenDialog={handleOpenDialog}
          />
        </Grid>
      </Grid>
      <DashboardDialog
        open={openDialog}
        fileInputRef={fileInputRef}
        file={file}
        error={error}
        isUploading={isUploading}
        handleClose={() => {
          setOpenDialog(false)
          handleDiscardFile()
        }}
        handleUploadClick={handleUploadClick}
        handleFileChange={handleFileChange}
        handleDiscardFile={handleDiscardFile}
        handleUploadFile={handleUploadFile}
      />
    </>
  )
}

export default Index
