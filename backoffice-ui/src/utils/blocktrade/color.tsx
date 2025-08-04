import { DecimalNumber } from '@/utils/blocktrade/decimal'
import { Side } from '@/constants/blocktrade/GlobalEnums'

export const colorTextPnL = (value: number, decimal?: number) => {
  let formattedValue = DecimalNumber(value, decimal ?? 2)
  let color = 'red'

  if (Number(value) > 0) {
    color = '#02C359'
    formattedValue = `+${formattedValue}`
  } else if (Number(value) === 0) {
    color = '#FBB033'
  }

  return <span style={{ color: color }}>{formattedValue}</span>
}

export const colorTextLS = (value: string) => {
  return value === Side.LONG ? (
    <span style={{ color: '#00A3A3', fontWeight: 600 }}>{value}</span>
  ) : (
    <span style={{ color: '#A771BF', fontWeight: 600 }}>{value}</span>
  )
}
