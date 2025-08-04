package config

import (
	"fmt"

	"github.com/go-playground/validator/v10"
	"github.com/labstack/gommon/log"
	"github.com/spf13/viper"
)

type Config struct {
	CommonDbDsn   string        `mapstructure:"COMMON_DB_DSN"`
	BOTHost       string        `mapstructure:"BOT_HOST"`
	BOTClientId   string        `mapstructure:"BOT_CLIENT_ID"`
	RedisEnabled  bool          `mapstructure:"REDIS_ENABLED"`
	RedisHost     string        `mapstructure:"REDIS_HOST"`
	RedisUser     string        `mapstructure:"REDIS_USER"`
	RedisPassword string        `mapstructure:"REDIS_PASSWORD"`
	RedisDB       string        `mapstructure:"REDIS_DB"`
	RedisSSL      string        `mapstructure:"REDIS_SSL"`
	DatadogConfig DatadogConfig `mapstructure:",squash"`
}

type DatadogConfig struct {
	DDEnv            string `mapstructure:"DD_ENV"`
	DDService        string `mapstructure:"DD_SERVICE"`
	DDComponent      string `mapstructure:"DD_COMPONENT"`
	DDVersion        string `mapstructure:"DD_VERSION"`
	DDAgentHost      string `mapstructure:"DD_AGENT_HOST"`
	DDLogLevel       string `mapstructure:"DD_LOG_LEVEL"`
	DDTraceAgentPort string `mapstructure:"DD_TRACE_AGENT_PORT"`
	DDLogsEnabled    bool   `mapstructure:"DD_LOGS_ENABLED"`
}

func LoadConfig() (Config, error) {
	var config Config

	viper.AddConfigPath(".")
	viper.SetConfigType("env")
	viper.SetConfigFile(".env")
	viper.AutomaticEnv()
	err := viper.MergeInConfig()
	if err != nil {
		return config, err
	}

	viper.SetConfigFile(".env.local")
	err = viper.MergeInConfig()
	if err != nil {
		log.Info(fmt.Sprintf("Error loading local env file: %s", err))
	}

	if err := viper.Unmarshal(&config); err != nil {
		return config, err
	}

	if err := validator.New().Struct(&config); err != nil {
		return config, err
	}

	return config, nil
}
