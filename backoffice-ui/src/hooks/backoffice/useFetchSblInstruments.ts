import { getSblInstruments } from '@/lib/api/clients/backoffice/sbl'
import { IGetInstrumentsRequest, IGetInstrumentsResponse } from '@/lib/api/clients/backoffice/sbl/types'
import axios from 'axios'
import { useState } from 'react'

const useFetchSblInstruments = () => {
  const [instrumentResponse, setInstrumentResponse] = useState<IGetInstrumentsResponse>()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<Error>()

  const fetchSblInstruments = async (filter: IGetInstrumentsRequest) => {
    setLoading(true)
    try {
      const response = await getSblInstruments(filter)
      setInstrumentResponse(response)
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

  return { instrumentResponse, loading, error, fetchSblInstruments }
}

export default useFetchSblInstruments
