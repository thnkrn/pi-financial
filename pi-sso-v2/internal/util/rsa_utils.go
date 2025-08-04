package util

import (
	"crypto/rand"
	"crypto/rsa"
	"crypto/x509"
	"encoding/base64"
	"encoding/pem"
	"errors"
	"log"
	"os"
)

// ฟังก์ชันสำหรับโหลด private key จากไฟล์
func loadPrivateKeyFromEnv() (string, error) {
	// Load the private key from the environment
	privateKeyBase64 := os.Getenv("PRIVATE_KEY_BASE64")

	// Decode private key
	privateKey, err := DecodeFromBase64(privateKeyBase64)
	// log.Printf("Decoded private key: %v", privateKey)

	if err != nil {
		log.Fatalf("Failed to decode private key: %v", err)
	}

	return privateKey, nil
}

// Decrypt decrypts a Base64 encoded string `value` using the RSA private key `key`.
func RsaDecryption(value string) (string, error) {

	// Load the RSA private key from the file
	key, err := loadPrivateKeyFromEnv()
	if err != nil {
		// log.Printf("Error loading private key: %v", err)
		return "", err
	}

	// Decode the Base64 encoded value
	preppedInput, err := base64.StdEncoding.DecodeString(value)
	// log.Printf("Decoded preppedInput: %v", preppedInput)
	if err != nil {
		log.Printf("Error decoding Base64: %v", err)
		return "", err
	}

	// Parse the PEM block containing the private key
	block, _ := pem.Decode([]byte(key))
	if block == nil || block.Type != "RSA PRIVATE KEY" {
		err := errors.New("failed to parse PEM block containing the key")
		log.Printf("RsaDecryption Error: %v", err)
		return "", err
	}

	// Parse the private key from the PEM block
	parsedKey, err := x509.ParsePKCS1PrivateKey(block.Bytes)
	if err != nil {
		log.Printf("Error parsing private key: %v", err)
		return "", err
	}

	// Decrypt the input using the RSA private key
	result, err := rsa.DecryptPKCS1v15(nil, parsedKey, preppedInput)
	if err != nil {
		//log.Printf("Error decrypting: %v", err)
		return "", err
	}

	// Convert decrypted result to a string
	preppedOutput := string(result)
	return preppedOutput, nil
}

func loadPublicKeyFromEnv() (string, error) {
	// Load the private key from the environment
	privateKeyBase64 := os.Getenv("PUBLIC_KEY_BASE64")

	// Decode private key
	privateKey, err := DecodeFromBase64(privateKeyBase64)
	if err != nil {
		log.Fatalf("Failed to decode private key: %v", err)
	}

	return privateKey, nil
}

// Encrypt encrypts a string `value` using the RSA public key.
func RsaEncryption(value string) (string, error) {
	// Load the RSA public key from the file
	key, err := loadPublicKeyFromEnv()
	if err != nil {
		//log.Printf("Error loading public key: %v", err)
		return "", err
	}

	// Parse the PEM block containing the public key
	block, _ := pem.Decode([]byte(key))
	if block == nil || block.Type != "PUBLIC KEY" {
		err := errors.New("failed to parse PEM block containing the public key")
		//log.Printf("RsaEncryption Error: %v", err)
		return "", err
	}

	// Parse the public key from the PEM block
	parsedKey, err := x509.ParsePKIXPublicKey(block.Bytes)
	if err != nil {
		//log.Printf("Error parsing public key: %v", err)
		return "", err
	}

	// Assert the key type to rsa.PublicKey
	publicKey, ok := parsedKey.(*rsa.PublicKey)
	if !ok {
		err := errors.New("not an RSA public key")
		//log.Printf("RsaEncrypublicKeyption Error: %v", err)
		return "", err
	}

	// Encrypt the input using the RSA public key
	// ใช้ crypto/rand.Reader เพื่อหลีกเลี่ยง nil pointer dereference
	encryptedBytes, err := rsa.EncryptPKCS1v15(rand.Reader, publicKey, []byte(value))
	if err != nil {
		log.Printf("Error encrypting: %v", err)
		return "", err
	}

	// Encode the encrypted result to Base64
	encodedOutput := base64.StdEncoding.EncodeToString(encryptedBytes)
	return encodedOutput, nil
}
