package service

import (
	"context"
	otp "github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"github.com/pi-financial/pi-sso-v2/internal/repository"
	"go.uber.org/zap"
)

type OtpService struct {
	otpRepository repository.OtpRepository
	logger        log.Logger
}

func NewOtpService(logger log.Logger, otpRepository repository.OtpRepository) OtpService {
	return OtpService{
		otpRepository: otpRepository,
		logger:        logger,
	}
}

func (s *OtpService) SendOtp(ctx context.Context, userId string, deviceId string, platform otp.Platform, lang string, destination string) (*string, error) {
	otpRef, err := s.otpRepository.SendOtp(ctx, userId, deviceId, platform, lang, destination)
	if err != nil {
		s.logger.Error(ctx, "otpService.SendOTP failed", zap.Error(err))
		return nil, err
	}

	return otpRef, nil
}

func (s *OtpService) ResendOtp(ctx context.Context, userId string, deviceId string, platform otp.Platform, lang string, otpRef string) (*string, error) {
	newOtpRef, err := s.otpRepository.ResendOtp(ctx, userId, deviceId, platform, lang, otpRef)

	if err != nil {
		s.logger.Error(ctx, "otpService.ReSendOTP failed", zap.Error(err))

		return nil, err
	}

	return newOtpRef, nil
}

func (s *OtpService) SubmitOtp(ctx context.Context, userId string, deviceId string, platform otp.Platform, otpCode string, otpRef string) error {
	err := s.otpRepository.SubmitOtp(ctx, userId, deviceId, platform, otpCode, otpRef)
	if err != nil {
		s.logger.Error(ctx, "otpService.SubmitOtp failed", zap.Error(err))

		return err
	}
	return nil
}
