package client

import (
	"context"
	"errors"
	"fmt"
	"net/url"

	goclient "github.com/pi-financial/onboard-srv/go-client"
	"github.com/pi-financial/user-srv-v2/config"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
)

type OnboardClient struct {
	OnboardSrv *goclient.APIClient
}

func NewOnboardClient(cfg config.Config) interfaces.OnboardClient {
	url, err := url.Parse(cfg.OnboardSrvHost)
	if err != nil {
		panic(err)
	}

	return &OnboardClient{
		OnboardSrv: goclient.NewAPIClient(&goclient.Configuration{
			Scheme: "http",
			Servers: []goclient.ServerConfiguration{
				{
					URL: url.String(),
				},
			},
		}),
	}
}

func (o *OnboardClient) GetTradingAccountByCustomerCode(ctx context.Context, customerCode string, withBankAccounts bool, withExternalAccounts bool) ([]goclient.PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccount, error) {
	result, httpResp, err := o.OnboardSrv.TradingAccountAPI.InternalGetTradingAccountListByCustomerCodeV2(ctx).
		CustomerCode(customerCode).
		WithBankAccounts(withBankAccounts).
		WithExternalAccounts(withExternalAccounts).
		Execute()
	if httpResp.StatusCode == 404 {
		return nil, errors.New("trading account not found")
	}
	if err != nil {
		return nil, err
	}

	return result.Data, nil
}

func (o *OnboardClient) GetBanksByUserId(ctx context.Context, userId string) ([]goclient.PiOnboardServiceAPIModelsBankInfoDto, error) {
	result, httpResp, err := o.OnboardSrv.BankAccountAPI.
		GetBankList(ctx).
		UserId(userId).
		Execute()
	if err != nil {
		return nil, err
	}
	if httpResp.StatusCode == 404 {
		return nil, errors.New("bank account not found")
	}

	return result.Data, nil
}

func (o *OnboardClient) GetBankAccountForDepositWithdrawalByProductName(ctx context.Context, customerCode string, purpose string, productName string) (*goclient.PiOnboardServiceApplicationModelsBankAccountDetail, error) {
	result, _, err := o.OnboardSrv.BankAccountAPI.GetBankAccountForDepositWithdrawalByProductName(ctx).
		CustomerCode(customerCode).
		Purpose(purpose).
		Product(productName).
		Execute()
	if err != nil {
		return nil, err
	}

	return result.Data, nil
}

func (o *OnboardClient) GetExamQuestions(ctx context.Context, userId string, examName string) ([]goclient.PiOnboardServiceAPIModelsExamQuestionsAnswersDto, error) {
	result, _, err := o.OnboardSrv.ExamAPI.GetExamQuestions(ctx).
		UserId(userId).
		ExamName(examName).
		Execute()
	if err != nil {
		return nil, fmt.Errorf("in GetExamQuestions user id %q, exam name %q: %w: %w",
			userId, examName, constants.ErrOnboardSrvGetExamQuestions, err)
	}

	return result.Data, nil
}
