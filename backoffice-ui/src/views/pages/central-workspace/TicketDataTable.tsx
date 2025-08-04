import { IGetTicketsRequest } from '@/lib/api/clients/backoffice/central-workspace/types'
import { fetchTickets, updateFilterState } from '@/store/apps/central-workspace'
import Card from '@mui/material/Card'
import { DataGrid } from '@mui/x-data-grid'
import { PiBackofficeServiceAPIModelsTicketDetailResponse } from '@pi-financial/backoffice-srv/src/models/PiBackofficeServiceAPIModelsTicketDetailResponse'
import { ThunkDispatch } from '@reduxjs/toolkit'
import extend from 'lodash/extend'
import { useEffect, useRef, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { RootState } from 'src/store'
import { columns } from './TicketColumns'

const TicketDataTable = () => {
  const [total, setTotal] = useState<number>(1)
  const [rows, setRows] = useState<PiBackofficeServiceAPIModelsTicketDetailResponse[]>([])
  const [paginationModel, setPaginationModel] = useState({ page: 0, pageSize: 10 })

  const store = useSelector((state: RootState) => state.centralWorkspace)
  const dispatch = useDispatch<ThunkDispatch<any, any, any>>()
  const initialRender = useRef(true)

  useEffect(() => {
    if (initialRender.current) {
      initialRender.current = false
    } else {
      const currentFilter: IGetTicketsRequest = extend(
        {},
        { ...store.filter },
        {
          page: paginationModel.page + 1,
          pageSize: paginationModel.pageSize,
          orderBy: store.filter.orderBy,
          orderDir: store.filter.orderDir,
        }
      )

      dispatch(updateFilterState(currentFilter))

      dispatch(fetchTickets(currentFilter))
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paginationModel])

  useEffect(() => {
    setRows(store.tickets)
    setTotal(store.filter.total ?? 0)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store.tickets])

  return (
    <div>
      <Card sx={{ marginTop: '40px' }}>
        <DataGrid
          autoHeight
          pagination
          rows={rows}
          rowCount={total}
          columns={columns}
          getRowId={row => `${row.correlationId}_${row.transactionId}`}
          paginationMode='server'
          pageSizeOptions={[10, 20, 50, 100]}
          paginationModel={paginationModel}
          onPaginationModelChange={setPaginationModel}
          initialState={{
            sorting: {
              sortModel: [
                {
                  field: 'createdAt',
                  sort: 'desc',
                },
              ],
            },
          }}
        />
      </Card>
    </div>
  )
}

export default TicketDataTable
