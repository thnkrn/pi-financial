package bankBranch

type BankBranch struct {
	Id             string `json:"id" gorm:"column:id"`
	BankCode       string `json:"bank_code" gorm:"column:bank_code"`
	BankBranchCode string `json:"bank_branch_code" gorm:"column:bank_branch_code"`
	BranchName     string `json:"branch_name" gorm:"column:branch_name"`
}

func (BankBranch) TableName() string {
	return "bank_branches"
}
