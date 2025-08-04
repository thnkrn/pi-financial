package repositories

import (
	"context"
	"errors"
	"fmt"

	"github.com/pi-financial/information-srv/internal/core/domain/address"
	"github.com/pi-financial/information-srv/internal/driver/log"
	"github.com/pi-financial/information-srv/internal/driver/mysql"
	"gorm.io/gorm"
)

type AddressRepository struct {
	db     *gorm.DB
	logger log.Logger
}

func NewAddressRepository(db *mysql.CommonDb, logger log.Logger) *AddressRepository {
	addressAdapter := &AddressRepository{db: (*gorm.DB)(db), logger: logger}
	return addressAdapter
}

const queryErrorMsg = "error executing query: %v"
const dbConnectErrorMsg = "unable to connect to common-db"

const baseQuery = `
SELECT p.name_th AS province_th,
	p.name_en AS province_en,
	d.name_th  AS district_th,
	d.name_en  AS district_en,
	sd.name_th AS sub_district_th,
	sd.name_en AS sub_district_en,
	sd.zip_code 
FROM provinces p 
INNER JOIN districts d ON d.province_id = p.id 
INNER JOIN sub_districts sd ON sd.district_id = d.id
`

const commonDbNilMsg = "common-db is nil"

func (repo *AddressRepository) GetProvinces(ctx context.Context) ([]address.Province, error) {
	if repo.db == nil {
		repo.logger.Error(commonDbNilMsg)
		return nil, errors.New(dbConnectErrorMsg)
	}

	var provinces []address.Province

	err := repo.db.Raw(`
SELECT name_en, name_th, code, iso3166code as iso_code
FROM provinces p 
ORDER BY iso3166code`).Scan(&provinces).Error
	if err != nil {
		return nil, fmt.Errorf(queryErrorMsg, err)
	}
	return provinces, nil
}

func (repo *AddressRepository) GetAddresses(ctx context.Context) ([]address.Address, error) {
	if repo.db == nil {
		repo.logger.Error(commonDbNilMsg)
		return nil, errors.New(dbConnectErrorMsg)
	}

	var addresses []address.Address

	query := fmt.Sprintf("%s ORDER BY zip_code ASC", baseQuery)
	err := repo.db.Raw(query).Scan(&addresses).Error
	if err != nil {
		return nil, fmt.Errorf(queryErrorMsg, err)
	}
	return addresses, nil
}

func (repo *AddressRepository) GetAddressesByZipCode(ctx context.Context, zipCode int) ([]address.Address, error) {
	if repo.db == nil {
		repo.logger.Error(commonDbNilMsg)
		return nil, errors.New(dbConnectErrorMsg)
	}

	var addresses []address.Address
	query := fmt.Sprintf("%s WHERE sd.zip_code = %d", baseQuery, zipCode)
	err := repo.db.Raw(query).Scan(&addresses).Error

	if err != nil {
		return nil, fmt.Errorf(queryErrorMsg, err)
	}
	return addresses, nil
}

func (repo *AddressRepository) GetAddressesByProvince(ctx context.Context, province string, lang string) ([]address.Address, error) {
	if repo.db == nil {
		repo.logger.Error(commonDbNilMsg)
		return nil, errors.New(dbConnectErrorMsg)
	}

	var addresses []address.Address
	var name string
	if lang == "en" || lang == "EN" {
		name = "name_en"
	} else {
		name = "name_th"
	}
	query := fmt.Sprintf("%s WHERE p.%s = '%s' ORDER BY zip_code ASC", baseQuery, name, province)
	err := repo.db.Raw(query).Scan(&addresses).Error
	if err != nil {
		return nil, fmt.Errorf(queryErrorMsg, err)
	}
	return addresses, nil
}
