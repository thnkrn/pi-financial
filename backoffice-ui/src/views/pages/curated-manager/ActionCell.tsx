import { DeleteOutlined, ViewListOutlined } from '@mui/icons-material'
import { IconButton, Tooltip } from '@mui/material'
import { useRouter } from 'next/router'
import { useState } from 'react'
import DeleteConfirmDialog from './DeleteConfirmDialog'

interface ActionCell {
  id: string
  curatedListId: number
  name: string
  onDelete: (id: string) => void
}

const ActionCell = ({ id, curatedListId, name, onDelete }: ActionCell) => {
  const [openDialog, setOpenDialog] = useState(false)
  const router = useRouter()

  const handleView = () => {
    router.push(`/curated-manager/members/${curatedListId}?name=${name}`)
  }

  return (
    <>
      <Tooltip title='View members'>
        <IconButton color='primary' size='medium' onClick={handleView}>
          <ViewListOutlined fontSize='medium' />
        </IconButton>
      </Tooltip>

      <Tooltip title='Delete'>
        <IconButton
          color='primary'
          size='medium'
          onClick={e => {
            e.stopPropagation()
            setOpenDialog(true)
          }}
        >
          <DeleteOutlined fontSize='medium' />
        </IconButton>
      </Tooltip>

      <DeleteConfirmDialog
        open={openDialog}
        onClose={() => setOpenDialog(false)}
        onConfirm={() => {
          onDelete(id)
          setOpenDialog(false)
        }}
        itemName={name}
      />
    </>
  )
}

export default ActionCell
