import {
  clearSuccessAction,
  fetchCuratedList,
  resetState,
  setActiveTab,
  updateCuratedListState,
} from '@/store/apps/curated-manager/list'
import { AppDispatch, RootState } from '@/store/index'
import { ActionType, CuratedListState, CuratedType } from '@/types/curated-manager/list'
import ErrorModal from '@/views/components/ErrorModal'
import { Grid, Tabs } from '@mui/material'
import isEmpty from 'lodash/isEmpty'
import React, { useCallback, useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { StyledTab } from '../StyledTab'
import SuccessAlert from '../SuccessAlert'
import LogicalTabContent from './LogicalTab'
import ManualTabContent from './ManualTab'

const initialState: CuratedListState = {
  activeTab: CuratedType.Manual,
  isLoading: false,
  errorMessage: '',
  logicalList: [],
  manualList: [],
  successAction: ActionType.NONE,
}

const actionMessages = {
  [ActionType.UPLOAD]: 'Curated list uploaded successfully',
  [ActionType.UPDATE]: 'Curated list updated successfully',
  [ActionType.DELETE]: 'Curated list deleted successfully',
  [ActionType.NONE]: '',
}

const Index = () => {
  const [openSuccessAlert, setOpenSuccessAlert] = useState(false)
  const [successMessage, setSuccessMessage] = useState('')
  const dispatch = useDispatch<AppDispatch>()
  const store = useSelector((state: RootState) => state.curatedList)
  const successAction = useSelector((state: RootState) => state.curatedList.successAction)

  useEffect(() => {
    dispatch(updateCuratedListState(initialState))
    dispatch(fetchCuratedList())

    return () => {
      dispatch(updateCuratedListState(initialState))
      dispatch(resetState())
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const handleSuccessAlertClose = useCallback(() => {
    setOpenSuccessAlert(false)
    dispatch(clearSuccessAction())
  }, [dispatch])

  useEffect(() => {
    if (successAction !== ActionType.NONE) {
      setSuccessMessage(actionMessages[successAction])
      setOpenSuccessAlert(true)
    }
  }, [successAction])

  const handleTabChange = useCallback(
    (event: React.SyntheticEvent, newValue: CuratedType) => {
      dispatch(setActiveTab(newValue))
    },
    [dispatch]
  )

  return (
    <Grid container spacing={6}>
      <Grid item xs={12}>
        <Tabs
          value={store.activeTab}
          onChange={handleTabChange}
          sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}
        >
          <StyledTab label='Logical' value={CuratedType.Logical} />
          <StyledTab label='Manual' value={CuratedType.Manual} />
        </Tabs>

        {store.activeTab === CuratedType.Logical ? <LogicalTabContent /> : <ManualTabContent />}
      </Grid>
      <ErrorModal
        isError={!isEmpty(store.errorMessage)}
        errorMessage={store.errorMessage}
        dependencies={[store.errorMessage]}
      />

      <SuccessAlert open={openSuccessAlert} onClose={handleSuccessAlertClose} message={successMessage} />
    </Grid>
  )
}

export default Index
