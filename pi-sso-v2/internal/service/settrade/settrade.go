package settrade

import (
	"bytes"
	"context"
	"fmt"
	"io"
	"net/http"
	"time"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"github.com/pi-financial/pi-sso-v2/internal/util"
	"go.uber.org/zap"

	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/service/sba"
)

type Service struct {
	Host              string
	logger            log.Logger
	sbaAccountService sba.Service
}

func NewService(logger log.Logger, cfg config.Config, sbaAccountService sba.Service) Service {
	return Service{
		Host:              cfg.SettradeHost,
		logger:            logger,
		sbaAccountService: sbaAccountService,
	}
}

// SendSettradeRequestSyncPin SendSettradeRequest ฟังก์ชันสำหรับส่ง request ไปยัง Settrade API พร้อม timeout
func (s *Service) SendSettradeRequestSyncPin(ctx context.Context, username, pin string) error {
	apiURL := s.Host + "/api/pi/ActivateUserToSettrade/createPin"
	jsonData := []byte(fmt.Sprintf(`{"username": "%s", "pin": "%s"}`, username, pin))
	client := &http.Client{
		Timeout: 10 * time.Second,
	}

	// สร้าง request
	req, err := http.NewRequest("POST", apiURL, bytes.NewBuffer(jsonData))

	if err != nil {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPin: Failed to create request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))
		return fmt.Errorf("failed to create request: %v", err)
	}
	req.Header.Set("Content-Type", "application/json")

	// ส่ง request ไปยัง Settrade API
	resp, err := client.Do(req)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPin: Failed to Send Request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))

		return fmt.Errorf("failed to send request to trade API: %v", err)
	}

	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// อ่าน response body
	body, err := io.ReadAll(resp.Body)

	if err != nil {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPin: Failed to Read Response Body", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))

		return fmt.Errorf("failed to read response body: %v", err)
	}

	// ตรวจสอบสถานะการตอบกลับ
	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPin: Got Error from downstream service", zap.Error(err), zap.String("username", username), zap.String("url", apiURL), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))

		return fmt.Errorf("failed to send request to trade API: %s, Response: %s", resp.Status, string(body))
	}

	err = s.sbaAccountService.SyncPinToSsoV1(ctx, username, pin)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPin: Failed To Sync to SSO v1", zap.Error(err), zap.String("username", username), zap.String("url", apiURL), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))

		return fmt.Errorf("failed to sync pin to SSO v1: %v", err)
	}

	s.logger.Info(ctx, "settradeService.SendSettradeRequestSyncPin Success with response", zap.String("username", username), zap.String("url", apiURL), zap.String("body", string(body)))

	return nil
}

// SyncMember calls the syncMember API via GET request and logs the response.
func (s *Service) SyncMemberToSsoV1(ctx context.Context, username string) error {

	if !util.IsCustCodeValid(username) {
		s.logger.Error(ctx, "settradeService.SyncMember: Invalid custcode", zap.String("custcode", username))
		return fmt.Errorf("invalid custcode: %s", username)
	}

	apiURL := s.Host + fmt.Sprintf("/api/pi/Trading/syncMember/%s", username)
	client := &http.Client{
		Timeout: 10 * time.Second,
	}

	req, err := http.NewRequest("GET", apiURL, nil)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SyncMember: Failed to create request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))
		return fmt.Errorf("failed to create request: %v", err)
	}

	resp, err := client.Do(req)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SyncMember: Failed to send request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))
		return fmt.Errorf("failed to send request: %v", err)
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SyncMember: Failed to read response body", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))
		return fmt.Errorf("failed to read response body: %v", err)
	}

	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "settradeService.SyncMember: Got error from downstream service", zap.String("username", username), zap.String("url", apiURL), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return fmt.Errorf("failed to sync member: %s, response: %s", resp.Status, string(body))
	}

	s.logger.Info(ctx, "settradeService.SyncMember success", zap.String("username", username), zap.String("url", apiURL), zap.String("body", string(body)))
	return nil
}

