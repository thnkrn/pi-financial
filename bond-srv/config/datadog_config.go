package config

import (
	"github.com/go-playground/validator/v10"
	"github.com/spf13/viper"
)

type DatadogConfig struct {
	DDEnv            string `mapstructure:"DD_ENV"`
	DDService        string `mapstructure:"DD_SERVICE"`
	DDComponent      string `mapstructure:"DD_COMPONENT"`
	DDVersion        string `mapstructure:"DD_VERSION"`
	DDAgentHost      string `mapstructure:"DD_AGENT_HOST"`
	DDLogLevel       string `mapstructure:"DD_LOG_LEVEL"`
	DDTraceAgentPort string `mapstructure:"DD_TRACE_AGENT_PORT"`
}

func LoadDatadogConfig() (DatadogConfig, error) {
	var envs = []string{
		"DD_ENV",
		"DD_SERVICE",
		"DD_COMPONENT",
		"DD_VERSION",
		"DD_AGENT_HOST",
		"DD_LOG_LEVEL",
		"DD_TRACE_AGENT_PORT",
	}

	var config DatadogConfig

	viper.AddConfigPath("./")
	viper.AutomaticEnv()

	for _, env := range envs {
		if err := viper.BindEnv(env); err != nil {
			return config, err
		}
	}

	if err := viper.Unmarshal(&config); err != nil {
		return config, err
	}

	if err := validator.New().Struct(&config); err != nil {
		return config, err
	}

	return config, nil
}
