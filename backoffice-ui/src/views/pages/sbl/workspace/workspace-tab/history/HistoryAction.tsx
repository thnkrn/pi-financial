import Button from '@mui/material/Button'
import Card from '@mui/material/Card'
import CardActions from '@mui/material/CardActions'
import CardContent from '@mui/material/CardContent'
import Grid from '@mui/material/Grid'
import TextField from '@mui/material/TextField'
import { Controller, SubmitHandler, useForm } from 'react-hook-form'
import { IFormInput } from './index'

interface Props {
  onSubmit: SubmitHandler<IFormInput>
  onSearchChange: (searchValue: { account: string }) => void
}

const defaultValues = {
  account: '',
}

const HistoryAction = ({ onSubmit, onSearchChange }: Props) => {
  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({
    defaultValues,
    mode: 'onChange',
  })

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Grid container spacing={6}>
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Controller
                name='account'
                control={control}
                render={({ field: { value, onChange } }) => (
                  <TextField
                    size='small'
                    fullWidth
                    value={value}
                    label='Account'
                    placeholder='Account'
                    aria-describedby='validation-schema-account'
                    error={Boolean(errors.account)}
                    onChange={e => {
                      onChange(e)
                      onSearchChange({ account: e.target.value })
                    }}
                  />
                )}
              />
            </CardContent>

            <CardActions sx={{ display: 'flex', alignItems: 'right', justifyContent: 'right' }}>
              <Button size='medium' type='submit' sx={{ width: 250 }} fullWidth variant='contained'>
                <span>Search</span>
              </Button>
            </CardActions>
          </Card>
        </Grid>
      </Grid>
    </form>
  )
}

export default HistoryAction
