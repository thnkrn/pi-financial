package config

import (
	"fmt"

	"github.com/go-playground/validator/v10"
	"github.com/spf13/viper"
)

type Config struct {
	OnePortUrl                      string `mapstructure:"ONEPORT_URL"`
	FisUrl                          string `mapstructure:"FIS_URL"`
	UserUrl                         string `mapstructure:"USER_URL"`
	UserV2Url                       string `mapstructure:"USER_V2_URL"`
	DBUser                          string `mapstructure:"DB_USER"`
	DBPassword                      string `mapstructure:"DB_PASSWORD"`
	DBHost                          string `mapstructure:"DB_HOST"`
	DBName                          string `mapstructure:"DB_NAME"`
	OneportMaintenanceStartDateTime string `mapstructure:"ONEPORT_MAINTENANCE_START_DATETIME"`
	OneportMaintenanceEndDateTime   string `mapstructure:"ONEPORT_MAINTENANCE_END_DATETIME"`
	GrowthbookHost                  string `mapstructure:"GROWTHBOOK_HOST"`
	GrowthBookApiKey                string `mapstructure:"GROWTHBOOK_API_KEY"`
	GrowthbookProjectId             string `mapstructure:"GROWTHBOOK_PROJECT_ID"`
	PortfolioV2Url                  string `mapstructure:"PORTFOLIO_V2_URL"`
}

func LoadConfig() (Config, error) {
	var envs = []string{
		"ONEPORT_URL",
		"FIS_URL",
		"USER_URL",
		"USER_V2_URL",
		"DB_USER",
		"DB_PASSWORD",
		"DB_HOST",
		"DB_NAME",
		"ONEPORT_MAINTENANCE_START_DATETIME",
		"ONEPORT_MAINTENANCE_END_DATETIME",
		"GROWTHBOOK_HOST",
		"GROWTHBOOK_API_KEY",
		"GROWTHBOOK_PROJECT_ID",
		"PORTFOLIO_V2_URL",
	}

	var config Config

	viper.AddConfigPath("./")
	viper.SetConfigFile(".env")

	if err := viper.ReadInConfig(); err != nil {
		fmt.Println("Not found .env file, using OS environment variables instead")
		viper.AutomaticEnv()
	}

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
