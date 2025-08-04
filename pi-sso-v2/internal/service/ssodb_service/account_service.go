package ssodb_service

import (
	"context"
	"errors"
	"fmt"
	"net/smtp"
	"os"
	"strconv"
	"strings"
	"time"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"github.com/samber/lo"
	"go.uber.org/zap"

	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/config"
	constants "github.com/pi-financial/pi-sso-v2/const"
	"github.com/pi-financial/pi-sso-v2/internal/domain"
	"github.com/pi-financial/pi-sso-v2/internal/repository/onboard_repository"
	"github.com/pi-financial/pi-sso-v2/internal/repository/user_v2_repository"
	"github.com/pi-financial/pi-sso-v2/internal/types"

	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"github.com/pi-financial/pi-sso-v2/internal/repository/ssodb_repository"
	service "github.com/pi-financial/pi-sso-v2/internal/service/otp"
	"github.com/pi-financial/pi-sso-v2/internal/service/sba"
	"github.com/pi-financial/pi-sso-v2/internal/service/settrade"
	"github.com/pi-financial/pi-sso-v2/internal/util"
	"gorm.io/gorm"

	gomail "gopkg.in/mail.v2"
)

type AccountService struct {
	AccountRepo                    ssodb_repository.AccountRepository
	PasswordResetRepository        ssodb_repository.PasswordResetRepository
	Cfg                            config.Config
	SettradeService                settrade.Service
	SendLinkAccountRepo            ssodb_repository.SendLinkAccountRepo
	OtpService                     service.OtpService
	GenerateOtpToEmailForSetupRepo ssodb_repository.GenerateOtpToEmailForSetupRepo
	GenerateOtpToPhoneForSetupRepo ssodb_repository.GenerateOtpToPhoneForSetupRepo
	SyncTokenRepo                  ssodb_repository.SyncTokenRepo
	SbaAccountService              sba.Service
	logger                         log.Logger
	UserV2Repo                     user_v2_repository.UserV2Repository
	OnboardRepo                    onboard_repository.OnboardRepository
}

// NewAccountService สร้าง service ใหม่
func NewAccountService(logger log.Logger, accountRepo ssodb_repository.AccountRepository, cfg config.Config, settradeService settrade.Service, passwordResetRepository ssodb_repository.PasswordResetRepository, sendLinkAccountRepo ssodb_repository.SendLinkAccountRepo, otpService service.OtpService, generateOtpToEmailForSetupRepo ssodb_repository.GenerateOtpToEmailForSetupRepo, generateOtpToPhoneForSetupRepo ssodb_repository.GenerateOtpToPhoneForSetupRepo, syncTokenRepo ssodb_repository.SyncTokenRepo, sbaAccountService sba.Service, userV2Repo user_v2_repository.UserV2Repository, onboardRepo onboard_repository.OnboardRepository) AccountService {
	return AccountService{
		AccountRepo:                    accountRepo,
		SettradeService:                settradeService,
		PasswordResetRepository:        passwordResetRepository,
		Cfg:                            cfg,
		SendLinkAccountRepo:            sendLinkAccountRepo,
		OtpService:                     otpService,
		GenerateOtpToEmailForSetupRepo: generateOtpToEmailForSetupRepo,
		GenerateOtpToPhoneForSetupRepo: generateOtpToPhoneForSetupRepo,
		SyncTokenRepo:                  syncTokenRepo,
		logger:                         logger,
		SbaAccountService:              sbaAccountService,
		UserV2Repo:                     userV2Repo,
		OnboardRepo:                    onboardRepo,
	}
}

// GetAllAccounts ดึงข้อมูลบัญชีทั้งหมด โดยเรียกใช้ account_repository
func (s *AccountService) GetAllAccounts(query types.GetAccountsQuery) ([]ssodb.Account, error) {
	// Build filter
	filters := map[string]interface{}{}
	if query.Username != "" {
		filters["username"] = query.Username
	}
	if query.UserId != "" {
		filters["user_id"] = query.UserId
	}

	// Get all accounts
	accounts, err := s.AccountRepo.GetAll(0, 0, filters)
	if err != nil {
		return nil, err
	}

	return accounts, nil
}

// GetLinkAccount ดึงข้อมูลบัญชีโดยใช้ Id
func (s *AccountService) GetLinkAccount(sendLinkAccountId string) (*ssodb.SendLinkAccount, error) {
	sendLinkAccount, err := s.SendLinkAccountRepo.FindById(sendLinkAccountId)
	if err != nil {
		return nil, constants.ErrSendLinkAccountFindById
	}

	if sendLinkAccount.IsUsed {
		return nil, constants.ErrSendLinkAccountIsUsed
	}

	return sendLinkAccount, nil
}

func (s *AccountService) LinkAccount(ctx context.Context, accountID string, sendLinkAccountId, custcode *string, encryptPassword string) (*ssodb.SendLinkAccount, error) {
	if sendLinkAccountId != nil {
		// ค้นหาข้อมูล account จาก database
		sendLinkAccount, err := s.SendLinkAccountRepo.FindById(*sendLinkAccountId)
		if err != nil {
			s.logger.Error(ctx, "accountService.LinkAccount Unable to find LinkAccountId", zap.Error(err), zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)))
			return nil, constants.ErrSendLinkAccountFindById
		}

		if sendLinkAccount.IsUsed {
			s.logger.Error(ctx, "accountService.LinkAccount LinkAccount already used", zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)))
			return nil, constants.ErrSendLinkAccountIsUsed
		}

		// ตรวจสอบว่า username เป็น email ไม่สามารถ link account ได้
		if !util.IsCustCodeValid(sendLinkAccount.Custcode) {
			s.logger.Error(ctx, "accountService.LinkAccount Failed Invalid Custcode", zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)))

			return nil, constants.ErrEmailUsernameCannotLinkAccount
		}
		custcode = &sendLinkAccount.Custcode
	} else if custcode != nil {
		if !util.IsCustCodeValid(*custcode) {
			return nil, constants.ErrEmailUsernameCannotLinkAccount
		}
		// ค้นหาข้อมูล account จาก database
		sendLinkAccount, err := s.SendLinkAccountRepo.FindByCustcode(*custcode)
		if err != nil {
			return nil, constants.ErrSendLinkAccountFindById
		}

		sendLinkAccountIdStr := sendLinkAccount.ID.String()
		sendLinkAccountId = &sendLinkAccountIdStr
	}

	checkDuplicateAccount, _ := s.AccountRepo.FindByUsername(util.SafeStringPtr(custcode))

	if checkDuplicateAccount != nil {
		s.logger.Error(ctx, "accountService.LinkAccount Unable To Link Account, AccountId is already existed for Custcode", zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)), zap.String("custcode", util.SafeStringPtr(custcode)))
		return nil, constants.ErrDuplicateUsername
	}

	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindById(accountID)
	if err != nil {
		s.logger.Error(ctx, "accountService.LinkAccount Unable To Link Account, Unable to find Email AccountID", zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)), zap.String("custcode", util.SafeStringPtr(custcode)), zap.String("accountId", accountID))
		return nil, constants.ErrAccountIdNotFound
	}

	if account.UserID == nil {
		s.logger.Error(ctx, "accountService.LinkAccount Unable To Link Account, Unable to find UserID", zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)), zap.String("custcode", util.SafeStringPtr(custcode)), zap.String("accountId", accountID))
		return nil, constants.ErrAccountIdNotFound
	}

	user, err := s.UserV2Repo.FindById(*account.UserID)

	if err != nil {
		s.logger.Error(ctx, "accountService.LinkAccount Unable To Link Account, Unable to Get UserByUserID", zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)), zap.String("custcode", util.SafeStringPtr(custcode)), zap.String("accountId", accountID))
		return nil, constants.ErrUserNotFound
	}

	customerInfo, err := s.SbaAccountService.GetCustomerInfo(ctx, util.SafeStringPtr(custcode))

	if err != nil || customerInfo == nil || customerInfo.ResultList == nil || len(customerInfo.ResultList) == 0 {
		s.logger.Error(ctx,
			"accountService.LinkAccount Unable To Link Account, Unable to Get SBA CustomerInfo",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
		)
		return nil, constants.ErrGetCustomerInfo
	}

	// ตรวจสอบ card id ถ้าไม่เป็นค่า nil
	if user.IdCardNo != nil {
		cardID := customerInfo.ResultList[0].CardID
		if cardID == nil {
			s.logger.Error(ctx,
				"accountService.LinkAccount CardID is nil while IdCardNo is provided",
				zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
				zap.String("custcode", util.SafeStringPtr(custcode)),
				zap.String("accountId", accountID),
			)
			return nil, constants.ErrCardIDNotMatch
		}
		if *cardID != *user.IdCardNo {
			s.logger.Error(ctx,
				"accountService.LinkAccount CardID mismatch",
				zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
				zap.String("custcode", util.SafeStringPtr(custcode)),
				zap.String("accountId", accountID),
			)
			return nil, constants.ErrCardIDNotMatch
		}
	}

	// อัปเดตข้อมูลผู้ใช้
	err = s.UserV2Repo.UpdateInfoUser(
		ctx,
		account.UserID,
		customerInfo.ResultList[0].CardID,
		customerInfo.ResultList[0].TName,
		customerInfo.ResultList[0].TSurname,
		customerInfo.ResultList[0].EName,
		customerInfo.ResultList[0].ESurname,
		customerInfo.ResultList[0].Birthday,
		customerInfo.ResultList[0].Email,
		customerInfo.ResultList[0].MobileNo,
	)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Unable to update UserInfo",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
		)

		return nil, constants.ErrUpdateUserInfo
	}

	decryptPassword, err := util.RsaDecryption(encryptPassword)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Sync Password to Settrade Unable to decrypt password",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
		)

		return nil, constants.ErrDecrypt
	}

	// เทียบ password ในฐานข้อมูล
	if err := CheckPassword(*account, decryptPassword, s.Cfg.EncryptKey); err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Sync Password to Settrade Password Incorrect",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
		)
		return nil, constants.ErrPasswordNotMatch
	}

	// ตรวจสอบความถูกต้องของ CustCode และส่งคำขอ Sync Password
	if err := s.syncPasswordWithSettrade(ctx, util.SafeStringPtr(custcode), decryptPassword); err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Sync Password to Settrade Failed",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
			zap.Error(err),
		)
		return nil, constants.ErrFailedToSendSettradeRequest
	}

	newSalt, err := GenerateSalt()
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Creating New CustCode Account Failed unable to generate salt",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
			zap.Error(err),
		)
		return nil, constants.ErrFailedToGenerateSalt
	}

	// สร้าง hash สำหรับรหัสผ่านใหม่
	hashedNewPassword, err := util.HashPasswordWithSalt(decryptPassword, newSalt)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Creating New CustCode Account Failed unable to hash password",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
			zap.Error(err),
		)
		return nil, constants.ErrFailedToHashNewPassword
	}

	// สร้าง account ใหม่
	err = s.AccountRepo.CreateMemberAccount(ctx, util.SafeStringPtr(custcode), &hashedNewPassword, &newSalt, account.UserID)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Creating New CustCode Account Failed unable to create",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
			zap.Error(err),
		)
		return nil, constants.ErrCreateAccount
	}

	err = s.SettradeService.SyncMemberToSsoV1(ctx, util.SafeStringPtr(custcode))
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Sync Member to SSO V1 Failed",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
			zap.Error(err),
		)
		return nil, constants.ErrFailedToSendSyncMembeToSsoV1
	}

	// อัปเดต sendLinkAccount ให้เป็น isUsed = true
	updatedSendLinkAccount, err := s.SendLinkAccountRepo.UpdateUserId(ctx, util.SafeStringPtr(sendLinkAccountId), *account.UserID)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Creating New CustCode Account Failed unable to generate salt",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
			zap.String("accountId", accountID),
			zap.Error(err),
		)
		return nil, constants.ErrUpdateSendLinkAccount
	}

	// create user account
	err = s.UserV2Repo.CreateUserAccount(ctx, *account.UserID, util.SafeStringPtr(custcode))
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Unable to create user account",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
		)
	}

	// sync user to userV2
	err = s.UserV2Repo.Sync(ctx, util.SafeStringPtr(custcode), user_v2_repository.SyncTypeAll)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Unable to sync user to userV2",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			zap.String("custcode", util.SafeStringPtr(custcode)),
		)
	}

	// get user info from userV2
	userInfo, err := s.UserV2Repo.GetUserByCustomerCode(ctx, util.SafeStringPtr(custcode))
	if err != nil {
		s.logger.Error(ctx,
			"accountService.LinkAccount Unable to get user info from userV2",
			zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
		)
	}
	_, haveGlobalEquity := lo.Find(userInfo.TradingAccounts, func(account string) bool {
		return account == fmt.Sprintf("%s-2", util.SafeStringPtr(custcode))
	})
	if haveGlobalEquity {
		err := s.OnboardRepo.GlobalEquityMapping(ctx, util.SafeStringPtr(custcode))
		if err != nil {
			s.logger.Error(ctx,
				"accountService.LinkAccount Unable to map global equity",
				zap.String("linkAccountId", util.SafeStringPtr(sendLinkAccountId)),
			)
		}
	}

	return updatedSendLinkAccount, nil
}

