package interfaces

import (
	"context"

	goclient "github.com/pi-financial/onboard-srv/go-client"
)

type OnboardClient interface {
	GetTradingAccountByCustomerCode(ctx context.Context, customerCode string, withBankAccounts bool, withExternalAccounts bool) ([]goclient.PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccount, error)
	GetBanksByUserId(ctx context.Context, userId string) ([]goclient.PiOnboardServiceAPIModelsBankInfoDto, error)
	GetBankAccountForDepositWithdrawalByProductName(ctx context.Context, customerCode string, purpose string, productName string) (*goclient.PiOnboardServiceApplicationModelsBankAccountDetail, error)
	GetExamQuestions(ctx context.Context, userId string, examName string) ([]goclient.PiOnboardServiceAPIModelsExamQuestionsAnswersDto, error)
}
