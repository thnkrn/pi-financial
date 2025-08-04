package ssodb_service

import (
	"context"
	"crypto/rand"
	"encoding/hex"
	"strings"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"

	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/config"
	constants "github.com/pi-financial/pi-sso-v2/const"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"github.com/pi-financial/pi-sso-v2/internal/repository/ssodb_repository"
	"github.com/pi-financial/pi-sso-v2/internal/repository/user_v2_repository"
	service "github.com/pi-financial/pi-sso-v2/internal/service/otp"
	"github.com/pi-financial/pi-sso-v2/internal/service/sba"
	"github.com/pi-financial/pi-sso-v2/internal/service/settrade"
	"github.com/pi-financial/pi-sso-v2/internal/types"
	"github.com/pi-financial/pi-sso-v2/internal/util"
)

type AuthService struct {
	AccountRepo             ssodb_repository.AccountRepository
	BiometricRepo           ssodb_repository.BiometricRepository
	PasswordResetRepo       ssodb_repository.PasswordResetRepository
	UserV2Repo              user_v2_repository.UserV2Repository
	Cfg                     config.Config
	OtpService              service.OtpService
	LoginWith2FASectionRepo ssodb_repository.LoginWith2FASectionRepository
	SettradeService         settrade.Service
	logger                  log.Logger
	SbaAccountService       sba.Service
}

func NewAuthService(
	logger log.Logger,
	accountRepo ssodb_repository.AccountRepository,
	biometricRepo ssodb_repository.BiometricRepository,
	passwordResetRepo ssodb_repository.PasswordResetRepository,
	userV2Repo user_v2_repository.UserV2Repository,
	cfg config.Config,
	otpService service.OtpService,
	loginWith2FASectionRepo ssodb_repository.LoginWith2FASectionRepository,
	settradeService settrade.Service,
	sbaAccountService sba.Service) AuthService {
	return AuthService{
		AccountRepo:             accountRepo,
		BiometricRepo:           biometricRepo,
		PasswordResetRepo:       passwordResetRepo,
		UserV2Repo:              userV2Repo,
		Cfg:                     cfg,
		OtpService:              otpService,
		LoginWith2FASectionRepo: loginWith2FASectionRepo,
		SettradeService:         settradeService,
		logger:                  logger,
		SbaAccountService:       sbaAccountService,
	}
}

func (s *AuthService) RefreshToken(accountId string, userId *string) (string, string, error) {
	// สร้าง Access Token
	token, err := util.GenerateAccessToken(s.Cfg, uuid.MustParse(accountId), userId)
	if err != nil {
		return "", "", err
	}
	// สร้าง Refresh Token
	refreshToken, err := util.GenerateRefreshToken(s.Cfg, uuid.MustParse(accountId), userId)
	if err != nil {
		return "", "", err
	}

	return token, refreshToken, nil
}

func (s *AuthService) Login(ctx context.Context, username, password string) (string, string, error) {
	// Decrypt the password using RSA.
	decryptedPassword, err := util.RsaDecryption(password)
	if err != nil {
		return "", "", constants.ErrDecrypt
	}
	password = decryptedPassword

	// Retrieve the account from the database by username.
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return "", "", constants.ErrInvalidUsernameOrPassword
	}

	// Check if the account is already locked.
	if account.IsLocked {
		return "", "", constants.ErrAccountLocked
	}

	// Verify the password using the VerifyPassword method (includes salt logic).
	failedAttempt, err := account.VerifyPassword(password)
	if err != nil {
		if failedAttempt >= 5 {
			// ตรวจสอบว่า username เป็น custcode ต้อง sync to settrade
			if util.IsCustCodeValid(username) {
				err := s.SettradeService.SendUnlockPasswordToSettrade(ctx, username, false)
				if err != nil {
					return "", "", constants.ErrSettradeAPIRequestFailed
				}
			}
		}
		s.logger.Error(ctx, "authService.Login VerifyPassword Failed", zap.Error(err))
		// Update login attempts in the database.
		err = s.AccountRepo.UpdateLoginAttempts(account)
		if err != nil {
			return "", "", constants.ErrFailedToUpdateLoginAttempts
		}
		return "", "", constants.ErrInvalidUsernameOrPassword
	}

	// On successful login, update (reset) the login attempts in the database.
	err = s.AccountRepo.UpdateLoginAttempts(account)
	if err != nil {
		return "", "", constants.ErrFailedToResetLoginAttempts
	}

	// Generate JWT access token.
	token, err := util.GenerateAccessToken(s.Cfg, account.ID, account.UserID)
	if err != nil {
		return "", "", err
	}

	// Generate JWT refresh token.
	refreshToken, err := util.GenerateRefreshToken(s.Cfg, account.ID, account.UserID)
	if err != nil {
		return "", "", err
	}

	return token, refreshToken, nil
}