func (s *AccountService) ResetPassword(ctx context.Context, token string, newPassword string) error {
	resetToken, err := s.PasswordResetRepository.CheckAvailableByToken(ctx, token)
	if err != nil {
		if err.Error() == "token expired" {
			return constants.ErrTokenExpired
		} else if err.Error() == "token already used" {
			return constants.ErrTokenAlreadyUsed
		}
		return constants.ErrTokenNotFound
	}

	// ดึงข้อมูลผู้ใช้ทั้งหมดตาม userId
	acc, err := s.AccountRepo.FindById(resetToken.AccountID)
	if err != nil {
		return constants.ErrAccountIdNotFound
	}

	// ตรวจสอบว่า username เป็น custcode ต้อง sync to settrade
	if util.IsCustCodeValid(acc.Username) {
		// เรียกฟังก์ชันส่ง request ไปยัง Settrade API
		err := s.SettradeService.SendSettradeRequestSyncPassword(ctx, acc.Username, newPassword)
		if err != nil {
			return constants.ErrSendSettradeRequest
		}
	}

	salt, err := GenerateSalt()
	if err != nil {
		return constants.ErrFailedToGenerateSalt
	}

	// สร้าง hash สำหรับรหัสผ่านใหม่
	hashedNewPassword, err := util.HashPasswordWithSalt(newPassword, salt)
	if err != nil {
		return constants.ErrFailedToHashPassword
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	acc.Password = &hashedNewPassword

	if err = s.AccountRepo.UpdatePasswordAndSaltByAccountId(acc.ID.String(), *acc.Password, salt); err != nil {
		return constants.ErrFailedToUpdatePassword
	}

	// อัปเดต token ถูกใช้งานแล้ว
	err = s.PasswordResetRepository.UpdateTokenStatus(ctx, token)
	if err != nil {
		return constants.ErrUpdateTokenStatus
	}

	return nil
}

func (s *AccountService) ResetPinByRequest(ctx context.Context, token string, newPin string) error {
	resetToken, err := s.PasswordResetRepository.CheckAvailableByToken(ctx, token)
	if err != nil {
		if err.Error() == "token expired" {
			return constants.ErrTokenExpired
		} else if err.Error() == "token already used" {
			return constants.ErrTokenAlreadyUsed
		}
		return constants.ErrTokenNotFound
	}

	// ดึงข้อมูลผู้ใช้ทั้งหมดตาม userId
	acc, err := s.AccountRepo.FindById(resetToken.AccountID)
	if err != nil {
		return constants.ErrAccountIdNotFound
	}

	// ตรวจสอบว่า username เป็น custcode ต้อง sync to settrade
	if util.IsCustCodeValid(acc.Username) {
		// เรียกฟังก์ชันส่ง request ไปยัง Settrade API
		err := s.SettradeService.SendSettradeRequestSyncPin(ctx, acc.Username, newPin)
		if err != nil {
			return constants.ErrSendSettradeRequest
		}
	}

	salt, err := GenerateSalt()
	if err != nil {
		return constants.ErrFailedToGenerateSalt
	}

	// สร้าง hash สำหรับรหัสผ่านใหม่
	hashedNewPin, err := util.HashPasswordWithSalt(newPin, salt)
	if err != nil {
		return constants.ErrFailedToHashPin
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	acc.Pin = &hashedNewPin

	if err = s.AccountRepo.UpdatePinAndSaltByAccountId(acc.ID.String(), *acc.Pin, salt); err != nil {
		return constants.ErrUpdatePinFailed
	}

	// อัปเดต token ถูกใช้งานแล้ว
	err = s.PasswordResetRepository.UpdateTokenStatus(ctx, token)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.ResetPinByRequest UpdateTokenStatus Failed",
			zap.String("token", token),
			zap.String("accountId", acc.ID.String()),
			zap.Error(err),
		)
		return constants.ErrUpdateTokenStatus
	}

	s.logger.Info(ctx, "accountService.ResetPinByRequest UpdateTokenStatus Success")
	return nil
}

// ChangePassword ตรวจสอบรหัสผ่านเดิมและเปลี่ยนรหัสผ่านใหม่
func (s *AccountService) ChangePassword(ctx context.Context, userId, username, oldPassword, newPassword string) error {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUserId(userId, username)
	if err != nil {
		return errors.New("E114")
	}

	// ตรวจสอบรหัสผ่านเดิม
	if err := CheckPassword(account, oldPassword, s.Cfg.EncryptKey); err != nil {
		return err
	}

	// ตรวจสอบว่า username เป็น email ไม่ต้องเซ็ทพิน
	if util.IsCustCodeValid(username) {
		// เรียกฟังก์ชันส่ง request ไปยัง Settrade API
		err := s.SettradeService.SendSettradeRequestSyncPassword(ctx, username, newPassword)
		if err != nil {
			return err
		}
	}

	salt, err := GenerateSalt()
	if err != nil {
		return errors.New("E120")
	}

	// สร้าง hash สำหรับรหัสผ่านใหม่
	hashedNewPassword, err := util.HashPasswordWithSalt(newPassword, salt)
	if err != nil {
		return errors.New("E121")
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	account.Password = &hashedNewPassword

	return s.AccountRepo.UpdatePasswordAndSaltByAccountId(account.ID.String(), *account.Password, salt)
}

// CheckPassword ตรวจสอบความถูกต้องของรหัสผ่านเก่า
func CheckPassword(account ssodb.Account, oldPassword string, encryptionKeyString string) error {
	if account.SaltPassword != nil {
		// ใช้ bcrypt และ Salt
		if !util.CheckPasswordHashWithSalt(oldPassword, *account.SaltPassword, *account.Password) {
			username, err := ssodb.DecryptUsername(account.Username, encryptionKeyString)
			if err != nil {
				return errors.New("Old password of " + username + " is incorrect")
			}
			return errors.New("Old password of " + username + " is incorrect")
		}
	} else {
		// ใช้ฟังก์ชัน Hash แบบเดิม (SHA-1)
		hashedOldPassword := util.Hash(oldPassword, "")
		if *account.Password != hashedOldPassword {
			username, err := ssodb.DecryptUsername(account.Username, encryptionKeyString)
			if err != nil {
				return errors.New("Old password of " + username + " is incorrect")
			}
			return errors.New("Old password of " + username + " is incorrect")
		}
	}
	return nil
}

func generateUUIDToken() (string, error) {
	token, err := uuid.NewRandom()
	if err != nil {
		return "", fmt.Errorf("failed to generate UUID token: %w", err)
	}
	return token.String(), nil
}

func (s *AccountService) SendResetPasswordLink(ctx context.Context, user domain.User, username string) error {
	// ตรวจสอบว่า Email ไม่เป็น nil และแปลงค่าเป็น string
	if user.Email == nil {
		return constants.ErrEmailNotFound
	}
	email := strings.ToLower(*user.Email) // แปลงจาก *string เป็น string

	if !util.IsEmailFormat(email) {
		return constants.ErrInvalidEmailFormat
	}

	// สร้าง token และกำหนดวันหมดอายุ
	token, err := generateUUIDToken()
	if err != nil {
		return constants.ErrGenerateUUIDToken
	}
	resetPasswordExpired := os.Getenv("RESET_PASSWORD_EXPIRATION")
	minute, err := strconv.Atoi(resetPasswordExpired)
	if err != nil {
		// Handle error
		return constants.ErrConvertStringToInt
	}

	// กำหนดวันหมดอายุของ token เป็น 30 นาที
	expiresAt := time.Now().Add(time.Duration(minute) * time.Minute)

	// find account by username
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return constants.ErrAccountNotFound
	}

	err = s.PasswordResetRepository.Create(ctx, account.ID.String(), token, expiresAt)
	if err != nil {
		return constants.ErrCreatePasswordReset
	}

	// FIX
	to := []string{email}
	linkResetPassword := os.Getenv("LINK_RESET_PASSWORD")

	// ส่งอีเมลที่มีลิงก์รีเซ็ตรหัสผ่าน
	resetLink := fmt.Sprintf("%s?token=%s", linkResetPassword, token)
	linkWebTrading := os.Getenv("LINK_WEB_TRADING")

	// เตรียมข้อมูล HTML และแทนที่ placeholders
	htmlTemplate, err := os.ReadFile("email_template/reset_password_template.html")
	if err != nil {
		return constants.ErrReadHTMLTemplate
	}

	body := string(htmlTemplate)
	body = strings.ReplaceAll(body, "{USERNAME}", username)
	body = strings.ReplaceAll(body, "{LINK_RESET}", resetLink)
	body = strings.ReplaceAll(body, "{LINK_WEBSITE}", linkWebTrading)

	// subject := "Test Email"
	// body := `<h1>Hello, this is a test email!</h1><p>This email is sent from a Go application using SMTP.</p><br> reset password link : ` + resetLink

	subject := "Password Reset Request"

	// กำหนดค่าของ SMTPConfig
	smtpConfig := SMTPConfig{
		Host:     s.Cfg.Smtp.Host,
		Port:     s.Cfg.Smtp.Port,
		Username: s.Cfg.Smtp.Username,
		Password: s.Cfg.Smtp.Password,
	}

	// กำหนดค่าของ EmailContent
	emailContent := EmailContent{
		From:    s.Cfg.Smtp.Form,
		To:      to,
		Subject: subject,
		Body:    body,
	}

	// ส่งอีเมลที่มีลิงก์รีเซ็ตรหัสผ่าน
	err = s.SendEmailVia365(smtpConfig, emailContent)
	if err != nil {
		return constants.ErrSendEmail
	}

	s.logger.Info(ctx, "accountService.SendResetPasswordLink Send reset password email success")

	return nil
}

// SendResetPinLink SendResetPinLink
func (s *AccountService) SendResetPinLink(ctx context.Context, user domain.User, username string) error {
	// ตรวจสอบว่า Email ไม่เป็น nil และแปลงค่าเป็น string
	if user.Email == nil {
		return constants.ErrEmailNotFound
	}
	email := *user.Email // แปลงจาก *string เป็น string

	if !util.IsEmailFormat(email) {
		return constants.ErrInvalidEmailFormat
	}

	// สร้าง token และกำหนดวันหมดอายุ
	token, err := generateUUIDToken()
	if err != nil {
		return constants.ErrGenerateUUIDToken
	}
	resetPasswordExpired := os.Getenv("RESET_PASSWORD_EXPIRATION")
	minute, err := strconv.Atoi(resetPasswordExpired)
	if err != nil {
		// Handle error
		return constants.ErrConvertStringToInt
	}

	// กำหนดวันหมดอายุของ token เป็น 30 นาที
	expiresAt := time.Now().Add(time.Duration(minute) * time.Minute)

	// find account by username
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return constants.ErrAccountNotFound
	}

	err = s.PasswordResetRepository.Create(ctx, account.ID.String(), token, expiresAt)
	if err != nil {
		return constants.ErrCreatePasswordReset
	}

	// FIX
	to := []string{email}
	linkResetPassword := os.Getenv("LINK_RESET_PIN")

	// ส่งอีเมลที่มีลิงก์รีเซ็ตรหัสผ่าน
	resetLink := fmt.Sprintf("%s?token=%s", linkResetPassword, token)
	linkWebTrading := os.Getenv("LINK_WEB_TRADING")

	// เตรียมข้อมูล HTML และแทนที่ placeholders
	htmlTemplate, err := os.ReadFile("email_template/reset_pin_template.html")
	if err != nil {
		return constants.ErrReadHTMLTemplate
	}

	body := string(htmlTemplate)
	body = strings.ReplaceAll(body, "{USERNAME}", username)
	body = strings.ReplaceAll(body, "{LINK_RESET}", resetLink)
	body = strings.ReplaceAll(body, "{LINK_WEBSITE}", linkWebTrading)

	// subject := "Test Email"
	// body := `<h1>Hello, this is a test email!</h1><p>This email is sent from a Go application using SMTP.</p><br> reset password link : ` + resetLink

	subject := "Pin Reset Request"

	// กำหนดค่าของ SMTPConfig
	smtpConfig := SMTPConfig{
		Host:     s.Cfg.Smtp.Host,
		Port:     s.Cfg.Smtp.Port,
		Username: s.Cfg.Smtp.Username,
		Password: s.Cfg.Smtp.Password,
	}

	// กำหนดค่าของ EmailContent
	emailContent := EmailContent{
		From:    s.Cfg.Smtp.Form,
		To:      to,
		Subject: subject,
		Body:    body,
	}

	// ส่งอีเมลที่มีลิงก์รีเซ็ตรหัสผ่าน
	err = s.SendEmailVia365(smtpConfig, emailContent)
	if err != nil {
		return constants.ErrSendEmail
	}

	s.logger.Info(ctx, "accountService.SendResetPinLink Send reset pin email success")

	return nil

}

type SMTPConfig struct {
	Host     string
	Port     string
	Username string
	Password string
}

type EmailContent struct {
	From    string
	To      []string
	Subject string
	Body    string
}

func (s *AccountService) SendEmail(config SMTPConfig, content EmailContent) error {

	// เตรียมส่วนหัวของอีเมล
	headers := make(map[string]string)
	headers["From"] = content.From
	headers["To"] = strings.Join(content.To, ",")
	headers["Subject"] = content.Subject
	headers["MIME-Version"] = "1.0"
	headers["Content-Type"] = "text/html; charset=\"UTF-8\""

	// รวมส่วนหัวและข้อความของอีเมล
	message := ""
	for key, value := range headers {
		message += fmt.Sprintf("%s: %s\r\n", key, value)
	}
	message += "\r\n" + content.Body

	// ดึงอีเมลสำหรับ BCC จาก environment variable
	bccEmail := os.Getenv("SMTP_BCC")

	// รวมผู้รับทั้งหมด รวมถึง BCC
	allRecipients := append(content.To, bccEmail)

	// ส่งอีเมล (Send Email / 发送邮件 (Fā sòng yóu jiàn))
	err := smtp.SendMail(
		config.Host+":"+config.Port,
		nil,
		content.From,
		allRecipients, // ส่งให้ทั้งผู้รับปกติและ BCC
		[]byte(message),
	)
	if err != nil {
		return fmt.Errorf("failed to send email: %w", err)
	}

	return nil
}

