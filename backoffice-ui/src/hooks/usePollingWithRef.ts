import { useEffect, useRef, useState } from 'react'

type UsePollingOptions<T> = {
  fetcher: () => Promise<T>
  interval: number
  autoStart?: boolean
}

type UsePollingResult = {
  start: () => void
  stop: () => void
}

// NOTE: usage
// const filterRef = useRef(filter)
//   filterRef.current = filter
// const { start, stop } = UsePollingResult<IGetOrdersResponse>({
//   fetcher: async () => {
//     await fetchSblOrders(filterRef.current)

//     return orderResponse!
//   },
//   interval: 10000,
//   autoStart: false,
// })

function usePollingWithRef<T>({ fetcher, interval, autoStart = true }: UsePollingOptions<T>): UsePollingResult {
  const [isPolling, setIsPolling] = useState<boolean>(autoStart)
  const intervalIdRef = useRef<NodeJS.Timeout | null>(null)

  const fetchData = async () => {
    if (!isPolling) return
    await fetcher()
  }

  useEffect(() => {
    if (!isPolling) return

    fetchData()
    intervalIdRef.current = setInterval(fetchData, interval)

    return () => {
      if (intervalIdRef.current) clearInterval(intervalIdRef.current)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isPolling, interval])

  const start = () => setIsPolling(true)
  const stop = () => {
    setIsPolling(false)
    if (intervalIdRef.current) clearInterval(intervalIdRef.current)
  }

  return { start, stop }
}

export default usePollingWithRef
