package mysql

import (
	"fmt"

	"gorm.io/driver/mysql"
	"gorm.io/gorm"

	config "github.com/pi-financial/bond-srv/config"
)

func ConnectDatabase(cfg config.Config) (*gorm.DB, error) {
	dsnInfo := fmt.Sprintf("%s:%s@tcp(%s)/%s?charset=utf8mb4&parseTime=True&loc=Local", cfg.DBUser, cfg.DBPassword, cfg.DBHost, cfg.DBName)
	db, err := gorm.Open(mysql.Open(dsnInfo), &gorm.Config{})

	return db, err
}
