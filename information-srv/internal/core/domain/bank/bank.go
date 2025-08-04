package bank

type Bank struct {
	Id        string `json:"id" gorm:"column:id"`
	Name      string `json:"name" gorm:"column:name"`
	ShortName string `json:"short_name" gorm:"column:short_name"`
	Code      string `json:"code" gorm:"column:code"`
	IconUrl   string `json:"icon_url" gorm:"column:icon_url"`
	NameTh    string `json:"name_th" gorm:"column:name_th"`
}

func (Bank) TableName() string {
	return "bank_infos"
}
