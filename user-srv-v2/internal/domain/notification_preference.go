package domain

type NotificationPreference struct {
	Id               string  `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	Important        bool    `gorm:"column:important" json:"important"`
	Order            bool    `gorm:"column:order" json:"order"`
	Portfolio        bool    `gorm:"column:portfolio" json:"portfolio"`
	Wallet           bool    `gorm:"column:wallet" json:"wallet"`
	Market           bool    `gorm:"column:market" json:"market"`
	DeviceForeignKey string  `gorm:"column:device_foreign_key;type:varchar(36)" json:"deviceForeignKey"`
	UserInfoId       *string `gorm:"column:user_info_id;type:varchar(36)" json:"userInfoId"`
}
