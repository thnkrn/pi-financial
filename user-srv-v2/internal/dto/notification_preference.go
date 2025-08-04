package dto

type NotificationPreference struct {
	Important bool `json:"important"`
	Order     bool `json:"order"`
	Market    bool `json:"market"`
	Portfolio bool `json:"portfolio"`
	Wallet    bool `json:"wallet"`
}
