'use client'

import {
  Cancel,
  CheckCircle,
  Email,
  Lock,
  LockOpen,
  Password,
  Pin,
  Visibility,
  VisibilityOff,
} from '@mui/icons-material'
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  IconButton,
  InputAdornment,
  TextField,
  Tooltip,
} from '@mui/material'
import { DataGrid } from '@mui/x-data-grid'
import { useState } from 'react'
import { DataTableProps } from './types'

// ย้าย Dialog Confirm ไปเป็น Component แยก
interface ConfirmDialogProps {
  open: boolean
  title: string
  contentText: string
  onClose: () => void
  onConfirm: () => void
  isProcessing?: boolean
  confirmText?: string
}

const ConfirmDialog = ({
  open,
  title,
  contentText,
  onClose,
  onConfirm,
  isProcessing = false,
  confirmText = 'Confirm',
}: ConfirmDialogProps) => {
  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent>
        <DialogContentText>{contentText}</DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button disabled={isProcessing} onClick={onConfirm} color='primary' variant='contained'>
          {isProcessing ? 'Processing...' : confirmText}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

// ย้าย Dialog Input (รับค่าจาก TextField) ไปเป็น Component แยก
interface InputDialogProps {
  open: boolean
  title: string
  label: string
  contentText: string
  value: string
  onChange: (val: string) => void
  onClose: () => void
  onConfirm: () => void
  isProcessing?: boolean
  disabledCondition?: boolean
  confirmText?: string
  type?: string
  canToggleVisibility?: boolean // เพิ่ม prop สำหรับ toggle การมองเห็น
}

const InputDialog = ({
  open,
  title,
  label,
  contentText,
  value,
  onChange,
  onClose,
  onConfirm,
  isProcessing = false,
  disabledCondition = false,
  confirmText = 'Confirm',
  type = 'password',
  canToggleVisibility = false,
}: InputDialogProps) => {
  const [showValue, setShowValue] = useState(false) // state สำหรับซ่อน/แสดงค่าที่พิมพ์

  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent>
        <DialogContentText>{contentText}</DialogContentText>
        <TextField
          autoFocus
          margin='dense'
          label={label}
          type={showValue ? 'text' : type}
          fullWidth
          variant='outlined'
          value={value}
          onChange={e => onChange(e.target.value)}
          InputProps={{
            endAdornment: canToggleVisibility ? (
              <InputAdornment position='end'>
                <IconButton onClick={() => setShowValue(prev => !prev)}>
                  {showValue ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              </InputAdornment>
            ) : null,
          }}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button disabled={isProcessing || disabledCondition} onClick={onConfirm} color='primary' variant='contained'>
          {isProcessing ? 'Processing...' : confirmText}
        </Button>
      </DialogActions>
    </Dialog>
  )
}

const DataTable = ({ rows, pagination, onPageFilter, loading, fetchAccountData }: DataTableProps) => {
  // จัดกลุ่ม state ที่เกี่ยวข้องกับ Dialog
  const [isProcessing, setIsProcessing] = useState(false)
  const [selectedRow, setSelectedRow] = useState<any>(null)

  // Unlock Dialog
  const [openUnlockDialog, setOpenUnlockDialog] = useState(false)

  // Send Email Reset Password Dialog`
  const [openSendEmailResetPwdDialog, setOpenSendEmailResetPwdDialog] = useState(false)
  const [openSendEmailResetPINDialog, setOpenSendEmailResetPINDialog] = useState(false)
  const [openUnlockPinDialog, setOpenUnlockPinDialog] = useState(false)

  // Change Password Dialog
  const [openChangePasswordDialog, setOpenChangePasswordDialog] = useState(false)
  const [newPassword, setNewPassword] = useState('')

  // Change PIN Dialog
  const [openChangePinDialog, setOpenChangePinDialog] = useState(false)
  const [newPin, setNewPin] = useState('')

  // custom locale
  const customLocaleText = { noRowsLabel: 'No results found' }

  /** ---------- DIALOG HANDLERS ---------- */
  const handleOpenUnlockDialog = (row: any) => {
    setSelectedRow(row)
    setOpenUnlockDialog(true)
  }
  const handleCloseUnlockDialog = () => {
    setOpenUnlockDialog(false)
    setSelectedRow(null)
  }

  const handleOpenSendEmailResetPwdDialog = (row: any) => {
    setSelectedRow(row)
    setOpenSendEmailResetPwdDialog(true)
  }
  const handleCloseSendEmailResetPwdDialog = () => {
    setOpenSendEmailResetPwdDialog(false)
    setSelectedRow(null)
  }

  const handleOpenSendEmailResetPINDialog = (row: any) => {
    setSelectedRow(row)
    setOpenSendEmailResetPINDialog(true)
  }
  const handleCloseSendEmailResetPINDialog = () => {
    setOpenSendEmailResetPINDialog(false)
    setSelectedRow(null)
  }

  const handleCloseUnlockPinDialog = () => {
    setOpenUnlockPinDialog(false)
    setSelectedRow(null)
  }

  const handleOpenChangePasswordDialog = (row: any) => {
    setSelectedRow(row)
    setOpenChangePasswordDialog(true)
  }
  const handleCloseChangePasswordDialog = () => {
    setOpenChangePasswordDialog(false)
    setSelectedRow(null)
    setNewPassword('')
  }

  const handleOpenChangePinDialog = (row: any) => {
    setSelectedRow(row)
    setOpenChangePinDialog(true)
  }
  const handleCloseChangePinDialog = () => {
    setOpenChangePinDialog(false)
    setSelectedRow(null)
    setNewPin('')
  }

  /** ---------- API HANDLERS ---------- */
  const handleConfirmUnLock = async () => {
    try {
      setIsProcessing(true)
      const response = await fetch('/api/sso/account-management/unlockUser', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: selectedRow.username }),
      })
      if (!response.ok) {
        throw new Error(`Error: ${response.status} - ${response.statusText}`)
      }

      alert('Account unlocked successfully.')
    } catch (error) {
      alert(error)
    } finally {
      setIsProcessing(false)
      handleCloseUnlockDialog()
      fetchAccountData()
    }
  }

  const handleConfirmUnLockPin = async () => {
    try {
      setIsProcessing(true)
      const response = await fetch('/api/sso/account-management/unlockPin', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: selectedRow.username }),
      })
      if (!response.ok) {
        throw new Error(`Error: ${response.status} - ${response.statusText}`)
      }

      //const data = await response.json()
      alert('Account unlocked successfully.')
    } catch (error) {
      alert('Failed to unlock account. Please try again.')
    } finally {
      setIsProcessing(false)
      handleCloseUnlockPinDialog()
      fetchAccountData()
    }
  }

  const handleConfirmSendEmailResetPwd = async () => {
    try {
      setIsProcessing(true)
      const response = await fetch('/api/sso/account-management/sendResetPwd', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: selectedRow.username }),
      })
      if (!response.ok) {
        throw new Error(`Error: ${response.status} - ${response.statusText}`)
      }

      //const data = await response.json()
      alert('Reset password email sent successfully.')

      //datadogLogs.logger.info('Account send reset pwd  successfully:', data)
    } catch (error) {
      //datadogLogs.logger.error('Error send reset pwd account:' + error)
      alert('Failed to send reset pwd account. Please try again.')
    } finally {
      setIsProcessing(false)
      handleCloseSendEmailResetPwdDialog()
      fetchAccountData()
    }
  }

  const handleConfirmSendEmailResetPIN = async () => {
    try {
      setIsProcessing(true)

      const response = await fetch('/api/sso/account-management/sendResetPin', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: selectedRow.username }),
      })

      if (!response.ok) {
        throw new Error(`Error: ${response.status} - ${response.statusText}`)
      }

      //const data = await response.json()
      alert('Reset pin email sent successfully.')

      //datadogLogs.logger.info('Account send reset pin successfully:', data)
    } catch (error) {
      //datadogLogs.logger.error('Error send reset pin  account:' + error)
      alert('Failed to send reset pin  account. Please try again.')
    } finally {
      setIsProcessing(false)
      handleCloseSendEmailResetPINDialog()
      fetchAccountData()
    }
  }

  const handleConfirmChangePassword = async () => {
    try {
      setIsProcessing(true)

      // ตรวจสอบเงื่อนไข Password (ตัวพิมพ์ใหญ่ ตัวพิมพ์เล็ก ตัวเลข อย่างน้อย 1 ตัว และยาว >= 6)
      const passwordPattern = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d~!@#$%^&*()_+]{6,}$/
      const isPasswordValid = passwordPattern.test(newPassword)
      if (!isPasswordValid) {
        alert('Password must be at least 6 characters long and contain uppercase, lowercase, and digits.')
        setIsProcessing(false)

        return
      }

      const response = await fetch('/api/sso/account-management/changePassword', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: selectedRow.username, newPassword }),
      })
      if (!response.ok) {
        throw new Error(`Error: ${response.status} - ${response.statusText}`)
      }

      //const data = await response.json()
      alert('Password changed successfully.')
    } catch (error) {
      alert('Failed to change password. Please try again.')
    } finally {
      setIsProcessing(false)
      handleCloseChangePasswordDialog()
      fetchAccountData()
    }
  }

  const handleConfirmChangePin = async () => {
    try {
      setIsProcessing(true)

      // ตรวจสอบว่า newPin เป็นตัวเลขหกหลักหรือไม่
      const isPinValid = /^\d{6}$/.test(newPin)
      if (!isPinValid) {
        alert('PIN must be a 6-digit numeric value.')
        setIsProcessing(false)

        return
      }

      const response = await fetch('/api/sso/account-management/changePin', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: selectedRow.username, newPin }),
      })
      if (!response.ok) {
        throw new Error(`Error: ${response.status} - ${response.statusText}`)
      }

      alert('Pin changed successfully.')
    } catch (error) {
      alert('Failed to change pin. Please try again.')
    } finally {
      setIsProcessing(false)
      handleCloseChangePinDialog()
      fetchAccountData()
    }
  }

  /** ---------- TABLE COLUMNS ---------- */
  const columns = [
    { field: 'username', headerName: 'Username', width: 270 },
    {
      field: 'userInfo',
      headerName: 'User Info',
      width: 300,
      renderCell: (params: any) => (
        <div>
          <div>
            <strong>ID:</strong> {params.row.userId}
          </div>
          <div>
            <strong>Email:</strong> {params.row.email}
          </div>
          <div>
            <strong>Mobile:</strong> {params.row.mobile}
          </div>
          <div>
            <strong>CardID:</strong> {params.row.cardId}
          </div>
        </div>
      ),
    },
    {
      field: 'isSyncPassword',
      headerName: 'S.Pwd',
      width: 60,
      renderCell: (params: any) => (params.value ? <CheckCircle color='primary' /> : <Cancel color='error' />),
    },
    {
      field: 'isSyncPin',
      headerName: 'S.Pin',
      width: 60,
      renderCell: (params: any) => (params.value ? <CheckCircle color='primary' /> : <Cancel color='error' />),
    },
    { field: 'loginPwdFailCount', headerName: 'PWD.Fail', width: 80 },
    { field: 'loginPinFailCount', headerName: 'PIN.Fail', width: 80 },
    {
      field: 'timeStamp',
      headerName: 'Time Stamp',
      width: 230,
      renderCell: (params: any) => (
        <div>
          <div>
            <strong>Created:</strong> {params.row.createdAt}
          </div>
          <div>
            <strong>Updated:</strong> {params.row.updatedAt}
          </div>
        </div>
      ),
    },
    {
      field: 'password-actions',
      headerName: 'Password',
      width: 100,
      renderCell: (params: any) => (
        <>
          <Tooltip title='Reset PWD'>
            <IconButton color='primary' size='small' onClick={() => handleOpenChangePasswordDialog(params.row)}>
              <Password />
            </IconButton>
          </Tooltip>
          <Tooltip title='Send EMail Reset PWD'>
            <IconButton color='primary' size='small' onClick={() => handleOpenSendEmailResetPwdDialog(params.row)}>
              <Email />
            </IconButton>
          </Tooltip>
        </>
      ),
    },
    {
      field: 'pin-actions',
      headerName: 'Pin',
      width: 100,
      renderCell: (params: any) => {
        // ตรวจสอบว่า username เป็นตัวเลขทั้งหมด และมีความยาว 7 หรือไม่
        const isAllDigits = /^\d+$/.test(params.row.username)
        const isLength7 = params.row.username.length === 7

        // ถ้าไม่ผ่านเงื่อนไข ให้ return null เพื่อซ่อน (ไม่แสดง) pin-actions
        if (!isAllDigits || !isLength7) {
          return null
        }

        // ถ้าเงื่อนไขผ่าน จะแสดง pin-actions ตามปกติ
        return (
          <>
            <Tooltip title='Reset PIN'>
              <IconButton color='primary' size='small' onClick={() => handleOpenChangePinDialog(params.row)}>
                <Pin />
              </IconButton>
            </Tooltip>
            <Tooltip title='Send EMail Reset PIN'>
              <IconButton color='primary' size='small' onClick={() => handleOpenSendEmailResetPINDialog(params.row)}>
                <Email />
              </IconButton>
            </Tooltip>
          </>
        )
      },
    },
    {
      field: 'unlock-actions',
      headerName: 'Unlock Account',
      width: 100,
      renderCell: (params: any) => {
        // ถ้าเงื่อนไขผ่าน จะแสดง pin-actions ตามปกติ
        return (
          <>
            <Tooltip title='Unlock Account'>
              <IconButton size='small' onClick={() => handleOpenUnlockDialog(params.row)}>
                {params.row.isLock ? <Lock color='error' /> : <LockOpen color='primary' />}
              </IconButton>
            </Tooltip>
          </>
        )
      },
    },
  ]

  // ตัวแปรสำหรับใช้ใน disabledCondition ของ PIN Dialog
  const pinIsNotValid = !/^\d{6}$/.test(newPin)

  // ตัวแปรสำหรับใช้ใน disabledCondition ของ Password Dialog
  const passwordPattern = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d~!@#$%^&*()_+]{6,}$/
  const passwordIsNotValid = !passwordPattern.test(newPassword)

  return (
    <>
      {/* Data Grid */}
      <Box sx={{ height: 'auto', width: '100%', marginTop: 4 }}>
        <DataGrid
          autoHeight
          pagination
          rows={rows || []}
          rowCount={pagination.total}
          columns={columns}
          paginationMode='server'
          pageSizeOptions={[20, 50, 100]}
          paginationModel={pagination}
          onPaginationModelChange={onPageFilter}
          loading={loading}
          rowHeight={100}
          localeText={customLocaleText}
          sx={{
            '& .MuiDataGrid-cell:hover, .MuiChip-root:hover': {
              cursor: 'pointer',
            },
          }}
        />
      </Box>

      {/* Unlock Password Dialog */}
      <ConfirmDialog
        open={openUnlockDialog}
        title='Confirm UnLock Account'
        contentText='Are you sure you want to unlock this account?'
        onClose={handleCloseUnlockDialog}
        onConfirm={handleConfirmUnLock}
        isProcessing={isProcessing}
        confirmText='Confirm'
      />

      {/* Send Email Reset Password Dialog */}
      <ConfirmDialog
        open={openSendEmailResetPwdDialog}
        title='Confirm Send Email Reset Password Account'
        contentText='Are you sure you want to Send Email Reset Password this account?'
        onClose={handleCloseSendEmailResetPwdDialog}
        onConfirm={handleConfirmSendEmailResetPwd}
        isProcessing={isProcessing}
        confirmText='Confirm'
      />

      {/* Send Email Reset PIN Dialog */}
      <ConfirmDialog
        open={openSendEmailResetPINDialog}
        title='Confirm Send Email Reset PIN Account'
        contentText='Are you sure you want to Send Email Reset PIN this account?'
        onClose={handleCloseSendEmailResetPINDialog}
        onConfirm={handleConfirmSendEmailResetPIN}
        isProcessing={isProcessing}
        confirmText='Confirm'
      />

      {/* Change Password Dialog */}
      <InputDialog
        open={openChangePasswordDialog}
        title='Change Password'
        label='New Password'
        contentText={`Enter a new password for the user: ${selectedRow?.username || ''}`}
        value={newPassword}
        onChange={val => setNewPassword(val)}
        onClose={handleCloseChangePasswordDialog}
        onConfirm={handleConfirmChangePassword}
        isProcessing={isProcessing}
        disabledCondition={passwordIsNotValid}
        confirmText='Confirm'
        canToggleVisibility={true} // เปิดให้ toggle visibility ได้
      />

      {/* Change Pin Dialog */}
      <InputDialog
        open={openChangePinDialog}
        title='Change PIN'
        label='New PIN'
        contentText={`Enter a new PIN for user: ${selectedRow?.username || ''}`}
        value={newPin}
        onChange={val => setNewPin(val)}
        onClose={handleCloseChangePinDialog}
        onConfirm={handleConfirmChangePin}
        isProcessing={isProcessing}
        disabledCondition={pinIsNotValid}
        confirmText='Confirm'
        canToggleVisibility={true} // เปิดให้ toggle visibility ได้
      />

      {/* Unlock Pin Dialog */}
      <ConfirmDialog
        open={openUnlockPinDialog}
        title='Confirm UnLock Pin'
        contentText='Are you sure you want to unlock this account?'
        onClose={handleCloseUnlockPinDialog}
        onConfirm={handleConfirmUnLockPin}
        isProcessing={isProcessing}
        confirmText='Confirm'
      />
    </>
  )
}

export default DataTable
