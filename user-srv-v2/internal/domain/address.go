package domain

import (
	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/utils"
	"gorm.io/gorm"
)

type Address struct {
	Id           uuid.UUID `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	UserId       uuid.UUID `gorm:"column:user_id;type:varchar(36)" json:"userId"`
	Place        string    `gorm:"column:place" json:"place"`
	HomeNo       string    `gorm:"column:home_no" json:"homeNo"`
	Town         string    `gorm:"column:town" json:"town"`
	Building     string    `gorm:"column:building" json:"building"`
	Village      string    `gorm:"column:village" json:"village"`
	Floor        string    `gorm:"column:floor" json:"floor"`
	Soi          string    `gorm:"column:soi" json:"soi"`
	Road         string    `gorm:"column:road" json:"road"`
	SubDistrict  string    `gorm:"column:sub_district" json:"subDistrict"`
	District     string    `gorm:"column:district" json:"district"`
	Province     string    `gorm:"column:province" json:"province"`
	Country      string    `gorm:"column:country" json:"country"`
	ZipCode      string    `gorm:"column:zip_code" json:"zipCode"`
	CountryCode  string    `gorm:"column:country_code" json:"countryCode"`
	ProvinceCode string    `gorm:"column:province_code" json:"provinceCode"`
}

func (a *Address) BeforeCreate(tx *gorm.DB) (err error) {
	a.Id = uuid.New()
	return a.encryptAddressFields()
}

func (a *Address) BeforeUpdate(tx *gorm.DB) (err error) {
	return a.encryptAddressFields()
}

func (a *Address) encryptAddressFields() error {
	fieldsToEncrypt := []struct {
		value *string
	}{
		{&a.Place},
		{&a.HomeNo},
		{&a.Town},
		{&a.Building},
		{&a.Village},
		{&a.Floor},
		{&a.Soi},
		{&a.Road},
		{&a.SubDistrict},
		{&a.District},
		{&a.Province},
		{&a.Country},
	}

	for _, field := range fieldsToEncrypt {
		encrypted, err := a.encryptField(*field.value)
		if err != nil {
			return err
		}
		*field.value = encrypted
	}

	return nil
}

func (a *Address) encryptField(value string) (string, error) {
	//check empty string will do nothing
	if value == "" {
		return value, nil // Return unchanged
	}
	encryptedStr, err := utils.RsaEncryption(value)
	if err != nil {
		return "", err
	}
	return encryptedStr, nil
}

func (a *Address) decryptField(value string) (string, error) {
	//check empty string will do nothing
	if value == "" {
		return value, nil // Return unchanged
	}
	decryptedStr, err := utils.RsaDecryption(value)
	if err != nil {
		return "", err
	}
	return decryptedStr, nil
}

func (a *Address) AfterFind(tx *gorm.DB) (err error) {
	if a.Place, err = a.decryptField(a.Place); err != nil {
		return err
	}
	if a.HomeNo, err = a.decryptField(a.HomeNo); err != nil {
		return err
	}
	if a.Town, err = a.decryptField(a.Town); err != nil {
		return err
	}
	if a.Building, err = a.decryptField(a.Building); err != nil {
		return err
	}
	if a.Village, err = a.decryptField(a.Village); err != nil {
		return err
	}
	if a.Floor, err = a.decryptField(a.Floor); err != nil {
		return err
	}
	if a.Soi, err = a.decryptField(a.Soi); err != nil {
		return err
	}
	if a.Road, err = a.decryptField(a.Road); err != nil {
		return err
	}
	if a.SubDistrict, err = a.decryptField(a.SubDistrict); err != nil {
		return err
	}
	if a.District, err = a.decryptField(a.District); err != nil {
		return err
	}
	if a.Province, err = a.decryptField(a.Province); err != nil {
		return err
	}
	if a.Country, err = a.decryptField(a.Country); err != nil {
		return err
	}

	return nil
}
