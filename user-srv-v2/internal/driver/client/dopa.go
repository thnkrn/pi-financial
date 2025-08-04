package client

import (
	"bytes"
	"context"
	"crypto/aes"
	"crypto/cipher"
	"crypto/rand"
	"encoding/base64"
	"encoding/json"
	"fmt"
	"io"
	"math/big"
	"net/http"
	"net/url"
	"strings"

	goclient "github.com/pi-financial/dopa-client/go-client"
	"github.com/pi-financial/go-common/logger"
	"github.com/pi-financial/user-srv-v2/config"
	"github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
)

// DopaTransportObject represents the encrypted request body
type DopaTransportObject struct {
	Data string `json:"data"`
}

// DopaResponseObject represents the response structure from Dopa API
type DopaResponseObject struct {
	Success bool   `json:"success"`
	Code    int    `json:"code"`
	Message string `json:"message"`
	Data    string `json:"data"`
	Error   string `json:"error"`
}

// DopaSecurityHandler handles encryption/decryption for Dopa API communication
type DopaSecurityHandler struct {
	aesKey     string
	ivLength   int
	ivLetters  string
	httpClient *http.Client
}

// NewDopaSecurityHandler creates a new security handler
func NewDopaSecurityHandler(aesKey string) *DopaSecurityHandler {
	return &DopaSecurityHandler{
		aesKey:     aesKey,
		ivLength:   16,
		ivLetters:  "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ",
		httpClient: http.DefaultClient,
	}
}

// RoundTrip implements the http.RoundTripper interface
func (h *DopaSecurityHandler) RoundTrip(req *http.Request) (*http.Response, error) {
	// Encrypt request body
	if err := h.encryptRequestBody(req); err != nil {
		return nil, fmt.Errorf("failed to encrypt request body: %w", err)
	}

	// Send the request
	resp, err := h.httpClient.Do(req)

	if err != nil {
		return nil, err
	}

	// Decrypt response body
	if err := h.decryptResponseBody(resp); err != nil {
		return nil, fmt.Errorf("failed to decrypt response body: %w", err)
	}

	return resp, nil
}

// encryptRequestBody encrypts the request body
func (h *DopaSecurityHandler) encryptRequestBody(req *http.Request) error {
	if req.Body == nil {
		return nil
	}

	// Read the original body
	bodyBytes, err := io.ReadAll(req.Body)
	if err != nil {
		return fmt.Errorf("failed to read request body: %w", err)
	}
	_ = req.Body.Close()

	if len(bodyBytes) == 0 {
		return nil
	}

	// Encrypt the body
	iv := h.generateIV()
	encryptedData := h.encryptString(string(bodyBytes), iv)

	// Create transport object
	transportObj := DopaTransportObject{
		Data: encryptedData,
	}

	// Serialize and set as new body
	transportBytes, err := json.Marshal(transportObj)
	if err != nil {
		return fmt.Errorf("failed to marshal transport object: %w", err)
	}

	req.Body = io.NopCloser(bytes.NewBuffer(transportBytes))
	req.ContentLength = int64(len(transportBytes))
	req.Header.Set("Content-Type", "application/json")

	return nil
}

// decryptResponseBody decrypts the response body
func (h *DopaSecurityHandler) decryptResponseBody(resp *http.Response) error {
	// Read response body
	bodyBytes, err := io.ReadAll(resp.Body)
	if err != nil {
		return fmt.Errorf("failed to read response body: %w", err)
	}
	_ = resp.Body.Close()

	// Deserialize response object
	var dopaResp DopaResponseObject
	if err := json.Unmarshal(bodyBytes, &dopaResp); err != nil {
		return fmt.Errorf("failed to unmarshal response: %w", err)
	}

	// Check for errors
	if dopaResp.Code >= 300 {
		switch dopaResp.Code {
		case 401:
			return fmt.Errorf("unauthorized: %s", dopaResp.Message)
		default:
			return fmt.Errorf("HTTP error %d: %s", dopaResp.Code, dopaResp.Message)
		}
	}

	// Decrypt the data
	decryptedData := h.decryptString(dopaResp.Data)

	// Set the decrypted data as the new response body
	resp.Header.Set("Content-Type", "application/json")
	resp.Body = io.NopCloser(bytes.NewBufferString(decryptedData))
	resp.ContentLength = int64(len(decryptedData))

	return nil
}

