import { getTransferCashTransaction } from '@/lib/api/clients/backoffice/transactions'
import { IGetTransferCashTransactionResponse } from '@/lib/api/clients/backoffice/transactions/types'
import axios from 'axios'
import { useState } from 'react'

const useFetchTransferCashTransaction = () => {
  const [transaction, setTransaction] = useState<IGetTransferCashTransactionResponse>()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<Error>()

  const fetchTransaction = async (transactionNo: string) => {
    setLoading(true)
    try {
      const response = await getTransferCashTransaction(transactionNo)
      setTransaction(response)
    } catch (error) {
      if (axios.isAxiosError(error)) {
        setError(new Error(error.response?.statusText ?? error.response?.data?.title))
      } else {
        setError(error as Error)
      }
    } finally {
      setLoading(false)
    }
  }

  return { transaction, loading, error, fetchTransaction }
}

export default useFetchTransferCashTransaction
