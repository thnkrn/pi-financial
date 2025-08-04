import { CloseOutlined, EditOutlined, SaveOutlined } from '@mui/icons-material'
import { IconButton, TextField } from '@mui/material'
import { memo, useCallback, useMemo, useState } from 'react'

interface InlineEditProps {
  value: string
  field: string
  rowId: string
  onSave: (id: string, field: string, newValue: string) => Promise<void>
}

const InlineEdit = ({ value = '', field, rowId, onSave }: InlineEditProps) => {
  const [isEditing, setIsEditing] = useState(false)
  const [editValue, setEditValue] = useState(value)

  const hasChanged = useMemo(() => editValue !== value, [editValue, value])

  const handleEdit = useCallback(() => {
    setIsEditing(true)
    setEditValue(value || '')
  }, [value])

  const handleSave = useCallback(async () => {
    if (hasChanged) {
      await onSave(rowId, field, editValue)
    }
    setIsEditing(false)
  }, [hasChanged, onSave, rowId, field, editValue])

  const handleCancel = useCallback(() => {
    setEditValue(value || '')
    setIsEditing(false)
  }, [value])

  const handleKeyPress = useCallback(
    (e: React.KeyboardEvent) => {
      if (e.key === 'Enter') {
        handleSave()
      } else if (e.key === 'Escape') {
        handleCancel()
      }

      e.stopPropagation()
    },
    [handleSave, handleCancel]
  )

  if (isEditing) {
    return (
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px', width: '100%' }}>
        <TextField
          autoFocus
          size='small'
          value={editValue}
          onChange={e => setEditValue(e.target.value)}
          onKeyDown={handleKeyPress}
          variant='outlined'
          fullWidth
        />
        <IconButton size='small' onClick={handleSave} color='primary' disabled={!hasChanged}>
          <SaveOutlined sx={{ fontSize: 16 }} />
        </IconButton>
        <IconButton size='small' onClick={handleCancel} color='error'>
          <CloseOutlined sx={{ fontSize: 16 }} />
        </IconButton>
      </div>
    )
  }

  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
      <span>{value}</span>
      <IconButton color='primary' size='small' onClick={handleEdit}>
        <EditOutlined sx={{ fontSize: 16 }} />
      </IconButton>
    </div>
  )
}

export default memo(InlineEdit)
