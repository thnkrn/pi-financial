package handler

import (
	"net/http"
	"regexp"
	"time"

	"github.com/pi-financial/pi-sso-v2/internal/service/sba"

	"github.com/pi-financial/pi-sso-v2/internal/handler/internal"
	"github.com/pi-financial/pi-sso-v2/internal/repository"
	service "github.com/pi-financial/pi-sso-v2/internal/service/otp"
	"github.com/pi-financial/pi-sso-v2/internal/service/settrade"

	"github.com/pi-financial/pi-sso-v2/internal/repository/onboard_repository"
	"github.com/pi-financial/pi-sso-v2/internal/repository/user_v2_repository"

	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/driver"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"github.com/pi-financial/pi-sso-v2/internal/repository/ssodb_repository"
	"github.com/pi-financial/pi-sso-v2/internal/service/ssodb_service"
	"github.com/pi-financial/pi-sso-v2/middleware"

	_ "github.com/pi-financial/pi-sso-v2/docs"
	echoSwagger "github.com/swaggo/echo-swagger"
	"gorm.io/gorm"
)

type Middlewares struct {
	ErrorHandler *middleware.ErrorHandler
}

type ServerHTTP struct {
	app *echo.Echo
	db  *gorm.DB
	cfg config.Config
}

// CustomValidator struct
type CustomValidator struct {
	validator *validator.Validate
}

// Validate method
func (cv *CustomValidator) Validate(i interface{}) error {
	return cv.validator.Struct(i)
}

// ฟังก์ชันสำหรับตรวจสอบว่าเป็นตัวเลขตามรูปแบบที่ต้องการ
func isValidCustcode(fl validator.FieldLevel) bool {
	match, _ := regexp.MatchString(`^[0-9]{7}$`, fl.Field().String())
	return match
}

func isValidCustcodes(fl validator.FieldLevel) bool {
	// ตรวจสอบแต่ละค่าของ custcode
	custcodes, ok := fl.Field().Interface().([]string)
	if !ok {
		return false
	}
	for _, custcode := range custcodes {
		match := regexp.MustCompile(`^[0-9]{7}$`).MatchString(custcode)
		if !match {
			return false
		}
	}
	return true
}

func isValidEmailOrCustcode(fl validator.FieldLevel) bool {
	value := fl.Field().String()

	// ตรวจสอบว่าเป็นอีเมล
	emailMatch, _ := regexp.MatchString(`^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`, value)
	if emailMatch {
		return true
	}

	// ตรวจสอบว่าเป็น Custcode (ตัวเลข 7 หลัก)
	custcodeMatch, _ := regexp.MatchString(`^[0-9]{7}$`, value)
	return custcodeMatch
}

func isValidPin(fl validator.FieldLevel) bool {
	match, _ := regexp.MatchString(`^[0-9]{6}$`, fl.Field().String())
	return match
}

// isValidPhoneNumber ตรวจสอบว่าเป็นเบอร์โทรศัพท์หรือไม่
func isValidPhoneNumber(fl validator.FieldLevel) bool {
	// ตัวอย่าง regex สำหรับเบอร์โทรศัพท์ในประเทศไทย: 10 หลัก และต้องเริ่มต้นด้วย 0
	match, _ := regexp.MatchString(`^0[0-9]{9}$`, fl.Field().String())
	return match
}

func isValidPassword(fl validator.FieldLevel) bool {
	password := fl.Field().String()

	// ตรวจสอบความยาวของรหัสผ่านมากกว่า 6 ตัวอักษร
	if len(password) <= 6 {
		return false
	}

	// ตรวจสอบว่ามีตัวพิมพ์ใหญ่ (A-Z)
	hasUpper := regexp.MustCompile(`[A-Z]`).MatchString(password)

	// ตรวจสอบว่ามีตัวเลข (0-9)
	hasDigit := regexp.MustCompile(`\d`).MatchString(password)

	// รหัสผ่านจะผ่านเมื่อมีทั้งตัวพิมพ์ใหญ่และตัวเลข และความยาวเกิน 6
	return hasUpper && hasDigit
}

func isValidBirthday(fl validator.FieldLevel) bool {
	value := fl.Field().String()

	// ตรวจสอบรูปแบบ YYYY-MM-DD ด้วย Regex
	match, _ := regexp.MatchString(`^\d{4}-\d{2}-\d{2}$`, value)
	if !match {
		return false
	}

	// ตรวจสอบว่าเป็นวันที่ที่ถูกต้องตามปฏิทินหรือไม่
	_, err := time.Parse("2006-01-02", value)
	return err == nil
}

