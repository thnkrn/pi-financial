import { IEquityOrderRequest } from '@/lib/api/clients/blocktrade/equity/types'
import { IGetMyUserDataResponse } from '@/lib/api/clients/blocktrade/users/types'
import { IGetSymbolListResponse } from '@/lib/api/clients/blocktrade/symbol/types'
import { IGetCustomerListResponse } from '@/lib/api/clients/blocktrade/customers/types'
import { IOrderBookResponse } from '@/lib/api/clients/blocktrade/market/types'

export const PositionInitialState = {
  pnl: true,
  page: 1,
  pageSize: 100,
  orderBy: 'CreatedAt',
  orderDir: 'DESC',
}

export const ActivityLogsInitialState = {
  page: 1,
  pageSize: 20,
  orderBy: 'CreatedAt',
  orderDir: 'DESC',
}

export const CustomerListInitialState: IGetCustomerListResponse = [
  {
    id: null,
    customerName: null,
    minDays: null,
    interestRate: null,
    icId: null,
    commissionRate: null,
    accountNo: null,
    createdAt: null,
    updatedAt: null,
    limitPerOrder: null,
    ic: null,
  },
]

export const SymbolListInitialState: IGetSymbolListResponse = [
  {
    id: null,
    symbolId: null,
    symbol: null,
    seriesId: null,
    series: null,
    blocksize: null,
    mm: null,
    multiplier: null,
    expDate: null,
    createdAt: null,
    updatedAt: null,
  },
]

export const UserInitialState: IGetMyUserDataResponse = {
  id: null,
  keycloakId: null,
  role: null,
  name: null,
  employeeId: null,
  brokerId: null,
  contact: null,
  verified: null,
  createdAt: null,
  updatedAt: null,
  lineToken: null,
}

export const FuturesOrderFilterInitialState = {
  page: 1,
  pageSize: 200,
  orderBy: 'createdAt',
  orderDir: 'desc',
}

export const EquityOrderFilterInitialState: IEquityOrderRequest = {
  page: 1,
  pageSize: 200,
  orderBy: 'createdAt',
  orderDir: 'desc',
}

export const MarketDataInitialState: IOrderBookResponse = {
  symbolInfo: {
    symbol: null,
    lastPrice: null,
    high: null,
    low: null,
    previousClose: null,
    lastOpen: null,
    floor: null,
    celling: null,
    timestamp: null,
  },
  orderBook: [
    {
      id: 1,
      bidVolume: null,
      bidPrice: null,
      askPrice: null,
      askVolume: null,
    },
    {
      id: 2,
      bidVolume: null,
      bidPrice: null,
      askPrice: null,
      askVolume: null,
    },
    {
      id: 3,
      bidVolume: null,
      bidPrice: null,
      askPrice: null,
      askVolume: null,
    },
    {
      id: 4,
      bidVolume: null,
      bidPrice: null,
      askPrice: null,
      askVolume: null,
    },
    {
      id: 5,
      bidVolume: null,
      bidPrice: null,
      askPrice: null,
      askVolume: null,
    },
  ],
}
