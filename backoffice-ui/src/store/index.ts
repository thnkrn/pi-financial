// ** Toolkit imports
import { configureStore } from '@reduxjs/toolkit'

// ** Reducers
import btCalculator from './apps/blocktrade/calculator'
import btCustomerList from './apps/blocktrade/customer-list'
import btDropdown from './apps/blocktrade/dropdown'
import btEquity from './apps/blocktrade/equity'
import btFutures from './apps/blocktrade/futures'
import btLog from './apps/blocktrade/log'
import btMarket from './apps/blocktrade/market'
import btMonitor from './apps/blocktrade/monitor'
import btOrder from './apps/blocktrade/order'
import btPosition from './apps/blocktrade/position'
import btSymbolList from './apps/blocktrade/symbol-list'
import btUser from './apps/blocktrade/user'
import centralWorkspace from './apps/central-workspace'
import curatedFilter from './apps/curated-manager/filter'
import curatedList from './apps/curated-manager/list'
import curatedMember from './apps/curated-manager/member'
import deposit from './apps/deposit'
import dropdown from './apps/dropdown'
import report from './apps/report'
import transferCash from './apps/transfer-cash'
import withdraw from './apps/withdraw'

export const store = configureStore({
  reducer: {
    deposit: deposit,
    withdraw: withdraw,
    transferCash: transferCash,
    report: report,
    centralWorkspace: centralWorkspace,
    dropdown: dropdown,
    btEquity: btEquity,
    btOrder: btOrder,
    btMarket: btMarket,
    btLog: btLog,
    btPosition: btPosition,
    btUser: btUser,
    btDropdown: btDropdown,
    btSymbolList: btSymbolList,
    btCustomerList: btCustomerList,
    btFutures: btFutures,
    btCalculator: btCalculator,
    btMonitor: btMonitor,
    curatedList: curatedList,
    curatedFilter: curatedFilter,
    curatedMember: curatedMember,
  },
  middleware: getDefaultMiddleware =>
    getDefaultMiddleware({
      serializableCheck: false,
    }),
})

export type AppDispatch = typeof store.dispatch
export type RootState = ReturnType<typeof store.getState>
