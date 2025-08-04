package ssodb

import (
	"fmt"
	"os"
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type GenerateOtpToPhoneForSetup struct {
	ID          uuid.UUID  `gorm:"type:varchar(36);primaryKey" json:"id"`
	Phone       string     `json:"phone"`
	HashedPhone string     `json:"hashed_phone"`
	CreatedAt   time.Time  `gorm:"autoCreateTime" json:"createdAt"`
	UpdatedAt   time.Time  `gorm:"autoUpdateTime" json:"updatedAt"`
	RefCode     string     `json:"refCode"`
	ExpiresAt   *time.Time `json:"expiresAt"`
	UsedAt      *time.Time `json:"usedAt"`
	IsUsed      bool       `gorm:"default:false" json:"isUsed"`
	Flow        string     `json:"flow"`
}

// BeforeCreate ฟังก์ชัน BeforeCreate ใช้สำหรับสร้าง UUID และจัดการก่อนบันทึก
func (a *GenerateOtpToPhoneForSetup) BeforeCreate(tx *gorm.DB) (err error) {
	// ตรวจสอบว่า ENCRYPT_KEY ถูกกำหนดใน Environment หรือไม่
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	if encryptionKey == "" {
		return fmt.Errorf("encryption key (ENCRYPT_KEY) is not set in environment variables")
	}

	// ตรวจสอบว่า Phone ไม่ใช่ค่า nil หรือว่างเปล่า
	if a.Phone != "" {
		// เข้ารหัส Email
		phone := a.Phone
		encrypted, err := EncryptUsername(phone, encryptionKey)
		if err != nil {
			return fmt.Errorf("failed to encrypt email: %w", err)
		}
		a.Phone = encrypted
		a.HashedPhone = HashUsername(phone)
	}

	return nil
}

// BeforeUpdate ฟังก์ชัน BeforeUpdate ใช้สำหรับเข้ารหัส Email ก่อนอัปเดต
func (a *GenerateOtpToPhoneForSetup) BeforeUpdate(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	if encryptionKey == "" {
		return fmt.Errorf("encryption key (ENCRYPT_KEY) is not set in environment variables")
	}

	if a.Phone != "" {
		phone := a.Phone
		encrypted, err := EncryptUsername(phone, encryptionKey)
		if err != nil {
			return fmt.Errorf("failed to encrypt email during update: %w", err)
		}
		a.Phone = encrypted
		a.HashedPhone = HashUsername(phone)
	}

	return nil
}

// AfterFind ฟังก์ชัน AfterFind ใช้สำหรับถอดรหัส Email หลังจากค้นหา
func (a *GenerateOtpToPhoneForSetup) AfterFind(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	if encryptionKey == "" {
		return fmt.Errorf("encryption key (ENCRYPT_KEY) is not set in environment variables")
	}

	if a.Phone != "" {
		decrypted, err := DecryptUsername(a.Phone, encryptionKey)
		if err != nil {
			return fmt.Errorf("failed to decrypt email: %w", err)
		}
		a.Phone = decrypted
	}

	return nil
}
