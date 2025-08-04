export enum Side {
  LONG = 'LONG',
  SHORT = 'SHORT',
}

export enum OC {
  OPEN = 'OPEN',
  CLOSE = 'CLOSE',
}

export enum UserRoles {
  ADMIN = 'ADMIN',
  IC = 'IC',
  TL_IC = 'TL_IC',
}

export enum OrderType {
  LIMIT = 'LIMIT',
  ATO = 'ATO',
  ATC = 'ATC',
  MKT = 'MKT',
}

export enum JPSide {
  BUY = 'Buy',
  BUY_COVER = 'Buy Cover',
  SELL = 'Sell',
}

export enum OrderStatus {
  PROCESSING = 'PROCESSING',
  INITIAL_REQUEST = 'INITIAL_REQUEST',
  INITIAL_PENDING = 'INITIAL_PENDING',
  INITIAL_REJECTED = 'INITIAL_REJECTED',
  WORKING = 'WORKING',
  PARTIAL = 'PARTIAL',
  S_FILLED = 'S_FILLED',
  F_SENT = 'F_SENT',
  F_MATCHED = 'F_MATCHED',
  EDIT_REQUEST = 'EDIT_REQUEST',
  EDIT_REJECTED = 'EDIT_REJECTED',
  EDITED = 'EDITED',
  CANCEL_REQUEST = 'CANCEL_REQUEST',
  CANCEL_REJECTED = 'CANCEL_REJECTED',
  CANCELLED = 'CANCELLED',
  UNKNOWN = 'UNKNOWN',
}
