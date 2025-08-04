package client

type UserInfo struct {
	CitizenId       string           `json:"citizenId"`
	CustCodes       []string         `json:"custCodes"`
	DateOfBirth     string           `json:"dateOfBirth"` // 2024-12-27
	Devices         []UserInfoDevice `json:"devices"`
	Email           string           `json:"email"`
	FirstnameEn     string           `json:"firstnameEn"`
	FirstnameTh     string           `json:"firstnameTh"`
	Id              string           `json:"id"`
	LastnameEn      string           `json:"lastnameEn"`
	LastnameTh      string           `json:"lastnameTh"`
	PhoneNumber     string           `json:"phoneNumber"`
	TradingAccounts []string         `json:"tradingAccounts"`
	WealthType      string           `json:"wealthType"`
}

type UserInfoDevice struct {
	DeviceId               string                         `json:"deviceId"`
	DeviceIdentifier       string                         `json:"deviceIdentifier"`
	DeviceToken            string                         `json:"deviceToken"`
	Language               string                         `json:"language"`
	NotificationPreference UserInfoNotificationPreference `json:"notificationPreference"`
	Platform               string                         `json:"platform"`
}

type UserInfoNotificationPreference struct {
	Important bool `json:"important"`
	Market    bool `json:"market"`
	Order     bool `json:"order"`
	Portfolio bool `json:"portfolio"`
	Wallet    bool `json:"wallet"`
}
