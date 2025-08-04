package ssodb

import (
	"crypto/aes"
	"crypto/cipher"
	"crypto/rand"
	"crypto/sha256"
	"encoding/base64"
	"encoding/hex"
	"fmt"
	"io"
	"os"
	"strings"
	"time"

	"github.com/google/uuid"
	constants "github.com/pi-financial/pi-sso-v2/const"
	"github.com/pi-financial/pi-sso-v2/internal/util"
	"gorm.io/gorm"
)

type Account struct {
	ID                     uuid.UUID      `gorm:"type:varchar(36);primaryKey" json:"id"`
	Username               string         `gorm:"unique;not null" json:"username"`
	UsernameHash           string         `gorm:"unique;not null" json:"username_hash"`
	Password               *string        `json:"password"`
	SaltPassword           *string        `json:"salt_password"`
	FailedPasswordAttempts int            `gorm:"default:0" json:"failed_password_attempts"`
	Pin                    *string        `json:"pin"`
	SaltPin                *string        `json:"salt_pin"`
	FailedPinAttempts      int            `gorm:"default:0" json:"failed_pin_attempts"`
	IsLocked               bool           `gorm:"default:false" json:"is_locked"`
	UserID                 *string        `json:"user_id"`
	CreatedAt              time.Time      `gorm:"autoCreateTime" json:"created_at"`
	UpdatedAt              time.Time      `gorm:"autoUpdateTime" json:"updated_at"`
	DeletedAt              gorm.DeletedAt `gorm:"index" json:"deleted_at"`
}

// EncryptUsername encrypts the username using AES.
func EncryptUsername(plaintext string, encryptionKeyString string) (string, error) {

	plaintext = strings.ToLower(plaintext)

	encryptionKey := []byte(encryptionKeyString)
	block, err := aes.NewCipher(encryptionKey)
	if err != nil {
		return "", fmt.Errorf("failed to create cipher: %w", err)
	}

	ciphertext := make([]byte, aes.BlockSize+len(plaintext))
	iv := ciphertext[:aes.BlockSize]
	if _, err := io.ReadFull(rand.Reader, iv); err != nil {
		return "", fmt.Errorf("failed to generate IV: %w", err)
	}

	stream := cipher.NewCFBEncrypter(block, iv)
	stream.XORKeyStream(ciphertext[aes.BlockSize:], []byte(plaintext))
	return base64.StdEncoding.EncodeToString(ciphertext), nil
}

// DecryptUsername decrypts the encrypted username using AES.
func DecryptUsername(ciphertext, encryptionKeyString string) (string, error) {
	data, err := base64.StdEncoding.DecodeString(ciphertext)
	if err != nil {
		return "", fmt.Errorf("failed to decode base64: %w", err)
	}
	encryptionKey := []byte(encryptionKeyString)
	block, err := aes.NewCipher(encryptionKey)
	if err != nil {
		return "", fmt.Errorf("failed to create cipher: %w", err)
	}

	iv := data[:aes.BlockSize]
	plaintext := data[aes.BlockSize:]
	stream := cipher.NewCFBDecrypter(block, iv)
	stream.XORKeyStream(plaintext, plaintext)
	return string(plaintext), nil
}

// HashUsername generates a SHA256 hash for the username.
func HashUsername(username string) string {
	username = strings.ToLower(username)
	hash := sha256.Sum256([]byte(username))
	return hex.EncodeToString(hash[:])
}

// AfterFind is a GORM hook to handle operations after retrieving an account record.
func (a *Account) AfterFind(tx *gorm.DB) (err error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	if &a.Username != nil {
		decrypted, err := DecryptUsername(a.Username, encryptionKey)
		if err != nil {
			return err
		}
		a.Username = decrypted
	}
	return
}

// VerifyPIN verifies the entered PIN using either bcrypt (with salt) or legacy SHA-1 hashing.
func (a *Account) VerifyPIN(inputPIN string) (int, error) {
	var isPinValid bool
	if a.SaltPin != nil {
		// Use bcrypt with salt if available.
		isPinValid = util.CheckPasswordHashWithSalt(inputPIN, *a.SaltPin, *a.Pin)
	} else {
		// Use legacy hashing (e.g., SHA-1) if no salt is set.
		hashedPIN := util.Hash(inputPIN, "")
		isPinValid = *a.Pin == hashedPIN
	}

	if !isPinValid {
		// Increment the counter for failed PIN attempts.
		a.FailedPinAttempts++
		// Lock the account if failed PIN attempts reach or exceed 5.
		if a.FailedPinAttempts >= 5 {
			a.IsLocked = true
			return a.FailedPinAttempts, constants.ErrAccountLocked
		}
		return a.FailedPinAttempts, constants.ErrPinIncorrect
	}

	// Reset the failed PIN attempts counter if the PIN is correct.
	a.FailedPinAttempts = 0
	return a.FailedPinAttempts, nil
}

// VerifyPassword verifies the entered password and locks the account after more than 5 incorrect attempts.
func (a *Account) VerifyPassword(inputPassword string) (int, error) {
	var isPasswordValid bool

	if a.SaltPassword != nil {
		// Use bcrypt with salt if available.
		isPasswordValid = util.CheckPasswordHashWithSalt(inputPassword, *a.SaltPassword, *a.Password)
	} else {
		// Use legacy hashing (e.g., SHA-1) if no salt is set.
		hashedPassword := util.Hash(inputPassword, "")
		isPasswordValid = *a.Password == hashedPassword
	}

	if !isPasswordValid {
		// Increment failed password attempts.
		a.FailedPasswordAttempts++
		// Lock the account if failed attempts reach 5 or more.
		if a.FailedPasswordAttempts >= 5 {
			a.IsLocked = true
		}
		return a.FailedPasswordAttempts, constants.ErrPasswordIncorrect
	}

	// Reset failed attempts on successful login.
	a.FailedPasswordAttempts = 0
	return a.FailedPasswordAttempts, nil
}
