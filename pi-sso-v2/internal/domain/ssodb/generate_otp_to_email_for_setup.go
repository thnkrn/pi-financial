package ssodb

import (
	"fmt"
	"os"
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type GenerateOtpToEmailForSetup struct {
	ID          uuid.UUID  `gorm:"type:varchar(36);primaryKey" json:"id"`
	Email       string     `json:"email"`
	HashedEmail string     `json:"hashed_email"`
	CreatedAt   time.Time  `gorm:"autoCreateTime" json:"createdAt"`
	UpdatedAt   time.Time  `gorm:"autoUpdateTime" json:"updatedAt"`
	RefCode     string     `json:"refCode"`
	OtpCode     string     `json:"otpCode"`
	ExpiresAt   *time.Time `json:"expiresAt"`
	UsedAt      *time.Time `json:"usedAt"`
	IsUsed      bool       `gorm:"default:false" json:"isUsed"`
	Flow        string     `json:"flow"`
}

// BeforeCreate ฟังก์ชัน BeforeCreate ใช้สำหรับสร้าง UUID และจัดการก่อนบันทึก
func (a *GenerateOtpToEmailForSetup) BeforeCreate(tx *gorm.DB) (err error) {
	// ตรวจสอบว่า ENCRYPT_KEY ถูกกำหนดใน Environment หรือไม่
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	if encryptionKey == "" {
		return fmt.Errorf("encryption key (ENCRYPT_KEY) is not set in environment variables")
	}

	// สร้าง UUID
	a.ID = uuid.New()

	// ตรวจสอบว่า Email ไม่ใช่ค่า nil หรือว่างเปล่า
	if a.Email != "" {
		// เข้ารหัส Email
		email := a.Email
		encrypted, err := EncryptUsername(email, encryptionKey)
		if err != nil {
			return fmt.Errorf("failed to encrypt email: %w", err)
		}
		a.Email = encrypted
		a.HashedEmail = HashUsername(email)
	}

	return nil
}

// BeforeUpdate ฟังก์ชัน BeforeUpdate ใช้สำหรับเข้ารหัส Email ก่อนอัปเดต
func (a *GenerateOtpToEmailForSetup) BeforeUpdate(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	if encryptionKey == "" {
		return fmt.Errorf("encryption key (ENCRYPT_KEY) is not set in environment variables")
	}

	if a.Email != "" {
		email := a.Email
		encrypted, err := EncryptUsername(email, encryptionKey)
		if err != nil {
			return fmt.Errorf("failed to encrypt email during update: %w", err)
		}
		a.Email = encrypted
		a.HashedEmail = HashUsername(email)
	}

	return nil
}

// AfterFind ฟังก์ชัน AfterFind ใช้สำหรับถอดรหัส Email หลังจากค้นหา
func (a *GenerateOtpToEmailForSetup) AfterFind(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	if encryptionKey == "" {
		return fmt.Errorf("encryption key (ENCRYPT_KEY) is not set in environment variables")
	}

	if a.Email != "" {
		decrypted, err := DecryptUsername(a.Email, encryptionKey)
		if err != nil {
			return fmt.Errorf("failed to decrypt email: %w", err)
		}
		a.Email = decrypted
	}

	return nil
}
