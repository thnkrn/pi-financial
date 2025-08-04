package ssodb

import (
	"os"
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type LoginWith2FASection struct {
	ID          uuid.UUID      `gorm:"type:varchar(36);primaryKey" json:"id"` // ใช้ UUID จาก Go
	UserID      string         `json:"user_id"`
	PhoneNumber string         `json:"phone_number"`
	RefCode     string         `json:"ref_code"`
	DeviceID    string         `json:"device_id"`
	AccountID   uuid.UUID      `json:"account_id"`
	IsVerify    bool           `json:"is_verify"`
	CreatedAt   time.Time      `gorm:"autoCreateTime" json:"created_at"`
	ExpiredAt   time.Time      `json:"expired_at"`
	UpdatedAt   time.Time      `gorm:"autoUpdateTime" json:"updated_at"`
	DeletedAt   gorm.DeletedAt `gorm:"index" json:"deleted_at"`
}

// ฟังก์ชัน BeforeCreate เข้ารหัส PhoneNumber ก่อนสร้าง
func (a *LoginWith2FASection) BeforeCreate(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")

	// เข้ารหัส PhoneNumber
	if a.PhoneNumber != "" {
		PhoneNumber := a.PhoneNumber
		encrypted, err := EncryptUsername(PhoneNumber, encryptionKey)
		if err != nil {
			return err
		}
		a.PhoneNumber = encrypted

	}

	return
}

// ฟังก์ชัน BeforeUpdate เข้ารหัส PhoneNumber ก่อนอัปเดต
func (a *LoginWith2FASection) BeforeUpdate(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")

	// เข้ารหัส PhoneNumber
	if a.PhoneNumber != "" {
		PhoneNumber := a.PhoneNumber
		encrypted, err := EncryptUsername(PhoneNumber, encryptionKey)
		if err != nil {
			return err
		}
		a.PhoneNumber = encrypted

	}

	return
}

// ฟังก์ชัน AfterFind ใช้สำหรับถอดรหัส PhoneNumber หลังจากค้นหา
func (a *LoginWith2FASection) AfterFind(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")

	// ถอดรหัส PhoneNumber
	if &a.PhoneNumber != nil {
		PhoneNumber := a.PhoneNumber
		decrypted, err := DecryptUsername(PhoneNumber, encryptionKey)
		if err != nil {
			return err
		}
		a.PhoneNumber = decrypted

	}

	return
}
