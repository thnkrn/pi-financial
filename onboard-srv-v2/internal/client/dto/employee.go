package client

type EmployeeInfo struct {
	Id            string  `json:"id"`
	DivisionCode  string  `json:"divisionCode"`
	NameTh        string  `json:"nameTh"`
	NameEn        string  `json:"nameEn"`
	EffectiveDate *string `json:"effectiveDate"` // 2025-12-31
	CloseDate     *string `json:"closeDate"`     // 2025-12-31
	TeamId        *string `json:"teamId"`
	Email         *string `json:"email"`
}
