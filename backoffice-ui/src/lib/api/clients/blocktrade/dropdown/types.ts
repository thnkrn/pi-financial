export interface IGetDropdownRequest {
  field: string
  url: string
}

export interface IGetDropdownResponse {
  field: string
  data:
    | IGetTeamsResponse[]
    | IGetMemberInfoFromGroupsResponse[]
    | IGetCloseCustomersResponse[]
    | ISeriesGetListResponse[]
    | IGetCloseSeriesResponse[]
}

export interface IGetTeamsResponse {
  id: number
  name: string
  members: number[]
  active: boolean
  createdAt: string
  updatedAt: string
}

export interface IGetMemberInfoFromGroupsResponse {
  key: number
  value: string
  extension: number
  broker: number
}

export interface IGetCloseCustomersResponse {
  customerAccount: string
}

export interface ISeriesGetListResponse {
  id: number
  series: string
  expDate: string
  createdAt: string | null
  updatedAt: string | null
}

export interface IGetCloseSeriesResponse {
  series: string
}
