package repository

import (
	"context"
	"fmt"
	"github.com/google/uuid"
	otp "github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"github.com/pi-financial/pi-sso-v2/internal/util"
	"go.uber.org/zap"
	"gorm.io/gorm"
	"io"

	otpRepository "github.com/pi-financial/otp-srv/go-client"
)

type OtpRepository struct {
	db           *gorm.DB
	otpApiClient otpRepository.APIClient
	logger       log.Logger
}

func NewOtpRepository(logger log.Logger, db *gorm.DB, otpApiClient otpRepository.APIClient) OtpRepository {
	return OtpRepository{
		db:           db,
		otpApiClient: otpApiClient,
		logger:       logger,
	}
}

// Api Part
func (r *OtpRepository) parsePlatform(platform otp.Platform) (*otpRepository.OtpPlatform, error) {
	switch platform {
	case otp.Mobile:
		return otpRepository.SMS.Ptr(), nil
	case otp.Email:
		return otpRepository.EMAIL.Ptr(), nil
	default:
		return nil, fmt.Errorf("invalid platform")
	}
}

func (r *OtpRepository) SendOtp(ctx context.Context, userId string, deviceId string, platform otp.Platform, lang string, destination string) (*string, error) {
	parsedPlatform, err := r.parsePlatform(platform)

	if err != nil {
		r.logger.Error(ctx, "otpRepository.SendOTP failed to parse platform", zap.String("platform", string(platform)), zap.Error(err))
		return nil, err
	}

	reqBody := *otpRepository.NewSendOtpRequest()

	requestRef := uuid.New().String()
	reqBody.RequestRef.Set(&requestRef)

	reqBody.Destination.Set(&destination)
	reqBody.DeviceId = &deviceId
	reqBody.UserId = &userId

	r.logger.Info(
		ctx,
		"otpRepository.SendOTP sending OTP request",
		zap.String("requestRef", requestRef),
		zap.String("platform", string(platform)),
	)

	resp, _, err := r.otpApiClient.OtpAPI.
		InternalPlatformSendPost(ctx, *parsedPlatform).
		SendOtpRequest(reqBody).
		Lang(lang).
		Execute()

	if err != nil {
		r.logger.Error(
			ctx,
			"otpRepository.SendOTP failed",
			zap.String("requestRef", requestRef),
			zap.String("platform", string(platform)),
			zap.Error(err),
		)

		return nil, err
	}

	r.logger.Info(ctx,
		"otpRepository.SendOTP OTP sent successfully",
		zap.String("requestRef", requestRef),
		zap.Bool("otpRefPresent", resp.Data.OtpRef.IsSet()),
	)

	return resp.Data.OtpRef.Get(), nil
}

func (r *OtpRepository) ResendOtp(ctx context.Context, userId string, deviceId string, platform otp.Platform, lang string, otpRef string) (*string, error) {
	parsedPlatform, err := r.parsePlatform(platform)
	if err != nil {
		r.logger.Error(ctx, "otpRepository.ResendOtp failed to parse platform",
			zap.String("platform", string(platform)),
			zap.Error(err),
		)
		return nil, err
	}

	reqBody := *otpRepository.NewResendOtpRequest()
	reqBody.RefCode.Set(&otpRef)

	r.logger.Info(ctx, "otpRepository.ResendOtp sending OTP request",
		zap.String("otpRef", otpRef),
		zap.String("platform", string(platform)),
	)

	resp, _, err := r.otpApiClient.OtpAPI.
		SecurePlatformResendPut(ctx, *parsedPlatform).
		ResendOtpRequest(reqBody).
		Lang(lang).
		UserId(userId).
		DeviceId(deviceId).
		Execute()

	if err != nil {
		r.logger.Error(ctx, "otpRepository.ResendOtp failed API call",
			zap.String("otpRef", otpRef),
			zap.Error(err),
		)
		return nil, err
	}

	return resp.Data.RefCode.Get(), nil
}

func (r *OtpRepository) SubmitOtp(ctx context.Context, userId string, deviceId string, platform otp.Platform, otpCode string, otpRef string) error {
	parsedPlatform, err := r.parsePlatform(platform)
	if err != nil {
		r.logger.Error(ctx, "otpRepository.SubmitOtp failed to parse platform",
			zap.String("platform", string(platform)),
			zap.Error(err),
		)
		return err
	}

	reqBody := *otpRepository.NewSubmitOtpRequest()
	reqBody.OtpCode.Set(&otpCode)
	reqBody.RefCode.Set(&otpRef)

	r.logger.Info(ctx, "otpRepository.SubmitOtp submitting OTP",
		zap.String("otpRef", otpRef),
		zap.String("platform", string(platform)),
	)

	res, err := r.otpApiClient.OtpAPI.
		SecurePlatformSubmitPost(ctx, *parsedPlatform).
		SubmitOtpRequest(reqBody).
		UserId(userId).
		DeviceId(deviceId).
		Execute()

	if err != nil {
		if res == nil {
			r.logger.Error(
				ctx,
				"otpRepository.SubmitOtp response is nil",
			)

			return fmt.Errorf("otpRepository.SubmitOtp failed: %w", err)
		}

		bodyBytes, _ := io.ReadAll(res.Body)
		msg := *util.ExtractDetail(string(bodyBytes))

		r.logger.Error(ctx, "otpRepository.SubmitOtp failed",
			zap.String("otpRef", otpRef),
			zap.Error(err),
			zap.String("detail", msg),
		)
		return fmt.Errorf("%s", msg)
	}

	return nil
}
