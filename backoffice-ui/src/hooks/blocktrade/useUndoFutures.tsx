import toast from 'react-hot-toast'
import { AppDispatch } from 'src/store'
import Swal from 'sweetalert2'
import axios from 'axios'
import { datadogLogs } from '@datadog/browser-logs'

// ** Custom Components Imports
import { IFuturesOrder } from '@/types/blocktrade/futures/types'
import { undoFuturesOrders } from '@/lib/api/clients/blocktrade/allocation'

const handleUndo = async (dispatch: AppDispatch, row: IFuturesOrder) => {
  try {
    const result = await Swal.fire({
      title: 'Are you sure?',
      text: 'Do you confirm to UNDO the futures order?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, undo it!',
    })

    if (result.isConfirmed) {
      const payload = {
        futuresId: Number(row.id),
      }

      try {
        datadogLogs.logger.info('blocktrade/undoFuturesOrders', {
          action: 'blocktrade/undoFuturesOrders',
          payload: payload,
          action_status: 'request',
        })

        await undoFuturesOrders(payload)

        const message = 'The undo of the futures order was successful.'
        toast.success(message)

        return { success: true }
      } catch (error) {
        if (axios.isAxiosError(error)) {
          datadogLogs.logger.error(
            'blocktrade/undoFuturesOrders',
            { action: 'blocktrade/undoFuturesOrders' },
            Error(error.response?.statusText ?? error.response?.data?.title)
          )
        } else {
          datadogLogs.logger.error(
            'blocktrade/undoFuturesOrders',
            { action: 'blocktrade/undoFuturesOrders' },
            Error((error as Error).message)
          )
        }
        const message = 'Undo of the futures order was error'
        toast.error(message)

        return { success: false }
      }
    } else {
      const message = 'Canceled undo the futures order'
      toast.error(message)

      return { success: false }
    }
  } catch (error) {
    const message = 'Undo of the futures order was error'
    toast.error(message)

    return { success: false }
  }
}

export { handleUndo }
