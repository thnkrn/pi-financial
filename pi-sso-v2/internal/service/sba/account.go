package sba

import (
	"context"
	"encoding/json"
	"fmt"
	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/domain/sba"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"github.com/pi-financial/pi-sso-v2/internal/util"
	"go.uber.org/zap"
	"io"
	"net/http"
)

type Service struct {
	Host   string
	logger log.Logger
}

func NewSBAAccountService(logger log.Logger, cfg config.Config) Service {
	return Service{
		Host:   cfg.SettradeHost,
		logger: logger,
	}
}

func (s *Service) GetAccountInfo(ctx context.Context, custcode string) (*sba.ResponseAccountInfo, error) {
	// URL ของ API
	url := fmt.Sprintf("%s/api/pi/SsoDb/AccountInfoByCustcode/%s", s.Host, custcode)

	// สร้าง HTTP GET request
	resp, err := http.Get(url)
	if err != nil {
		s.logger.Error(ctx, "sbaService.GetAccountInfo: Failed to make GET request", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url))
		return nil, fmt.Errorf("failed to make GET request: %v", err)
	}

	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// ตรวจสอบว่าการร้องขอสำเร็จหรือไม่
	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "sbaService.GetAccountInfo: Got Error from downstream service", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode))
		return nil, fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	// อ่านข้อมูลจาก body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		s.logger.Error(ctx, "sbaService.GetAccountInfo: : Failed to Read Response Body", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	// สร้าง struct สำหรับเก็บข้อมูลจาก API
	var response sba.ResponseAccountInfo

	// แปลงข้อมูล JSON เป็น struct
	if err := json.Unmarshal(body, &response); err != nil {
		s.logger.Error(ctx, "sbaService.GetAccountInfo: : Failed to Unmarshal Response Body", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to unmarshal JSON: %v", err)
	}

	return &response, nil
}

func (s *Service) GetAccountTabInfo(ctx context.Context, custcode string) (*[]sba.ResponseAccountTabInfo, error) {
	// URL ของ API
	url := fmt.Sprintf("%s/api/pi/SsoDb/AccountTabByCustcode/%s", s.Host, custcode)

	// สร้าง HTTP GET request
	resp, err := http.Get(url)
	if err != nil {
		s.logger.Error(ctx, "sbaService.GetAccountTabInfo: Failed to make GET request", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url))
		return nil, fmt.Errorf("failed to make GET request: %v", err)
	}
	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// ตรวจสอบว่าการร้องขอสำเร็จหรือไม่
	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "sbaService.GetAccountTabInfo: Got Error from downstream service", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode))
		return nil, fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	// อ่านข้อมูลจาก body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		s.logger.Error(ctx, "sbaService.GetAccountTabInfo: : Failed to Read Response Body", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	// สร้าง struct สำหรับเก็บข้อมูลจาก API
	var response []sba.ResponseAccountTabInfo

	// แปลงข้อมูล JSON เป็น struct
	if err := json.Unmarshal(body, &response); err != nil {
		s.logger.Error(ctx, "sbaService.GetAccountTabInfo: : Failed to Unmarshal Response Body", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to unmarshal JSON: %v", err)
	}

	return &response, nil
}

