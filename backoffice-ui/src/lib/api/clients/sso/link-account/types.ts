export interface ISendLinkAccountInfo {
    id: string
    email: string
    custcode: string
    userId: string
    createdAt: string
    usedAt: string | null
    isUsed: boolean
  }
  
  export interface ISSOResponse {
    code: string
    msg: string
    data: ISendLinkAccountInfo[] | null
  }
  export interface ISSOError {
    status: number
    title: string
    detail: string
  }