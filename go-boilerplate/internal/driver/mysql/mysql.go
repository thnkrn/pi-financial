package mysql

import (
	"gorm.io/driver/mysql"
	"gorm.io/gorm"

	config "github.com/pi-financial/go-boilerplate/config"
)

func ConnectDatabase(cfg config.Config) (*gorm.DB, error) {
	// dsn := fmt.Sprintf("%s:%s@tcp(%s:%s)/%s?charset=utf8mb4&parseTime=True&loc=Local", cfg.DBHost, cfg.DBUser, cfg.DBName, cfg.DBPort, cfg.DBPassword)
	dsn := "go-boilerplate-user:P@ssword@tcp(127.0.0.1:3306)/go-boilerplate-db?charset=utf8mb4&parseTime=True&loc=Local"
	db, dbErr := gorm.Open(mysql.Open(dsn), &gorm.Config{
		SkipDefaultTransaction: true,
	})

	return db, dbErr
}
