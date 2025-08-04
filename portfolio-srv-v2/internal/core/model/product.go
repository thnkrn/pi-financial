package model

type ProductType string

const (
	Derivatives          ProductType = "Derivatives"
	CashBalance          ProductType = "CashBalance"
	Cash                 ProductType = "Cash"
	CashBalanceSbl       ProductType = "CashBalanceSbl"
	Crypto               ProductType = "Crypto"
	GlobalEquities       ProductType = "GlobalEquities"
	Funds                ProductType = "Funds"
	Bond                 ProductType = "Bond"
	CashSbl              ProductType = "CashSbl"
	CreditBalanceSbl     ProductType = "CreditBalanceSbl"
	CreditBalance        ProductType = "CreditBalance"
	StructureNoteOnShore ProductType = "StructureNoteOnShore"
	Drx                  ProductType = "Drx"
	LiveX                ProductType = "LiveX"
	BorrowCash           ProductType = "BorrowCash"
	BorrowCashBalance    ProductType = "BorrowCashBalance"
	Unknown              ProductType = "Unknown"
)
