package mysql

import (
	"fmt"

	"github.com/pi-financial/information-srv/internal/driver/log"
	"github.com/spf13/viper"
	gormtracer "gopkg.in/DataDog/dd-trace-go.v1/contrib/gorm.io/gorm.v1"
	gorm_mysql "gorm.io/driver/mysql"
	"gorm.io/gorm"
)

type CommonDb gorm.DB

type MySqlAdapter struct {
	CommonDb *CommonDb
}

func NewMySqlAdapter(logger log.Logger) *MySqlAdapter {
	// Example of DSN: root:P@ssw0rd@tcp(127.0.0.1:3306)/common_db?charset=utf8mb4&parseTime=True&loc=Local
	commonDbDsn := viper.GetString("COMMON_DB_DSN")
	commonDb, err := gormtracer.Open(
		gorm_mysql.New(gorm_mysql.Config{
			DSN: commonDbDsn,
		}),
		&gorm.Config{DisableAutomaticPing: true},
	)

	if err != nil {
		errMsg := fmt.Sprintf("Error: Could not connect to common-db: %v", err)
		logger.Error(errMsg)
		// return &MySqlAdapter{CommonDb: nil}
		panic(err)
	}

	configSqlConnection(*commonDb, logger)

	return &MySqlAdapter{CommonDb: (*CommonDb)(commonDb)}
}

func configSqlConnection(db gorm.DB, logger log.Logger) {
	sqlDb, err := db.DB()
	if err != nil {
		logger.Error(fmt.Sprintf("Error getting *sql.DB object: %v", err))
	}

	sqlDb.SetMaxOpenConns(100)
	sqlDb.SetMaxIdleConns(10)
	sqlDb.SetMaxOpenConns(0)

}
