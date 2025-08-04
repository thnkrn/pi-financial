// ** React Imports
import { ChangeEvent, Dispatch, SetStateAction, useEffect, useMemo, useState } from 'react'

// ** MUI Imports
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth'
import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import Checkbox from '@mui/material/Checkbox'
import FormControlLabel from '@mui/material/FormControlLabel'
import FormGroup from '@mui/material/FormGroup'
import Paper from '@mui/material/Paper'
import Typography from '@mui/material/Typography'
import CircularProgress from '@mui/material/CircularProgress'

// ** Other Imports
import Dropdown from './Dropdown'
import { IChoice } from './types'
import { getMarginDate, getMarginPDF } from '@/lib/api/clients/blocktrade/margin'
import { IGetMarginDateResponse } from '@/lib/api/clients/blocktrade/margin/types'
import DisplayAlert from '@/views/components/blocktrade/DisplayAlert'
import base64ToBlobWithMime from '@/utils/blocktrade/base64ToBlob'

const InstituteCheckBox = ({
  isInstitute,
  setIsInstitue,
}: {
  isInstitute: boolean
  setIsInstitue: Dispatch<SetStateAction<boolean>>
}) => {
  return (
    <FormGroup>
      <FormControlLabel
        control={
          <Checkbox
            checked={isInstitute}
            onChange={(event: ChangeEvent<HTMLInputElement>, checked: boolean) => setIsInstitue(checked)}
          />
        }
        label='Institute'
      />
    </FormGroup>
  )
}

const DownloadMarginReport = ({ isUploaded }: { isUploaded: boolean }) => {
  const [dateChoices, setDateChoices] = useState<IChoice[]>([])
  const [selectedDate, setSelectedDate] = useState<string>('')
  const [compareDate, setCompareDate] = useState<string>('')
  const [isLoading, setIsLoading] = useState<boolean>(false)
  const [isInstitute, setIsInstitute] = useState<boolean>(false)

  const fetchDate = async () => {
    const result: IChoice[] = []
    try {
      const res = await getMarginDate()
      const data: IGetMarginDateResponse = Object.values(res)
      data.forEach((date: string) => {
        result.push({ name: date })
      })
    } catch (error: any) {
      DisplayAlert(error.message)
      setDateChoices([])
    }
    setDateChoices(result)
  }

  const handleDownloadReportAtEffDate = async () => {
    setIsLoading(true)
    try {
      const params = {
        effectiveDate: selectedDate,
        institute: isInstitute,
        ...(compareDate && { effectiveDateFrom: compareDate }),
      }
      const res = await getMarginPDF(params)
      const pdfBlob = base64ToBlobWithMime(res.pdfBase64, 'application/pdf')
      const url = window.URL.createObjectURL(pdfBlob)
      const link = document.createElement('a')
      link.href = url
      link.setAttribute('download', res.filename)
      document.body.appendChild(link)
      link.click()
      link.parentNode?.removeChild(link)
      DisplayAlert('Downloaded Successfully', 'success')
    } catch (error: any) {
      DisplayAlert(error.message)
    }
    setIsLoading(false)
  }

  const handleReset = () => {
    setSelectedDate('')
    setCompareDate('')
  }

  useEffect(() => {
    fetchDate()
  }, [])

  useEffect(() => {
    if (isUploaded) fetchDate()
  }, [isUploaded])

  const isCompareDateCorrect = useMemo(() => {
    if (!selectedDate || !compareDate) return true
    const isCondition = Date.parse(selectedDate) > Date.parse(compareDate)
    if (!isCondition) {
      DisplayAlert('Compare Date must be before Effective Date')
    }

    return isCondition
  }, [selectedDate, compareDate])

  const isDisabledButton = isLoading || !selectedDate || !isCompareDateCorrect

  return (
    <div>
      <Paper sx={{ padding: 10, mt: 5 }}>
        <Typography variant='h5' display='block'>
          DATA LIST
        </Typography>
        <Box sx={{ mt: 8 }}>
          <InstituteCheckBox isInstitute={isInstitute} setIsInstitue={setIsInstitute} />
          <Dropdown
            choices={dateChoices}
            value={selectedDate}
            setValue={setSelectedDate}
            label='Effective Date'
            IconComponent={CalendarMonthIcon}
          />
          <Dropdown
            choices={dateChoices}
            value={compareDate}
            setValue={setCompareDate}
            label='Compare Date (Optional)'
            IconComponent={CalendarMonthIcon}
            sx={{ mt: 5 }}
          />
          <Button
            variant='contained'
            onClick={handleDownloadReportAtEffDate}
            disabled={isDisabledButton}
            sx={{ mt: 5 }}
          >
            Download {isLoading && <CircularProgress size={20} thickness={8} />}
          </Button>
          <Button variant='contained' onClick={handleReset} sx={{ mt: 5, ml: 3 }} color='error'>
            Reset
          </Button>
        </Box>
      </Paper>
    </div>
  )
}

export default DownloadMarginReport
