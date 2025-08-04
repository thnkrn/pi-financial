package util

import (
	"crypto/sha1"
	"encoding/hex"
	"strings"

	"golang.org/x/crypto/bcrypt"
)

// HashPasswordWithSalt ทำการ hash รหัสผ่านร่วมกับ salt โดยใช้ bcrypt
func HashPasswordWithSalt(password, salt string) (string, error) {
	// รวมรหัสผ่านกับ salt
	passwordWithSalt := password + salt

	// ใช้ bcrypt.GenerateFromPassword ในการ hash รหัสผ่านที่รวมกับ salt
	hash, err := bcrypt.GenerateFromPassword([]byte(passwordWithSalt), bcrypt.DefaultCost)

	if err != nil {
		return "", err
	}

	return string(hash), nil
}

func Hash(password string, salt string) string {
	// Trim spaces from the password
	password = strings.TrimSpace(password)

	// รวม Salt กับ Password
	passwordWithSalt := password + salt

	// Compute SHA-1 hash
	hasher := sha1.New()
	hasher.Write([]byte(passwordWithSalt))
	hashed := hex.EncodeToString(hasher.Sum(nil))

	// Reorder the resulting hash string
	lowerInvariant := strings.ToLower(hashed)
	result := lowerInvariant[8:25] + lowerInvariant[25:38] + lowerInvariant[0:8] + lowerInvariant[38:40]

	return result
}

// CheckPasswordHashWithSalt ตรวจสอบรหัสผ่านที่รวมกับ salt
func CheckPasswordHashWithSalt(password, salt, hash string) bool {
	// รวมรหัสผ่านกับ salt
	passwordWithSalt := password + salt

	// เปรียบเทียบ hash ของรหัสผ่าน
	err := bcrypt.CompareHashAndPassword([]byte(hash), []byte(passwordWithSalt))
	return err == nil
}
