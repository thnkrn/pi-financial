import { submitNewOrder } from '@/store/apps/blocktrade/order'
import { AppDispatch } from '@/store/index'
import { IOrderState } from '@/types/blocktrade/orders/types'
import toast from 'react-hot-toast'
import { ISubmitNewOrderRequest } from '@/lib/api/clients/blocktrade/equity/types'

export const handleNewOrder = async (dispatch: AppDispatch, orderStore: IOrderState) => {
  try {
    const orderData: ISubmitNewOrderRequest = {
      saleId: orderStore.ic,
      openClose: orderStore.oc,
      side: orderStore.side,
      symbol: orderStore.futuresProperty.symbol,
      series: orderStore.futuresProperty.series,
      numOfContract: orderStore.contractAmount,
      orderPrice: Number(orderStore.equityPrice),
      orderType: orderStore.orderType,
      customerAccount: orderStore.customer,
    }
    const response = await dispatch(submitNewOrder(orderData))

    if (response.meta.requestStatus === 'fulfilled') {
      const message = 'The order is submitted successfully'
      toast.success(message)

      return { success: true, message }
    } else {
      const message = 'Order submitting was unsuccessful'
      toast.error(message)

      return { success: false, message }
    }
  } catch (error) {
    const message = 'Order submitting was error'
    toast.error(message)

    return { success: false, message }
  }
}
