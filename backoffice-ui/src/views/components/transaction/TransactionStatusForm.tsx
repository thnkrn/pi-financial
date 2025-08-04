import InputDropDown from '@/layouts/components/InputDropDown'
import InputText from '@/layouts/components/InputText'
import { IAction } from '@/lib/api/clients/backoffice/transactions/types'
import { zodResolver } from '@hookform/resolvers/zod'
import { Button, Grid } from '@mui/material'
import { useForm } from 'react-hook-form'
import * as z from 'zod'

const FormValuesSchema = z.object({
  remarks: z.string().min(1).max(2000),
  action: z.string(),
})

export type FormValues = z.infer<typeof FormValuesSchema>

interface Props {
  initialValues: FormValues
  onSubmit: (values: FormValues) => void
  isSaving?: boolean
  actions: IAction[]
}

export const TransactionStatusForm = ({ initialValues, onSubmit, isSaving = false, actions }: Props) => {
  const { control, handleSubmit } = useForm({
    mode: 'onChange',
    resolver: zodResolver(FormValuesSchema),
    defaultValues: initialValues,
  })

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Grid container spacing={2}>
        <Grid item xs={9}>
          <InputText name='remarks' placeholder='Remarks' control={control} required editable />
        </Grid>
        <Grid item xs={3}>
          <Grid container direction='column' justifyItems='center'>
            <Grid item xs>
              <InputDropDown name='action' label='Select Action' control={control} items={actions} required={true} />
            </Grid>
            <Grid item xs>
              <Button
                type='submit'
                color='primary'
                fullWidth
                sx={{ mt: 2, height: 50, typography: 'button' }}
                variant='contained'
                disabled={isSaving}
              >
                Submit
              </Button>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </form>
  )
}

export default TransactionStatusForm
