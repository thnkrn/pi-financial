package config

import (
	"log"

	"github.com/go-playground/validator/v10"
	"github.com/joho/godotenv"
	"github.com/kelseyhightower/envconfig"
)

type server struct {
	Port string `envconfig:"PORT" default:"8080"`
}

type app struct {
	ServiceName  string `envconfig:"SERVICE_NAME" default:"onboard-srv-v2"`
	IsProduction bool   `envconfig:"IS_PRODUCTION" default:"true"`
}

type database struct {
	Host              string `envconfig:"DB_HOST"`
	Port              string `envconfig:"DB_PORT"`
	Database          string `envconfig:"DB_NAME"`
	Username          string `envconfig:"DB_USER"`
	Password          string `envconfig:"DB_PASSWORD"`
	ConnectionStrings string `envconfig:"CONNECTIONSTRINGS__ONBOARD"`
}

type datadog struct {
	DDEnv            string `envconfig:"DD_ENV"`
	DDService        string `envconfig:"DD_SERVICE"`
	DDComponent      string `envconfig:"DD_COMPONENT"`
	DDVersion        string `envconfig:"DD_VERSION"`
	DDAgentHost      string `envconfig:"DD_AGENT_HOST"`
	DDLogLevel       string `envconfig:"DD_LOG_LEVEL"`
	DDTraceAgentPort string `envconfig:"DD_TRACE_AGENT_PORT"`
	DDLogsEnabled    bool   `envconfig:"DD_LOGS_ENABLED" default:"false"`
}

type hardcode struct {
	ServiceType string `envconfig:"SERVICE_TYPE"`
	PackageType string `envconfig:"PACKAGE_TYPE"`
}

type client struct {
	UserSrvV2Host       string `envconfig:"USER_SRV_V2_HOST"`
	EmployeeSrvHost     string `envconfig:"EMPLOYEE_SRV_HOST"`
	NotificationSrvHost string `envconfig:"NOTIFICATION_SRV_HOST"`
}

type notification struct {
	MT4MT5EnrollmentEmailNotificationTemplateId int64 `envconfig:"MT4_MT5_ENROLLMENT_EMAIL_NOTIFICATION_TEMPLATE_ID"`
}

type email struct {
	TesterEmails string `envconfig:"TESTER_EMAILS"`
}

type Config struct {
	Server       server
	App          app
	Database     database
	Datadog      datadog
	HardCode     hardcode
	Client       client
	Notification notification
	Email        email
}

var cfg Config

func LoadConfig() error {
	_ = godotenv.Load()
	if err := envconfig.Process("", &cfg); err != nil {
		log.Fatalf("read env error : %s", err.Error())
	}
	if err := validator.New().Struct(&cfg); err != nil {
		return err
	}

	return nil
}

func Get() Config {
	return cfg
}