// generateIV generates a random IV string
func (h *DopaSecurityHandler) generateIV() string {
	result := make([]byte, h.ivLength)
	for i := 0; i < h.ivLength; i++ {
		randomIndex, _ := rand.Int(rand.Reader, big.NewInt(int64(len(h.ivLetters))))
		result[i] = h.ivLetters[randomIndex.Int64()]
	}
	return string(result)
}

// encryptString encrypts a string using AES-256-CBC
func (h *DopaSecurityHandler) encryptString(plainText, iv string) string {
	keyBytes := []byte(h.aesKey)
	ivBytes := []byte(iv)

	// Create cipher block
	block, err := aes.NewCipher(keyBytes)
	if err != nil {
		panic(fmt.Sprintf("failed to create cipher: %v", err))
	}

	// Pad the plaintext to block size
	paddedText := h.pkcs7Pad([]byte(plainText), aes.BlockSize)

	// Encrypt
	encrypted := make([]byte, len(paddedText))
	mode := cipher.NewCBCEncrypter(block, ivBytes)
	mode.CryptBlocks(encrypted, paddedText)

	// Return IV:Base64(encrypted)
	return iv + ":" + base64.StdEncoding.EncodeToString(encrypted)
}

// decryptString decrypts a string using AES-256-CBC
func (h *DopaSecurityHandler) decryptString(cipherText string) string {
	if cipherText == "" {
		panic("cipherText cannot be empty")
	}
	if h.aesKey == "" {
		panic("aesKey cannot be empty")
	}

	// Split IV and encrypted data
	parts := strings.Split(cipherText, ":")
	if len(parts) != 2 {
		panic("invalid cipherText format")
	}

	iv := parts[0]
	encryptedData := parts[1]

	if iv == "" {
		panic("IV cannot be empty")
	}

	keyBytes := []byte(h.aesKey)
	ivBytes := []byte(iv)

	// Decode base64
	encryptedBytes, err := base64.StdEncoding.DecodeString(encryptedData)
	if err != nil {
		panic(fmt.Sprintf("failed to decode base64: %v", err))
	}

	// Create cipher block
	block, err := aes.NewCipher(keyBytes)
	if err != nil {
		panic(fmt.Sprintf("failed to create cipher: %v", err))
	}

	// Decrypt
	decrypted := make([]byte, len(encryptedBytes))
	mode := cipher.NewCBCDecrypter(block, ivBytes)
	mode.CryptBlocks(decrypted, encryptedBytes)

	// Remove padding
	unpadded := h.pkcs7Unpad(decrypted)

	return string(unpadded)
}

// pkcs7Pad adds PKCS7 padding
func (h *DopaSecurityHandler) pkcs7Pad(data []byte, blockSize int) []byte {
	padding := blockSize - len(data)%blockSize
	padtext := bytes.Repeat([]byte{byte(padding)}, padding)
	return append(data, padtext...)
}

// pkcs7Unpad removes PKCS7 padding
func (h *DopaSecurityHandler) pkcs7Unpad(data []byte) []byte {
	length := len(data)
	if length == 0 {
		return data
	}
	unpadding := int(data[length-1])
	if unpadding > length {
		return data
	}
	return data[:(length - unpadding)]
}

type DopaClient struct {
	DopaService *goclient.APIClient
	Log         logger.Logger
	AesKey      string
	Username    string
	Password    string
}

