package driver

import (
	"fmt"

	"github.com/pi-financial/onboard-srv-v2/config"
	"gorm.io/driver/mysql"
	"gorm.io/gorm"
	"gorm.io/gorm/logger"
)

func NewDatabase() *gorm.DB {
	dsn := fmt.Sprintf(
		"%s:%s@tcp(%s:%s)/%s?charset=utf8mb4&parseTime=true&loc=Local",
		config.Get().Database.Username,
		config.Get().Database.Password,
		config.Get().Database.Host,
		config.Get().Database.Port,
		config.Get().Database.Database,
	)

	db, err := gorm.Open(mysql.Open(dsn), &gorm.Config{
		Logger: logger.Default.LogMode(logger.Info), // Enable logging for debugging
	})
	if err != nil {
		panic(err)
	}

	return db
}
