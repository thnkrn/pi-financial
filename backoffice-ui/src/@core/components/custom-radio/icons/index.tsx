import Content from '@/@core/components/content'
import { CustomRadioIconsProps } from '@/@core/components/custom-radio/types'
import Icon from '@/@core/components/icon'
import Title from '@/@core/components/title'
import { getBorderHover } from '@/@core/utils/styles'
import Box from '@mui/material/Box'
import Grid from '@mui/material/Grid'
import Radio from '@mui/material/Radio'

const CustomRadioIcons = (props: CustomRadioIconsProps) => {
  const { data, icon, name, selected, gridProps, iconProps, handleChange, color = 'primary' } = props

  const { title, value, content } = data

  const renderComponent = () => {
    return (
      <Grid item {...gridProps}>
        <Box
          onClick={() => handleChange(value)}
          sx={{
            p: 4,
            height: '100%',
            display: 'flex',
            borderRadius: 1,
            cursor: 'pointer',
            position: 'relative',
            alignItems: 'center',
            flexDirection: 'column',
            border: theme => `1px solid ${theme.palette.divider}`,
            ...(selected === value ? { borderColor: `${color}.main` } : getBorderHover()),
          }}
        >
          {icon && <Icon icon={icon} {...iconProps} />}
          {title && <Title title={title} content={content} />}
          {content && <Content content={content} />}
          <Radio
            name={name}
            size='small'
            color={color}
            value={value}
            onChange={handleChange}
            checked={selected === value}
            sx={{ mb: -2, ...(!icon && !title && !content && { mt: -2 }) }}
          />
        </Box>
      </Grid>
    )
  }

  return data ? renderComponent() : null
}

export default CustomRadioIcons