func (s *AccountService) SendEmailVia365(config SMTPConfig, content EmailContent) error {
	fmt.Printf("config: %+v\n", config)
	// ดึง BCC จาก ENV
	bccEmail := os.Getenv("SMTP_BCC")

	// สร้าง email ด้วย gomail
	m := gomail.NewMessage()
	m.SetHeader("From", content.From)
	m.SetHeader("To", content.To...)
	if bccEmail != "" {
		m.SetHeader("Bcc", bccEmail)
	}
	m.SetHeader("Subject", content.Subject)
	m.SetBody("text/html", content.Body)

	// Dialer สำหรับ Office365
	d := gomail.NewDialer(config.Host, 587, config.Username, config.Password)
	d.StartTLSPolicy = gomail.MandatoryStartTLS

	// ส่งอีเมล
	if err := d.DialAndSend(m); err != nil {
		fmt.Println("❌ Error sending email via gomail:", err)
		return fmt.Errorf("failed to send email via gomail: %w", err)
	}

	fmt.Println("✅ Email sent successfully via gomail")
	return nil
}

func (s *AccountService) VerifyUserDetails(ctx context.Context, username, idCardNo, birthday string) (*domain.User, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return nil, constants.ErrAccountNotFound
	}

	if account.UserID == nil {
		return nil, constants.ErrUserIdNotFound
	}

	user, err := s.UserV2Repo.FindById(*account.UserID)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}
	if *user.IdCardNo != idCardNo {
		s.logger.Error(ctx, "accountService.VerifyUserDetails IdCardNo not match", zap.String("username", username))

		return nil, constants.ErrIdCardNoNotMatch
	}

	if *user.Birthday != birthday {
		s.logger.Error(ctx, "accountService.VerifyUserDetails Birthday not match", zap.String("username", username))
		return nil, constants.ErrBirthdayNotMatch
	}

	return user, nil

}

func (s *AccountService) VerifyUserUsernameDetails(username string) (*domain.User, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return nil, constants.ErrAccountNotFound
	}

	if account.UserID == nil {
		return nil, constants.ErrUserIdNotFound
	}

	user, err := s.UserV2Repo.FindById(*account.UserID)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	return user, nil
}

// ResetPin ResetPin
func (s *AccountService) ResetPin(ctx context.Context, userId, username, password, newPin string) error {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUserId(userId, username)
	if err != nil {
		return errors.New("E114")
	}

	// ตรวจสอบรหัสผ่านเดิม
	if err := CheckPassword(account, password, s.Cfg.EncryptKey); err != nil {
		return err
	}

	// ตรวจสอบว่า username เป็น email ไม่ต้องเซ็ทพิน
	if util.IsCustCodeValid(username) {
		// เรียกฟังก์ชันส่ง request ไปยัง Settrade API
		err := s.SettradeService.SendSettradeRequestSyncPin(ctx, username, newPin)
		if err != nil {
			return err
		}
	}

	salt, err := GenerateSalt()
	if err != nil {
		return errors.New("E120")
	}

	// สร้าง hash สำหรับรหัสพินใหม่
	hashedNewPin, err := util.HashPasswordWithSalt(newPin, salt)
	if err != nil {
		return errors.New("failed to hash new pin")
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	account.Pin = &hashedNewPin

	err = s.AccountRepo.UpdatePinAndSaltByAccountId(account.ID.String(), *account.Pin, salt)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.ResetPin UpdatePinAndSaltByAccountId Failed",
			zap.String("accountId", account.ID.String()),
			zap.String("newPin", *account.Pin),
			zap.String("salt", salt),
			zap.Error(err),
		)
		return constants.ErrUpdatePinAndSalt
	}
	s.logger.Info(ctx, "accountService.ResetPin UpdatePinAndSaltByAccountId Success", zap.String("accountId", account.ID.String()))
	return nil
}

func (s *AccountService) ResetPinAll(ctx context.Context, userId, password, newPin string) error {
	// ค้นหาข้อมูลผู้ใช้จาก database
	accounts, err := s.AccountRepo.FindByUserIds(userId)
	if err != nil {
		return errors.New("E114")
	}

	for _, account := range accounts {

		// ตรวจสอบรหัสผ่านเดิม
		if account.SaltPassword != nil {
			// ใช้ bcrypt และ Salt
			if !util.CheckPasswordHashWithSalt(password, *account.SaltPassword, *account.Password) {
				return errors.New("E124")
			}
		} else {
			// ใช้ฟังก์ชัน Hash แบบเดิม (SHA-1)
			hashedOldPassword := util.Hash(password, "")
			if *account.Password != hashedOldPassword {
				return errors.New("E124")
			}
		}

		username, err := ssodb.DecryptUsername(account.Username, s.Cfg.EncryptKey)

		if err != nil {
			return errors.New("E123")
		}

		// ตรวจสอบว่า username เป็น custcode ถึงจะเซ็ทพิน
		if util.IsCustCodeValid(username) {
			// เรียกฟังก์ชันส่ง request ไปยัง Settrade API
			err := s.SettradeService.SendSettradeRequestSyncPin(ctx, username, newPin)
			if err != nil {
				return err
			}

			salt, err := GenerateSalt()
			if err != nil {
				return errors.New("E120")
			}

			// สร้าง hash สำหรับรหัสพินใหม่
			hashedNewPin, err := util.HashPasswordWithSalt(newPin, salt)
			if err != nil {
				return errors.New("E125")
			}

			// อัปเดตรหัสผ่านในฐานข้อมูล
			account.Pin = &hashedNewPin

			err = s.AccountRepo.UpdatePinAndSaltByAccountId(account.ID.String(), *account.Pin, salt)
			if err != nil {
				s.logger.Error(ctx,
					"accountService.ResetPin UpdatePinAndSaltByAccountId Failed",
					zap.String("accountId", account.ID.String()),
					zap.String("newPin", *account.Pin),
					zap.String("salt", salt),
					zap.Error(err),
				)
				return err
			}
		}

	}
	s.logger.Info(ctx, "accountService.ResetPinAll Reset pin for all accounts success", zap.String("userId", userId))
	return nil
}

func (s *AccountService) ResetPinByList(ctx context.Context, custcodes []string, accountId, password, newPin string) error {
	// ดึงข้อมูลผู้ใช้ทั้งหมดตาม userId ครั้งเดียว
	account, err := s.AccountRepo.FindById(accountId)
	if err != nil {
		return constants.ErrAccountIdNotFound
	}

	// ตรวจสอบรหัสผ่านเดิม
	if err := CheckPassword(*account, password, s.Cfg.EncryptKey); err != nil {
		return constants.ErrPasswordNotMatch
	}

	users, err := s.AccountRepo.FindByUserIds(*account.UserID)
	if err != nil {
		return constants.ErrUserNotFound
	}
	// สร้าง map สำหรับเก็บข้อมูล user โดย key คือ username
	userMap := make(map[string]ssodb.Account)
	for _, user := range users {
		userMap[user.UsernameHash] = user
	}
	// ตรวจสอบว่ารายการใน usernames มีอยู่ใน users ทั้งหมดหรือไม่
	if err := CheckUsernamesExistsInUsers(custcodes, users); err != nil {
		return constants.ErrUserNotFound
	}

	// วนลูปสำหรับ usernames โดยไม่ต้องดึงข้อมูลจากฐานข้อมูลซ้ำ
	for _, username := range custcodes {
		// ค้นหาข้อมูลจาก map แทนการ query database
		hashUsername := ssodb.HashUsername(username)
		account, ok := userMap[hashUsername]
		if !ok {
			return constants.ErrUserNotFound
		}

		// ตรวจสอบว่า username เป็น email ถ้าใช่ให้ข้ามไปรายการถัดไป
		if util.IsEmailFormat(username) {
			s.logger.Info(ctx, "Skipping email username", zap.String("username", username))
			continue
		}

		// เรียกฟังก์ชันส่ง request ไปยัง Settrade API
		err := s.SettradeService.SendSettradeRequestSyncPin(ctx, username, newPin)
		if err != nil {
			return constants.ErrSettradeAPIRequest
		}

		// สร้าง Salt สำหรับพินใหม่
		salt, err := GenerateSalt()
		if err != nil {
			return constants.ErrFailedToGenerateSalt
		}

		// สร้าง hash สำหรับรหัสพินใหม่
		hashedNewPin, err := util.HashPasswordWithSalt(newPin, salt)
		if err != nil {
			return constants.ErrFailedToHashNewPin
		}

		// อัปเดตรหัสพินและ Salt ในฐานข้อมูลสำหรับผู้ใช้
		err = s.AccountRepo.UpdatePinAndSaltByAccountId(account.ID.String(), hashedNewPin, salt)
		if err != nil {
			s.logger.Error(ctx,
				"accountService.ResetPinByList UpdatePinAndSaltByAccountId Failed",
				zap.String("accountId", account.ID.String()),
				zap.String("newPin", hashedNewPin),
				zap.String("salt", salt),
				zap.Error(err),
			)
			return constants.ErrFailedToUpdatePin
		}
	}
	s.logger.Info(ctx, "accountService.ResetPinByList Reset pin for all accounts success", zap.String("userId", *account.UserID))
	return nil
}

// ChangePasswordByUserId เปลี่ยนรหัสผ่านใหม่
func (s *AccountService) ChangePasswordByUserId(ctx context.Context, req types.InternalChangePasswordRequest) error {
	// ค้นหาผู้ใช้จาก userId
	users, err := s.AccountRepo.FindByUserIds(req.UserId)
	if err != nil {
		return constants.ErrUserNotFound
	}

	decryptPassword, err := util.RsaDecryption(req.NewPassword)
	if err != nil {
		s.logger.Error(ctx, "accountService.ChangePasswordByUserId Error decrypt: %v", zap.Error(err))
		return constants.ErrDecrypt
	}
	req.NewPassword = decryptPassword

	if len(req.Username) == 0 {
		// อัปเดตรหัสผ่านสำหรับผู้ใช้ทั้งหมด
		for _, user := range users {
			if err := s.updateUserPassword(ctx, user.Username, req.NewPassword); err != nil {

				return err
			}
		}
	} else {

		// ตรวจสอบว่ามี usernames ทั้งหมดใน users หรือไม่
		if err := CheckUsernamesExistsInUsers(req.Username, users); err != nil {
			return err
		}

		// อัปเดตรหัสผ่านสำหรับแต่ละ username
		for _, username := range req.Username {
			if err := s.updateUserPassword(ctx, username, req.NewPassword); err != nil {
				return err
			}
		}
	}
	return nil
}

// อัปเดตรหัสผ่านของผู้ใช้โดยการตรวจสอบและอัปเดตไปยัง AccountRepo
func (s *AccountService) updateUserPassword(ctx context.Context, username, newPassword string) error {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return constants.ErrUserNotFound
	}

	// ตรวจสอบความถูกต้องของ CustCode และส่งคำขอ Sync Password
	if util.IsCustCodeValid(username) {
		if err := s.syncPasswordWithSettrade(ctx, username, newPassword); err != nil {
			return constants.ErrFailedToSendSettradeRequest
		}
	}

	// สร้าง salt และแฮชรหัสผ่านใหม่
	salt, err := GenerateSalt()
	if err != nil {
		return constants.ErrFailedToGenerateSalt
	}
	hashedNewPassword, err := util.HashPasswordWithSalt(newPassword, salt)
	if err != nil {
		return constants.ErrFailedToHashNewPassword
	}

	// อัปเดตข้อมูลใน AccountRepo
	account.Password = &hashedNewPassword
	err = s.AccountRepo.UpdatePasswordAndSaltByAccountId(account.ID.String(), *account.Password, salt)
	if err != nil {
		return constants.ErrFailedToUpdateLoginAttempts
	}
	return nil
}

// ส่งคำขอ Sync Password ไปยัง settrade API
func (s *AccountService) syncPasswordWithSettrade(ctx context.Context, username, newPassword string) error {
	err := s.SettradeService.SendSettradeRequestSyncPassword(ctx, username, newPassword)
	if err != nil {
		return constants.ErrFailedToSendSettradeRequest
	}
	return nil
}

// ChangePin ตรวจสอบรหัสพินเดิมและเปลี่ยนพินใหม่
func (s *AccountService) ChangePin(ctx context.Context, userId string, req types.ChangePinRequest) error {
	// decrypt pin
	decryptNewPin, err := util.RsaDecryption(req.NewPin)
	if err != nil {
		s.logger.Error(ctx, "accountService.ChangePin Decryption failed:", zap.Error(err))
		return constants.ErrDecrypt
	}

	isSequencePin := util.HasSequenceNumbers(decryptNewPin, 6)
	if isSequencePin {
		return constants.ErrPinSequence
	}

	decryptOldPin, err := util.RsaDecryption(req.OldPin)
	if err != nil {
		return constants.ErrDecrypt
	}

	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUserId(userId, req.Custcode)
	if err != nil {
		return constants.ErrUserNotFound
	}

	// ตรวจสอบว่า username เป็น email ไม่ต้องเซ็ทพิน
	if util.IsEmailFormat(req.Custcode) {
		return constants.ErrEmailUsernameCannotSetPin
	}

	// ตรวจสอบ Pin มีค่าเท่ากับ nil ให้คืนค่าว่ายังไม่ได้กำหนด Pin เป็นภาษาอังกฤษ
	if account.Pin == nil {
		return constants.ErrPinNotSet
	}

	// ตรวจสอบรหัสผ่านเดิม
	if account.SaltPin != nil {
		// ใช้ bcrypt และ Salt
		if !util.CheckPasswordHashWithSalt(decryptOldPin, *account.SaltPin, *account.Pin) {
			return constants.ErrOldPinIncorrect
		}
	} else {
		// ใช้ฟังก์ชัน Hash แบบเดิม (SHA-1)
		hashedOldPin := util.Hash(decryptOldPin, "")
		if *account.Pin != hashedOldPin {
			return constants.ErrOldPinIncorrect
		}
	}

	// เรียกฟังก์ชันส่ง request ไปยัง Settrade API
	err = s.SettradeService.SendSettradeRequestSyncPin(ctx, req.Custcode, decryptNewPin)
	if err != nil {
		return err
	}

	salt, err := GenerateSalt()
	if err != nil {
		return constants.ErrFailedToGenerateSalt
	}

	// สร้าง hash สำหรับรหัสพินใหม่
	hashedNewPin, err := util.HashPasswordWithSalt(decryptNewPin, salt)
	if err != nil {
		return constants.ErrFailedToHashNewPin
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	account.Pin = &hashedNewPin

	err = s.AccountRepo.UpdatePinAndSaltByAccountId(account.ID.String(), *account.Pin, salt)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.ChangePin UpdatePinAndSaltByAccountId Failed",
			zap.String("accountId", account.ID.String()),
			zap.String("newPin", *account.Pin),
			zap.String("salt", salt),
			zap.Error(err),
		)
		return constants.ErrUpdatePinAndSalt
	}

	s.logger.Info(ctx, "accountService.ChangePin Change pin success", zap.String("userId", userId))
	return nil
}