func (s *AuthService) LoginWithOTP(ctx context.Context, username, password, deviceId string) (*types.EFinTradeLoginWithOtpResponse, error) {
	// Retrieve the account from the database by username.
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return nil, constants.ErrInvalidUsernameOrPassword
	}

	// Check if the account is locked.
	if account.IsLocked {
		return nil, constants.ErrAccountLocked
	}

	// Verify the password using the VerifyPassword method (handles salt logic).
	failedAttempt, err := account.VerifyPassword(password)
	if err != nil {
		if failedAttempt >= 5 {
			// ตรวจสอบว่า username เป็น custcode ต้อง sync to settrade
			if util.IsCustCodeValid(username) {
				err := s.SettradeService.SendUnlockPasswordToSettrade(ctx, username, false)
				if err != nil {
					return nil, constants.ErrSettradeAPIRequestFailed
				}
			}
		}
		// Update failed password attempts in the database.
		if err2 := s.AccountRepo.UpdateLoginAttempts(account); err2 != nil {
			return nil, constants.ErrFailedToUpdateLoginAttempts
		}
		return nil, constants.ErrInvalidUsernameOrPassword
	}

	// Reset the failed login attempts after successful password verification.
	if err = s.AccountRepo.UpdateLoginAttempts(account); err != nil {
		return nil, constants.ErrFailedToResetLoginAttempts
	}

	// Retrieve user information using account.UserID.
	userInfo, err := s.UserV2Repo.FindById(*account.UserID)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	if userInfo.Phone == nil {
		return nil, constants.ErrPhoneNotFound
	}

	// check username is not email
	if !util.IsEmailFormat(account.Username) {
		customerInfo, err := s.SbaAccountService.GetCustomerInfo(ctx, account.Username)
		if err != nil {
			return nil, constants.ErrGetCustomerInfo
		}

		if len(customerInfo.ResultList) == 1 {

			if customerInfo.ResultList[0].MobileNo != nil {
				userInfo.Phone = customerInfo.ResultList[0].MobileNo
			}
		}
	}

	// Create OTP session in the database.
	loginID := uuid.New()
	phone := strings.Replace(*userInfo.Phone, "-", "", -1)
	err = s.LoginWith2FASectionRepo.CreateLoginWith2FASection(ctx, loginID, *account.UserID, phone, deviceId, account.ID)
	if err != nil {
		return nil, err
	}

	// Retrieve customer account information.
	accountInfo, err := s.SbaAccountService.GetAccountInfo(ctx, username)
	if err != nil {
		return nil, constants.ErrGetCustomerInfo
	}

	efinUser, err := s.SbaAccountService.GetEfinUser(ctx, username)
	if err != nil {
		return nil, constants.ErrGetEfinUser
	}

	maskPhoneNumber, err := util.MaskFormattedPhoneNumber(phone)
	if err != nil {
		return nil, constants.ErrMaskPhoneNumber
	}

	// Prepare the OTP response.
	var response types.EFinTradeLoginWithOtpResponse
	response.OtpGenerateKey = loginID.String()
	response.RequireOtp = "Y"
	response.Autotrade = "N"
	response.PhoneNumber = maskPhoneNumber
	response.User = username
	response.Account = username + ""
	response.Efin = efinUser.UserName

	accountTab, err := s.SbaAccountService.GetAccountTabInfo(ctx, username)
	if err != nil {
		return nil, constants.ErrGetAccountTabInfo
	}

	for i := range accountInfo.AccountList {
		if accountInfo.AccountList[i].XchgMkt == "1" {
			cantrade := false
			// Filter accountTab by AccountNo and check if both CanBuy and CanSell are "Y".
			for _, accTab := range *accountTab {
				if accTab.AccountNo == strings.Replace(accountInfo.AccountList[i].Account, "-", "", -1) &&
					accTab.CanBuy == "Y" && accTab.CanSell == "Y" {
					cantrade = true
					break
				}
			}

			if cantrade {
				response.Account += "|E:" + strings.Replace(accountInfo.AccountList[i].Account, "-", "", -1) + ":Y"
			} else {
				response.Account += "|E:" + strings.Replace(accountInfo.AccountList[i].Account, "-", "", -1) + ":N"
			}
		}
	}

	return &response, nil
}

