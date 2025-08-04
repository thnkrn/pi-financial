import toast from 'react-hot-toast'
import { AppDispatch } from 'src/store'
import Swal, { SweetAlertOptions } from 'sweetalert2'
import { datadogLogs } from '@datadog/browser-logs'
import { submitFuturesOrders } from '@/lib/api/clients/blocktrade/allocation'
import axios from 'axios'

export const handleSubmitFutures = async (
  dispatch: AppDispatch,
  futuresId: number,
  forIC: boolean,
  isAdmin: boolean
) => {
  try {
    const baseSwalOptions: SweetAlertOptions = {
      title: 'Are you sure?',
      text: 'Did you already submit the futures order?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, submitted it!',
    }

    const swalOptions: SweetAlertOptions =
      isAdmin && forIC
        ? { ...baseSwalOptions, input: 'checkbox', inputPlaceholder: 'Place Trade Report to TFEX', inputValue: 0 }
        : baseSwalOptions

    const result = await Swal.fire(swalOptions)

    if (result.isConfirmed) {
      const placeTradeReport = Boolean(result.value)
      const payload = {
        futuresId,
        isIC: forIC,
        placeTradeReport,
      }

      try {
        datadogLogs.logger.info('blocktrade/submitFuturesOrders', {
          action: 'blocktrade/submitFuturesOrders',
          payload: payload,
          action_status: 'request',
        })

        await submitFuturesOrders(payload)

        const message = 'The submitting of a futures order has been reported.'
        toast.success(message)

        return { success: true }
      } catch (error) {
        if (axios.isAxiosError(error)) {
          datadogLogs.logger.error(
            'blocktrade/submitFuturesOrders',
            { action: 'blocktrade/submitFuturesOrders' },
            Error(error.response?.statusText ?? error.response?.data?.title)
          )
        } else {
          datadogLogs.logger.error(
            'blocktrade/submitFuturesOrders',
            { action: 'blocktrade/submitFuturesOrders' },
            Error((error as Error).message)
          )
        }
        const message = 'Error on submitting the report of a futures order'
        toast.error(message)

        return { success: false }
      }
    } else {
      const message = 'Canceled submitting the report of a futures order'
      toast.error(message)

      return { success: false }
    }
  } catch (error) {
    const message = 'Submitting was error'
    toast.error(message)

    return { success: false }
  }
}