// ChangePinOtw ตรวจสอบรหัสพินเดิมและเปลี่ยนพินใหม่
func (s *AccountService) ChangePinOtw(ctx context.Context, userId string, req types.ChangePinOTWRequest) error {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUserId(userId, req.Custcode)
	if err != nil {
		return constants.ErrUserNotFound
	}

	// ตรวจสอบว่า username เป็น email ไม่ต้องเซ็ทพิน
	if util.IsEmailFormat(req.Custcode) {
		return constants.ErrEmailUsernameCannotSetPin
	}

	// ตรวจสอบ Pin มีค่าเท่ากับ nil ให้คืนค่าว่ายังไม่ได้กำหนด Pin เป็นภาษาอังกฤษ
	if account.Pin == nil {
		return constants.ErrPinNotSet
	}

	// ตรวจสอบรหัสผ่านเดิม
	if account.SaltPassword != nil {
		// ใช้ bcrypt และ Salt
		if !util.CheckPasswordHashWithSalt(req.Password, *account.SaltPassword, *account.Password) {
			return constants.ErrPasswordIncorrect
		}
	} else {
		// ใช้ฟังก์ชัน Hash แบบเดิม (SHA-1)
		hashedPassword := util.Hash(req.Password, "")
		if *account.Password != hashedPassword {
			return constants.ErrPasswordIncorrect
		}
	}

	// เรียกฟังก์ชันส่ง request ไปยัง Settrade API
	err = s.SettradeService.SendSettradeRequestSyncPin(ctx, req.Custcode, req.NewPin)
	if err != nil {
		return err
	}

	salt, err := GenerateSalt()
	if err != nil {
		return constants.ErrFailedToGenerateSalt
	}

	// สร้าง hash สำหรับรหัสพินใหม่
	hashedNewPin, err := util.HashPasswordWithSalt(req.NewPin, salt)
	if err != nil {
		return constants.ErrFailedToHashNewPin
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	account.Pin = &hashedNewPin

	err = s.AccountRepo.UpdatePinAndSaltByAccountId(account.ID.String(), *account.Pin, salt)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.ChangePinOtw UpdatePinAndSaltByAccountId Failed",
			zap.String("accountId", account.ID.String()),
			zap.String("newPin", *account.Pin),
			zap.String("salt", salt),
			zap.Error(err),
		)
		return constants.ErrUpdatePinAndSalt
	}
	s.logger.Info(ctx, "accountService.ChangePinOtw Change pin success", zap.String("userId", userId))
	return nil
}

func (s *AccountService) VerifyPin(ctx context.Context, userId string, rawCustcode, pin string) error {
	// Retrieve account from the database by userId and rawCustcode.
	account, err := s.AccountRepo.FindByUserId(userId, rawCustcode)
	if err != nil {
		s.logger.Error(ctx, "accountService.VerifyPin FindByUserId Failed", zap.String("userId", userId), zap.Error(err))
		return constants.ErrUserNotFound
	}

	// Check if the username is in email format; if so, PIN should not be set.
	if util.IsEmailFormat(rawCustcode) {
		s.logger.Info(ctx, "accountService.VerifyPin Skipping email username")
		return constants.ErrEmailUsernameCannotSetPin
	}

	if account.IsLocked {
		s.logger.Error(ctx, "accountService.VerifyPin Account is locked", zap.String("userId", userId))
		return constants.ErrAccountLocked
	}
	// Check if the PIN is set.
	if account.Pin == nil {
		s.logger.Error(ctx, "accountService.VerifyPin PIN not set", zap.String("userId", userId))
		return constants.ErrPinNotSet
	}
	// Use the Account's VerifyPIN method to validate the PIN.
	failedAttempt, err := account.VerifyPIN(pin)
	if err != nil {
		if failedAttempt >= 5 {
			if util.IsCustCodeValid(account.Username) {
				err := s.SettradeService.SendUnlockPinToSettrade(ctx, account.Username, false)
				if err != nil {
					s.logger.Error(ctx, "accountService.VerifyPin SendUnlockPinToSettrade Failed", zap.String("username", account.Username), zap.Error(err))
					return constants.ErrSettradeAPIRequestFailed
				}
			}
		}
		// Update login attempts in the database.
		err = s.AccountRepo.UpdateLoginAttempts(&account)
		if err != nil {
			s.logger.Error(ctx, "accountService.VerifyPin UpdateLoginAttempts Failed", zap.String("userId", userId), zap.Error(err))
			return constants.ErrFailedToUpdateLoginAttempts
		}
		return constants.ErrPinIncorrect
	}

	err = s.AccountRepo.UpdateLoginAttempts(&account)
	if err != nil {
		s.logger.Error(ctx, "accountService.VerifyPin UpdateLoginAttempts Failed", zap.String("userId", userId), zap.Error(err))
		return constants.ErrFailedToUpdateLoginAttempts
	}
	s.logger.Info(ctx, "accountService.VerifyPin Verify PIN success", zap.String("userId", userId), zap.String("rawCustcode", rawCustcode))
	return nil
}

// CheckUsernamesExistsInUsers ตรวจสอบว่า username อยู่ใน users ทั้งหมดที่ดึงมาจาก userId หรือไม่
func CheckUsernamesExistsInUsers(usernames []string, accounts []ssodb.Account) error {
	userMap := make(map[string]bool)
	for _, user := range accounts {
		userMap[user.UsernameHash] = true
	}

	for _, username := range usernames {
		hashUsername := ssodb.HashUsername(username)
		if _, exists := userMap[hashUsername]; !exists {
			return constants.ErrUserNotFound
		}
	}
	return nil
}

func CheckUsernamesExistsInUser(username string, accounts []ssodb.Account) error {
	userMap := make(map[string]bool)
	for _, user := range accounts {
		userMap[user.UsernameHash] = true
	}

	hashUsername := ssodb.HashUsername(username)
	if _, exists := userMap[hashUsername]; !exists {
		return constants.ErrUserNotFound
	}

	return nil
}

func (s *AccountService) CreatePin(ctx context.Context, userId string, req types.CreatePinRequest) error {
	users, err := s.AccountRepo.FindByUserIds(userId)
	if err != nil {
		return constants.ErrUserNotFound
	}

	userMap := make(map[string]ssodb.Account)
	for _, user := range users {
		userMap[user.UsernameHash] = user
	}

	if err := CheckUsernamesExistsInUsers(req.Custcode, users); err != nil {
		return constants.ErrUserNotFound
	}

	for _, username := range req.Custcode {
		hashUsername := ssodb.HashUsername(username)
		account, ok := userMap[hashUsername]
		fmt.Printf("hashUsername %s: \n", hashUsername)
		fmt.Printf("account %s: \n", account.ID.String())
		if !ok {
			return constants.ErrUserNotFoundInPreloadedData
		}

		if !util.IsCustCodeValid(username) {
			s.logger.Info(ctx, "Skipping email username", zap.String("userId", userId))
			continue
		}

		err := s.SettradeService.SendSettradeRequestSyncPin(ctx, username, req.NewPin)
		if err != nil {
			return constants.ErrSettradeAPIRequestFailed
		}

		salt, err := GenerateSalt()
		if err != nil {
			return constants.ErrFailedToGenerateSalt
		}

		hashedNewPin, err := util.HashPasswordWithSalt(req.NewPin, salt)
		if err != nil {
			return constants.ErrFailedToHashNewPassword
		}

		err = s.AccountRepo.UpdatePinAndSaltByAccountId(account.ID.String(), hashedNewPin, salt)
		if err != nil {
			s.logger.Error(ctx,
				"accountService.CreatePin UpdatePinAndSaltByAccountId Failed",
				zap.String("accountId", account.ID.String()),
				zap.String("newPin", hashedNewPin),
				zap.String("salt", salt),
				zap.Error(err),
			)
			return constants.ErrFailedToUpdatePinAndSalt
		}
	}
	s.logger.Info(ctx, "accountService.CreatePin Create pin success", zap.String("userId", userId))
	return nil
}

func (s *AccountService) ForceChangePin(ctx context.Context, req types.ForceChangePinRequest) error {
	users, err := s.AccountRepo.FindByUsername(req.Username)
	if err != nil {
		return constants.ErrUserNotFound
	}

	if !util.IsCustCodeValid(req.Username) {
		s.logger.Info(ctx, "accountService.ForceChangePin Skipping email username")
		return constants.ErrEmailUsernameCannotSetPin
	}

	err = s.SettradeService.SendSettradeRequestSyncPin(ctx, req.Username, req.NewPin)
	if err != nil {
		return constants.ErrSettradeAPIRequestFailed
	}

	salt, err := GenerateSalt()
	if err != nil {
		return constants.ErrFailedToGenerateSalt
	}

	hashedNewPin, err := util.HashPasswordWithSalt(req.NewPin, salt)
	if err != nil {
		return constants.ErrFailedToHashNewPassword
	}

	err = s.AccountRepo.UpdatePinAndSaltByAccountId(users.ID.String(), hashedNewPin, salt)
	if err != nil {
		s.logger.Error(ctx,
			"accountService.ForceChangePin UpdatePinAndSaltByAccountId Failed",
			zap.String("accountId", users.ID.String()),
			zap.String("newPin", hashedNewPin),
			zap.String("salt", salt),
			zap.Error(err),
		)
		return constants.ErrFailedToUpdatePinAndSalt
	}
	s.logger.Info(ctx, "accountService.ForceChangePin Force change pin success", zap.String("userId", *users.UserID))
	return nil
}

// CreateAccountByTrading - สร้างบัญชีใหม่
func (s *AccountService) CreateAccountByTrading(ctx context.Context, req types.CreateAccountRequest) error {
	s.logger.Info(ctx, "accountService.CreateAccountByTrading request", zap.String("username", req.Username))
	// ตรวจสอบว่าชื่อผู้ใช้มีอยู่ในระบบแล้วหรือไม่
	account, err := s.AccountRepo.FindByUsername(req.Username)
	if err != nil && !errors.Is(err, gorm.ErrRecordNotFound) { // ตรวจสอบ error ว่าไม่ใช่ error ที่บอกว่า record ไม่พบ
		return constants.ErrFindByUsername
	}

	customerInfo, err := s.SbaAccountService.GetCustomerInfo(ctx, req.Username)
	if err != nil {
		return constants.ErrGetCustomerInfo
	}

	user, err := s.UserV2Repo.FindUserIdByCustCode(ctx, req.Username)
	if err != nil {
		return constants.ErrFindByEmail
	}

	if customerInfo.ResultList[0].Email == nil {
		if user == nil {
			// สร้าง user ใหม่
			user, err = s.UserV2Repo.CreateUserInfo(ctx, nil, customerInfo.ResultList[0].MobileNo, customerInfo.ResultList[0].CardID, customerInfo.ResultList[0].TName, customerInfo.ResultList[0].TSurname, customerInfo.ResultList[0].EName, customerInfo.ResultList[0].ESurname, customerInfo.ResultList[0].WealthType)
			if err != nil {
				return constants.ErrCreateUser
			}
		}

		if account == nil {
			// สร้างบัญชีใหม่
			err = s.AccountRepo.CreateAccountByTrading(req.Username, req.Password, req.Pin, &user.Id)
			if err != nil {
				s.logger.Error(ctx, "accountService.CreateAccountByTrading Error accountRepo CreateAccountByTrading", zap.Error(err))
				return constants.ErrCreateAccount
			}
		} else {
			// อัปเดตรหัสผ่านและพินในฐานข้อมูล
			err = s.AccountRepo.UpdateAccountTradingSync(req.Username, &req.Password, req.Pin)
			if err != nil {
				return constants.ErrUpdatePassword
			}
		}
		return nil
	}

	if user == nil {
		// re get by email
		user, err = s.UserV2Repo.FindByEmail(customerInfo.ResultList[0].Email)
		if err != nil {
			if err.Error() == "Email is required" {
				return constants.ErrEmailNotFound
			}

			if !strings.Contains(err.Error(), "404 Not Found") {
				return constants.ErrFindByEmail
			}
		}

		if user == nil {
			chkAccount, err := s.AccountRepo.FindByUsername(strings.ToLower(*customerInfo.ResultList[0].Email))
			if err != nil && !errors.Is(err, gorm.ErrRecordNotFound) { // ตรวจสอบ error ว่าไม่ใช่ error ที่บอกว่า record ไม่พบ
				return constants.ErrFindByUsername
			}
			if chkAccount != nil {
				user, err = s.UserV2Repo.FindById(*chkAccount.UserID)
				if err != nil {
					return constants.ErrFindByUsername
				}
			}
		}

		if user == nil {
			// สร้าง user ใหม่
			user, err = s.UserV2Repo.CreateUserInfo(ctx, customerInfo.ResultList[0].Email, customerInfo.ResultList[0].MobileNo, customerInfo.ResultList[0].CardID, customerInfo.ResultList[0].TName, customerInfo.ResultList[0].TSurname, customerInfo.ResultList[0].EName, customerInfo.ResultList[0].ESurname, customerInfo.ResultList[0].WealthType)
			if err != nil {
				return constants.ErrCreateUser
			}
		}
	}

	if account == nil {
		// สร้างบัญชีใหม่
		err = s.AccountRepo.CreateAccountByTrading(req.Username, req.Password, req.Pin, &user.Id)

		if err != nil {
			s.logger.Error(ctx, "accountService.CreateAccountByTrading Error accountRepo CreateAccountByTrading", zap.Error(err))

			return constants.ErrCreateAccount
		}
	}

	if user == nil {
		return constants.ErrUserNotFound
	}

	// อัปเดตข้อมูลผู้ใช้
	err = s.UserV2Repo.UpdateInfoUser(
		ctx,
		&user.Id,
		customerInfo.ResultList[0].CardID,
		customerInfo.ResultList[0].TName,
		customerInfo.ResultList[0].TSurname,
		customerInfo.ResultList[0].EName,
		customerInfo.ResultList[0].ESurname,
		customerInfo.ResultList[0].Birthday,
		customerInfo.ResultList[0].Email,
		customerInfo.ResultList[0].MobileNo,
	)

	if err != nil {
		return constants.ErrUpdateInfoUser
	}

	return nil
}

