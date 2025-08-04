package client

import (
	"context"
	"fmt"
	"net/url"

	"github.com/pi-financial/go-common/logger"
	goclient "github.com/pi-financial/information-srv/client"
	"github.com/pi-financial/user-srv-v2/config"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
)

type InformationClient struct {
	InformationSrv *goclient.APIClient
	Log            logger.Logger
}

func NewInformationClient(log logger.Logger, cfg config.Config) interfaces.InformationClient {
	url, err := url.Parse(cfg.InformationSrvHost)
	if err != nil {
		panic(err)
	}

	return &InformationClient{
		Log: log,
		InformationSrv: goclient.NewAPIClient(&goclient.Configuration{
			Scheme: "http",
			Servers: []goclient.ServerConfiguration{
				{
					URL: url.String(),
				},
			},
		}),
	}
}

func (o *InformationClient) GetProductByAccountTypeCode(ctx context.Context, accountTypeCode string) ([]goclient.ProductProduct, error) {
	result, _, err := o.InformationSrv.ProductAPI.InternalProductGet(ctx).
		AccountTypeCode(accountTypeCode).
		Execute()

	if err != nil {
		o.Log.Error(fmt.Sprintf("Error getting product for accountTypeCode %s with error: %+v", accountTypeCode, err))
		return nil, err
	}

	return result.Data, nil
}

func (o *InformationClient) GetProductByProductName(ctx context.Context, productName string) ([]goclient.ProductProduct, error) {
	result, _, err := o.InformationSrv.ProductAPI.InternalProductGet(ctx).
		Name(productName).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetProductByProductName product name %q: %w: %w",
			productName, constants.ErrInformationSrvGetProduct, err)
	}

	return result.Data, nil
}

func (o *InformationClient) GetBankByBankCode(ctx context.Context, bankCode string) ([]goclient.BankBank, error) {
	result, _, err := o.InformationSrv.BankAPI.InternalBankGet(ctx).
		Code(bankCode).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetBankByBankCode bank code %q: client %q: %w: %w",
			bankCode, "InformationSrv.BankAPI.InternalBankGet", constants.ErrInformationSrvGetBank, err)
	}

	return result.Data, nil
}

func (o *InformationClient) GetBankBranchByBankCodeAndBranchCode(ctx context.Context, bankCode string, branchCode string) ([]goclient.BankBranchBankBranch, error) {
	result, _, err := o.InformationSrv.BankBranchAPI.InternalBankBranchGet(ctx).
		BankCode(bankCode).
		BankBranchCode(branchCode).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetBankBranchByBankCodeAndBranchCode bank code %q and branch code %q: %w", bankCode, branchCode, err)
	}

	return result.Data, nil
}
