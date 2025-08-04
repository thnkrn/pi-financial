import { makeStyles } from '@mui/styles'

export const useTableHeaderStyles = makeStyles({
  tableHead: {
    fontWeight: 'bold',
    backgroundColor: '#888888',
    color: '#fff',
    textTransform: 'uppercase',
  },
  tableBody: {
    fontVariant: 'tabular-nums',
    position: 'relative',
  },
})

export const resetInputTypeNumber = makeStyles({
  input: {
    '& input[type=number]': {
      '-moz-appearance': 'textfield',
    },
    '& input[type=number]::-webkit-outer-spin-button': {
      '-webkit-appearance': 'none',
      margin: 0,
    },
    '& input[type=number]::-webkit-inner-spin-button': {
      '-webkit-appearance': 'none',
      margin: 0,
    },
  },
})
