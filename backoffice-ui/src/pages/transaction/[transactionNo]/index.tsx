import useFetchTransaction from '@/hooks/backoffice/useFetchTransaction'
import Layout from '@/layouts/index'
import { createTicket, getTicketPayload, verifyTicket } from '@/lib/api/clients/backoffice/transactions'
import {
  ICreateTicketRequest,
  IVerifyTicketRequest,
  UpdateBillPaymentReferencePayload,
} from '@/lib/api/clients/backoffice/transactions/types'
import TransactionsPage from '@/views/pages/transactions'
import { datadogLogs } from '@datadog/browser-logs'
import { Button, CircularProgress } from '@mui/material'
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import Grid from '@mui/material/Grid'
import TextField from '@mui/material/TextField'
import Typography from '@mui/material/Typography'
import { useRouter } from 'next/router'
import { useEffect, useState } from 'react'
import Swal from 'sweetalert2'
import withReactContent from 'sweetalert2-react-content'

const Transactions = () => {
  const [loading, setLoading] = useState(false)
  const router = useRouter()
  const { transactionNo } = router.query
  const { transaction, loading: transactionLoading, error, fetchTransaction } = useFetchTransaction()

  useEffect(() => {
    if (transactionNo) {
      fetchTransaction(transactionNo as string)
    }
  }, [transactionNo])

  const showConfirmAccountNoDialog = async (): Promise<string | null> => {
    return new Promise(resolve => {
      let inputValue = ''
      const MySwal = withReactContent(Swal)
      MySwal.fire({
        html: (
          <div>
            <Typography py={1} variant='h6' textAlign='left'>
              Please enter correct Account No
            </Typography>
            <Typography variant='h6' textAlign='left'>
              <TextField fullWidth label='Account No' onChange={e => (inputValue = e.target.value)} id='accountNo' />
            </Typography>
          </div>
        ),
        showCancelButton: true,
        cancelButtonText: (
          <Button style={{ fontWeight: 'bold', width: '200px' }} fullWidth>
            Cancel
          </Button>
        ),
        reverseButtons: true,
        buttonsStyling: true,
        cancelButtonColor: '#f1ecec',
        confirmButtonColor: '#3dd884',
        confirmButtonText: <Button style={{ fontWeight: 'bold', width: '200px', color: 'white' }}>Confirm</Button>,
        didOpen: () => {
          const input = document.getElementById('accountNo') as HTMLInputElement
          const confirmButton = document.querySelector('.swal2-confirm') as HTMLButtonElement
          confirmButton.disabled = true
          input.addEventListener('input', () => {
            confirmButton.disabled = !input.value.trim()
          })
        },
      }).then(result => resolve(result.isConfirmed ? inputValue : null))
    })
  }

  const handleStatusChange = async ({ remark, action }: { remark: string; action: string }) => {
    try {
      setLoading(true)
      if (!transaction) return

      if (transaction.makerType.length > transaction.checkerType.length) {
        await handleVerifyTicket(action, remark)
      } else {
        await handleCreateTicket(action, remark)
      }

      await fetchTransaction(transactionNo as string)
    } catch (e) {
      logAndDisplayError(e)
    } finally {
      setLoading(false)
    }
  }

  const handleVerifyTicket = async (action: string, remark: string) => {
    const MySwal = withReactContent(Swal)
    let verifyTicketPayload: IVerifyTicketRequest

    if (action === 'UpdateBillPaymentReference') {
      const res = await getTicketPayload(transaction?.pendingTicketNo as string)
      const payload = JSON.parse(res.payload as string) as UpdateBillPaymentReferencePayload

      const result = await MySwal.fire({
        html: (
          <div>
            <Typography variant='h6' textAlign='left'>
              <Typography mb={2} variant='h5' textAlign='left'>
                Please check Account No
              </Typography>
              <Typography variant='h6' fontWeight='bold'>
                Account No :
              </Typography>
              {payload.newReference}
            </Typography>
          </div>
        ),
        showCancelButton: true,
        cancelButtonText: (
          <Button style={{ fontWeight: 'bold', width: '200px' }} fullWidth>
            Cancel
          </Button>
        ),
        reverseButtons: true,
        buttonsStyling: true,
        cancelButtonColor: '#f1ecec',
        confirmButtonColor: '#3dd884',
        confirmButtonText: <Button style={{ fontWeight: 'bold', width: '200px', color: 'white' }}>Confirm</Button>,
      })

      if (!result.isConfirmed) return
      verifyTicketPayload = {
        method: action,
        remark,
        payload: JSON.stringify(payload),
      }
    } else {
      verifyTicketPayload = { method: action, remark }
    }

    datadogLogs.logger.info('verifyTicket', { action: 'verifyTicket', verifyTicketPayload, action_status: 'request' })
    const res = await verifyTicket(transaction?.pendingTicketNo as string, verifyTicketPayload)

    if (!res?.isSuccess) {
      const displayAlert = (await import('@/views/components/DisplayAlert')).default
      displayAlert((res?.errorMsg as string) ?? 'Error while verifying ticket, please try again')
    }
  }

  const handleCreateTicket = async (action: string, remark: string) => {
    let createTicketPayload: ICreateTicketRequest
    if (action === 'UpdateBillPaymentReference') {
      const additionalInputResult = await showConfirmAccountNoDialog()
      if (!additionalInputResult) return

      createTicketPayload = {
        transactionNo: transaction?.transactionDetail?.transactionNo as string,
        transactionType: transaction?.transactionDetail?.transactionType as string,
        method: action,
        remark,
        payload: JSON.stringify({
          oldReference: transaction?.billPaymentDeposit?.reference1 ?? '',
          newReference: additionalInputResult,
        }),
      }
    } else {
      createTicketPayload = {
        transactionNo: transaction?.transactionDetail?.transactionNo as string,
        transactionType: transaction?.transactionDetail?.transactionType as string,
        method: action,
        remark,
        payload: null,
      }
    }

    datadogLogs.logger.info('createTicket', { action: 'createTicket', createTicketPayload, action_status: 'request' })
    await createTicket(createTicketPayload)
  }

  const logAndDisplayError = async (e: unknown) => {
    datadogLogs.logger.error('createTicket/verifyTicket', { action: ['verifyTicket', 'createTicket'] }, e as Error)
    const displayAlert = (await import('@/views/components/DisplayAlert')).default
    displayAlert(e instanceof Error ? e.message : 'Unexpected error occurred')
  }

  if (error) {
    router.push({ pathname: '/error', query: { reason: error.message } })
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
      title={`${transaction?.productDetail?.customerAccountNumber} - ${transaction?.customerProfile?.customerAccountName}`}
    >
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardHeader title={`Transaction ID: ${transactionNo}`} />
            <TransactionsPage
              transaction={transaction}
              handleStatusChange={handleStatusChange}
              refreshTransactionDetails={() => fetchTransaction(transactionNo as string)}
            />
          </Card>
        </Grid>
      </Grid>
    </Layout>
  )
}

export default Transactions
