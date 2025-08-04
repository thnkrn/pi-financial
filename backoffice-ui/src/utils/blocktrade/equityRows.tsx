import { EquityRowType } from '@/views/pages/blocktrade/dashboard/types'

const processedEquityOrderRows = (rows: EquityRowType[]) => {
  const processedData = rows.map(item => ({
    ...item,
    employeeId: item.blockOrders.sales?.employeeId ?? '',
    openClose: item.blockOrders.openClose,
    side: item.blockOrders.clientSide,
    symbol: (item.blockOrders.symbol?.symbol ?? '') + (item.blockOrders.series?.series ?? ''),
    numOfContract: item.blockOrders.numOfContract,
    numOfShareFilled: item.numOfShareFilled,
    orderPrice: item.orderPrice,
    customerAccount: item.blockOrders.customerAccount,
    icName: item.blockOrders.sales?.name ?? '',
  }))

  return processedData
}

export default processedEquityOrderRows
