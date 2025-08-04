import { getSblOrders } from '@/lib/api/clients/backoffice/sbl'
import { IGetOrdersRequest, IGetOrdersResponse } from '@/lib/api/clients/backoffice/sbl/types'
import axios from 'axios'
import { useState } from 'react'

const useFetchSblOrders = () => {
  const [orderResponse, setOrderResponse] = useState<IGetOrdersResponse>()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<Error>()

  const fetchSblOrders = async (filter: IGetOrdersRequest) => {
    setLoading(true)
    try {
      const response = await getSblOrders(filter)
      setOrderResponse(response)
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

  return { orderResponse, loading, error, fetchSblOrders }
}

export default useFetchSblOrders
