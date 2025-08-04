package dto

import "github.com/pi-financial/user-srv-v2/internal/utils"

type MigrateUserInfo struct {
	FirstnameTh         string `json:"firstnameTh"`
	LastnameTh          string `json:"lastnameTh"`
	FirstnameEn         string `json:"firstnameEn"`
	LastnameEn          string `json:"lastnameEn"`
	Email               string `json:"email"`
	PhoneNumber         string `json:"phoneNumber"`
	DateOfBirth         string `json:"dateOfBirth"`
	PlaceOfBirthCountry string `json:"placeOfBirthCountry"`
	PlaceOfBirthCity    string `json:"placeOfBirthCity"`
	WealthType          string `json:"wealthType"`
}

type MigrateUserKyc struct {
	ReviewDate  utils.DateOnly `json:"reviewDate"`
	ExpiredDate utils.DateOnly `json:"expiredDate"`
}

type MigrateUserExternalAccount struct {
	Id         string `json:"id"`
	Value      string `json:"value"`
	ProviderId int    `json:"providerId"`
}

type MigrateUserTradeAccount struct {
	AccountNumber      string                       `json:"accountNumber"`
	AccountType        string                       `json:"accountType"`
	AccountTypeCode    string                       `json:"accountTypeCode"`
	ExchangeMarketId   string                       `json:"exchangeMarketId"`
	AccountStatus      string                       `json:"accountStatus"`
	CreditLine         float64                      `json:"creditLine"`
	CreditLineCurrency string                       `json:"creditLineCurrency"`
	EffectiveDate      utils.DateOnly               `json:"effectiveDate"`
	EndDate            utils.DateOnly               `json:"endDate"`
	MarketingId        string                       `json:"marketingId"`
	SaleLicense        string                       `json:"saleLicense"`
	OpenDate           utils.DateOnly               `json:"openDate"`
	ExternalAccount    []MigrateUserExternalAccount `json:"externalAccount"`
	FrontName          string                       `json:"frontName"`
	EnableBuy          string                       `json:"enableBuy"`
	EnableSell         string                       `json:"enableSell"`
	EnableDeposit      string                       `json:"enableDeposit"`
	EnableWithdraw     string                       `json:"enableWithdraw"`
}

type MigrateUserBankAccount struct {
	AccountNo        string         `json:"accountNo"`
	AccountName      string         `json:"accountNme"`
	BankCode         string         `json:"bankCode"`
	BranchCode       string         `json:"branchCode"`
	PaymentToken     string         `json:"paymentToken"`
	AtsEffectiveDate utils.DateOnly `json:"atsEffectiveDate" type:"date"`
	Status           string         `json:"status"`
}

type MigrateUserRequest struct {
	TradeAccountBankAccounts []struct {
		CustomerCode string                    `json:"customerCode"`
		TradeAccount []MigrateUserTradeAccount `json:"tradeAccount"`
	} `json:"tradeAccountBankAccounts"`
	Address struct {
		Place        string `json:"place"`
		HomeNo       string `json:"homeNo"`
		Town         string `json:"town"`
		Building     string `json:"building"`
		Village      string `json:"village"`
		Floor        string `json:"floor"`
		Soi          string `json:"soi"`
		Road         string `json:"road"`
		SubDistrict  string `json:"subDistrict"`
		District     string `json:"district"`
		Province     string `json:"province"`
		Country      string `json:"country"`
		ZipCode      string `json:"zipCode"`
		CountryCode  string `json:"countryCode"`
		ProvinceCode string `json:"provinceCode"`
	} `json:"address"`
	SuitabilityTests []struct {
		Score       string         `json:"score"`
		Grade       string         `json:"grade"`
		Version     string         `json:"version"`
		ReviewDate  utils.DateOnly `json:"reviewDate"`
		ExpiredDate utils.DateOnly `json:"expiredDate"`
	} `json:"suitabilityTests"`
	UserInfo MigrateUserInfo `json:"userInfo"`
	Kyc      MigrateUserKyc  `json:"kyc"`
}

type MigrateUserResponse struct {
}
