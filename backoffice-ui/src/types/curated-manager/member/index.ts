export interface CuratedMemberItem {
  id: string
  logo: string
  symbol: string
  friendlyName: string
  figi: string
  units: string
  exchange: string
  dataVenderCode: string
  dataVenderCode2: string
}

export interface CuratedMembersResponse {
  data: CuratedMemberItem[]
}

export interface CuratedMemberState {
  isLoading: boolean
  errorMessage: string
  members: CuratedMemberItem[]
}
