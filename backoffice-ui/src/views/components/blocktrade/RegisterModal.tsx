// ** React Imports
import { useState } from 'react'
import { useRouter } from 'next/router'
import { useDispatch } from 'react-redux'
import { ThunkDispatch } from '@reduxjs/toolkit'

// ** MUI Imports
import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import Dialog from '@mui/material/Dialog'
import DialogTitle from '@mui/material/DialogTitle'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import TextField from '@mui/material/TextField'
import SelectOptionPre from '@/views/pages/blocktrade/SelectOptionPre'

// ** Custom Components
import { fetchUserData } from '@/store/apps/blocktrade/user'
import { createUser } from '@/lib/api/clients/blocktrade/users'
import { ICreateUserRequest } from '@/lib/api/clients/blocktrade/users/types'

type RegisterModalProps = {
  name?: string | null
}

const RegisterModal = ({ name }: RegisterModalProps) => {
  const [open, setOpen] = useState<boolean>(true)
  const [team, setTeam] = useState<number>(0)
  const [employeeId, setEmployeeId] = useState<number | null>(null)
  const [message, setMessage] = useState<string>('')

  const router = useRouter()
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()

  const handleClose = () => {
    setOpen(false)
  }

  const handleSubmit = async (empId: number, tId: number) => {
    const data: ICreateUserRequest = {
      employeeId: empId,
      teamId: tId,
    }
    try {
      const response = await createUser(data)
      if (response) {
        setMessage('success')
        dispatch(fetchUserData())
      } else {
        setMessage('error')
      }
    } catch (error) {
      setMessage('error')
    }
  }

  return (
    <Dialog open={open} disableEscapeKeyDown>
      <DialogTitle>FIRST ENTER TO BLOCKTRADE ?</DialogTitle>
      {message === '' && name ? (
        <DialogContent>
          <DialogContentText sx={{ mb: 3 }}>
            Welcome <b>{name}</b>
            <br />
            Please provide your information
          </DialogContentText>
          <TextField
            id='name'
            size={'small'}
            autoFocus
            fullWidth
            type='Employee ID'
            label='Employee ID'
            sx={{ mb: 3 }}
            onChange={event => {
              setEmployeeId(Number(event.target.value))
            }}
          />
          <Box sx={{ mb: 3 }}>
            <SelectOptionPre
              id={'teamList'}
              labelId={'teamList'}
              label={'Team'}
              remote={{
                field: 'teamList',
                url: 'users/getTeams?active=true',
                key: 'id',
                value: 'name',
              }}
              onChange={key => {
                setTeam(Number(key))
              }}
            />
          </Box>
          <Button
            size='medium'
            onClick={async () => {
              if (employeeId && employeeId / 1000 >= 1 && team) {
                await handleSubmit(employeeId, team)
              }
            }}
            sx={{ width: '100%', fontWeight: 700, marginTop: 0 }}
            color='primary'
            variant='contained'
            disabled={(employeeId || 0) / 1000 < 1 || !team}
          >
            Submit
          </Button>
        </DialogContent>
      ) : (
        <DialogContent>
          <DialogContentText sx={{ mb: 3 }}>
            {message === 'success'
              ? 'Blocktrade Platform for IC is ready to use'
              : 'Got an error. Please contact the derivatives team.'}
          </DialogContentText>
          <Button
            size='medium'
            sx={{ width: '100%', fontWeight: 700, marginTop: 0 }}
            color='primary'
            variant='contained'
            onClick={() => {
              message === 'success'
                ? router
                    .push({
                      pathname: `/blocktrade/dashboard`,
                    })
                    .then(() => handleClose())
                : router.push({
                    pathname: `/dashboard`,
                  })
            }}
          >
            {message === 'success' ? 'Start' : 'Back to Main Dashboard'}
          </Button>
        </DialogContent>
      )}
    </Dialog>
  )
}

export default RegisterModal
