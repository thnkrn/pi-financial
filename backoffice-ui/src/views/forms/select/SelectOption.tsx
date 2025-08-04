import { fetchDropdown } from '@/store/apps/dropdown'
import { RootState } from '@/store/index'
import { Select, SelectChangeEvent } from '@mui/material'
import FormControl from '@mui/material/FormControl'
import InputLabel from '@mui/material/InputLabel'
import MenuItem from '@mui/material/MenuItem'
import { ThunkDispatch } from '@reduxjs/toolkit'
import map from 'lodash/map'
import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'

const ITEM_HEIGHT = 48
const ITEM_PADDING_TOP = 8
const MenuProps = {
  PaperProps: {
    style: {
      width: 250,
      maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
    },
  },
}

interface IOption {
  [key: string]: any
}

interface IRemote {
  field: string
  url: string
  key: string
  value: string
  extension?: string
  prefix?: string
}

interface SelectMultipleProps {
  id: string
  label: string
  labelId: string
  options?: any[]
  onChange: (value: string) => any
  defaultValue?: any
  remote?: IRemote
  disabled?: boolean
  disabledAll?: boolean
  placeholder?: string
}

const SelectOption = ({
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
  const initialValue = disabledAll ? '' : defaultValue || 'ALL'
  const allOptions: IOption[] = [{ key: 'ALL', value: 'ALL' }]
  const initialOptions: IOption[] = options ? allOptions.concat(options) : allOptions

  const [value, setValue] = useState<string>(initialValue)
  const [selectOptions, setSelectOptions] = useState<IOption[]>(disabledAll ? [] : initialOptions)

  const store = useSelector((state: RootState) => state.dropdown)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const handleChange = (event: SelectChangeEvent<string>) => {
    setValue(event.target.value)
    onChange(event.target.value)
  }

  useEffect(() => {
    setValue(initialValue)
    if (remote) {
      dispatch(fetchDropdown({ field: remote.field, url: remote.url }))
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialValue])

  useEffect(() => {
    if (remote) {
      if (store.values[remote.field]) {
        const key = remote?.key
        const value = remote?.value
        const extension = remote?.extension
        const prefix = remote?.prefix

        let mapOptions: IOption[] = map(store.values[remote.field] as IOption[], option => {
          return {
            key: option[key],
            value: option[value],
            extension: extension && option[extension],
            prefix: prefix && option[prefix],
          }
        })

        // NOTE: hardcode to remove SetTrade from channel option for ThaiEquity and GlobalEquity since BE treat SetTrade as a part of ThaiEquity
        if (remote.field === 'channel') {
          mapOptions = mapOptions.filter(option => {
            return option.key !== 'SetTrade'
          })
        }

        const values: IOption[] = [...(disabledAll ? [] : allOptions), ...mapOptions]
        setSelectOptions(values)
      }
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
              {`${option?.prefix ? `${option?.prefix} - ` : ''}${option.value}${
                option?.extension ? ` - ${option?.extension}` : ''
              }`}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </div>
  )
}

export default SelectOption
