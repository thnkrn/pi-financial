import { ACTION_AFFINITY, RESPONSE_ACTIONS } from '@/constants/ResponseActions'
import { IAction } from '@/lib/api/clients/backoffice/transactions/types'
import ConfirmDialogText from '@/views/components/transaction/ConfirmDialogText'
import TransactionStatusForm, { FormValues } from '@/views/components/transaction/TransactionStatusForm'
import { Button, Card, CardContent, CardHeader, Divider, Grid } from '@mui/material'
import Swal from 'sweetalert2'
import withReactContent from 'sweetalert2-react-content'

interface Props {
  onStatusChange: ({ remark, action }: { remark: string; action: string }) => void
  isLoading: boolean
  refreshTransactionDetails: any
  actions: IAction[]
}

export const AddTransactionStatus = ({ onStatusChange, isLoading, refreshTransactionDetails, actions }: Props) => {
  const initialValues = {
    remarks: '',
    action: '',
  }

  const handleSubmit = (values: FormValues) => {
    const MySwal = withReactContent(Swal)
    MySwal.fire({
      html: <ConfirmDialogText remark={values.remarks} action={values.action} />,
      showCancelButton: true,
      cancelButtonText: (
        <Button style={{ fontWeight: 'bold', width: '200px' }} fullWidth disabled={isLoading}>
          Cancel
        </Button>
      ),
      reverseButtons: true,
      buttonsStyling: true,
      cancelButtonColor: '#f1ecec',
      confirmButtonColor:
        RESPONSE_ACTIONS.find(v => v.name === values.action)?.affinity === ACTION_AFFINITY.POSITIVE
          ? '#3dd884'
          : '#ff6f6f',
      confirmButtonText: (
        <Button style={{ fontWeight: 'bold', width: '200px', color: 'white' }} disabled={isLoading}>
          Confirm
        </Button>
      ),
    }).then(async result => {
      if (result.isConfirmed) {
        onStatusChange({
          remark: values.remarks,
          action: values.action,
        })
      }
      await refreshTransactionDetails()
    })
  }

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Divider sx={{ m: '0 !important' }} />
        <Card sx={{ marginTop: '40px' }}>
          <CardHeader title='Post Action'></CardHeader>
          <CardContent>
            <TransactionStatusForm
              initialValues={initialValues}
              onSubmit={handleSubmit}
              isSaving={false}
              actions={actions}
            />
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  )
}

export default AddTransactionStatus
