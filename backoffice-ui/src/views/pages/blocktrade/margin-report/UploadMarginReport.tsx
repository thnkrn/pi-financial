// ** React Imports
import { ChangeEvent, Dispatch, SetStateAction, useState } from 'react'

// ** MUI Imports
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth'
import DeleteForeverIcon from '@mui/icons-material/DeleteForever'
import UploadIcon from '@mui/icons-material/Upload'
import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import FormLabel from '@mui/material/FormLabel'
import Paper from '@mui/material/Paper'
import Typography from '@mui/material/Typography'
import CircularProgress from '@mui/material/CircularProgress'

// ** Other Imports
import dayjs from 'dayjs'
import Datepicker from '@/views/components/blocktrade/DatePicker'
import { importMargin } from '@/lib/api/clients/blocktrade/margin'
import DisplayAlert from '@/views/components/blocktrade/DisplayAlert'

const XLSX_MIME_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'

const UploadMarginReport = ({ setIsUploaded }: { setIsUploaded: Dispatch<SetStateAction<boolean>> }) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
  const [effectiveDate, setEffectiveDate] = useState<Date>(new Date())
  const [value, setValue] = useState<string>('')
  const [isLoading, setIsLoading] = useState<boolean>(false)

  const handleSelectFile = (e: ChangeEvent<HTMLInputElement>) => {
    setIsUploaded(false)
    const fileObj = e.target.files?.[0] || null
    if (fileObj?.type !== XLSX_MIME_TYPE) {
      DisplayAlert('Incorrect file format, please upload xlsx file')

      return
    }
    setValue(e.target.value)
    setSelectedFile(fileObj)
  }

  const handleSelectDate = (date: Date) => {
    setEffectiveDate(date)
  }

  const handleClearFile = () => {
    setSelectedFile(null)
    setValue('')
  }

  const handleUploadFile = async () => {
    setIsLoading(true)
    if (!selectedFile) return
    const reader = new FileReader()
    reader.readAsDataURL(selectedFile)
    reader.onload = async () => {
      const base64String = reader.result as string
      const fileBase64 = base64String.split(',')[1]

      const payload = {
        fileBase64,
        effectiveDate: dayjs(effectiveDate).format('YYYY-MM-DD'),
      }

      try {
        const response = await importMargin(payload)
        if (response) {
          DisplayAlert('Upload successful', 'success')
          setSelectedFile(null)
          setValue('')
          setIsLoading(false)
          setIsUploaded(true)
        }
      } catch (error: any) {
        DisplayAlert(error.message)

        return
      }
    }

    reader.onerror = error => {
      DisplayAlert(`Error reading file: ${error}`)
      setIsLoading(false)
    }
  }

  return (
    <Paper sx={{ padding: 10 }}>
      <Typography variant='h5' display='block'>
        UPLOAD MARGIN SHEET
      </Typography>
      <Typography variant='caption' display='block' gutterBottom>
        Supported file type is .xlsx
      </Typography>
      <Button variant='contained' component='label' sx={{ mt: 5 }}>
        add file <input type='file' hidden onChange={handleSelectFile} value={value} />
      </Button>
      <Paper
        elevation={7}
        sx={{
          mt: 5,
          width: '100%',
          padding: 3,
          display: 'flex',
          flexWrap: 'wrap',
          justifyContent: 'space-between',
          alignItems: 'center',
          paddingLeft: 6,
        }}
      >
        <Typography display='block'>
          <b>File name:</b> {selectedFile?.name ?? '-'}
        </Typography>
        <div>
          <Button color='error' onClick={handleClearFile} size='small'>
            <DeleteForeverIcon />
          </Button>
        </div>
      </Paper>
      <Box
        sx={{
          margin: 0,
          mt: 5,
          padding: 0,
          display: 'flex',
          flexWrap: 'wrap',
          justifyContent: 'space-between',
          alignItems: 'end',
        }}
      >
        <div>
          <FormLabel>
            <CalendarMonthIcon /> Effective Date
          </FormLabel>
          <Datepicker defaultDate={effectiveDate} onChange={handleSelectDate} />
        </div>
        <Button
          onClick={handleUploadFile}
          size='large'
          variant='contained'
          disabled={!selectedFile || isLoading}
          sx={{ mt: { xs: 3 } }}
        >
          <UploadIcon /> Upload {isLoading && <CircularProgress size={20} thickness={8} />}
        </Button>
      </Box>
    </Paper>
  )
}

export default UploadMarginReport
