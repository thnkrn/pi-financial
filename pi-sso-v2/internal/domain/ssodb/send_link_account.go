package ssodb

import (
	"fmt"
	"os"
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type SendLinkAccount struct {
	ID        uuid.UUID  `gorm:"type:varchar(36);primaryKey" json:"id"`
	Email     string     `json:"email"`
	Custcode  string     `gorm:"unique;not null" json:"custcode"`
	UserId    *string    `json:"userId"`
	CreatedAt time.Time  `gorm:"autoCreateTime" json:"createdAt"`
	UsedAt    *time.Time `json:"usedAt"`
	IsUsed    bool       `gorm:"default:false" json:"isUsed"`
}

// ฟังก์ชัน BeforeCreate ใช้สำหรับสร้าง UUID และจัดการก่อนบันทึก
func (a *SendLinkAccount) BeforeCreate(tx *gorm.DB) (err error) {
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
		encrypted, err := EncryptUsername(a.Email, encryptionKey)
		if err != nil {
			return fmt.Errorf("failed to encrypt email: %w", err)
		}
		a.Email = encrypted
	}

	return nil
}

// ฟังก์ชัน BeforeUpdate ใช้สำหรับเข้ารหัส Email ก่อนอัปเดต
func (a *SendLinkAccount) BeforeUpdate(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	if encryptionKey == "" {
		return fmt.Errorf("encryption key (ENCRYPT_KEY) is not set in environment variables")
	}

	if a.Email != "" {
		encrypted, err := EncryptUsername(a.Email, encryptionKey)
		if err != nil {
			return fmt.Errorf("failed to encrypt email during update: %w", err)
		}
		a.Email = encrypted
	}

	return nil
}

// ฟังก์ชัน AfterFind ใช้สำหรับถอดรหัส Email หลังจากค้นหา
func (a *SendLinkAccount) AfterFind(tx *gorm.DB) (err error) {
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
