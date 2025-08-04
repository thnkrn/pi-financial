package mysql

import (
	"fmt"

	"gorm.io/driver/mysql"

	"gorm.io/gorm"

	config "github.com/pi-financial/user-srv-v2/config"
	gormtracer "gopkg.in/DataDog/dd-trace-go.v1/contrib/gorm.io/gorm.v1"
)

func ConnectDatabase(cfg config.Config) (*gorm.DB, error) {
	dsn := fmt.Sprintf("%s:%s@tcp(%s:%s)/%s?charset=utf8mb4&parseTime=True&loc=Local", cfg.DBUsername, cfg.DBPassword, cfg.DBHost, cfg.DBPort, cfg.DBName)
	db, err := gormtracer.Open(mysql.Open(dsn), &gorm.Config{
		SkipDefaultTransaction: true,
	})

	return db, err
}
