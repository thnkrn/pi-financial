export enum ACTION_AFFINITY {
  POSITIVE = 'positive',
  NEGATIVE = 'negative',
}
export const RESPONSE_ACTIONS = [
  {
    name: 'Approve',
    alias: 'Approve to Profile',
    affinity: ACTION_AFFINITY.POSITIVE,
  },
  {
    name: 'ChangeStatusToSuccess',
    alias: 'Change status to success',
    affinity: ACTION_AFFINITY.POSITIVE,
  },
  {
    name: 'ChangeSetTradeStatusToSuccess',
    alias: 'Change Settrade status to success',
    affinity: ACTION_AFFINITY.POSITIVE,
  },
  {
    name: 'UpdateBillPaymentReference',
    alias: 'Edit Account No and Approve',
    affinity: ACTION_AFFINITY.POSITIVE,
  },
  {
    name: 'Refund',
    alias: 'Refund to Customer',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
  {
    name: 'ChangeStatusToFail',
    alias: 'Change status to fail',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
  {
    name: 'ChangeSetTradeStatusToFail',
    alias: 'Change Settrade status to fail',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
  {
    name: 'DepositKkpConfirm',
    alias: 'KKP Deposit Confirm',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
  {
    name: 'CcyAllocationTransfer',
    alias: 'CCY Allocation Transfer',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
  {
    name: 'SbaDepositConfirm',
    alias: 'SBA Deposit Confirm',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
  {
    name: 'SbaAllocationTransfer',
    alias: 'SBA Allocation Transfer',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
  {
    name: 'SbaDepositAtsCallbackConfirm',
    alias: 'SBA ATS Confirm',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
  {
    name: 'SetTradeAllocationTransfer',
    alias: 'Settrade Allocation Transfer',
    affinity: ACTION_AFFINITY.NEGATIVE,
  },
]
