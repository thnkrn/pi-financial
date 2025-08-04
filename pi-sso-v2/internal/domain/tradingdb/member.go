package tradingdb

import "time"

type Member struct {
	ID                   int       `gorm:"column:ID;primaryKey;autoIncrement"`
	UserName             string    `gorm:"column:UserName;size:10;not null"`
	Password             string    `gorm:"column:Password;size:64;not null"`
	PinCode              *string   `gorm:"column:PinCode;size:64"`
	Created              time.Time `gorm:"column:Created"`
	Modified             time.Time `gorm:"column:Modified"`
	Activated            time.Time `gorm:"column:Activated"`
	LastLoggedIn         time.Time `gorm:"column:LastLoggedIn"`
	LoginFailCount       int8      `gorm:"column:LoginFailCount"`
	IsActivate           bool      `gorm:"column:IsActivate"`
	IsLock               bool      `gorm:"column:IsLock"`
	IsAllow2Trade        bool      `gorm:"column:IsAllow2Trade"`
	IsAllowSettrade      bool      `gorm:"column:IsAllowSettrade"`
	IsAllowStreaming     bool      `gorm:"column:IsAllowStreaming"`
	IsAllowAspen         bool      `gorm:"column:IsAllowAspen"`
	IsAllowEfinance      bool      `gorm:"column:IsAllowEfinance"`
	IsSyncedPin          bool      `gorm:"column:IsSyncedPin"`
	IsDeleted            bool      `gorm:"column:IsDeleted"`
	ResetPasswordKey     string    `gorm:"column:ResetPasswordKey;size:64"`
	SessionID            string    `gorm:"column:SessionID;size:50"`
	Type                 string    `gorm:"column:Type;size:1"`
	SettradePinLock      bool      `gorm:"column:SettradePinLock"`
	SettradePinFailCount int       `gorm:"column:SettradePinFailCount"`
	LoginPinLock         bool      `gorm:"column:LoginPinLock"`
	LoginPinFailCount    int       `gorm:"column:LoginPinFailCount"`
}

// TableName method sets the table name for GORM
func (Member) TableName() string {
	return "Member" // ใส่ชื่อตารางที่ตรงกับตารางในฐานข้อมูล MSSQL ของคุณ
}
