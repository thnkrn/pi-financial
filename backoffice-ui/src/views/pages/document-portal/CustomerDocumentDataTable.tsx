import QuickViewModal from '@/views/components/QuickViewModal'
import { makeStyles } from '@mui/styles'
import { DataGrid, GridCellParams } from '@mui/x-data-grid'
import JSZip from 'jszip'
import useTranslation from 'next-translate/useTranslation'
import { useState } from 'react'
import Swal from 'sweetalert2'
import { columnsConfig } from './CustomerColumns'
import CustomerDetail from './CustomerDetail'
import { AttachmentData, CustomerDetailType, DocumentPortalRowType } from './types'

const initialState: CustomerDetailType = {
  open: false,
  data: {} as DocumentPortalRowType,
}

const useStyles = makeStyles(() => ({
  customSwalModal: {
    zIndex: 1400,
  },
}))

interface CustomerDocumentDataTableProps {
  rows: DocumentPortalRowType[]
  loading: boolean
}

const CustomerDocumentDataTable = ({ rows, loading }: CustomerDocumentDataTableProps) => {
  const [{ open, data }, setCustomerModal] = useState<CustomerDetailType>(initialState)
  const [isDownloading, setIsDownloading] = useState<boolean>(false)
  const { t } = useTranslation('document_portal')

  const classes = useStyles()

  const getTypeFromBlob = (string: string) => {
    const regex = /\/([^/]+)$/
    const match = RegExp(regex).exec(string)

    if (match) {
      return match[1]
    } else {
      return null
    }
  }

  const handleDownloadAttachments = async (attachmentData: AttachmentData[], custCode: string | null) => {
    const zip = new JSZip()

    setIsDownloading(true)

    const downloadPromises = attachmentData.map(data =>
      fetch(data?.url as string, {
        method: 'GET',
        headers: {
          'Cache-Control': 'no-cache',
        },
      })
        .then(response => {
          if (!response.ok) {
            throw new Error(`Failed to download file: ${data.url}`)
          }

          return response.blob()
        })
        .then(blobData => {
          const fileNameHasExt = data?.fileName && data.fileName.includes('.')
          const urlObject = new URL(data?.url as string)
          const pathname = urlObject.pathname
          const segments = pathname.split('/')
          const fileNameFromURL = `${segments[segments.length - 1]}.${getTypeFromBlob(blobData.type)}`

          return { fileName: fileNameHasExt ? data?.fileName : fileNameFromURL, blobData }
        })
    )

    Promise.all(downloadPromises)
      .then(fileDataArray => {
        fileDataArray.forEach(fileData => {
          const { fileName, blobData } = fileData
          zip.file(fileName as string, blobData)
        })

        return zip.generateAsync({ type: 'blob' })
      })
      .then(blob => {
        const link = document.createElement('a')
        link.href = URL.createObjectURL(blob)
        link.download = custCode ? `downloaded_files_${custCode}.zip` : `downloaded_files.zip`
        link.click()

        setIsDownloading(false)
      })
      .catch(error => {
        setIsDownloading(false)

        Swal.fire({
          title: 'Error!',
          text: `${error.message}`,
          icon: 'error',
          confirmButtonText: 'OK',
          confirmButtonColor: '#21CE99',
          customClass: {
            container: classes.customSwalModal,
          },
        })
      })
  }

  const columns = columnsConfig(t, handleDownloadAttachments, isDownloading)

  const handleModalClose = () => setCustomerModal(initialState)

  const onViewCustomerDetail = (params: GridCellParams) => {
    if (params.field !== 'fileDownloadButton') {
      setCustomerModal({
        open: true,
        data: params.row,
      })
    }
  }

  const customLocaleText = {
    noRowsLabel: t('DOCUMENT_PORTAL_DATA_GRID_TEXT', {}, { default: 'Use the search above to see results' }),
  }

  return (
    <>
      <DataGrid
        disableColumnFilter
        rows={rows}
        autoHeight
        loading={loading}
        columns={columns}
        getRowId={row => `${row.id}`}
        localeText={customLocaleText}
        sx={{
          '& .MuiDataGrid-cell:hover, .MuiChip-root:hover': {
            cursor: 'pointer',
          },
        }}
        pageSizeOptions={[10, 20]}
        disableRowSelectionOnClick
        onCellClick={onViewCustomerDetail}
        hideFooter
      />

      {open && (
        <QuickViewModal
          maxWidth='md'
          title={t('DOCUMENT_PORTAL_QUICK_VIEW', {}, { default: 'Quick view' })}
          open={open}
          onClose={handleModalClose}
        >
          <CustomerDetail data={data} action={{ handleDownloadAttachments }} downloading={isDownloading} />
        </QuickViewModal>
      )}
    </>
  )
}

export default CustomerDocumentDataTable
