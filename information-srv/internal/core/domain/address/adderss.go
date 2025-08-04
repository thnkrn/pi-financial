package address

type Province struct {
	ProvinceTH string `json:"province_th" gorm:"column:name_th"`
	ProvinceEN string `json:"province_en" gorm:"column:name_en"`
	Code       string `json:"code" gorm:"column:code"`
	ISOCode    string `json:"iso_code" gorm:"column:iso_code"`
}

type Address struct {
	ProvinceTH    string `json:"province_th" gorm:"column:province_th"`
	ProvinceEN    string `json:"province_en" gorm:"column:province_en"`
	DistrictTH    string `json:"district_th" gorm:"column:district_th"`
	DistrictEN    string `json:"district_en" gorm:"column:district_en"`
	SubDistrictTH string `json:"sub_district_th" gorm:"column:sub_district_th"`
	SubDistrictEN string `json:"sub_district_en" gorm:"column:sub_district_en"`
	ZipCode       string `json:"zip_code" gorm:"column:zip_code"`
}
