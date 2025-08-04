package util

import (
	"encoding/base64"
	"encoding/json"

	"github.com/pi-financial/pi-sso-v2/internal/types"
)

// ฟังก์ชันสำหรับถอดรหัสคีย์จาก Base64 ที่เก็บไว้ใน .env
func DecodeFromBase64(encodedKey string) (string, error) {
	// Decode Base64
	keyData, err := base64.StdEncoding.DecodeString(encodedKey)
	if err != nil {
		return "", err
	}

	return string(keyData), nil
}

func ExtractDetail(jsonStr string) *string {
	var errResp types.ErrorResponse
	err := json.Unmarshal([]byte(jsonStr), &errResp)
	if err != nil {
		return nil
	}
	return &errResp.Detail
}
