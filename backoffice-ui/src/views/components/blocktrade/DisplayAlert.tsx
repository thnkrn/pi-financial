import APP_CONSTANTS from '@/constants/app'
import Swal from 'sweetalert2'

const SUCCESS_STATUS = 'success'

const DisplayAlert = (message: string, status?: string) => {
  Swal.fire({
    title: status === SUCCESS_STATUS ? 'Success!' : 'Error!',
    text: message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE,
    icon: status === SUCCESS_STATUS ? 'success' : 'error',
  })
}

export default DisplayAlert
