package config

import (
	"github.com/go-playground/validator/v10"
	"github.com/joho/godotenv"
	"github.com/kelseyhightower/envconfig"
)

type SMTP struct {
	Host     string `envconfig:"HOST" validate:"required"`
	Port     string `envconfig:"PORT" validate:"required"`
	Username string `envconfig:"USERNAME" validate:"required"`
	Password string `envconfig:"PASSWORD" validate:"required"`
	Form     string `envconfig:"FROM" validate:"required"`
}

type Database struct {
	Host     string `envconfig:"HOST" validate:"required"`
	Port     string `envconfig:"PORT" validate:"required"`
	User     string `envconfig:"USER" validate:"required"`
	Password string `envconfig:"PASSWORD" validate:"required"`
	Name     string `envconfig:"NAME" validate:"required"`
}

type Trading struct {
	Host     string `envconfig:"HOST" validate:"required"`
	Port     string `envconfig:"PORT" validate:"required,numeric"`
	User     string `envconfig:"USER" validate:"required"`
	Password string `envconfig:"PASSWORD" validate:"required"`
	DB       string `envconfig:"DB" validate:"required"`
}

type DatadogConfig struct {
	DDEnv            string `envconfig:"DD_ENV"`
	DDService        string `envconfig:"DD_SERVICE"`
	DDComponent      string `envconfig:"DD_COMPONENT"`
	DDVersion        string `envconfig:"DD_VERSION"`
	DDAgentHost      string `envconfig:"DD_AGENT_HOST"`
	DDLogLevel       string `envconfig:"DD_LOG_LEVEL"`
	DDTraceAgentPort string `envconfig:"DD_TRACE_AGENT_PORT"`
	DDLogsEnabled    bool   `envconfig:"DD_LOGS_ENABLED"`
}

// Config struct ที่เก็บคอนฟิกจากไฟล์ ..env
type Config struct {
	Database                Database      `envconfig:"DB"`
	Trading                 Trading       `envconfig:"TRADING"`
	JwtSecret               string        `envconfig:"JWT_SECRET" validate:"required"`
	JwtExpiration           int           `envconfig:"JWT_EXPIRATION" validate:"required,numeric"`
	RefreshExpiration       int           `envconfig:"REFRESH_EXPIRATION" validate:"required,numeric"`
	UserSrvHost             string        `envconfig:"USER_SRV_HOST" validate:"required"`
	UserSrvV2Host           string        `envconfig:"USER_SRV_V2_HOST" validate:"required"`
	EncryptKey              string        `envconfig:"ENCRYPT_KEY" validate:"required"`
	SettradeHost            string        `envconfig:"SETTRADE_HOST" validate:"required"`
	Smtp                    SMTP          `envconfig:"SMTP"`
	ResetPasswordExpiration string        `envconfig:"RESET_PASSWORD_EXPIRATION" validate:"required"`
	LinkResetPassword       string        `envconfig:"LINK_RESET_PASSWORD" validate:"required"`
	LinkWebTrading          string        `envconfig:"LINK_WEB_TRADING" validate:"required"`
	LinkResetPin            string        `envconfig:"LINK_RESET_PIN" validate:"required"`
	LinkNewWebTrading       string        `envconfig:"LINK_NEW_WEB_TRADING" validate:"required"`
	PrivateKeyBase64        string        `envconfig:"PRIVATE_KEY_BASE64" validate:"required"`
	PublicKeyBase64         string        `envconfig:"PUBLIC_KEY_BASE64" validate:"required"`
	OtpSrvHost              string        `envconfig:"OTP_SRV_HOST" validate:"required"`
	OnboardSrvHost          string        `envconfig:"ONBOARD_SRV_HOST" validate:"required"`
	DatadogConfig           DatadogConfig `envconfig:"DATADOG"`
}

// LoadConfig โหลดคอนฟิกจากไฟล์ ..env
func LoadConfig() (Config, error) {
	var config Config

	_ = godotenv.Load()
	if err := envconfig.Process("", &config); err != nil {
		return config, err
	}

	// ตรวจสอบค่าคอนฟิกที่โหลดมา
	validate := validator.New()
	if err := validate.Struct(&config); err != nil {
		return config, err
	}

	return config, nil
}
