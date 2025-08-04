package constants

import (
	"fmt"
	"time"

	"github.com/pi-financial/go-common/errorx"
)

var (
	ErrAddressNotFound                    = errorx.NewErrCodeMsg("USR0004", "Address not found")
	ErrUserIdRequired                     = errorx.NewErrCodeMsg("USR0005", "UserId is required")
	ErrInvalidUUIDFormat                  = errorx.NewErrCodeMsg("USR0006", "Invalid UUID format")
	ErrKycNotFound                        = errorx.NewErrCodeMsg("USR0010", "Kyc not found")
	ErrInvalidDate                        = errorx.NewErrCodeMsg("USR0011", "Invalid date format")
	ErrFindSuitabilityTestsByUserId       = errorx.NewErrCodeMsg("USR0012", "Error while finding suitability tests by user id")
	ErrCreateSuitabilityTest              = errorx.NewErrCodeMsg("USR0013", "Error creating suitability test. Please make sure the user-id exists and request data format is correct.")
	ErrSuitabilityTestScoreNotANumber     = errorx.NewErrCodeMsg("USR0014", "Error suitability test score can't be converted to a number")
	ErrInvalidDateStringFormat            = errorx.NewErrCodeMsg("USR0015", "Error date string is not in a valid format")
	ErrReviewDateInvalidDateStringFormat  = errorx.NewErrCodeMsg("USR0016", fmt.Sprintf("Error review date string is not in a valid format: e.g. %s", time.DateOnly))
	ErrExpiredDateInvalidDateStringFormat = errorx.NewErrCodeMsg("USR0017", fmt.Sprintf("Error expired date string is not in a valid format: e.g. %s", time.DateOnly))
	ErrFindBankInfoByBankCode             = errorx.NewErrCodeMsg("USR0018", "Error finding bank info by bank code")
	ErrBankInfoNotFound                   = errorx.NewErrCodeMsg("USR0019", "Bank info not found")
	ErrBankAccountNotFound                = errorx.NewErrCodeMsg("USR0020", "Bank account not found")
	ErrGetProductByProductName            = errorx.NewErrCodeMsg("USR0021", "Error finding product with the given product name")
	ErrNoProductWithProductName           = errorx.NewErrCodeMsg("USR0022", "No product with the given product name")
	ErrFindTradeAccountByAccountNumber    = errorx.NewErrCodeMsg("USR0023", "Error finding trade account by account number")
	ErrCreateExternalAccount              = errorx.NewErrCodeMsg("USR0024", "Error creating external account. Please make sure that the product exists, customer code must belong to an existing user, and the account value is a valid external account number.")
	ErrUpsertExternalAccount              = errorx.NewErrCodeMsg("USR0026", "Error upserting external account. Please make sure that the product exists, customer code must belong to an existing user, user must have a trade account associated with that customer code, and the account value is a valid external account number.")
	ErrCitizenIdRequired                  = errorx.NewErrCodeMsg("USR0028", "CitizenId is required")
	ErrCustomerCodeRequired               = errorx.NewErrCodeMsg("USR0039", "Customer code is required")
)

// Watchlist db errors
var (
	ErrDuplicateWatchlist = errorx.NewErrCodeMsg("USR0001", "watchlist already exists")
	ErrFindingWatchlist   = errorx.NewErrCodeMsg("USR00039", "problem getting watchlists from db")
)

// Product errors
var (
	ErrNoProduct     = errorx.NewErrCodeMsg("USR0028", "product not found")
	ErrNoProductCode = errorx.NewErrCodeMsg("USR0029", "product code not found")
)

// Bank account errors
var (
	ErrNoBankInfo                        = errorx.NewErrCodeMsg("USR0030", "bank info not found")
	ErrNoAtsBankAccount                  = errorx.NewErrCodeMsg("USR0031", "ats bank account not found")
	ErrNoAtsBankAccounts                 = errorx.NewErrCodeMsg("USR0032", "ats bank accounts not found")
	ErrNoPurposeRpType                   = errorx.NewErrCodeMsg("USR0033", "purpose have no associated rp type")
	ErrNoAccountTypeCodeTransactionTypes = errorx.NewErrCodeMsg("USR0034", "account type code have no associated transaction types")
)

// External api errors
var (
	ErrItDataSrvInvalidApiKey      = errorx.NewErrCodeMsg("USR0035", "invalid api key for it data service")
	ErrItDataSrvGetAtsBankAccounts = errorx.NewErrCodeMsg("USR0036", "problem getting ats bank accounts from it data service")
	ErrInformationSrvGetProduct    = errorx.NewErrCodeMsg("USR0037", "problem getting product from information service")
	ErrInformationSrvGetBank       = errorx.NewErrCodeMsg("USR0038", "problem getting bank from information service")
	ErrItDataSrvGetKyc             = errorx.NewErrCodeMsg("USR0039", "problem getting kyc from it data service")
	ErrItDataSrvGetSuitTest        = errorx.NewErrCodeMsg("USR0042", "problem getting suit test from it data service")
	ErrItDataSrvGetAddress         = errorx.NewErrCodeMsg("USR0043", "problem getting address from it data service")
	ErrItDataSrvGetAccount         = errorx.NewErrCodeMsg("USR0044", "problem getting account from it data service")
	ErrOnboardSrvGetExamQuestions  = errorx.NewErrCodeMsg("USR0044", "problem getting exam questions from onboard service")
)

// Trading account errors
var (
	ErrInvalidTradingAccountNo = errorx.NewErrCodeMsg("USR0027", "invalid trading account no format")
	ErrNoTradingAccounts       = errorx.NewErrCodeMsg("USR0040", "trading accounts not found")
)

// User account errors
var (
	ErrUpsertUserAccount             = errorx.NewErrCodeMsg("USR0002", "upsert user account error")
	ErrFindUserAccountByUserId       = errorx.NewErrCodeMsg("USR0007", "error while finding user accounts by user id")
	ErrFindUserAccountByCustomerCode = errorx.NewErrCodeMsg("USR0008", "error while finding user accounts by customer code. Customer code must exist and the user account type must be Freewill.")
	ErrUserAccountNotFound           = errorx.NewErrCodeMsg("USR0009", "user account not found")
	ErrUserAccountUserIdMismatch     = errorx.NewErrCodeMsg("USR0025", "user account's user id does not match the provided user id.")
	ErrCustomerInfoNotFound          = errorx.NewErrCodeMsg("USR0049", "customer info not found")
)

// User info errors
var (
	ErrFindingUserInfo          = errorx.NewErrCodeMsg("USR0041", "error while finding user info")
	ErrDuplicateSubUser         = errorx.NewErrCodeMsg("USR0042", "duplicate sub user")
	ErrEmailAlreadyExists       = errorx.NewErrCodeMsg("USR0043", "email already exists")
	ErrCitizenIdAlreadyExists   = errorx.NewErrCodeMsg("USR0044", "citizen id already exists")
	ErrPhoneNumberAlreadyExists = errorx.NewErrCodeMsg("USR0045", "phone number already exists")
)

// Change request errors
var (
	ErrNoChangesDetected     = errorx.NewErrCodeMsg("USR0046", "no changes detected: all current values match change values")
	ErrChangeRequestNotFound = errorx.NewErrCodeMsg("USR0047", "change request not found")
	ErrDopaVerify            = errorx.NewErrCodeMsg("USR0048", "invalid id card information or id card expired")
)
