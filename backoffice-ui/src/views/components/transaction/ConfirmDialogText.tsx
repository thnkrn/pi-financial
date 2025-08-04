import { Typography } from '@mui/material'

interface ConfirmDialogProps {
  remark: string
  action: string
}

const ConfirmDialogText = ({ remark, action }: ConfirmDialogProps) => {
  return (
    <>
      <Typography mb={2} variant='h5' textAlign='left'>
        Confirm Action
      </Typography>
      <Typography py={1} variant='h6' textAlign='left'>
        Are you sure you want to perform the following action?
      </Typography>
      <Typography variant='h6' textAlign='left'>
        <Typography variant='h6' fontWeight='bold'>
          Action :{' '}
        </Typography>
        {action}
        <Typography variant='h6' fontWeight='bold'>
          Remark :{' '}
        </Typography>
        {remark}
      </Typography>
    </>
  )
}

export default ConfirmDialogText
