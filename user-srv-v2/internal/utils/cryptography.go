package utils

import (
	"encoding/base64"

	"github.com/pi-financial/go-common/cryptography"
	"github.com/pi-financial/user-srv-v2/config"
)

func Hash(value string) string {
	cfg, err := config.LoadConfig()
	if err != nil {
		return ""
	}

	decodedSalt, err := base64.StdEncoding.DecodeString(cfg.DBSalt)
	if err != nil {
		return ""
	}

	return cryptography.Hash(value, string(decodedSalt))
}

func RsaEncryption(value string) (string, error) {
	cfg, err := config.LoadConfig()
	if err != nil {
		return "", err
	}

	return cryptography.RsaEncryption(value, cfg.DBPublicKey)
}

func RsaDecryption(value string) (string, error) {
	cfg, err := config.LoadConfig()
	if err != nil {
		return "", err
	}

	return cryptography.RsaDecryption(value, cfg.DBPrivateKey)
}
