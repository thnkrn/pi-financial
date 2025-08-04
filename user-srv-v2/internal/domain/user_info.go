package domain

import (
	"strings"
	"time"

	"github.com/google/uuid"
	common "github.com/pi-financial/go-common/utils"
	"github.com/pi-financial/user-srv-v2/internal/utils"
	"gorm.io/gorm"
)

type UserInfo struct {
	Id                  uuid.UUID     `gorm:"type:varchar(36);primaryKey" json:"id"`
	CustomerId          string        `gorm:"column:customer_id" json:"customerId"`
	FirstnameTh         string        `gorm:"column:firstname_th;default:null" json:"firstnameTh"`
	FirstnameThHash     string        `gorm:"column:firstname_th_hash;default:null" json:"firstnameThHash"`
	LastnameTh          string        `gorm:"column:lastname_th;default:null" json:"lastnameTh"`
	LastnameThHash      string        `gorm:"column:lastname_th_hash;default:null" json:"lastnameThHash"`
	FirstnameEn         string        `gorm:"column:firstname_en;default:null" json:"firstnameEn"`
	FirstnameEnHash     string        `gorm:"column:firstname_en_hash;default:null" json:"firstnameEnHash"`
	LastnameEn          string        `gorm:"column:lastname_en;default:null" json:"lastnameEn"`
	LastnameEnHash      string        `gorm:"column:lastname_en_hash;default:null" json:"lastnameEnHash"`
	CitizenId           string        `gorm:"column:citizen_id;default:null" json:"citizenId"`
	CitizenIdHash       string        `gorm:"column:citizen_id_hash;default:null" json:"citizenIdHash"`
	PhoneNumber         string        `gorm:"column:phone_number;default:null" json:"phoneNumber"`
	PhoneNumberHash     string        `gorm:"column:phone_number_hash;default:null" json:"phoneNumberHash"`
	Email               string        `gorm:"column:email" json:"email"`
	EmailHash           string        `gorm:"column:email_hash" json:"emailHash"`
	GlobalAccount       string        `gorm:"column:global_account" json:"globalAccount"`
	PlaceOfBirthCountry string        `gorm:"column:place_of_birth_country" json:"placeOfBirthCountry"`
	PlaceOfBirthCity    string        `gorm:"column:place_of_birth_city" json:"placeOfBirthCity"`
	Devices             []Device      `gorm:"foreignKey:UserInfoId" json:"devices"`
	Accounts            []UserAccount `gorm:"foreignKey:UserId" json:"accounts"`
	DateOfBirth         string        `gorm:"column:date_of_birth" json:"dateOfBirth"`
	WealthType          string        `gorm:"column:wealth_type" json:"wealthType"`
	CreatedAt           time.Time     `gorm:"<-:created" json:"createdAt"`
	UpdatedAt           time.Time     `gorm:"autoUpdateTime" json:"updatedAt"`
}

func (a *UserInfo) encryptField(value string) (string, error) {
	encryptedStr, err := utils.RsaEncryption(value)
	if err != nil {
		return "", err
	}

	return encryptedStr, nil
}

func (a *UserInfo) decryptField(value string) (string, error) {
	if value == "" {
		return "", nil
	}

	decryptedStr, err := utils.RsaDecryption(value)
	if err != nil {
		return "", err
	}

	return decryptedStr, nil
}

func (a *UserInfo) encryptAndHashFields() error {
	// Generate hashes for searchable fields
	if a.CitizenId != "" {
		a.CitizenIdHash = utils.Hash(a.CitizenId)
	}
	if a.PhoneNumber != "" {
		a.PhoneNumberHash = utils.Hash(a.PhoneNumber)
	}
	if a.Email != "" {
		a.EmailHash = utils.Hash(a.Email)
	}
	if a.FirstnameTh != "" {
		a.FirstnameThHash = utils.Hash(a.FirstnameTh)
	}
	if a.LastnameTh != "" {
		a.LastnameThHash = utils.Hash(a.LastnameTh)
	}
	if a.FirstnameEn != "" {
		a.FirstnameEnHash = utils.Hash(a.FirstnameEn)
	}
	if a.LastnameEn != "" {
		a.LastnameEnHash = utils.Hash(a.LastnameEn)
	}

	fieldsToEncrypt := map[string]*string{
		"FirstnameTh": &a.FirstnameTh,
		"LastnameTh":  &a.LastnameTh,
		"FirstnameEn": &a.FirstnameEn,
		"LastnameEn":  &a.LastnameEn,
		"CitizenId":   &a.CitizenId,
		"PhoneNumber": &a.PhoneNumber,
		"Email":       &a.Email,
		"DateOfBirth": &a.DateOfBirth,
	}

	for _, value := range fieldsToEncrypt {
		encrypted, err := a.encryptField(*value)
		if err != nil {
			return err
		}
		*value = encrypted
	}

	return nil
}

func (a *UserInfo) NormalizeStringFieldsCases() {
	a.PhoneNumber = strings.ReplaceAll(a.PhoneNumber, "-", "")
	a.Email = strings.ToLower(a.Email)
	a.WealthType = strings.ToLower(a.WealthType)
}

func (a *UserInfo) BeforeCreate(tx *gorm.DB) error {
	a.NormalizeStringFieldsCases()

	a.Id = uuid.New()
	return a.encryptAndHashFields()
}

func (a *UserInfo) BeforeUpdate(tx *gorm.DB) error {
	a.NormalizeStringFieldsCases()

	return a.encryptAndHashFields()
}

func (a *UserInfo) AfterFind(tx *gorm.DB) (err error) {
	if a.FirstnameTh, err = a.decryptField(a.FirstnameTh); err != nil {
		return err
	}
	if a.LastnameTh, err = a.decryptField(a.LastnameTh); err != nil {
		return err
	}
	if a.FirstnameEn, err = a.decryptField(a.FirstnameEn); err != nil {
		return err
	}
	if a.LastnameEn, err = a.decryptField(a.LastnameEn); err != nil {
		return err
	}
	if a.CitizenId, err = a.decryptField(a.CitizenId); err != nil {
		return err
	}
	if a.PhoneNumber, err = a.decryptField(a.PhoneNumber); err != nil {
		return err
	}
	a.PhoneNumber = strings.ReplaceAll(a.PhoneNumber, "-", "")
	a.PhoneNumber = common.LimitString(a.PhoneNumber, 10)
	if a.Email, err = a.decryptField(a.Email); err != nil {
		return err
	}
	if a.DateOfBirth, err = a.decryptField(a.DateOfBirth); err != nil {
		return err
	}

	return nil
}
