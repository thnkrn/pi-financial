import useFetchTransferCashTransaction from '@/hooks/backoffice/useFetchTransferCashTransaction'
import Layout from '@/layouts/index'
import { createTicket, verifyTicket } from '@/lib/api/clients/backoffice/transactions'
import { ICreateTicketRequest, IVerifyTicketRequest } from '@/lib/api/clients/backoffice/transactions/types'
import LocalDateTime from '@/views/components/LocalDateTime'
import TransactionsPage from '@/views/pages/transfer-cash-detail'
import { datadogLogs } from '@datadog/browser-logs'
import { CircularProgress } from '@mui/material'
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import Grid from '@mui/material/Grid'
import Typography from '@mui/material/Typography'
import { useRouter } from 'next/router'
import { useEffect, useState } from 'react'

const TransferCashTransactions = () => {
  const [loading, setLoading] = useState(false)

  const router = useRouter()
  const { transactionNo } = router.query

  const { transaction, loading: transactionLoading, error, fetchTransaction } = useFetchTransferCashTransaction()

  useEffect(() => {
    if (transactionNo) {
      const fetchData = async () => {
        await fetchTransaction(transactionNo as string)
      }

      fetchData()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [transactionNo])

  const handleStatusChange = async ({ remark, action }: { remark: string; action: string }) => {
    try {
      setLoading(true)
      if (transaction && transaction?.makerType?.length > transaction?.checkerType?.length) {
        const payload: IVerifyTicketRequest = {
          method: action,
          remark: remark,
        }

        datadogLogs.logger.info('verifyTicket', { action: 'verifyTicket', payload, action_status: 'request' })

        const res = await verifyTicket(transaction?.pendingTicketNo as string, payload)

        if (!res?.isSuccess) {
          const displayAlert = (await import('@/views/components/DisplayAlert')).default
          displayAlert((res?.errorMsg as string) ?? 'Error while verifying ticket, please try again')
        }
      } else {
        const payload: ICreateTicketRequest = {
          transactionNo: transaction?.transactionDetail?.transactionNo as string,
          transactionType: transaction?.transactionDetail?.transactionType as string,
          method: action,
          remark: remark,
          payload: null,
        }

        datadogLogs.logger.info('createTicket', { action: 'createTicket', payload, action_status: 'request' })

        await createTicket(payload)
      }

      await fetchTransaction(transactionNo as string)
    } catch (e) {
      datadogLogs.logger.error('createTicket/verifyTicket', { action: ['verifyTicket', 'createTicket'] }, e as Error)

      const displayAlert = (await import('@/views/components/DisplayAlert')).default
      if (typeof e === 'string') {
        displayAlert(e)
      } else if (e instanceof Error) {
        displayAlert(e?.message)
      } else {
        displayAlert('Unexpected error from generate report')
      }
    } finally {
      setLoading(false)
    }
  }

  if (error) {
    router.push({ pathname: '/error', query: { reason: error?.message } })
  }

  if (transactionLoading || loading || !transaction) {
    return (
      <Layout>
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
          <CircularProgress />
        </div>
      </Layout>
    )
  }

  return (
    <Layout
      title={`${transaction?.accountInfo?.transferFromAccountCode} -> ${transaction?.accountInfo?.transferToAccountCode}`}
    >
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader
              title={
                <div style={{ display: 'flex', alignItems: 'flex-end' }}>
                  <Typography variant='h5' style={{ flex: '1' }}>
                    Transaction ID: {transactionNo}
                  </Typography>
                  <Typography variant='body1' style={{ flex: '1', textAlign: 'right' }}>
                    {'Transaction Created: '}
                    <LocalDateTime date={transaction?.transactionDetail?.createdAt} />
                  </Typography>
                </div>
              }
            />
            <TransactionsPage
              transaction={transaction}
              handleStatusChange={handleStatusChange}
              refreshTransactionDetails={async () => await fetchTransaction(transactionNo as string)}
            />
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default TransferCashTransactions