func (s *AuthService) GenerateOtp(ctx context.Context, otpRequestId string) (*types.GenerateOtpResponse, error) {
	session, err := s.LoginWith2FASectionRepo.FindByID(ctx, otpRequestId)
	if err != nil {
		return nil, constants.ErrOtpRequestNotFound
	}

	otpRef, err := s.OtpService.SendOtp(ctx, session.UserID, session.DeviceID, "mobile", "th", session.PhoneNumber)
	if err != nil {
		s.logger.Error(ctx, "authService.GenerateOtp SendOtp Failed", zap.Error(err))
		return nil, err
	}

	err = s.LoginWith2FASectionRepo.UpdateRefCode(ctx, session.ID, *otpRef)
	if err != nil {
		return nil, err
	}

	return &types.GenerateOtpResponse{
		RefCode:      *otpRef,
		VerifyOtpKey: session.ID.String(),
	}, nil

}

func (s *AuthService) VerifyOtpKey(ctx context.Context, otpRequestId, otpCode string) (string, string, error) {
	session, err := s.LoginWith2FASectionRepo.FindByID(ctx, otpRequestId)
	if err != nil {
		return "", "", constants.ErrOtpRequestNotFound
	}

	err = s.OtpService.SubmitOtp(ctx, session.UserID, session.DeviceID, "mobile", otpCode, session.RefCode)
	if err != nil {
		s.logger.Error(ctx, "authService.VerifyOtpKey SubmitOtp Failed", zap.Error(err))
		return "", "", constants.ErrInvalidOtpCode
	}

	err = s.LoginWith2FASectionRepo.UpdateIsVerify(ctx, session.ID, true)
	if err != nil {
		return "", "", constants.ErrFailedToUpdateIsVerify
	}

	// สร้าง JWT Token
	token, err := util.GenerateAccessToken(s.Cfg, session.AccountID, &session.UserID)
	if err != nil {
		return "", "", err
	}

	// สร้าง Refresh Token
	refreshToken, err := util.GenerateRefreshToken(s.Cfg, session.AccountID, &session.UserID)
	if err != nil {
		return "", "", err
	}

	return token, refreshToken, nil

}

