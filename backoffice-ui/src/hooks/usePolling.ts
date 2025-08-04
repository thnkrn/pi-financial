import { useEffect, useRef, useState } from 'react'

type UsePollingOptions<T> = {
  fetcher: () => Promise<T>
  interval: number
  autoStart?: boolean
}

type UsePollingResult<T> = {
  data: T | null
  error: string | null
  isLoading: boolean
  start: () => void
  stop: () => void
}

// NOTE: usage
// const { start, stop } = usePolling<IGetOrdersResponse>({
//   fetcher: async () => {
//     await fetchSblOrders(filter)
//     return orderResponse!
//   },
//   interval: 5000,
//   autoStart: true,
// })

function usePolling<T>({ fetcher, interval, autoStart = true }: UsePollingOptions<T>): UsePollingResult<T> {
  const [data, setData] = useState<T | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState<boolean>(false)
  const [isPolling, setIsPolling] = useState<boolean>(autoStart)

  const abortControllerRef = useRef<AbortController | null>(null)
  const intervalIdRef = useRef<NodeJS.Timeout | null>(null)

  const fetchData = async () => {
    if (!isPolling) return

    setIsLoading(true)
    setError(null)

    if (abortControllerRef.current) {
      abortControllerRef.current.abort()
    }

    abortControllerRef.current = new AbortController()

    try {
      const response = await fetcher()
      setData(response)
    } catch (err: any) {
      if (err.name !== 'AbortError') {
        setError(err.message || 'An error occurred')
      }
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    if (!isPolling) return

    fetchData()

    intervalIdRef.current = setInterval(() => {
      fetchData()
    }, interval)

    return () => {
      if (intervalIdRef.current) clearInterval(intervalIdRef.current)
      if (abortControllerRef.current) abortControllerRef.current.abort()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isPolling, interval])

  const start = () => setIsPolling(true)
  const stop = () => setIsPolling(false)

  return { data, error, isLoading, start, stop }
}

export default usePolling