// SendSettradeRequestSyncPassword SendSettradeRequest ฟังก์ชันสำหรับส่ง request ไปยัง Settrade API พร้อม timeout
func (s *Service) SendSettradeRequestSyncPassword(ctx context.Context, username, password string) error {
	apiURL := s.Host + "/api/pi/ActivateUserToSettrade/createPassword"
	jsonData := []byte(fmt.Sprintf(`{"username": "%s", "password": "%s"}`, username, password))

	client := &http.Client{
		Timeout: 10 * time.Second,
	}

	// สร้าง request
	req, err := http.NewRequest("POST", apiURL, bytes.NewBuffer(jsonData))

	if err != nil {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPassword: Failed to create request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))
		return fmt.Errorf("failed to create request: %v", err)
	}
	req.Header.Set("Content-Type", "application/json")

	// ส่ง request ไปยัง Settrade API
	resp, err := client.Do(req)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPassword: Failed to Send Request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))
		return fmt.Errorf("failed to send request to trade API: %v", err)
	}
	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// อ่าน response body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPassword: Failed to Read Response Body", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))
		return fmt.Errorf("failed to read response body: %v", err)
	}

	// ตรวจสอบสถานะการตอบกลับ
	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPassword: Got Error from downstream service", zap.Error(err), zap.String("username", username), zap.String("url", apiURL), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return fmt.Errorf("failed to send request to trade API: %s, Response: %s", resp.Status, string(body))
	}

	err = s.sbaAccountService.SyncPasswordToSsoV1(ctx, username, password)

	if err != nil {
		s.logger.Error(ctx, "settradeService.SendSettradeRequestSyncPassword: Failed To Sync to SSO v1", zap.Error(err), zap.String("username", username), zap.String("url", apiURL), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return fmt.Errorf("failed to sync password to SSO v1: %v", err)
	}

	s.logger.Info(ctx, "settradeService.SendSettradeRequestSyncPassword Success with response", zap.String("username", username), zap.String("url", apiURL), zap.String("body", string(body)))

	// ส่งกลับ response body
	return nil
}

func (s *Service) SendUnlockPinToSettrade(ctx context.Context, username string, value bool) error {
	apiURL := s.Host + "/api/pi/ActivateUserToSettrade/unlockPin"
	jsonData := []byte(fmt.Sprintf(`{"username": "%s" , "value": %t}`, username, value))

	// สร้าง client พร้อม timeout
	client := &http.Client{
		Timeout: 10 * time.Second,
	}

	// สร้าง request
	req, err := http.NewRequest("POST", apiURL, bytes.NewBuffer(jsonData))
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendUnlockPinToSettrade: Failed to create request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))

		return fmt.Errorf("failed to create request: %v", err)
	}
	req.Header.Set("Content-Type", "application/json")

	// ส่ง request ไปยัง Settrade API
	resp, err := client.Do(req)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendUnlockPinToSettrade: Failed to Send Request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))

		return fmt.Errorf("failed to send request to trade API: %v", err)
	}
	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// อ่าน response body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendUnlockPinToSettrade: Failed to Read Response Body", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))

		return fmt.Errorf("failed to read response body: %v", err)
	}

	// ตรวจสอบสถานะการตอบกลับ
	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "settradeService.SendUnlockPinToSettrade: Got Error from downstream service", zap.Error(err), zap.String("username", username), zap.String("url", apiURL), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))

		return fmt.Errorf("failed to send request to trade API: %s, Response: %s", resp.Status, string(body))
	}

	s.logger.Info(ctx, "settradeService.SendUnlockPinToSettrade Success with response", zap.String("username", username), zap.String("url", apiURL), zap.String("body", string(body)))

	// ส่งกลับ response body
	return nil
}

func (s *Service) SendUnlockPasswordToSettrade(ctx context.Context, username string, value bool) error {
	apiURL := s.Host + "/api/pi/ActivateUserToSettrade/unlockPassword"
	jsonData := []byte(fmt.Sprintf(`{"username": "%s" , "value": %t}`, username, value))

	// สร้าง client พร้อม timeout
	client := &http.Client{
		Timeout: 10 * time.Second,
	}
	// สร้าง request
	req, err := http.NewRequest("POST", apiURL, bytes.NewBuffer(jsonData))
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendUnlockPasswordToSettrade: Failed to create request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))

		return fmt.Errorf("failed to create request: %v", err)
	}

	req.Header.Set("Content-Type", "application/json")

	// ส่ง request ไปยัง Settrade API
	resp, err := client.Do(req)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendUnlockPasswordToSettrade: Failed to Send Request", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))
		return fmt.Errorf("failed to send request to trade API: %v", err)
	}

	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// อ่าน response body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		s.logger.Error(ctx, "settradeService.SendUnlockPasswordToSettrade: Failed to Read Response Body", zap.Error(err), zap.String("username", username), zap.String("url", apiURL))

		return fmt.Errorf("failed to read response body: %v", err)
	}

	// ตรวจสอบสถานะการตอบกลับ
	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "settradeService.SendUnlockPasswordToSettrade: Got Error from downstream service", zap.Error(err), zap.String("username", username), zap.String("url", apiURL), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))

		return fmt.Errorf("failed to send request to trade API: %s, Response: %s", resp.Status, string(body))
	}

	s.logger.Info(ctx, "settradeService.SendUnlockPasswordToSettrade Success with response", zap.String("username", username), zap.String("url", apiURL), zap.String("body", string(body)))

	// ส่งกลับ response body
	return nil
}