// CreateAccountByTradingV2 - สร้างบัญชีใหม่
func (s *AccountService) CreateAccountByTradingV2(ctx context.Context, req types.CreateAccountRequest) error {
	s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Request Username", zap.String("username", req.Username))

	// ตรวจสอบว่าชื่อผู้ใช้มีอยู่ในระบบแล้วหรือไม่
	account, err := s.AccountRepo.FindByUsername(req.Username)
	if err != nil && !errors.Is(err, gorm.ErrRecordNotFound) {
		s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error FindByUsername failed:", zap.Error(err))
		return constants.ErrFindByUsername
	}

	s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Account found")

	customerInfo, err := s.SbaAccountService.GetCustomerInfo(ctx, req.Username)
	if err != nil {
		s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error GetCustomerInfo failed for username", zap.String("username", req.Username), zap.Error(err))
		return constants.ErrGetCustomerInfo
	}

	if len(customerInfo.ResultList) == 0 {
		s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error CustomerInfo ResultList is empty for username", zap.String("username", req.Username))
		return constants.ErrCustomerNotFound
	}

	cust := customerInfo.ResultList[0]
	s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 CustomerInfo CustCode", zap.String("custcode", util.SafeStringPtr(cust.CustCode)))

	user, err := s.UserV2Repo.FindUserIdByCustCode(ctx, *cust.CustCode)
	if err != nil {
		s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error FindUserIdByCustCode failed for custCode", zap.String("custcode", util.SafeStringPtr(cust.CustCode)), zap.Error(err))
		return constants.ErrFindByCustCode
	}

	s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 User found by CustCode", zap.String("custcode", util.SafeStringPtr(cust.CustCode)))

	if cust.Email == nil {
		if user == nil {
			user, err = s.UserV2Repo.CreateUserInfo(ctx, nil, cust.MobileNo, cust.CardID, cust.TName, cust.TSurname, cust.EName, cust.ESurname, cust.WealthType)
			if err != nil {
				s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error CreateUserV2 failed", zap.Error(err))
				return constants.ErrCreateUser
			}

			s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Created new user (no email)", zap.String("userId", user.Id))
		}

		if account == nil {
			err = s.AccountRepo.CreateAccountByTrading(req.Username, req.Password, req.Pin, &user.Id)
			if err != nil {
				s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error CreateAccountByTrading failed", zap.Error(err))
				return constants.ErrCreateAccount
			}
			s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Created new account for username", zap.String("username", req.Username))
		} else {
			err = s.AccountRepo.UpdateAccountTradingSync(req.Username, &req.Password, req.Pin)
			if err != nil {
				s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error UpdateAccountTradingSync failed: %v", zap.Error(err))
				return constants.ErrUpdatePassword
			}
			s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Updated existing account for username: %s", zap.String("username", req.Username))
		}
		return nil
	}

	if user == nil {
		if cust.Email != nil {
			user, err = s.UserV2Repo.FindByEmail(cust.Email)
			if err != nil &&
				!errors.Is(err, gorm.ErrRecordNotFound) &&
				!strings.Contains(err.Error(), "404 Not Found") {

				s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error FindByEmail failed", zap.Error(err))
				return constants.ErrFindByEmail
			}

			if user == nil {
				chkAccount, err := s.AccountRepo.FindByUsername(strings.ToLower(*cust.Email))
				if err != nil && !errors.Is(err, gorm.ErrRecordNotFound) {
					s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error FindByUsername(email) failed", zap.Error(err))
					return constants.ErrFindByUsername
				}

				if chkAccount != nil {
					user, err = s.UserV2Repo.FindById(*chkAccount.UserID)
					if err != nil {
						s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error FindById failed", zap.Error(err))
						return constants.ErrFindByUserId
					}
				}
			}
		}

		if user == nil {
			user, err = s.UserV2Repo.CreateUserInfo(ctx, cust.Email, cust.MobileNo, cust.CardID, cust.TName, cust.TSurname, cust.EName, cust.ESurname, cust.WealthType)
			if err != nil {
				s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error CreateUser failed: %v", zap.Error(err))
				return constants.ErrCreateUser
			}
			s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Created new user (with email)")
		}
	}

	if account == nil {
		err = s.AccountRepo.CreateAccountByTrading(req.Username, req.Password, req.Pin, &user.Id)
		if err != nil {
			s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error CreateAccountByTrading failed", zap.Error(err))
			return constants.ErrCreateAccount
		}
		s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Created new account for username: %s", zap.String("username", req.Username))
	} else {
		err = s.AccountRepo.UpdateAccountTradingSync(req.Username, &req.Password, req.Pin)
		if err != nil {
			s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error UpdateAccountTradingSync failed: %v", zap.Error(err))
			return constants.ErrUpdatePassword
		}
		s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Updated existing account for username: %s", zap.String("username", req.Username))
	}

	err = s.UserV2Repo.UpdateInfoUser(ctx, &user.Id, cust.CardID, cust.TName, cust.TSurname, cust.EName, cust.ESurname, cust.Birthday, cust.Email, cust.MobileNo)
	if err != nil {
		s.logger.Error(ctx, "accountService.CreateAccountByTradingV2 Error UpdateInfoUser failed", zap.Error(err))
		return constants.ErrUpdateInfoUser
	}

	s.logger.Info(ctx, "accountService.CreateAccountByTradingV2 Updated user info successfully for userID", zap.String("userId", user.Id))

	return nil
}

// AccountTradingSync TradingSync
func (s *AccountService) AccountTradingSync(ctx context.Context, req types.AccountTradingSyncRequest) error {
	// ตรวจสอบว่าชื่อผู้ใช้มีอยู่ในระบบแล้วหรือไม่
	account, err := s.AccountRepo.FindByUsername(req.Username)
	if err != nil && !errors.Is(err, gorm.ErrRecordNotFound) { // ตรวจสอบ error ว่าไม่ใช่ error ที่บอกว่า record ไม่พบ
		return constants.ErrFindByUsername
	}

	customerInfo, err := s.SbaAccountService.GetCustomerInfo(ctx, req.Username)
	if err != nil {
		return constants.ErrGetCustomerInfo
	}

	if customerInfo.ResultList[0].Email != nil {
		if account == nil {
			// สร้างบัญชีใหม่
			err = s.AccountRepo.CreateAccountByTrading(req.Username, *req.Password, req.Pin, nil)
			if err != nil {
				s.logger.Error(ctx, "accountService.AccountTradingSync Failed Unable To CreateAccountByTrading", zap.Error(err))
				return constants.ErrCreateAccount
			}
		} else {
			// อัปเดตรหัสผ่านและพินในฐานข้อมูล
			err = s.AccountRepo.UpdateAccountTradingSync(req.Username, req.Password, req.Pin)
			if err != nil {
				return constants.ErrUpdatePassword
			}
		}
		return nil
	}

	user, err := s.UserV2Repo.FindUserIdByCustCode(ctx, req.Username)
	if err != nil {
		return constants.ErrFindByUsername
	}

	if user == nil {
		// re get by email
		user, err = s.UserV2Repo.FindByEmail(customerInfo.ResultList[0].Email)
		if err != nil {
			if err.Error() != "Email is required" {
				return constants.ErrEmailNotFound
			}

			return constants.ErrUserNotFound
		}

		if user == nil {
			// สร้าง user ใหม่
			user, err = s.UserV2Repo.CreateUserInfo(ctx, nil, customerInfo.ResultList[0].MobileNo, customerInfo.ResultList[0].CardID, customerInfo.ResultList[0].TName, customerInfo.ResultList[0].TSurname, customerInfo.ResultList[0].EName, customerInfo.ResultList[0].ESurname, customerInfo.ResultList[0].WealthType)
			if err != nil {
				return constants.ErrCreateUser
			}
		}
	}

	if account == nil {
		// สร้างบัญชีใหม่
		err = s.AccountRepo.CreateAccountByTrading(req.Username, *req.Password, req.Pin, &user.Id)
		if err != nil {
			s.logger.Error(ctx, "accountService.AccountTradingSync Failed Unable To CreateAccountByTrading 2", zap.Error(err))
			return constants.ErrCreateAccount
		}
	} else {
		// อัปเดตรหัสผ่านและพินในฐานข้อมูล
		err = s.AccountRepo.UpdateAccountTradingSync(req.Username, req.Password, req.Pin)
		if err != nil {
			return constants.ErrUpdatePassword
		}
	}

	// อัปเดตข้อมูลผู้ใช้
	err = s.UserV2Repo.UpdateInfoUser(
		ctx,
		&user.Id,
		customerInfo.ResultList[0].CardID,
		customerInfo.ResultList[0].TName,
		customerInfo.ResultList[0].TSurname,
		customerInfo.ResultList[0].EName,
		customerInfo.ResultList[0].ESurname,
		customerInfo.ResultList[0].Birthday,
		customerInfo.ResultList[0].Email,
		customerInfo.ResultList[0].MobileNo,
	)

	if err != nil {
		return constants.ErrUpdateInfoUser
	}

	return nil
}

// RegisterGuest - ฟังก์ชันสำหรับการลงทะเบียน guest
func (s *AccountService) RegisterGuest(ctx context.Context, req types.GuestRegisterRequest) (*types.GuestRegisterResponse, error) {
	decryptPassword, err := util.RsaDecryption(req.Password)
	if err != nil {
		return nil, constants.ErrDecryptPassword
	}

	password := decryptPassword
	username := strings.ToLower(req.Username)

	// ตรวจสอบว่าชื่อผู้ใช้มีอยู่ในระบบแล้วหรือไม่
	_, err = s.AccountRepo.FindByUsername(username)
	if err == nil {
		return nil, constants.ErrUsernameExists
	} else if !errors.Is(err, gorm.ErrRecordNotFound) {
		// ตรวจสอบ error ว่าไม่ใช่ error ที่บอกว่า record ไม่พบ
		return nil, err
	}

	// สร้าง user ใหม่
	user, err := s.UserV2Repo.CreateUserInfo(ctx, &username, req.PhoneNumber, nil, nil, nil, nil, nil, nil)
	if err != nil {
		s.logger.Error(ctx, "accountService.RegisterGuest Failed Create User", zap.Error(err))
		return nil, errors.New("Failed to create user, error: " + err.Error())
	}

	// สร้าง salt ใหม่
	salt, err := GenerateSalt()
	if err != nil {
		return nil, constants.ErrFailedToGenerateSalt
	}

	// สร้าง hash สำหรับรหัสผ่านใหม่
	hashedNewPassword, err := util.HashPasswordWithSalt(password, salt)
	if err != nil {
		return nil, constants.ErrFailedToHashNewPassword
	}

	// อัปเดตรหัสผ่านและ Salt ในฐานข้อมูล
	account, err := s.AccountRepo.CreateGuestAccount(username, hashedNewPassword, salt, user.Id)
	if err != nil {
		s.logger.Error(ctx, "accountService.RegisterGuest Failed CreateAccount", zap.Error(err))
		return nil, constants.ErrUpdatePassword
	}

	token, err := util.GenerateAccessToken(s.Cfg, account.ID, account.UserID)
	if err != nil {
		return nil, err
	}

	refreshToken, err := util.GenerateRefreshToken(s.Cfg, account.ID, account.UserID)
	if err != nil {
		return nil, err
	}

	return &types.GuestRegisterResponse{
		AccountId:          account.ID.String(),
		UserId:             user.Id,
		AccessToken:        token,
		AccessTokenExpiry:  strconv.Itoa(s.Cfg.JwtExpiration) + "h",
		RefreshToken:       refreshToken,
		RefreshTokenExpiry: strconv.Itoa(s.Cfg.RefreshExpiration) + "h",
	}, nil
}

func (s *AccountService) IsUsernameExisted(username string) bool {
	_, err := s.AccountRepo.FindByUsername(username)

	if err == nil {
		return true
	} else if !errors.Is(err, gorm.ErrRecordNotFound) {
		return false
	}

	return false
}

func (s *AccountService) IsPhoneNumberExisted(ctx context.Context, phoneNumber string) bool {
	_, err := s.UserV2Repo.FindByPhoneNumber(ctx, phoneNumber)

	return err == nil
}

