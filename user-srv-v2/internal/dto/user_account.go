package dto

import (
	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type UserAccountByCustomerCodeParam struct {
	CustomerCode string `query:"customerCode"`
}

type GetUserAccountsByFiltersParam struct {
	UserId    uuid.UUID `query:"userId"`
	CitizenId string    `query:"citizenId"`
}

type LinkUserAccountRequest struct {
	UserAccountId   string                   `json:"userAccountId" validate:"required"`
	UserAccountType domain.UserAccountType   `json:"userAccountType" validate:"required,oneof=CashWallet Freewill"`
	Status          domain.UserAccountStatus `json:"status" validate:"oneof=N C"`
}

type UserAccountResponse struct {
	UserAccountId   string                   `json:"userAccountId"`
	UserAccountType domain.UserAccountType   `json:"userAccountType"`
	Status          domain.UserAccountStatus `json:"status"`
}

type CreateUserInfoRequest struct {
	Email       string `json:"email" validate:"omitempty,email"`
	PhoneNumber string `json:"phoneNumber"`
	CitizenId   string `json:"citizenId"`
	FirstnameTh string `json:"firstnameTh"`
	LastnameTh  string `json:"lastnameTh"`
	FirstnameEn string `json:"firstnameEn"`
	LastnameEn  string `json:"lastnameEn"`
	DateOfBirth string `json:"dateOfBirth" validate:"omitempty,datetime=2006-01-02"`
	WealthType  string `json:"wealthType"`
}

type CreateUserInfoResponse struct {
	Id string `json:"id"`
}

type GetUserAccountByMarketingIdParams struct {
	MarketingId string `param:"marketingId"`
}

type GetUserAccountByMarketingIdResponse struct {
	UserId       string `json:"userId"`
	FirstnameTh  string `json:"firstnameTh"`
	LastnameTh   string `json:"lastnameTh"`
	FirstnameEn  string `json:"firstnameEn"`
	LastnameEn   string `json:"lastnameEn"`
	CustomerCode string `json:"customerCode"`
}

type GetCustomerInfoByAccountIdParams struct {
	AccountId string `param:"accountId"`
}

type GetCustomerInfoByAccountIdResponse struct {
	CustomerType    string `json:"customerType"`
	CustomerSubType string `json:"customerSubType"`
}