func NewServerHTTP(log log.Logger, middlewares *Middlewares, cfg config.Config) *ServerHTTP {
	e := echo.New()
	e.HTTPErrorHandler = middlewares.ErrorHandler.EchoErrorHandler
	// ลงทะเบียน custom validator$
	v := validator.New()
	_ = v.RegisterValidation("custcode", isValidCustcode)
	_ = v.RegisterValidation("custcodes", isValidCustcodes)
	_ = v.RegisterValidation("pin", isValidPin)
	_ = v.RegisterValidation("password", isValidPassword)
	_ = v.RegisterValidation("phone", isValidPhoneNumber)
	_ = v.RegisterValidation("username", isValidEmailOrCustcode)
	_ = v.RegisterValidation("birthday", isValidBirthday)
	e.Validator = &CustomValidator{validator: v}

	// เชื่อมต่อกับฐานข้อมูล
	db, err := driver.ConnectMySQL(log, &cfg)
	if err != nil {
		panic(err)
	}

	userApiClientV2 := driver.CreateUserSrvV2Client(cfg)
	onboardApiClient := driver.CreateOnboardSrvClient(cfg)
	_ = LoadErrorMessages("response_code.json")

	// สร้าง user_repository, service, และ handler
	otpApiClient := driver.CreateOtpSrvClient(cfg)
	otpRepo := repository.NewOtpRepository(log, db, *otpApiClient)

	userV2Repo := user_v2_repository.NewUserV2Repository(log, *userApiClientV2)
	onboardRepo := onboard_repository.NewOnboardRepository(log, *onboardApiClient)

	otpService := service.NewOtpService(log, otpRepo)
	loginWith2FARepo := ssodb_repository.NewLoginWith2FASectionRepository(log, db)
	sendLinkAccountRepo := ssodb_repository.NewSendLinkAccountRepository(log, db)
	accountRepo := ssodb_repository.NewAccountRepository(log, db, sendLinkAccountRepo)

	activityLogRepo := ssodb_repository.NewActivityLogRepository(db)
	passwordResetRepo := ssodb_repository.NewPasswordResetRepository(log, db)
	biometricRepo := ssodb_repository.NewBiometricRepository(log, db)
	generateOtpToEmailForSetupRepo := ssodb_repository.NewGenerateOtpToEmailForSetupRepository(log, db)
	generateOtpPhoneForSetupRepo := ssodb_repository.NewGenerateOtpToPhoneForSetupRepository(log, db)
	syncTokenRepo := ssodb_repository.NewSyncTokenRepository(log, db)

	sbaAccountService := sba.NewSBAAccountService(log, cfg)
	settradeService := settrade.NewService(log, cfg, sbaAccountService)
	accountService := ssodb_service.NewAccountService(log, accountRepo, cfg, settradeService, passwordResetRepo, sendLinkAccountRepo, otpService, generateOtpToEmailForSetupRepo, generateOtpPhoneForSetupRepo, syncTokenRepo, sbaAccountService, *userV2Repo, *onboardRepo)
	authenService := ssodb_service.NewAuthService(log, accountRepo, biometricRepo, passwordResetRepo, *userV2Repo, cfg, otpService, loginWith2FARepo, settradeService, sbaAccountService)

	activityLogService := ssodb_service.NewActivityLogService(activityLogRepo, cfg)

	// เรียกใช้ฟังก์ชัน GetAccounts สำหรับ route /accounts
	piMiddleware := middleware.NewPiMiddleware(e, cfg, activityLogRepo)

	// เพิ่ม ActivityLogMiddleware
	e.Use(piMiddleware.TracerMiddleware())
	e.Use(piMiddleware.ActivityLogMiddleware(log))
	NewAccountHandler(e, cfg, accountService, piMiddleware, log)
	NewAuthHandler(e, cfg, authenService, piMiddleware, accountService)
	NewActivityLogHandler(e, cfg, activityLogService)
	NewEfinTradeHandler(e, cfg, authenService, piMiddleware, accountService)
	internal.NewInternalAccountHandler(e, cfg, accountService, piMiddleware, authenService)

	e.GET("/", func(c echo.Context) error {
		return c.JSON(http.StatusOK, map[string]string{"Project": "pi-sso-v2", "Version": "2.0.001"})
	})

	e.GET("/swagger/*", echoSwagger.WrapHandler)

	log.InfoNoCtx("server started on port [::]:8080")
	e.HideBanner = true
	e.HidePort = true
	return &ServerHTTP{app: e, db: db, cfg: cfg}
}

func (sh *ServerHTTP) Start() {
	_ = sh.app.Start(":8080")
}
