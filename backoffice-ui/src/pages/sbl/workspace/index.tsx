import NotificationBadge from '@/@core/components/notification-badge'
import useFetchSblOrders from '@/hooks/backoffice/useFetchSblOrders'
import usePolling from '@/hooks/usePollingWithRef'
import Layout from '@/layouts/index'
import { IGetOrdersResponse, SBL_ORDER_STATUS } from '@/lib/api/clients/backoffice/sbl/types'
import SBLDashboardPage from '@/views/pages/sbl/workspace'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'
import Grid from '@mui/material/Grid'

import { useEffect, useState } from 'react'

const INITIAL_FILTER = {
  page: 1,
  pageSize: 1000,
  orderBy: 'createdAt',
  orderDir: 'desc',
  statues: SBL_ORDER_STATUS.pending,
  tradingAccountNo: '',
}

const SBLWorkspace = () => {
  const [amount, setAmount] = useState(0)
  const [isFirstRender, setIsFirstRender] = useState<boolean>(true)
  const [fetchNotification, setFetchNotification] = useState<boolean>(false)

  const { orderResponse, fetchSblOrders } = useFetchSblOrders()

  const { start } = usePolling<IGetOrdersResponse>({
    fetcher: async () => {
      await fetchSblOrders(INITIAL_FILTER)

      return orderResponse!
    },
    interval: 10000,
    autoStart: false,
  })

  useEffect(() => {
    const fetchData = async () => {
      await fetchSblOrders(INITIAL_FILTER)
    }

    if (isFirstRender || fetchNotification) {
      fetchData()
    } else {
      start()
    }

    setIsFirstRender(false)
    setFetchNotification(false)

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isFirstRender, fetchNotification])

  useEffect(() => {
    if (orderResponse) {
      setAmount(orderResponse?.total)
    } else {
      setAmount(0)
    }
  }, [orderResponse])

  return (
    <Layout title='SBL Workspace'>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
              <CardHeader title='SBL Workspace' />
              <NotificationBadge
                style={{ padding: '1.25rem', height: '100%' }}
                amount={amount}
                message={
                  <>
                    You have <span style={{ color: '#21CE99' }}>{amount}</span> pending orders
                  </>
                }
              />
            </div>
            <CardContent>
              <SBLDashboardPage setFetchNotification={setFetchNotification} />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default SBLWorkspace