func (s *AccountService) CheckSyncedPin(ctx context.Context, req types.CheckSyncedPinRequest, userId string) (types.CheckSyncedPinResponse, error) {

	response := types.CheckSyncedPinResponse{Result: false}
	// ค้นหาตาม userId 	ทั้งหมด
	accounts, err := s.AccountRepo.FindByUserIds(userId)
	if err != nil {
		return response, constants.ErrUserNotFound
	}

	// ตรวจสอบ custcode มีอยู่ใน accounts ทั้งหมดหรือไม่
	if err := CheckUsernamesExistsInUser(req.Custcode, accounts); err != nil {
		return response, err
	}

	hashUsername := ssodb.HashUsername(req.Custcode)

	// FILLTER accounts by req.Custcode
	var filteredAccounts ssodb.Account
	for _, account := range accounts {

		if account.UsernameHash == hashUsername {
			filteredAccounts = account
			break
		}
	}

	if filteredAccounts.Pin == nil {
		return response, nil
	}
	response.Result = true
	return response, nil

}

func (s *AccountService) CheckSyncedPinByInternal(ctx context.Context, req types.CheckSyncedPinRequest) (*types.CheckSyncedPinResponse, error) {

	// ค้นหาตาม userId 	ทั้งหมด
	account, err := s.AccountRepo.FindByUsername(req.Custcode)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	if account.Pin == nil {
		return &types.CheckSyncedPinResponse{Result: false}, nil
	}

	return &types.CheckSyncedPinResponse{Result: true}, nil
}

func (s *AccountService) MigrateGuest(ctx context.Context, req types.MigrateGuestAccountRequest) (*ssodb.Account, error) {
	// TODO: move to utils
	decryptPassword, err := util.RsaDecryption(req.Password)
	if err != nil {
		return nil, constants.ErrDecrypt
	}

	// ตรวจสอบว่าชื่อผู้ใช้มีอยู่ในระบบแล้วหรือไม่
	_, err = s.AccountRepo.FindByUsername(req.Username)
	if err == nil {
		return nil, constants.ErrDuplicateUsername
	} else if !errors.Is(err, gorm.ErrRecordNotFound) {
		// ตรวจสอบ error ว่าไม่ใช่ error ที่บอกว่า record ไม่พบ
		s.logger.Info(ctx, "accountService.MigrateGuest FindByUsername ERROR", zap.Error(err))
		return nil, err
	}

	// สร้าง salt ใหม่
	salt, err := GenerateSalt()
	if err != nil {
		return nil, constants.ErrFailedToGenerateSalt
	}

	// สร้าง hash สำหรับรหัสผ่านใหม่
	hashedNewPassword, err := util.HashPasswordWithSalt(decryptPassword, salt)
	if err != nil {
		return nil, constants.ErrFailedToHashNewPassword
	}

	// อัปเดตรหัสผ่านและ Salt ในฐานข้อมูล
	account, err := s.AccountRepo.CreateGuestAccount(strings.ToLower(req.Username), hashedNewPassword, salt, req.UserId)
	if err != nil {
		s.logger.Info(ctx, "accountService.MigrateGuest CreateGuestAccount ERROR", zap.Error(err))
		return nil, err
	}

	return account, nil
}

// GetUserIdByCustomerCode GetUserIdByCustomerCode
func (s *AccountService) GetUserIdByCustomerCode(ctx context.Context, custcode string) (*types.UserIdByCustomerCodeResponse, error) {
	user, err := s.UserV2Repo.FindUserIdByCustCode(ctx, custcode)
	if err != nil {
		return nil, err
	}

	var result = &types.UserIdByCustomerCodeResponse{}
	result.UserId = user.Id

	return result, nil
}

func (s *AccountService) GetCustomerInfoByCustomerCode(ctx context.Context, customerCode string) (*domain.User, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUsername(customerCode)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	user, err := s.UserV2Repo.FindById(*account.UserID)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	return user, nil
}

func (s *AccountService) GetAccountInfoByCustomerCode(ctx context.Context, customerCode string) (*ssodb.Account, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUsername(customerCode)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	return account, nil
}

func (s *AccountService) UpdateUserInfo(ctx context.Context, custcode string) (*domain.User, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUsername(custcode)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	user, err := s.UserV2Repo.FindById(*account.UserID)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	customerInfo, err := s.SbaAccountService.GetCustomerInfo(ctx, custcode)

	if err != nil {
		return nil, constants.ErrGetCustomerInfo
	}

	// อัปเดตข้อมูลผู้ใช้
	err = s.UserV2Repo.UpdateInfoUser(
		ctx,
		&user.Id,
		customerInfo.ResultList[0].CardID,
		customerInfo.ResultList[0].TName,
		customerInfo.ResultList[0].TSurname,
		customerInfo.ResultList[0].EName,
		customerInfo.ResultList[0].ESurname,
		customerInfo.ResultList[0].Birthday,
		customerInfo.ResultList[0].Email,
		customerInfo.ResultList[0].MobileNo,
	)

	if err != nil {
		return nil, constants.ErrUpdateInfoUser
	}

	user, err = s.UserV2Repo.FindById(*account.UserID)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	return user, nil
}

// SendLinkAccount
func (s *AccountService) SendAndResendLinkAccount(ctx context.Context, req types.SendLinkAccountRequest) error {
	// ตรวจสอบว่ามี custcode อยู่ในระบบหรือไม่
	sbaCustInfo, err := s.SbaAccountService.GetCustomerInfo(ctx, req.Custcode)
	if err != nil {
		return constants.ErrGetCustomerInfo
	}

	// ตรวจสอบว่าชื่อผู้ใช้มีอยู่ในระบบแล้วหรือไม่
	isUsernameExisted, err := s.AccountRepo.CheckUsernameExist(req.Custcode)
	if err != nil {
		s.logger.Error(ctx, "accountService.SendAndResendLinkAccount Error checking username exist", zap.Error(err))
		return constants.ErrUsernameExists
	}

	if isUsernameExisted {
		return constants.ErrUsernameExists
	}

	// ตรวจสอบว่ามี send link account อยู่แล้วหรือไม่
	var sendLinkAccount *ssodb.SendLinkAccount

	sendLinkAccount, _ = s.SendLinkAccountRepo.FindByCustcode(req.Custcode)
	if sendLinkAccount == nil {
		if req.Email == nil {
			if sbaCustInfo.ResultList == nil || len(sbaCustInfo.ResultList) == 0 || sbaCustInfo.ResultList[0].Email == nil {
				return constants.ErrEmailNotFound
			}
			req.Email = sbaCustInfo.ResultList[0].Email
		}

		var userId *string = nil
		user, err := s.UserV2Repo.FindByIdCardNo(ctx, *sbaCustInfo.ResultList[0].CardID)
		if err != nil {
			s.logger.Error(ctx, "accountService.SendAndResendLinkAccount Failed UserRepo FindByIdCard", zap.Error(err))
		}

		if user != nil {
			userId = &user.Id
		}

		// สร้างรายการ send link account ใน db
		sendLinkAccount, err = s.SendLinkAccountRepo.Create(ctx, *req.Email, req.Custcode, userId)
		if err != nil {
			return constants.ErrCreateSendLinkAccount
		}

		sendLinkAccount.Email = *req.Email
	}

	// ตรวจสอบว่าลิงก์ถูกใช้ไปแล้วหรือไม่
	if sendLinkAccount.IsUsed {
		return constants.ErrLinkAccountUsed
	}

	// ส่งอีเมล
	linkWebTrading := s.Cfg.LinkNewWebTrading
	//http://trading-web.nonprod.pi.internal/th/account/setup?params=5f7b3b7b-0b3b-4b3b-8b3b-3b3b3b3b3b3b
	urlLinkAccount := fmt.Sprintf("%s?params=%s", linkWebTrading, sendLinkAccount.ID)

	// เตรียมข้อมูล HTML และแทนที่ placeholders
	htmlTemplate, err := os.ReadFile("email_template/link_account_template.html")
	if err != nil {
		return fmt.Errorf("error reading HTML template: %w", err)
	}

	body := string(htmlTemplate)
	body = strings.ReplaceAll(body, "{LINK_WEBSITE}", urlLinkAccount)
	subject := "🎉 บัญชีของคุณเปิดใช้งานแล้ว! | Your Account is Now Active!"

	// กำหนดค่าของ SMTPConfig
	smtpConfig := SMTPConfig{
		Host:     s.Cfg.Smtp.Host,
		Port:     s.Cfg.Smtp.Port,
		Username: s.Cfg.Smtp.Username,
		Password: s.Cfg.Smtp.Password,
	}

	if sendLinkAccount.Email == *sbaCustInfo.ResultList[0].Email {
		s.logger.Info(ctx, "accountService.SendAndResendLinkAccount Email is same as SbaCustInfo")
	} else {
		s.logger.Info(ctx, "accountService.SendAndResendLinkAccount Email is different from SbaCustInfo")
		// อัปเดตอีเมลในฐานข้อมูล
		sendLinkAccount, err = s.SendLinkAccountRepo.UpdateEmail(ctx, sendLinkAccount.ID.String(), *sbaCustInfo.ResultList[0].Email)
		if err != nil {
			s.logger.Error(ctx, "accountService.SendAndResendLinkAccount Failed to update email", zap.Error(err))
			return constants.ErrUpdateEmail
		}
		sendLinkAccount.Email = *sbaCustInfo.ResultList[0].Email
	}

	to := []string{sendLinkAccount.Email}

	emailContent := EmailContent{
		From:    s.Cfg.Smtp.Form,
		To:      to,
		Subject: subject,
		Body:    body,
	}

	// ส่งอีเมล์
	err = s.SendEmailVia365(smtpConfig, emailContent)
	if err != nil {
		return constants.ErrSendEmail
	}

	return nil
}

// GetSendLinkAccountByCustcode GetSendLinkAccountByCustcode
func (s *AccountService) GetSendLinkAccountByCustcode(ctx context.Context, custcode string) (*ssodb.SendLinkAccount, error) {

	SendLinkAccount, err := s.SendLinkAccountRepo.FindByCustcode(custcode)
	if err != nil {
		return nil, constants.ErrSendLinkAccountNotFound
	}

	return SendLinkAccount, nil
}

// GetAccountInfoForAdminLast50 get GetAccountInfoAll
func (s *AccountService) GetAccountInfoForAdminLast50(ctx context.Context) ([]types.AccountInfoResponse, error) {
	accounts, err := s.AccountRepo.FindLast50()
	if err != nil {
		return nil, constants.ErrUserNotFound
	}
	// Transform accounts into the response format
	var response []types.AccountInfoResponse
	for _, account := range accounts {

		userInfo, err := s.UserV2Repo.FindById(*account.UserID)

		if err != nil {
			s.logger.Error(ctx, "accountService.GetAccountInfoForAdminLast50 Failed UserRepo.FindById", zap.Error(err))
			return nil, constants.ErrUserNotFound
		}

		response = append(response, types.AccountInfoResponse{
			ID:                account.ID.String(),
			Username:          account.Username,
			IsSyncPassword:    account.Password != nil,
			IsSyncPin:         account.Pin != nil,
			LoginPwdFailCount: account.FailedPasswordAttempts,
			LoginPinFailCount: account.FailedPinAttempts,
			IsLock:            account.IsLocked,
			UpdatedAt:         account.UpdatedAt.Format("2006-01-02 15:04:05"), // Format to string
			CreatedAt:         account.CreatedAt.Format("2006-01-02 15:04:05"),
			UserID:            account.UserID,
			Email:             userInfo.Email, // Replace with real data if available
			Mobile:            userInfo.Phone, // Replace with real data if available
		})
	}

	return response, nil
}

func (s *AccountService) GetAccountsInfoByUsername(ctx context.Context, username string) (*types.PaginatedResponse, error) {
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetAccountInfoByUsername: error finding account by username", zap.String("username", username), zap.Error(err))
		return nil, constants.ErrAccountNotFound
	}

	userInfo, err := s.UserV2Repo.FindById(*account.UserID)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetAccountInfoByUsername: error finding user info by userID", zap.String("userId", util.SafeStringPtr(account.UserID)), zap.Error(err))
		return nil, constants.ErrUserInfoNotFound
	}

	accounts, err := s.AccountRepo.FindByUserIds(*account.UserID)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetAccountInfoByUsername: error finding accounts by userID", zap.String("userId", util.SafeStringPtr(account.UserID)), zap.Error(err))
		return nil, constants.ErrAccountsListNotFound
	}

	var accountResponses []types.AccountInfoResponse
	for _, acc := range accounts {
		accountResponses = append(accountResponses, types.AccountInfoResponse{
			ID:                acc.ID.String(),
			Username:          acc.Username,
			IsSyncPassword:    acc.Password != nil && acc.SaltPassword != nil,
			IsSyncPin:         acc.Pin != nil && acc.SaltPin != nil,
			LoginPwdFailCount: acc.FailedPasswordAttempts,
			LoginPinFailCount: acc.FailedPinAttempts,
			IsLock:            acc.IsLocked,
			UpdatedAt:         acc.UpdatedAt.Format("2006-01-02 15:04:05"),
			CreatedAt:         acc.CreatedAt.Format("2006-01-02 15:04:05"),
			UserID:            acc.UserID,
			Email:             userInfo.Email,
			Mobile:            userInfo.Phone,
		})
	}

	// ส่งข้อมูลกลับแบบ PaginatedResponse
	return &types.PaginatedResponse{
		CurrentPage:     1,
		PageSize:        len(accountResponses),
		HasNextPage:     false,
		HasPreviousPage: false,
		TotalPages:      1,
		Data:            accountResponses,
	}, nil
}