// UnlockAccount unlocks the account by username and syncs with Settrade if required.
func (s *AuthService) UnlockAccount(ctx context.Context, username string) error {
	// Retrieve the account from the database by username.
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		s.logger.Error(ctx, "authService.UnlockAccount AccountId Not Found", zap.Error(err))
		return constants.ErrAccountIdNotFound
	}

	// Check if the account is locked and unlock it using the Account's UnlockAccount method.
	if account.IsLocked {
		// ตรวจสอบว่า username เป็น custcode ต้อง sync to settrade
		if util.IsCustCodeValid(username) {
			err := s.SettradeService.SendUnlockPasswordToSettrade(ctx, username, true)

			if err != nil {
				s.logger.Error(ctx, "authService.UnlockAccount unlock password from settrade service failed", zap.Error(err))
				return constants.ErrSettradeAPIRequestFailed
			}

			err = s.SettradeService.SendUnlockPinToSettrade(ctx, username, true)

			if err != nil {
				s.logger.Error(ctx, "authService.UnlockAccount unlock pin from settrade service failed", zap.Error(err))

				return constants.ErrSettradeAPIRequestFailed
			}
		}

		return s.AccountRepo.UnlockAccount(ctx, *account)
	}

	return nil
}

func (s *AuthService) UnlockPin(ctx context.Context, username string) error {

	// ดึงข้อมูลผู้ใช้ทั้งหมดตาม userId ครั้งเดียว
	_, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return constants.ErrAccountIdNotFound
	}

	// ตรวจสอบว่า username เป็น custcode ต้อง sync to settrade
	if util.IsCustCodeValid(username) {
		err := s.SettradeService.SendUnlockPinToSettrade(ctx, username, true)
		if err != nil {
			return constants.ErrSettradeAPIRequestFailed
		}
	}

	return nil
}

// GenerateSalt สร้าง Salt ขนาด 16 bytes
func GenerateSalt() (string, error) {
	salt := make([]byte, 16)
	_, err := rand.Read(salt)
	if err != nil {
		return "", err
	}
	return hex.EncodeToString(salt), nil
}

// RegisterBiometric - สร้าง Biometric Record
func (s *AuthService) RegisterBiometric(ctx context.Context, req types.RegisterBiometricRequest, deviceId, userId, accountId string) (*ssodb.Biometric, error) {
	decryptedToken, err := util.RsaDecryption(req.Token)
	if err != nil {
		return nil, constants.ErrDecrypt
	}

	hashedToken, err := util.HashPasswordWithSalt(decryptedToken, "")

	if err != nil {
		return nil, err
	}

	biometric, err := s.BiometricRepo.CreateBiometric(ctx, hashedToken, deviceId, userId, accountId)

	if err != nil {
		return nil, err
	}

	return biometric, nil
}

func (s *AuthService) LoginBiometric(ctx context.Context, req types.LoginBiometricRequest) (*string, *string, error) {
	decryptedToken, err := util.RsaDecryption(req.Token)

	if err != nil {
		return nil, nil, constants.ErrDecrypt
	}

	// Step 1: Find the account by username
	account, err := s.AccountRepo.FindByUsername(strings.ToLower(req.Username))

	if err != nil {
		return nil, nil, constants.ErrAccountIdNotFound
	}

	// Step 2: Find biometrics for the given account ID
	biometrics, err := s.BiometricRepo.FindBiometricsByAccountId(ctx, account.ID)

	if err != nil {
		return nil, nil, constants.ErrBiometricNotFound
	}

	// Step 3: Check if any stored biometric token matches the hashed token
	for _, biometric := range *biometrics {
		if util.CheckPasswordHashWithSalt(decryptedToken, "", biometric.Token) {
			// สร้าง JWT Token
			accessToken, err := util.GenerateAccessToken(s.Cfg, account.ID, account.UserID)
			if err != nil {
				return nil, nil, constants.ErrFailedToGenerateAccessToken
			}

			// สร้าง Refresh Token
			refreshToken, err := util.GenerateRefreshToken(s.Cfg, account.ID, account.UserID)
			if err != nil {
				return nil, nil, constants.ErrFailedToGenerateRefreshToken
			}

			return &accessToken, &refreshToken, nil
		}
	}

	// Step 4: If no matching biometric token was found, return an error
	return nil, nil, constants.ErrInvalidBiometricToken
}
