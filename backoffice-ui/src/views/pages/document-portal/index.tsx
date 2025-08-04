import { getOnboardingAccounts } from '@/lib/api/clients/backoffice/document-portal'
import Grid from '@mui/material/Grid'
import { isAxiosError } from 'axios'
import { useState } from 'react'
import CustomerDocumentDataTable from './CustomerDocumentDataTable'
import CustomerFilter from './CustomerFilter'
import { DocumentPortalRowType } from './types'

const Index = () => {
  const [filteredData, setFilteredData] = useState<DocumentPortalRowType[]>([])
  const [error, setError] = useState<string>('')
  const [isLoading, setIsLoading] = useState<boolean>(false)

  const getDataByFilterValue = async (custcode: string) => {
    try {
      setIsLoading(true)
      const result = await getOnboardingAccounts(custcode)

      if (typeof result === 'object' && result.accounts) {
        const filteredAccountsArr = result.accounts
        setFilteredData(filteredAccountsArr)
        setError('')
      } else {
        setFilteredData([])
        setError('No data found')
      }
    } catch (error) {
      if (isAxiosError(error)) {
        setError(error?.response?.data?.detail ?? 'An error occured while fetching data')
      } else {
        setError('An error occured while fetching data')
      }
    } finally {
      setIsLoading(false)
    }
  }

  const handleFilter = async (custcode: string) => {
    if (custcode) {
      getDataByFilterValue(custcode)
    }
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <CustomerFilter onChange={() => setError('')} onFilter={handleFilter} error={error} />
        <CustomerDocumentDataTable rows={filteredData} loading={isLoading} />
      </Grid>
    </Grid>
  )
}

export default Index
