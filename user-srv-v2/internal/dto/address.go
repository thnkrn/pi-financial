package dto

type Address struct {
	Place        string `json:"place"`
	HomeNo       string `json:"homeNo"`
	Town         string `json:"town"`
	Building     string `json:"building"`
	Village      string `json:"village"`
	Floor        string `json:"floor"`
	Soi          string `json:"soi"`
	Road         string `json:"road"`
	SubDistrict  string `json:"subDistrict"`
	District     string `json:"district"`
	Province     string `json:"province"`
	Country      string `json:"country"`
	ZipCode      string `json:"zipCode"`
	CountryCode  string `json:"countryCode"`
	ProvinceCode string `json:"provinceCode"`
}