// GetAccountInfoByUsername get
func (s *AccountService) GetAccountInfoByUsername(ctx context.Context, username string) (*types.PaginatedResponse, error) {
	s.logger.Info(ctx, "accountService.GetAccountInfoByUsername: username", zap.String("username", username))
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetAccountInfoByUsername error AccountRepo FindByUsername", zap.Error(err))
		return nil, constants.ErrUserNotFound
	}

	userInfo, err := s.UserV2Repo.FindById(*account.UserID)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	// ส่งข้อมูล AccountInfoResponse
	return &types.PaginatedResponse{
		CurrentPage:     1,
		PageSize:        1,
		HasNextPage:     false,
		HasPreviousPage: false,
		TotalPages:      1,
		Data: []types.AccountInfoResponse{
			{
				ID:                account.ID.String(),
				Username:          account.Username,
				IsSyncPassword:    account.Password != nil && account.SaltPassword != nil,
				IsSyncPin:         account.Pin != nil && account.SaltPin != nil,
				LoginPwdFailCount: account.FailedPasswordAttempts,
				LoginPinFailCount: account.FailedPinAttempts,
				IsLock:            account.IsLocked,
				UpdatedAt:         account.UpdatedAt.Format("2006-01-02 15:04:05"),
				CreatedAt:         account.CreatedAt.Format("2006-01-02 15:04:05"),
				UserID:            account.UserID,
				Email:             userInfo.Email,
				Mobile:            userInfo.Phone,
			},
		},
	}, nil
}

// GetAccountInfoByUsernameOrPage retrieves account information by username or paginated account list
func (s *AccountService) GetAccountInfoByUsernameOrPage(ctx context.Context, username string, page int, pageSize int) (*types.PaginatedResponse, error) {
	buildResponse := func(account *ssodb.Account, userInfo *domain.User) types.AccountInfoResponse {
		return types.AccountInfoResponse{
			ID:                account.ID.String(),
			Username:          account.Username,
			IsSyncPassword:    account.Password != nil,
			IsSyncPin:         account.Pin != nil,
			LoginPwdFailCount: account.FailedPasswordAttempts,
			LoginPinFailCount: account.FailedPinAttempts,
			IsLock:            account.IsLocked,
			UpdatedAt:         account.UpdatedAt.Format("2006-01-02 15:04:05"),
			CreatedAt:         account.CreatedAt.Format("2006-01-02 15:04:05"),
			UserID:            account.UserID,
			Email:             userInfo.Email,
			Mobile:            userInfo.Phone,
			CardID:            userInfo.IdCardNo,
		}
	}

	var data []types.AccountInfoResponse

	if username != "" {
		// Lookup account by username
		account, err := s.AccountRepo.FindByUsername(username)
		if err != nil {
			return nil, constants.ErrAccountNotFound
		}

		if account.UserID == nil {
			return nil, constants.ErrUserNotFound
		}

		userInfo, err := s.UserV2Repo.FindById(*account.UserID)
		if err != nil {
			return nil, constants.ErrUserNotFound
		}

		accounts, err := s.AccountRepo.FindByUserIds(userInfo.Id)
		if err != nil {
			return nil, constants.ErrAccountsListNotFound
		}

		for _, acc := range accounts {
			if acc.UserID == nil {
				continue
			}
			userInfo, err := s.UserV2Repo.FindById(*acc.UserID)
			if err != nil {
				continue
			}
			data = append(data, buildResponse(&acc, userInfo))
		}

		return &types.PaginatedResponse{
			CurrentPage:     1,
			PageSize:        1,
			HasNextPage:     false,
			HasPreviousPage: false,
			TotalPages:      1,
			Data:            data,
		}, nil
	}

	// Paginated fetch
	offset := (page - 1) * pageSize
	accounts, err := s.AccountRepo.FindPaginated(offset, pageSize)
	if err != nil {
		return nil, err
	}

	totalAccounts, err := s.AccountRepo.CountAll()
	if err != nil {
		return nil, err
	}

	totalPages := (totalAccounts + pageSize - 1) / pageSize
	hasNextPage := page < totalPages
	hasPreviousPage := page > 1

	for _, acc := range accounts {
		if acc.UserID == nil {
			continue
		}
		userInfo, err := s.UserV2Repo.FindById(*acc.UserID)
		if err != nil {
			continue
		}
		data = append(data, buildResponse(acc, userInfo))
	}

	return &types.PaginatedResponse{
		CurrentPage:     page,
		PageSize:        pageSize,
		HasNextPage:     hasNextPage,
		HasPreviousPage: hasPreviousPage,
		TotalPages:      totalPages,
		Data:            data,
	}, nil
}

