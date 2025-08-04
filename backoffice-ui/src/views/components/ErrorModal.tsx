import { useEffect } from 'react'
import Swal from 'sweetalert2'

interface ErrorModalProps {
  isError: boolean
  errorMessage: string
  dependencies: string[]
}

const ErrorModal = (props: ErrorModalProps) => {
  useEffect(() => {
    if (props.isError) {
      Swal.fire({
        title: 'Error!',
        text: props.errorMessage,
        icon: 'error',
        confirmButtonText: 'OK',
      })
    }
    /* eslint-disable */
  }, [...props.dependencies])

  return <div></div>
}

export default ErrorModal
