package sba

// Struct สำหรับ account_list
type Account struct {
	CardID         string  `json:"cardid"`
	CustCode       string  `json:"custcode"`
	CustAcct       string  `json:"custacct"`
	AppCreditLine  string  `json:"appcreditline"`
	IceName        string  `json:"icename"`
	Account        string  `json:"account"`
	AcctCode       string  `json:"acctcode"`
	CustAcctDesc   string  `json:"custacctdesc"`
	EnableSell     string  `json:"enablesell"`
	IctName        string  `json:"ictname"`
	EnableDeposit  string  `json:"enabledeposit"`
	CreditType     string  `json:"credittype"`
	EndDate        *string `json:"enddate"`
	EnableWithdraw string  `json:"enablewithdraw"`
	XchgMkt        string  `json:"xchgmkt"`
	LastTradeDate  *string `json:"lasttradedate"`
	EffDate        *string `json:"effdate"`
	OpenDate       string  `json:"opendate"`
	EnableBuy      string  `json:"enablebuy"`
	AcctStatus     string  `json:"acctstatus"`
	SaleLicence    string  `json:"salelicence"`
	MktID          string  `json:"mktid"`
	Channel        string  `json:"channel"`
	FrontName      *string `json:"frontname"`
}

// Struct สำหรับ xchgmkt_list
type XchgMkt struct {
	CardID             string `json:"cardid"`
	CustCode           string `json:"custcode"`
	LineExpire         string `json:"lineexpire"`
	ApprDate           string `json:"apprdate"`
	ShortAppCreditLine string `json:"shortappcreditline"`
	LineEffective      string `json:"lineeffective"`
	XchgMkt            string `json:"xchgmkt"`
	AppCreditLine      string `json:"appcreditline"`
}

// Struct สำหรับ response ทั้งหมด
type ResponseAccountTabInfo struct {
	AccountNo string `json:"accountNo"`
	CanBuy    string `json:"canBuy"`
	CanSell   string `json:"canSell"`
}

type ResponseAccountInfo struct {
	SendTime         string    `json:"send_time"`
	SendDate         string    `json:"send_date"`
	AccountListTotal string    `json:"account_list_total"`
	XchgMktListTotal string    `json:"xchgmkt_list_total"`
	AccountList      []Account `json:"account_list"`
	XchgMktList      []XchgMkt `json:"xchgmkt_list"`
}

type ResultCustInfo struct {
	CardID                    *string `json:"cardid"`
	CustCode                  *string `json:"custcode"`
	WealthType                *string `json:"wealthtype"`
	EquityExperience          *string `json:"equityexperience"`
	ClientType                *string `json:"clienttype"`
	Branch                    *string `json:"branch"`
	GroupName                 *string `json:"groupname"`
	TitleCode                 *string `json:"titlecode"`
	DiscloseOmni              *string `json:"discloseomni"`
	AppCreditLine             *string `json:"appcreditline"`
	Birthday                  *string `json:"birthday"`
	FinancialEducation        *string `json:"financialeducation"`
	VatFlag                   *string `json:"vatflag"`
	CorpCode                  *string `json:"corpcode"`
	FrontCustType             *string `json:"frontcusttype"`
	CustStatus                *string `json:"custstatus"`
	MktID                     *string `json:"mktid"`
	MktGrade                  *string `json:"mktgrade"`
	SuitFlag                  *string `json:"suitflag"`
	EdocMail                  *string `json:"edocmail"`
	Sex                       *string `json:"sex"`
	Email                     *string `json:"email"`
	OccpCode                  *string `json:"occpcode"`
	LineEffective             *string `json:"lineeffective"`
	CustGrp                   *string `json:"custgrp"`
	TTitle                    *string `json:"ttitle"`
	InstituteType             *string `json:"institutetype"`
	DutyFlag                  *string `json:"dutyflag"`
	CustodianID               *string `json:"custodianid"`
	TName                     *string `json:"tname"`
	CheckDgt                  *string `json:"checkdgt"`
	CardIssue                 *string `json:"cardissue"`
	Resource                  *string `json:"resource"`
	CustType                  *string `json:"custtype"`
	EName                     *string `json:"ename"`
	StaffFlag                 *string `json:"staffflag"`
	ESurname                  *string `json:"esurname"`
	SettleID                  *string `json:"settleid"`
	ApproveDate               *string `json:"approvedate"`
	CustodianFlag             *string `json:"custodianflag"`
	ETitle                    *string `json:"etitle"`
	LFullName                 *string `json:"lfullname"`
	HighRiskProductExperience *string `json:"highriskproductexperience"`
	WalletFlag                *string `json:"walletflag"`
	PersonCode                *string `json:"personcode"`
	EFullName                 *string `json:"efullname"`
	NationCode                *string `json:"nationcode"`
	ShortName                 *string `json:"shortname"`
	OperGrade                 *string `json:"opergrade"`
	Department                *string `json:"department"`
	TSurname                  *string `json:"tsurname"`
	SubType                   *string `json:"subtype"`
	TaxID                     *string `json:"taxid"`
	LineExpire                *string `json:"lineexpire"`
	CardIDType                *string `json:"cardidtype"`
	ContractMethod            *string `json:"contractmethod"`
	CardExpire                *string `json:"cardexpire"`
	ContactPerson             *string `json:"contactperson"`
	MobileNo                  *string `json:"mobileno"`
	Channel                   *string `json:"channel"`
}

type ResponseCustInfo struct {
	SendTime        *string          `json:"send_time"`
	SendDate        *string          `json:"send_date"`
	ResultListTotal *string          `json:"result_list_total"`
	ResultList      []ResultCustInfo `json:"result_list"`
}

type ResponseEfinUser struct {
	ID           int    `json:"id"`
	UserName     string `json:"userName"`
	Password     string `json:"password"`
	MemberID     int    `json:"memberId"`
	LastLoggedIn string `json:"lastLoggedIn"`
}

type ResponseLogSession struct {
	UserName  string `json:"userName"`
	AuthType  string `json:"authType"`
	SessionID string `json:"sessionId"`
	TimeStamp string `json:"timeStamp"`
}
