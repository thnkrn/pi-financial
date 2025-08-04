package config

import (
	"github.com/go-playground/validator/v10"
	"github.com/spf13/viper"
)

type Config struct {
	DBHost        string `mapstructure:"DB_HOST"`
	DBUsername    string `mapstructure:"DB_USERNAME"`
	DBName        string `mapstructure:"DB_NAME"`
	DBPassword    string `mapstructure:"DB_PASSWORD"`
	DBPort        string `mapstructure:"DB_PORT"`
	UserSrvV2Host string `mapstructure:"USER_SRV_V2_HOST"`
}

var envs = []string{
	"DB_HOST",
	"DB_USERNAME",
	"DB_NAME",
	"DB_PASSWORD",
	"DB_PORT",
	"USER_SRV_V2_HOST",
}

func LoadConfig() (Config, error) {
	var config Config

	viper.AddConfigPath("./")
	viper.SetConfigFile(".env")
	_ = viper.ReadInConfig()

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
