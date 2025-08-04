import { FormControl, InputLabel, MenuItem, Select, SelectChangeEvent } from '@mui/material'
import useTranslation from 'next-translate/useTranslation'

interface DocumentTypeSelectProps {
  documentType: string
  handleDocumentTypeChange: (event: SelectChangeEvent<string>) => void
  documentTypeSelectOptions: Record<string, string>
}
const DocumentTypeSelect = ({
  documentType,
  handleDocumentTypeChange,
  documentTypeSelectOptions,
}: DocumentTypeSelectProps) => {
  const { t } = useTranslation('common')

  return (
    <FormControl sx={{ marginBottom: '30px', width: '100%' }} size='small' required>
      <InputLabel>{t('DOCUMENT_TYPE', {}, { default: 'Document type' })}</InputLabel>
      <Select
        fullWidth
        id='ocr-document-type-select'
        value={documentType}
        label='Document Type'
        onChange={handleDocumentTypeChange}
      >
        {Object.entries(documentTypeSelectOptions).map(([value, label]) => (
          <MenuItem key={value} value={value}>
            {label}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  )
}

export default DocumentTypeSelect
