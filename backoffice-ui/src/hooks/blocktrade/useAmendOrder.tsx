import { submitAmendOrder, updateData } from 'src/store/apps/blocktrade/order'
import toast from 'react-hot-toast'
import { IOrderState, IEquityOrder } from 'src/types/blocktrade/orders/types'
import { AppDispatch } from 'src/store'

export const handleAmendOrder = async (dispatch: AppDispatch, orderStore: IOrderState) => {
  try {
    if (orderStore.id) {
      const orderData = {
        series: orderStore.futuresProperty?.series,
        orderPrice: Number(orderStore.equityPrice),
      }
      const response = await dispatch(submitAmendOrder({ id: orderStore.id, orderData }))

      if (response.meta.requestStatus === 'fulfilled') {
        const message = 'The order is amended successfully'
        toast.success(message)

        return { success: true, message }
      } else {
        const message = 'Order amendment was unsuccessful'
        toast.error(message)

        return { success: false, message }
      }
    } else {
      const message = 'Order amendment was error'
      toast.error(message)

      return { success: false, message }
    }
  } catch (error) {
    const message = 'Order amendment was error'
    toast.error(message)

    return { success: false, message }
  }
}

export const pushAmendData = async (dispatch: AppDispatch, data: IEquityOrder) => {
  dispatch(
    updateData({
      isAmend: true,
      id: data.id,
      ic: data.blockOrders.saleId,
      oc: data.blockOrders.openClose,
      side: data.blockOrders.clientSide,
      symbol: data.blockOrders.symbol.symbol + data.blockOrders.series.series,
      customer: data.blockOrders.customerAccount,
      contractAmount: data.blockOrders.numOfContract,
      equityPrice: data.orderPrice,
      orderStatus: data.status,
    })
  )

  const message = 'Pushed order data to forms'
  toast.success(message)

  return { success: true, message }
}
