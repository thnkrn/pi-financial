export type IGetCustomerListResponse = ICustomer[]

export type ICustomer = {
  id: number | null
  customerName: string | null
  minDays: number | null
  interestRate: number | null
  icId: number | null
  commissionRate: string | null
  accountNo: string | null
  createdAt: string | null
  updatedAt: string | null
  limitPerOrder: number | null
  ic: IIC | null
}

type IIC = {
  id: number
  role: string
  name: string
  employeeId: number
  brokerId: number
  contact: string | null
  verified: boolean
}
