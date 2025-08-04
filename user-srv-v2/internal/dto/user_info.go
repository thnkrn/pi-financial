package dto

import (
	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type UserInfo struct {
	Id                  string   `json:"id"`
	Devices             []Device `json:"devices"`
	CustCodes           []string `json:"custCodes"`
	TradingAccounts     []string `json:"tradingAccounts"`
	FirstnameTh         string   `json:"firstnameTh"`
	LastnameTh          string   `json:"lastnameTh"`
	FirstnameEn         string   `json:"firstnameEn"`
	LastnameEn          string   `json:"lastnameEn"`
	PhoneNumber         string   `json:"phoneNumber"`
	Email               string   `json:"email"`
	CitizenId           string   `json:"citizenId,omitempty"`
	DateOfBirth         string   `json:"dateOfBirth"` // 2024-12-27
	WealthType          string   `json:"wealthType"`
	PlaceOfBirthCountry string   `json:"placeOfBirthCountry"`
	PlaceOfBirthCity    string   `json:"placeOfBirthCity"`
}

func (u *UserInfo) ExcludeCitizenId() *UserInfo {
	if u == nil {
		return nil
	}
	u.CitizenId = ""
	return u
}

type GetUserInfoByFiltersRequest struct {
	Ids         string `query:"ids"`
	AccountId   string `query:"accountId"`
	CitizenId   string `query:"citizenId"`
	PhoneNumber string `query:"phoneNumber"`
	Email       string `query:"email"`
	FirstName   string `query:"firstName"`
	LastName    string `query:"lastName"`
}

type UserInfoQueryFilter struct {
	Ids         string
	AccountId   string
	UserId      uuid.UUID
	CitizenId   string
	Email       string
	PhoneNumber string
	FirstName   string
	LastName    string
}

type PatchUserInfoRequest struct {
	CitizenId           string `json:"citizenId"`
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

type SyncUserInfoType string

const (
	SyncUserInfoTypeKyc            SyncUserInfoType = "kyc"
	SyncUserInfoTypeSuitTest       SyncUserInfoType = "suitTest"
	SyncUserInfoTypeAddress        SyncUserInfoType = "address"
	SyncUserInfoTypeTradingAccount SyncUserInfoType = "tradingAccount"
	SyncUserInfoTypeUserInfo       SyncUserInfoType = "userInfo"
	SyncUserInfoTypeAll            SyncUserInfoType = "all"
)

type SyncUserInfoParams struct {
	CustomerCode string           `query:"customerCode" validate:"required,len=7"`
	SyncType     SyncUserInfoType `query:"syncType" validate:"required,oneof=kyc suitTest address tradingAccount userInfo all"`
}

type ProfileInfo struct {
	PhoneNumber      string            `json:"phoneNumber"`
	Email            string            `json:"email"`
	ContactInfo      []ContactInfo     `json:"contactInfo"`
	Signature        string            `json:"signature"`
	KycInfo          *KycInfo          `json:"kycInfo"`
	CurrentAddress   *AddressInfo      `json:"currentAddress"`
	Occupation       *OccupationInfo   `json:"occupation"`
	WorkplaceAddress *AddressInfo      `json:"workplaceAddress"`
	Investment       *InvestmentInfo   `json:"investment"`
	Declaration      *DeclarationInfo  `json:"declaration"`
	SuitTest         *SuitTestInfo     `json:"suitTest"`
	InfoTypeStatus   []InfoTypeStatus  `json:"infoTypeStatus"`
	BankAccountInfo  []BankAccountInfo `json:"bankAccountInfo"`
}

type ContactInfo struct {
	CustomerCode           string `json:"customerCode"`
	DocumentRecipientEmail string `json:"documentRecipientEmail"`
}

type KycInfo struct {
	IdCard     *IdCardInfo `json:"idCard"`
	ReviewDate string      `json:"reviewDate"`
	Address    AddressInfo `json:"address"`
}

type IdCardInfo struct {
	CitizenId   string `json:"citizenId"`
	TitleTh     string `json:"titleTh"`
	TitleEn     string `json:"titleEn"`
	TitleOther  string `json:"titleOther"`
	FirstNameTh string `json:"firstNameTh"`
	LastNameTh  string `json:"lastNameTh"`
	FirstNameEn string `json:"firstNameEn"`
	LastNameEn  string `json:"lastNameEn"`
	DateOfBirth string `json:"dateOfBirth"`
	CardExpiry  string `json:"cardExpiry"`
	Image       string `json:"image"`
	LaserCode   string `json:"laserCode"`
}

type AddressInfo struct {
	HouseNo     string `json:"houseNo"`
	Village     string `json:"village"`
	Building    string `json:"building"`
	Moo         string `json:"moo"`
	Soi         string `json:"soi"`
	Road        string `json:"road"`
	SubDistrict string `json:"subDistrict"`
	District    string `json:"district"`
	Province    string `json:"province"`
	PostalCode  string `json:"postalCode"`
}

type OccupationInfo struct {
	Occupation        string `json:"occupation"`
	OccupationOther   string `json:"occupationOther"`
	BusinessType      string `json:"businessType"`
	BusinessTypeOther string `json:"businessTypeOther"`
	JobTitle          string `json:"jobTitle"`
	WorkplaceNameEn   string `json:"workplaceNameEn"`
	WorkplaceNameTh   string `json:"workplaceNameTh"`
}

type InvestmentInfo struct {
	Income              string   `json:"income"`
	SourceOfIncome      []string `json:"sourceOfIncome"`
	PurposeOfInvestment []string `json:"purposeOfInvestment"`
}

type DeclarationInfo struct {
	PoliticalFlag         bool `json:"politicalFlag"`
	LaunderFlag           bool `json:"launderFlag"`
	DeniedTransactionFlag bool `json:"deniedTransactionFlag"`
}

type SuitTestInfo struct {
	Score            string             `json:"score"`
	ScoreDescription string             `json:"scoreDescription"`
	LatestDate       string             `json:"latestDate"`
	Questions        []SuitTestQuestion `json:"questions"`
}

type SuitTestQuestion struct {
	QuestionCmsId string           `json:"questionCmsId"`
	QuestionCode  string           `json:"questionCode"`
	Answers       []SuitTestAnswer `json:"answers"`
}

type SuitTestAnswer struct {
	AnswerCmsId string `json:"answerCmsId"`
	AnswerCode  string `json:"answerCode"`
}

type InfoTypeStatus struct {
	InfoType domain.ChangeRequestInfoType `json:"infoType"`
	Status   domain.ChangeRequestStatus   `json:"status"`
	Note     string                       `json:"note"`
}

type BankAccountInfo struct {
	TradingAccountNo string `json:"tradingAccountNo"`
	BookBankImage    string `json:"bookBankImage"`
	BankName         string `json:"bankName"`
	BankLogo         string `json:"bankLogo"`
	BankCode         string `json:"bankCode"`
	BankBranchName   string `json:"bankBranchName"`
	AccountNo        string `json:"accountNo"`
	AccountName      string `json:"accountName"`
	EffectiveDate    string `json:"effectiveDate"`
	IsPrimary        bool   `json:"isPrimary"`
}
