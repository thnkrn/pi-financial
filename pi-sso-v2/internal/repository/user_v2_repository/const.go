package user_v2_repository

type SyncType string

const (
	SyncTypeUser        SyncType = "user"
	SyncTypeKyc         SyncType = "kyc"
	SyncTypeSuitTest    SyncType = "suitTest"
	SyncTypeAddress     SyncType = "address"
	SyncTypeTradingAcct SyncType = "tradingAccount"
	SyncTypeAll         SyncType = "all"
)
