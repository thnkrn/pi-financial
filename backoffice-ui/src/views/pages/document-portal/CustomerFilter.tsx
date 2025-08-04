import { resetInputTypeNumber } from '@/utils/styles'
import { yupResolver } from '@hookform/resolvers/yup'
import { Button, FormControl, FormHelperText, TextField } from '@mui/material'
import Box from '@mui/material/Box'
import Stack from '@mui/material/Stack'
import { trim } from 'lodash'
import useTranslation from 'next-translate/useTranslation'
import { useEffect, useRef, useState } from 'react'
import { Controller, useForm } from 'react-hook-form'
import * as yup from 'yup'
import { CustomerFilterProps } from './types'

const defaultValues = {
  customerCode: '',
}

const schema = yup.object().shape({
  customerCode: yup.string().max(8, 'Please enter a Customer Code no longer than 8 digits'),
})

const CustomerFilter = ({ onFilter, error, onChange }: CustomerFilterProps) => {
  const classes = resetInputTypeNumber()
  const {
    control,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm({
    defaultValues,
    mode: 'onChange',
    resolver: yupResolver(schema),
  })

  const [customerCode, setCustomerCode] = useState<string>('')
  const { t } = useTranslation('document_portal')

  const handleCustomerCodeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const inputValue = trim(e.target.value)
    setCustomerCode(inputValue)
    onChange()
    if (inputValue.length === 0) {
      onFilter('')
    }
    setValue('customerCode', inputValue, { shouldValidate: true })
  }

  const handleFilterSubmit = () => {
    onFilter(customerCode)
  }

  const custCodeInputRef = useRef<HTMLInputElement | null>(null)

  useEffect(() => {
    const handleWheel = (e: WheelEvent) => {
      if (custCodeInputRef?.current) {
        e.preventDefault()
      }
    }

    const currentCustCodeInputRef = custCodeInputRef.current
    currentCustCodeInputRef?.addEventListener('wheel', handleWheel)

    return () => {
      currentCustCodeInputRef?.removeEventListener('wheel', handleWheel)
    }
  }, [custCodeInputRef])

  return (
    <Box sx={{ marginBottom: '20px' }}>
      <form onSubmit={handleSubmit(handleFilterSubmit)}>
        <Stack spacing={{ xs: 2, sm: 4 }} direction={{ xs: 'row' }} alignItems='flex-start'>
          <FormControl sx={{ flexGrow: 1 }}>
            <Controller
              name='customerCode'
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  type='number'
                  size={'small'}
                  autoFocus
                  label={t('DOCUMENT_PORTAL_SEARCH_BY_CUSTOMER_CODE', {}, { default: 'Search by customer code...' })}
                  variant='outlined'
                  name={'customerCode'}
                  value={customerCode}
                  onChange={handleCustomerCodeChange}
                  error={Boolean(errors?.customerCode) || Boolean(error)}
                  className={classes.input}
                  ref={custCodeInputRef}
                  data-testid={'customer-text-field-search'}
                />
              )}
            />
            {(errors?.customerCode || Boolean(error)) && (
              <FormHelperText sx={{ color: 'error.main' }}>{errors?.customerCode?.message ?? error}</FormHelperText>
            )}
          </FormControl>

          <Button type='submit' variant='contained' data-testid={'customer-search'}>
            {t('DOCUMENT_PORTAL_SEARCH', {}, { default: 'Search' })}
          </Button>
        </Stack>
      </form>
    </Box>
  )
}

export default CustomerFilter
