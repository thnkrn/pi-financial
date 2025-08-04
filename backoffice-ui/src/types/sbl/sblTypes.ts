export enum ACTION_TYPE {
  APPROVED = 'approved',
  REJECTED = 'rejected',
}

export interface IAction {
  orderId: string
  orderNo: string
  type: ACTION_TYPE
}

export interface IPaginationModel {
  page: number
  pageSize: number
}