// NewDopaClient creates a new Dopa client with encryption/decryption capabilities.
//
// Parameters:
//   - log: Logger for logging operations
//   - cfg: Configuration containing Dopa API settings
//
// Returns:
//   - interfaces.DopaClient: Configured Dopa client with security handler
//
// Implementation:
//  1. Parses the DopaHost URL from configuration
//  2. Validates that DopaKey is provided in configuration
//  3. Creates a DopaSecurityHandler with the provided AES key
//  4. Configures the goclient.APIClient with the security handler as transport
//  5. Returns the configured DopaClient
//
// Error cases:
//   - Panics if DopaHost URL is invalid
//   - Panics if DopaKey is not provided in configuration
//
// Notes: The client automatically handles encryption/decryption of all requests/responses
// using AES-256-CBC encryption with the provided DopaKey.
func NewDopaClient(log logger.Logger, cfg config.Config) interfaces.DopaClient {
	apiUrl, err := url.Parse(cfg.DopaHost)
	log.Info(fmt.Sprintf("DopaHost: %s", apiUrl.String()))
	if err != nil {
		panic(err)
	}

	aesKey := cfg.DopaKey
	username := cfg.DopaUsername
	password := cfg.DopaPassword
	if aesKey == "" || username == "" || password == "" {
		log.Error("no dopa aes key or username or password for Dopa service in .env (name must be DOPA_KEY, DOPA_USERNAME, DOPA_PASSWORD)")
		panic("no dopa aes key or username or password for Dopa service in .env (name must be DOPA_KEY, DOPA_USERNAME, DOPA_PASSWORD)")
	}

	// Create security handler
	securityHandler := NewDopaSecurityHandler(aesKey)

	return &DopaClient{
		Log:      log,
		AesKey:   aesKey,
		Username: username,
		Password: password,
		DopaService: goclient.NewAPIClient(&goclient.Configuration{
			Scheme: "https",
			Servers: []goclient.ServerConfiguration{
				{
					// WARN: If the url have a colon (e.g. <host>:<port>), MUST
					// include the http:// prefix (e.g. http://<host>:<port>).
					// Without the prefix, goclient will remove the <host> from the url.
					URL: apiUrl.String(),
				},
			},
			HTTPClient: &http.Client{
				Transport: securityHandler,
			},
		}),
	}
}

func (i DopaClient) getToken(ctx context.Context) (*string, error) {
	result, _, err := i.DopaService.DopaAPI.DopaGetTokenPost(ctx).GetTokenRequest(goclient.GetTokenRequest{
		UserName: i.Username,
		Password: i.Password,
	}).Execute()

	if err != nil {
		return nil, fmt.Errorf("in getToken: client %q: %w",
			"DopaService.DopaAPI.DopaGetTokenPost", err)
	}

	return &result, nil
}

// VerifyByLaserCode verifies a citizen's identity using laser code from their ID card.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - citizenId: The citizen ID number
//   - firstName: The person's first name
//   - lastName: The person's last name
//   - birthDay: The person's birth date
//   - laserCode: The laser code from the ID card
//
// Returns:
//   - *goclient.CheckCardByLaserResponse: Contains verification result from Dopa API
//   - error: Error if verification fails (e.g., network error, API error, or decryption error)
//
// Implementation:
//  1. Creates a CheckCardByLaserRequest with the provided parameters
//  2. Sends the request to Dopa API via DopaService.DopaAPI.DopaCheckCardByLaserPost
//  3. The request body is automatically encrypted by DopaSecurityHandler
//  4. The response is automatically decrypted by DopaSecurityHandler
//  5. Returns the verification result on success
//
// Error cases:
//   - Returns error if network communication fails
//   - Returns error if API returns error response (e.g., 401 Unauthorized)
//   - Returns error if response decryption fails
//   - Returns error if request encryption fails
//
// Notes: This method uses AES-256-CBC encryption for secure communication with Dopa API.
// The encryption/decryption is handled transparently by the DopaSecurityHandler.
func (i DopaClient) VerifyByLaserCode(ctx context.Context, citizenId, firstName, lastName, birthDay, laserCode *string) (*goclient.CheckCardByLaserResponse, error) {
	dopaToken, err := i.getToken(ctx)

	if err != nil {
		return nil, fmt.Errorf("in VerifyByLaserCode: client %q: %w",
			"DopaService.DopaAPI.DopaGetTokenPost", err)
	}

	result, _, err := i.DopaService.DopaAPI.DopaCheckCardByLaserPost(ctx).
		Authorization(*dopaToken).
		CheckCardByLaserRequest(goclient.CheckCardByLaserRequest{
			PID:       *citizenId,
			FirstName: *firstName,
			LastName:  *lastName,
			BirthDay:  *birthDay,
			Laser:     *laserCode,
		}).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("in VerifyByLaserCode citizen id %q: client %q: %w",
			*citizenId, "DopaService.DopaAPI.DopaCheckCardByLaserPost", err)
	}

	return result, nil
}
