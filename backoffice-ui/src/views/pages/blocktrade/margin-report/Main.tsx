// ** React Imports
import { useState } from 'react'

// ** MUI Imports
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'

// ** Other Imports
import DownloadMarginReport from '@/views/pages/blocktrade/margin-report/DownloadMarginReport'
import UploadMarginReport from '@/views/pages/blocktrade/margin-report/UploadMarginReport'

const BlocktradeMarginReport = () => {
  const [isUploaded, setIsUploaded] = useState<boolean>(false)

  return (
    <>
      <CardHeader title='Margin Report' />
      <CardContent>
        <UploadMarginReport setIsUploaded={setIsUploaded} />
        <DownloadMarginReport isUploaded={isUploaded} />
      </CardContent>
    </>
  )
}

export default BlocktradeMarginReport
