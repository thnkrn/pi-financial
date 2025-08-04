import { isURL } from '@/@core/utils/text-utilities'
import ArticleOutlinedIcon from '@mui/icons-material/ArticleOutlined'
import DownloadIcon from '@mui/icons-material/Download'
import { Typography } from '@mui/material'
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import useTranslation from 'next-translate/useTranslation'
import { AttachmentData, AttachmentsDataGridProps } from './types'

const AttachmentsDataGrid = ({ documents }: AttachmentsDataGridProps) => {
  const { t } = useTranslation('document_portal')

  const columns: GridColDef[] = [
    {
      field: 'documentType',
      headerName: t('DOCUMENT_PORTAL_DOCUMENT_TYPE', {}, { default: 'Document Type' }),
      sortable: false,
      filterable: false,
      width: 200,
      renderCell: (params: GridRenderCellParams) => (
        <>
          <ArticleOutlinedIcon sx={{ marginRight: '0.5rem' }} data-testid={'attachment-document-type'} />
          {params.row.documentType}
        </>
      ),
    },
    {
      field: 'fileUrl',
      headerName: 'URL',
      sortable: false,
      filterable: false,
      flex: 1,
      renderCell: (params: GridRenderCellParams) => (
        <Typography
          sx={{ whiteSpace: 'nowrap', textOverflow: 'ellipsis', overflow: 'hidden' }}
          data-testid={'attachment-file-url'}
        >
          {params.row.url}
        </Typography>
      ),
    },
    {
      field: 'downloadFile',
      headerName: '',
      sortable: false,
      filterable: false,
      renderCell: (params: GridRenderCellParams) =>
        isURL(params.row.url) && (
          <a href={params.row.url} target='_blank' rel='noopener noreferrer'>
            <DownloadIcon sx={{ color: 'text.primary' }} data-testid={'attachment-download-file'} />
          </a>
        ),
    },
  ]

  const rows = (documents as AttachmentData[]).map((attachment: AttachmentData, index: number) => ({
    ...attachment,
    id: index,
  }))

  return <DataGrid rows={rows} columns={columns} autoHeight disableRowSelectionOnClick hideFooter />
}

export default AttachmentsDataGrid