func (s *Service) GetCustomerInfo(ctx context.Context, custcode string) (*sba.ResponseCustInfo, error) {
	// URL ของ API
	url := fmt.Sprintf("%s/api/pi/SsoDb/GetCustInfoByCustCode/%s", s.Host, custcode)

	// สร้าง HTTP GET request
	resp, err := http.Get(url)
	if err != nil {
		s.logger.Error(ctx, "sbaService.GetCustomerInfo: Failed to make GET request", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url))
		return nil, fmt.Errorf("failed to make GET request: %v", err)
	}
	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// ตรวจสอบว่าการร้องขอสำเร็จหรือไม่
	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "sbaService.GetCustomerInfo: Got Error from downstream service", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode))
		return nil, fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	// อ่านข้อมูลจาก body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		s.logger.Error(ctx, "sbaService.GetCustomerInfo: : Failed to Read Response Body", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	// สร้าง struct สำหรับเก็บข้อมูลจาก API
	var response sba.ResponseCustInfo

	if string(body) == "{}" {
		s.logger.Error(ctx, "sbaService.GetCustomerInfo: : Failed to Unmarshal Response Body", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	// แปลงข้อมูล JSON เป็น struct
	if err := json.Unmarshal(body, &response); err != nil {
		return nil, fmt.Errorf("failed to unmarshal JSON: %v", err)
	}

	return &response, nil
}

func (s *Service) GetEfinUser(ctx context.Context, custcode string) (*sba.ResponseEfinUser, error) {
	// URL ของ API
	url := fmt.Sprintf("%s/api/pi/Trading/efinUser/%s", s.Host, custcode)

	// สร้าง HTTP GET request
	resp, err := http.Get(url)
	if err != nil {
		s.logger.Error(ctx, "sbaService.GetEfinUser: Failed to make GET request", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url))
		return nil, fmt.Errorf("failed to make GET request: %v", err)
	}
	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// ตรวจสอบว่าการร้องขอสำเร็จหรือไม่
	if resp.StatusCode != http.StatusOK {
		s.logger.Error(ctx, "sbaService.GetEfinUser: Got Error from downstream service", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode))
		return nil, fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	// อ่านข้อมูลจาก body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		s.logger.Error(ctx, "sbaService.GetEfinUser: Failed to Read Response Body", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	// สร้าง struct สำหรับเก็บข้อมูลจาก API
	var response sba.ResponseEfinUser

	if string(body) == "{}" {
		s.logger.Error(ctx, "sbaService.GetEfinUser: Failed to Read Response Body - body is {}", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	// แปลงข้อมูล JSON เป็น struct
	if err := json.Unmarshal(body, &response); err != nil {
		s.logger.Error(ctx, "sbaService.GetEfinUser: Failed to Unmarshal Response Body", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode), zap.String("body", string(body)))
		return nil, fmt.Errorf("failed to unmarshal JSON: %v", err)
	}

	return &response, nil
}

func (s *Service) GetLogSession(id string) (*sba.ResponseLogSession, error) {
	// URL ของ API
	url := fmt.Sprintf("%s/api/pi/Trading/logSession/%s", s.Host, id)

	// สร้าง HTTP GET request
	resp, err := http.Get(url)
	if err != nil {
		return nil, fmt.Errorf("failed to make GET request: %v", err)
	}
	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// ตรวจสอบว่าการร้องขอสำเร็จหรือไม่
	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	// อ่านข้อมูลจาก body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	// สร้าง struct สำหรับเก็บข้อมูลจาก API
	var response sba.ResponseLogSession

	if string(body) == "{}" {
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	// แปลงข้อมูล JSON เป็น struct
	if err := json.Unmarshal(body, &response); err != nil {
		s.logger.ErrorNoCtx("sbaAccountService.GetLogSession Error failed to unmarshal JSON", zap.Error(err))
		return nil, fmt.Errorf("failed to unmarshal JSON: %v", err)
	}

	return &response, nil
}

func (s *Service) SyncPasswordToSsoV1(ctx context.Context, custcode, password string) error {
	hashedPassword := util.Hash(password, "")

	// URL ของ API
	url := fmt.Sprintf("%s/api/pi/Trading/syncPassword/%s/%s", s.Host, custcode, hashedPassword)

	// สร้าง HTTP GET request
	resp, err := http.Get(url)
	if err != nil {
		s.logger.Error(ctx, "sbaService.SyncPasswordToSsoV1: Failed to make GET request", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url))
		return fmt.Errorf("failed to make GET request: %v", err)
	}
	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// ตรวจสอบว่าการร้องขอสำเร็จหรือไม่
	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusNoContent {
		s.logger.Error(ctx, "sbaService.SyncPasswordToSsoV1: Got Error from downstream service", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode))
		return fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	return nil
}

func (s *Service) SyncPinToSsoV1(ctx context.Context, custcode, pin string) error {
	hashedPin := util.Hash(pin, "")

	// URL ของ API
	url := fmt.Sprintf("%s/api/pi/Trading/syncPin/%s/%s?pin=%s", s.Host, custcode, hashedPin, hashedPin)

	// สร้าง HTTP GET request
	resp, err := http.Get(url)
	if err != nil {
		s.logger.Error(ctx, "sbaService.SyncPinToSsoV1: Failed to make GET request", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url))

		return fmt.Errorf("failed to make GET request: %v", err)
	}

	defer func(Body io.ReadCloser) {
		_ = Body.Close()
	}(resp.Body)

	// ตรวจสอบว่าการร้องขอสำเร็จหรือไม่
	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusNoContent {
		s.logger.Error(ctx, "sbaService.SyncPinToSsoV1: Got Error from downstream service", zap.Error(err), zap.String("custcode", custcode), zap.String("url", url), zap.Int("response_status_code", resp.StatusCode))
		return fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	return nil
}
