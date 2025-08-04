import { submitCancelOrder } from 'src/store/apps/blocktrade/order'
import toast from 'react-hot-toast'
import { AppDispatch } from 'src/store'
import { IEquityOrder } from 'src/types/blocktrade/orders/types'
import Swal from 'sweetalert2'

export const handleCancelOrder = async (dispatch: AppDispatch, data: IEquityOrder) => {
  try {
    const result = await Swal.fire({
      title: 'Are you sure?',
      text: 'You are about to cancel the order!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, cancel it!',
    })

    if (result.isConfirmed) {
      await dispatch(submitCancelOrder({ id: data.id }))

      const message = 'The order is cancelled successfully'
      toast.success(message)

      return { success: true, message }
    } else {
      const message = 'Order cancellation canceled'
      toast.error(message)

      return { success: false, message }
    }
  } catch (error) {
    const message = 'Order cancellation was error'
    toast.error(message)

    return { success: false, message }
  }
}