// ForceChangePassword ForceChangePassword
func (s *AccountService) ForceChangePassword(ctx context.Context, req types.ForceChangePasswordRequest) error {
	// ค้นหาข้อมูลผู้ใช้จาก database
	_, err := s.AccountRepo.FindByUsername(req.Username)
	if err != nil {
		return constants.ErrUserNotFound
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	err = s.updateUserPassword(ctx, req.Username, req.NewPassword)
	if err != nil {
		return constants.ErrFailedToUpdatePassword
	}

	return nil
}

// GenerateOtpToEmailForSetup Step 1 for guest sign up
func (s *AccountService) GenerateOtpToEmailForSetup(ctx context.Context, email string) (*types.GenerateEmailOtpForSetupResponse, error) {
	// ตรวจสอบว่าชื่อผู้ใช้มีอยู่ในระบบแล้วหรือไม่
	_, err := s.AccountRepo.FindByUsername(email)
	if err == nil {
		return nil, constants.ErrUsernameExists
	} else if !errors.Is(err, gorm.ErrRecordNotFound) {
		// ตรวจสอบ error ว่าไม่ใช่ error ที่บอกว่า record ไม่พบ
		return nil, err
	}
	return s.generateEmailOTP(ctx, email, "signUp")
}

// GenerateOtpToEmailForForgotPassword for forgot password
func (s *AccountService) GenerateOtpToEmailForForgotPassword(ctx context.Context, email string) (*types.GenerateEmailOtpForSetupResponse, error) {
	return s.generateEmailOTP(ctx, email, "forgotPassword")
}

func (s *AccountService) generateEmailOTP(ctx context.Context, email string, flow string) (*types.GenerateEmailOtpForSetupResponse, error) {
	var model *ssodb.GenerateOtpToEmailForSetup

	// create new row
	refcode, err := util.GenerateRandomUppercase(4)
	if err != nil {
		return nil, constants.ErrGenerateRandomUppercase
	}
	otpCode, err := util.GenerateRandomNumberString(6)
	if err != nil {
		return nil, constants.ErrGenerateRandomNumbers
	}

	if model == nil {
		id := uuid.New()
		model, err = s.GenerateOtpToEmailForSetupRepo.Create(ctx, id, email, *otpCode, refcode, flow)
		if err != nil {
			return nil, constants.ErrGenerateOtpToEmailForSetupCreate
		}
	}

	// send and re-send email otp
	// เตรียมข้อมูล HTML และแทนที่ placeholders
	htmlTemplate, err := os.ReadFile("email_template/otp_setup.html")
	if err != nil {
		return nil, constants.ErrReadEmailTemplate
	}

	body := string(htmlTemplate)
	body = strings.ReplaceAll(body, "{REF_OTP}", refcode)
	body = strings.ReplaceAll(body, "{OTP_CODE}", *otpCode)
	subject := "📩 ยืนยันการตั้งค่าบัญชีของคุณ! | Verify Your Account Setup!"

	// กำหนดค่าของ SMTPConfig
	smtpConfig := SMTPConfig{
		Host:     s.Cfg.Smtp.Host,
		Port:     s.Cfg.Smtp.Port,
		Username: s.Cfg.Smtp.Username,
		Password: s.Cfg.Smtp.Password,
	}
	to := []string{email}

	emailContent := EmailContent{
		From:    s.Cfg.Smtp.Form,
		To:      to,
		Subject: subject,
		Body:    body,
	}

	// ส่งอีเมล์
	err = s.SendEmailVia365(smtpConfig, emailContent)
	if err != nil {
		return nil, constants.ErrSendEmail
	}

	// return
	return &types.GenerateEmailOtpForSetupResponse{
		RefCode:   model.RefCode,
		Email:     model.Email,
		ExpiresAt: *model.ExpiresAt,
	}, nil
}

// SetupWithOTP Step 3 Complete Guest Register
func (s *AccountService) SetupWithOTP(ctx context.Context, emailRefId, email, phoneRefId, phone, encryptPassword string) (*types.LoginResponse, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database

	generateEmailOtpForSetup, err := s.GenerateOtpToEmailForSetupRepo.FindByIdAndEmail(emailRefId, email)
	if err != nil {
		return nil, constants.ErrGenerateOtpToEmailForSetupFindById
	}

	if generateEmailOtpForSetup.IsUsed || generateEmailOtpForSetup.Flow != "signUp" {
		return nil, constants.ErrOtpUsed
	}

	generatePhoneOtpForSetup, err := s.GenerateOtpToPhoneForSetupRepo.FindByIdAndPhone(phoneRefId, phone)
	if err != nil {
		return nil, constants.ErrGenerateOtpToPhoneForSetupFindById
	}

	if generatePhoneOtpForSetup.IsUsed || generateEmailOtpForSetup.Flow != "signUp" {
		return nil, constants.ErrOtpUsed
	}

	// ตรวจสอบว่า Email มีอยู่ใน db ไหม
	checkDuplicateAccount, _ := s.AccountRepo.FindByUsername(generateEmailOtpForSetup.Email)
	if checkDuplicateAccount != nil {
		return nil, constants.ErrDuplicateUsername
	}

	user, _ := s.UserV2Repo.FindByEmail(&generateEmailOtpForSetup.Email)

	if user == nil {
		// สร้าง user ใหม่
		user, err = s.UserV2Repo.CreateUserInfo(ctx, &generateEmailOtpForSetup.Email, &phone, nil, nil, nil, nil, nil, nil)
		if err != nil {
			s.logger.Error(ctx, "accountService.SetupWithOTP FailedToCreateUser", zap.Error(err))
			return nil, constants.ErrCreateUser
		}
	}

	// decrypt password
	decryptPassword, err := util.RsaDecryption(encryptPassword)
	if err != nil {
		return nil, constants.ErrDecryptPassword
	}

	salt, err := GenerateSalt()
	if err != nil {
		return nil, constants.ErrFailedToGenerateSalt
	}

	// สร้าง hash สำหรับรหัสผ่านใหม่
	hashedNewPassword, err := util.HashPasswordWithSalt(decryptPassword, salt)
	if err != nil {
		return nil, constants.ErrFailedToHashNewPassword
	}

	// สร้าง account email to account ใหม่
	err = s.AccountRepo.CreateMemberAccount(ctx, generateEmailOtpForSetup.Email, &hashedNewPassword, &salt, &user.Id)
	if err != nil {
		s.logger.Error(ctx, "accountService.SetupWithOTP Failed To CreateMemberAccount", zap.Error(err))
		return nil, constants.ErrCreateUser
	}

	// อัปเดตสถานะใน GenerateOtpToEmailForSetup
	generateEmailOtpForSetup, err = s.GenerateOtpToEmailForSetupRepo.UpdateStatus(ctx, generateEmailOtpForSetup.ID, true)
	if err != nil {
		return nil, constants.ErrGenerateOtpToEmailForSetupUpdateStatus
	}

	generatePhoneOtpForSetup, err = s.GenerateOtpToPhoneForSetupRepo.UpdateStatus(ctx, generatePhoneOtpForSetup.ID, true)
	if err != nil {
		return nil, constants.ErrGenerateOtpToPhoneForSetupUpdateStatus
	}

	// get account for login
	account, err := s.AccountRepo.FindByUsername(generateEmailOtpForSetup.Email)
	if err != nil {
		return nil, constants.ErrFindByUsername
	}

	// สร้าง token
	token, err := util.GenerateAccessToken(s.Cfg, account.ID, account.UserID)
	if err != nil {
		return nil, err
	}

	refreshToken, err := util.GenerateRefreshToken(s.Cfg, account.ID, account.UserID)
	if err != nil {
		return nil, err
	}

	response := util.GenerateLoginResponse(token, refreshToken)
	return &response, nil

}

// VerifySetupWithOTP Step 2 Verify OTP
func (s *AccountService) VerifySetupWithOTP(ctx context.Context, email, refCode, otpCode string) (*types.VerifyEmailOtpForSetupResponse, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	generateOtpForSetup, err := s.GenerateOtpToEmailForSetupRepo.FindByEmailAndRefCode(email, refCode)

	if err != nil {
		return nil, constants.ErrGenerateOtpToEmailForSetupFindById
	}

	if generateOtpForSetup.IsUsed {
		return nil, constants.ErrOtpUsed
	}

	// เช็คหมดอายุ
	if generateOtpForSetup.ExpiresAt.Before(time.Now()) {
		return nil, constants.ErrOtpExpired
	}

	// เช็ครหัส OTP
	if generateOtpForSetup.OtpCode != otpCode {
		return nil, constants.ErrOtpCodeNotMatch
	}

	return &types.VerifyEmailOtpForSetupResponse{
		RefId: generateOtpForSetup.ID.String(),
	}, nil
}

// GenerateOtpToPhoneForSetup generate phone otp for setup
func (s *AccountService) GenerateOtpToPhoneForSetup(ctx context.Context, phoneNumber string, sendLinkAccountId *string) (*types.GeneratePhoneOtpForSetupResponse, error) {
	// เช็คว่ามีเบอร์โทรศัพท์นี้อยู่ในระบบหรือไม่
	if s.IsPhoneNumberExisted(ctx, phoneNumber) {
		if sendLinkAccountId == nil {
			return nil, constants.ErrPhoneNumberExists
		} else {
			// ค้นหาข้อมูลผู้ใช้จาก database
			sendLinkAccount, err := s.SendLinkAccountRepo.FindById(*sendLinkAccountId)

			if err != nil {
				return nil, constants.ErrGenerateOtpToEmailForSetupFindById
			}

			if sendLinkAccount.IsUsed {
				return nil, constants.ErrSendLinkAccountIsUsed
			}
		}
	}

	return s.generatePhoneOTP(ctx, phoneNumber, "signUp")
}

// GenerateOtpToPhoneForForgotPassword for forgot password
func (s *AccountService) GenerateOtpToPhoneForForgotPassword(ctx context.Context, phoneNumber string, sendLinkAccountId *string) (*types.GeneratePhoneOtpForSetupResponse, error) {
	return s.generatePhoneOTP(ctx, phoneNumber, "forgotPassword")
}

func (s *AccountService) generatePhoneOTP(ctx context.Context, phoneNumber string, flow string) (*types.GeneratePhoneOtpForSetupResponse, error) {

	guid := uuid.New()
	// ส่ง SMS
	otpRef, err := s.OtpService.SendOtp(ctx, guid.String(), guid.String(), "mobile", "th", phoneNumber)
	if err != nil {
		s.logger.Error(ctx, "accountService.generatePhoneOTP Error SendPhoneOTP", zap.Error(err))
		return nil, constants.ErrSendOtp
	}

	model, err := s.GenerateOtpToPhoneForSetupRepo.Create(ctx, guid, phoneNumber, *otpRef, flow)
	if err != nil {
		return nil, constants.ErrGenerateOtpToPhoneForSetupCreate
	}

	response := &types.GeneratePhoneOtpForSetupResponse{
		RefCode:     model.RefCode,
		PhoneNumber: phoneNumber,
		ExpiresAt:   *util.GetExpireTime(),
	}

	s.logger.Info(ctx, "accountService.generatePhoneOTP Success", zap.String("ID", model.ID.String()))

	return response, nil
}

// VerifyPhoneOtpForSetup VerifyPhoneOtpForSetup
func (s *AccountService) VerifyPhoneOtpForSetup(ctx context.Context, phoneNumber, refCode, otpCode string) (*types.VerifyPhoneOtpForSetupResponse, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	generateOtpForSetup, err := s.GenerateOtpToPhoneForSetupRepo.FindByPhoneAndRefCode(phoneNumber, refCode)

	if err != nil {
		return nil, constants.ErrGenerateOtpToPhoneForSetupFindById
	}

	if generateOtpForSetup.IsUsed {
		return nil, constants.ErrOtpUsed
	}

	// เช็คหมดอายุ
	if generateOtpForSetup.ExpiresAt.Before(time.Now()) {
		return nil, constants.ErrOtpExpired
	}
	// เช็ครหัส OTP
	err = s.OtpService.SubmitOtp(ctx, generateOtpForSetup.ID.String(), generateOtpForSetup.ID.String(), "mobile", otpCode, generateOtpForSetup.RefCode)
	if err != nil {
		if err.Error() == "Invalid ref" {
			return nil, constants.ErrInvalidOtpRef
		}
		return nil, constants.ErrOtpCodeNotMatch
	}

	return &types.VerifyPhoneOtpForSetupResponse{
		RefId: generateOtpForSetup.ID.String(),
	}, nil
}

// FindEmailAccountByUserID find
func (s *AccountService) FindEmailAccountByUserID(ctx context.Context, userId string) (*types.CheckEmailAccountByUserIDResponse, error) {
	accounts, err := s.AccountRepo.FindByUserIds(userId)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	for _, account := range accounts {
		if util.IsEmailFormat(account.Username) {
			return &types.CheckEmailAccountByUserIDResponse{
				Email:             account.Username,
				IsUsernameExisted: true,
			}, nil
		}
	}

	return nil, constants.ErrUserNotFound
}

// AccountWithoutPinByUserId aa
func (s *AccountService) AccountWithoutPinByUserId(ctx context.Context, userId string) (*[]types.PinAccountInfoList, error) {
	accounts, err := s.AccountRepo.FindByUserIds(userId)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	// Transform accounts into the response format
	var response = &[]types.PinAccountInfoList{}
	for _, account := range accounts {
		if util.IsCustCodeValid(account.Username) {
			*response = append(*response, types.PinAccountInfoList{
				Username: account.Username,
				IsSetPin: account.Pin != nil,
			})
		}
	}

	return response, nil
}

// AccountWithoutPin aa
func (s *AccountService) AccountWithoutPin(ctx context.Context) (*types.UserList, error) {
	accounts, err := s.AccountRepo.FindAllWithoutPin()
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	// Transform accounts into the response format
	var response = &types.UserList{}
	for _, account := range accounts {
		if util.IsCustCodeValid(account.Username) {
			response.Username = append(response.Username, account.Username)
		}
	}

	return response, nil
}

// ChangePasswordByUserIdWithOtp aa
func (s *AccountService) ChangePasswordByUserIdWithOtp(ctx context.Context, emailRefId, phoneRefId *string, username, newPassword string) error {

	isVerify := false
	// ค้นหา account by username
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		return constants.ErrUserNotFound
	}

	// ค้นหาข้อมูลผู้ใช้จาก database emailRefId , phoneRefId
	if emailRefId != nil {
		generateEmailOtpForSetup, err := s.GenerateOtpToEmailForSetupRepo.FindById(*emailRefId)
		if err != nil {
			return constants.ErrGenerateOtpToEmailForSetupFindById
		}

		if generateEmailOtpForSetup.IsUsed || generateEmailOtpForSetup.Flow != "forgotPassword" {
			return constants.ErrOtpUsed
		}

		_, err = s.GenerateOtpToEmailForSetupRepo.UpdateStatus(ctx, generateEmailOtpForSetup.ID, true)
		if err != nil {
			return constants.ErrGenerateOtpToEmailForSetupUpdateStatus
		}
		isVerify = true

	} else if phoneRefId != nil {
		generatePhoneOtpForSetup, err := s.GenerateOtpToPhoneForSetupRepo.FindById(*phoneRefId)
		if err != nil {
			return constants.ErrGenerateOtpToPhoneForSetupFindById
		}

		if generatePhoneOtpForSetup.IsUsed || generatePhoneOtpForSetup.Flow != "forgotPassword" {
			return constants.ErrOtpUsed
		}

		_, err = s.GenerateOtpToPhoneForSetupRepo.UpdateStatus(ctx, generatePhoneOtpForSetup.ID, true)
		if err != nil {
			return constants.ErrGenerateOtpToPhoneForSetupUpdateStatus
		}
		isVerify = true
	}

	if !isVerify {
		return constants.ErrOtpNotFound
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	err = s.updateUserPassword(ctx, account.Username, newPassword)
	if err != nil {
		return constants.ErrFailedToUpdatePassword
	}

	return nil
}

func (s *AccountService) GetProfile(ctx context.Context, userId string) ([]ssodb.Account, error) {
	accounts, err := s.AccountRepo.FindByUserIds(userId)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	return accounts, nil
}

func (s *AccountService) SyncTokenToOtw(ctx context.Context, userId string, accountId string) (*ssodb.SyncToken, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	_, err := s.AccountRepo.FindByUserIds(userId)
	if err != nil {
		return nil, constants.ErrUserNotFound
	}

	// save to db and return value
	syncToken, err := s.SyncTokenRepo.Create(ctx, userId, accountId)
	if err != nil {
		return nil, constants.ErrCreateSyncToken
	}

	return syncToken, nil
}

func (s *AccountService) GetSyncToken(ctx context.Context, id string) (*types.LoginResponse, error) {
	syncToken, err := s.SyncTokenRepo.GetByID(id)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetSyncToken: error finding sync token by ID", zap.String("syncTokenId", id), zap.Error(err))
		return nil, constants.ErrSyncTokenNotFound
	}

	if syncToken.IsUse {
		s.logger.Error(ctx, "accountService.GetSyncToken: sync token already used", zap.String("syncTokenId", id))
		return nil, constants.ErrSyncTokenUsed
	}

	syncToken, err = s.SyncTokenRepo.UpdateIsUse(ctx, id, true)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetSyncToken: error updating sync token status", zap.String("syncTokenId", id), zap.Error(err))
		return nil, constants.ErrUpdateSyncToken
	}
	// convert string to uuid
	accountId, _ := uuid.Parse(syncToken.AccountID)
	// สร้าง JWT Token
	accessToken, err := util.GenerateAccessToken(s.Cfg, accountId, &syncToken.UserID)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetSyncToken: error generating access token", zap.String("syncTokenId", id), zap.Error(err))
		return nil, constants.ErrFailedToGenerateAccessToken
	}

	// สร้าง Refresh Token
	refreshToken, err := util.GenerateRefreshToken(s.Cfg, accountId, &syncToken.UserID)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetSyncToken: error generating refresh token", zap.String("syncTokenId", id), zap.Error(err))
		return nil, constants.ErrFailedToGenerateRefreshToken
	}

	s.logger.Info(ctx, "accountService.GetSyncToken: successfully generated tokens", zap.String("syncTokenId", id))

	return &types.LoginResponse{
		AccessToken:        accessToken,
		AccessTokenExpiry:  os.Getenv("JWT_EXPIRATION") + "h",
		RefreshToken:       refreshToken,
		RefreshTokenExpiry: os.Getenv("REFRESH_EXPIRATION") + "h",
	}, nil
}

// GetLogSession return token , refresh token
func (s *AccountService) GetLogSession(ctx context.Context, sessionId string) (*types.LoginResponse, error) {
	GetLogSession, err := s.SbaAccountService.GetLogSession(sessionId)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetLogSession: error finding log session", zap.String("sessionId", sessionId), zap.Error(err))
		return nil, constants.ErrLogSessionNotFound
	}

	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUsername(GetLogSession.UserName)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetLogSession: error finding account by username", zap.String("username", GetLogSession.UserName), zap.Error(err))
		return nil, constants.ErrUserNotFound
	}

	// Generate JWT access token.
	accessToken, err := util.GenerateAccessToken(s.Cfg, account.ID, account.UserID)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetLogSession: error generating access token", zap.String("username", GetLogSession.UserName), zap.Error(err))
		return nil, err
	}

	// Generate JWT refresh token.
	refreshToken, err := util.GenerateRefreshToken(s.Cfg, account.ID, account.UserID)
	if err != nil {
		s.logger.Error(ctx, "accountService.GetLogSession: error generating refresh token", zap.String("username", GetLogSession.UserName), zap.Error(err))
		return nil, err
	}

	s.logger.Info(ctx, "accountService.GetLogSession: successfully generated tokens", zap.String("username", GetLogSession.UserName))
	response := util.GenerateLoginResponse(accessToken, refreshToken)
	// ส่ง JWT token กลับ
	return &response, nil
}

// TODO: Remove Debug encrypt decrypt

func (s *AccountService) RsaDecryption(message string) (string, error) {
	// Decrypt the message
	decrypted, err := util.RsaDecryption(message)
	if err != nil {
		s.logger.ErrorNoCtx("accountService.RSADecryption failed:", zap.Error(err))
		return "", err
	}

	return decrypted, nil

}

func (s *AccountService) RsaEncryption(message string) (string, error) {
	// Encrypt the message
	encrypted, err := util.RsaEncryption(message)
	if err != nil {
		s.logger.ErrorNoCtx("accountService.RsaEncryption failed:", zap.Error(err))
		return "", err
	}
	return encrypted, nil

}

func (s *AccountService) TextEncrypt(ctx context.Context, text string) (*types.TextEncryptResponse, error) {
	encryptText, err := ssodb.EncryptUsername(text, s.Cfg.EncryptKey)
	if err != nil {
		return nil, constants.ErrEncryptText
	}

	return &types.TextEncryptResponse{
		Text: encryptText,
	}, nil
}

func (s *AccountService) TextDecrypt(ctx context.Context, text string) (*types.TextEncryptResponse, error) {
	decryptText, err := ssodb.DecryptUsername(text, s.Cfg.EncryptKey)
	if err != nil {
		return nil, constants.ErrDecryptText
	}

	return &types.TextEncryptResponse{
		Text: decryptText,
	}, nil
}

func (s *AccountService) ChangeUsername(ctx context.Context, username string, newUsername string, userId string) (*ssodb.Account, error) {
	// ค้นหาข้อมูลผู้ใช้จาก database
	account, err := s.AccountRepo.FindByUsername(username)
	if err != nil {
		s.logger.Error(ctx, "accountService.ChangeUsername: error finding account by username", zap.String("username", username), zap.Error(err))
		return nil, constants.ErrUserNotFound
	}
	fmt.Printf("account: %+v\n", account.UserID)
	// ตรวจสอบ userId ตรงกันไหม
	if account.UserID == nil || *account.UserID != userId {

		s.logger.Error(ctx, "accountService.ChangeUsername: userId not match", zap.String("userId", userId), zap.String("accountUserId", *account.UserID))
		return nil, constants.ErrUserIdNotMatch
	}
	// ตรวจสอบว่าชื่อผู้ใช้ใหม่มีอยู่ในระบบแล้วหรือไม่
	existingAccount, _ := s.AccountRepo.FindByUsername(newUsername)
	if existingAccount != nil {
		s.logger.Error(ctx, "accountService.ChangeUsername: username already exists", zap.String("newUsername", newUsername))
		return nil, constants.ErrUsernameExists
	}

	// อัปเดตรหัสผ่านในฐานข้อมูล
	account, err = s.AccountRepo.UpdateUsername(ctx, account.ID.String(), username, newUsername)
	if err != nil {
		s.logger.Error(ctx, "accountService.ChangeUsername: error updating username", zap.String("accountId", account.ID.String()), zap.Error(err))
		return nil, constants.ErrFailedToUpdateUsername
	}

	return account, nil
}
