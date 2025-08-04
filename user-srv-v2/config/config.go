package config

import (
	"sync"

	"github.com/go-playground/validator/v10"
	"github.com/joho/godotenv"
	"github.com/spf13/viper"
)

type Config struct {
	DBHost              string `mapstructure:"DB_HOST" validate:"required"`
	DBUsername          string `mapstructure:"DB_USER" validate:"required"`
	DBPassword          string `mapstructure:"DB_PASSWORD" validate:"required"`
	DBName              string `mapstructure:"DB_NAME" validate:"required"`
	DBPort              string `mapstructure:"DB_PORT" validate:"required"`
	DBSalt              string `mapstructure:"DB_SALT"`
	DBPrivateKey        string `mapstructure:"DB_PRIVATE_KEY"`
	DBPublicKey         string `mapstructure:"DB_PUBLIC_KEY"`
	DDEnv               string `mapstructure:"DD_ENV"`
	DDService           string `mapstructure:"DD_SERVICE"`
	DDComponent         string `mapstructure:"DD_COMPONENT"`
	DDVersion           string `mapstructure:"DD_VERSION"`
	DDAgentHost         string `mapstructure:"DD_AGENT_HOST"`
	DDLogLevel          string `mapstructure:"DD_LOG_LEVEL"`
	DDTraceAgentPort    string `mapstructure:"DD_TRACE_AGENT_PORT"`
	OnboardSrvHost      string `mapstructure:"ONBOARD_SRV_HOST"`
	InformationSrvHost  string `mapstructure:"INFORMATION_SRV_HOST"`
	ItDataSrvHost       string `mapstructure:"IT_DATA_SRV_HOST"`
	ItDataSrvApiKey     string `mapstructure:"IT_DATA_SRV_API_KEY"`
	GrowthbookHost      string `mapstructure:"GROWTHBOOK_HOST"`
	GrowthBookApiKey    string `mapstructure:"GROWTHBOOK_API_KEY"`
	GrowthbookProjectId string `mapstructure:"GROWTHBOOK_PROJECT_ID"`
	GrowthbookQaMode    bool   `mapstructure:"GROWTHBOOK_QA_MODE"`
	DopaHost            string `mapstructure:"DOPA_HOST"`
	DopaKey             string `mapstructure:"DOPA_KEY"`
	DopaUsername        string `mapstructure:"DOPA_USERNAME"`
	DopaPassword        string `mapstructure:"DOPA_PASSWORD"`
}

var (
	config Config
	once   sync.Once
	envs   = []string{"DB_HOST", "DB_USER", "DB_PASSWORD", "DB_NAME", "DB_PORT", "DB_SALT", "DB_PRIVATE_KEY", "DB_PUBLIC_KEY", "DD_ENV", "DD_SERVICE", "DD_COMPONENT", "DD_VERSION", "DD_AGENT_HOST", "DD_LOG_LEVEL", "DD_TRACE_AGENT_PORT", "ONBOARD_SRV_HOST", "INFORMATION_SRV_HOST", "IT_DATA_SRV_HOST", "IT_DATA_SRV_API_KEY", "GROWTHBOOK_HOST", "GROWTHBOOK_API_KEY", "GROWTHBOOK_PROJECT_ID", "GROWTHBOOK_QA_MODE", "DOPA_HOST", "DOPA_KEY", "DOPA_USERNAME", "DOPA_PASSWORD"}
	err    error
)

func init() {
	_ = godotenv.Load(".env")
	viper.AutomaticEnv()

	for _, env := range envs {
		_ = viper.BindEnv(env)
	}
}

func LoadConfig() (Config, error) {
	once.Do(func() {
		err = viper.Unmarshal(&config)
		if err != nil {
			return
		}
		err = validator.New().Struct(&config)
	})
	return config, err
}
