import APP_CONSTANTS from '@/constants/app'
import Swal from 'sweetalert2'

const displayAlert = (message: string) => {
  Swal.fire({
    title: 'Error!',
    text: message || APP_CONSTANTS.GLOBAL_ERROR_MESSAGE,
    icon: 'error',
  })
}

export default displayAlert
