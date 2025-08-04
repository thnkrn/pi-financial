package driver

import (
	"fmt"
	"github.com/pi-financial/pi-sso-v2/middleware"
	"go.uber.org/zap"
	"gorm.io/gorm/logger"

	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	gormtracer "gopkg.in/DataDog/dd-trace-go.v1/contrib/gorm.io/gorm.v1"
	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

func ConnectMySQL(log log.Logger, cfg *config.Config) (*gorm.DB, error) {
	gormLogger := middleware.NewZapGormLogger(log, logger.Warn)

	// สร้าง connection string สำหรับ MySQL
	dsn := fmt.Sprintf("%s:%s@tcp(%s:%s)/%s?charset=utf8mb4&parseTime=True&loc=Local",
		cfg.Database.User, cfg.Database.Password, cfg.Database.Host, cfg.Database.Port, cfg.Database.Name)

	db, err := gormtracer.Open(
		mysql.New(mysql.Config{
			DSN: dsn,
		}),
		&gorm.Config{
			DisableAutomaticPing: true,
			Logger:               gormLogger,
		},
	)

	if err != nil {
		log.ErrorNoCtx("Error connecting to MySQL", zap.Error(err))
		return nil, err
	}

	return db, nil
}
