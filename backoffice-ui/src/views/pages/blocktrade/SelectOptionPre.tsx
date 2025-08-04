import { fetchDropdown } from '@/store/apps/blocktrade/dropdown'
import { RootState } from '@/store/index'
import { Select, SelectChangeEvent } from '@mui/material'
import FormControl from '@mui/material/FormControl'
import InputLabel from '@mui/material/InputLabel'
import MenuItem from '@mui/material/MenuItem'
import { ThunkDispatch } from '@reduxjs/toolkit'
import map from 'lodash/map'
import { useRouter } from 'next/router'
import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'

const ITEM_HEIGHT = 48
const ITEM_PADDING_TOP = 8
const MenuProps = {
  PaperProps: {
    style: {
      width: 150,
      maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
    },
  },
}

interface SelectMultipleProps {
  id: string
  label: string
  labelId: string
  options?: any[]
  onChange: (value: string) => any
  defaultValue?: string | number | null
  remote?: { field: string; url: string; key: string; value: string; extension?: string }
  disabled?: boolean
  disabledAll?: boolean
  placeholder?: string
}

// TODO full replace to SelectOption
const SelectOptionPre = ({
  id,
  label,
  labelId,
  options,
  onChange,
  defaultValue,
  remote,
  disabled,
  disabledAll,
  placeholder,
}: SelectMultipleProps) => {
  const [allOptions, setAllOptions] = useState<any[]>([])

  const initialValue = disabledAll ? '' : defaultValue ?? ''
  const initialOptions = allOptions

  const [value, setValue] = useState<any>(initialValue)
  const [selectOptions, setSelectOptions] = useState<any[]>(disabledAll ? [] : initialOptions)
  const router = useRouter()

  const store = useSelector((state: any) => state.btDropdown)
  const error = useSelector((state: RootState) => state.btDropdown.errorMessage)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const handleChange = (event: SelectChangeEvent<string[]>) => {
    setValue(event.target.value as string)
    onChange(event.target.value as string)
  }

  useEffect(() => {
    setValue(initialValue)
    if (remote) {
      dispatch(fetchDropdown({ field: remote.field, url: remote.url }))
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialValue, defaultValue])

  useEffect(() => {
    if (options && options.length > 0) {
      setAllOptions(options)
    }
  }, [options])

  useEffect(() => {
    if (remote) {
      if (store.values[remote.field]) {
        const key = remote?.key
        const value = remote?.value
        const extension = remote?.extension

        let mapOptions = map(store.values[remote.field], option => {
          return {
            key: option[key],
            value: option[value],
            extension: extension && option[extension],
          }
        })

        // NOTE: hardcode to remove SetTrade from channel option for ThaiEquity and GlobalEquity since BE treat SetTrade as a part of ThaiEquity
        if (remote.field === 'channel') {
          mapOptions = mapOptions.filter(option => {
            return option.key !== 'SetTrade'
          })
        }

        const values = [...(disabledAll ? [] : allOptions), ...mapOptions]
        setSelectOptions(values)
      }
    } else if (store.isLoading === false && error === 'UNAUTHORIZED') {
      router.replace({ pathname: '/error', query: { reason: 'unauthorized' } })
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store])

  return (
    <div>
      <FormControl fullWidth size={'small'}>
        <InputLabel id={labelId}>{label}</InputLabel>
        <Select
          label={label}
          value={value}
          MenuProps={MenuProps}
          id={id}
          onChange={handleChange}
          labelId={labelId}
          disabled={disabled}
          placeholder={placeholder}
        >
          {selectOptions.map(option => (
            <MenuItem key={option.key} value={option.key}>
              {option?.extension ? `${option.extension} : ${option.value}` : option.value}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </div>
  )
}

export default SelectOptionPre
