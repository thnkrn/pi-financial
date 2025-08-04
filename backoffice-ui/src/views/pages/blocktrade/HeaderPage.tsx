import { Box, CardHeader, Hidden, Typography, IconButton } from '@mui/material'
import RefreshIcon from '@mui/icons-material/Refresh'
import SelectOptionPre from '@/views/pages/blocktrade/SelectOptionPre'

type HeaderPageProps = {
  title: string
  lastUpdated: string
  handleRefresh: () => void
  updateFilter: (filterValue: any) => void
  userValue: number
}

const HeaderPage = (props: HeaderPageProps) => {
  return (
    <>
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          paddingRight: 5,
        }}
      >
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'flex-start',
            alignItems: 'center',
          }}
        >
          <CardHeader title={props.title} />
          <Hidden lgDown>
            <Box sx={{ width: '300px' }}>
              <SelectOptionPre
                id={'accountList'}
                labelId={'accountList'}
                label={''}
                defaultValue={props.userValue}
                remote={{
                  field: 'accountList',
                  url: 'users/getMemberInfoFromGroups?dropdown=true',
                  key: 'key',
                  value: 'value',
                  extension: 'extension',
                }}
                onChange={key => props.updateFilter({ ofUser: key })}
              />
            </Box>
          </Hidden>
        </Box>
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'flex-start',
            alignItems: 'center',
          }}
        >
          <Typography>Last Update : {props.lastUpdated}</Typography>
          <IconButton color='inherit' aria-haspopup='true' onClick={props.handleRefresh} aria-controls='refresh-menu'>
            <RefreshIcon />
          </IconButton>
        </Box>
      </Box>
      <Hidden lgUp>
        <Box sx={{ marginX: 5, marginBottom: 2 }}>
          <SelectOptionPre
            id={'accountList'}
            labelId={'accountList'}
            label={''}
            defaultValue={props.userValue}
            remote={{
              field: 'accountList',
              url: 'users/getMemberInfoFromGroups?dropdown=true',
              key: 'key',
              value: 'value',
              extension: 'extension',
            }}
            onChange={key => props.updateFilter({ ofUser: key })}
          />
        </Box>
      </Hidden>
    </>
  )
}

export default HeaderPage
