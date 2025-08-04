package adapter

import (
	"context"
	"fmt"
	"net/http"

	domain "github.com/pi-financial/bond-srv/internal/domain/account"
	"github.com/samber/lo"

	config "github.com/pi-financial/bond-srv/config"
	adapterError "github.com/pi-financial/bond-srv/internal/adapter/error"
	"github.com/pi-financial/bond-srv/internal/adapter/interfaces"
	userClient "github.com/pi-financial/go-client/user/client"
)

type userAdapter struct {
	client *userClient.APIClient
}

var productMap = map[string]domain.Product{
	"cash":        domain.Cash,
	"cashBalance": domain.CashBalance,
	"bond":        domain.Bond,
}

func NewUserAdapter(cfg config.Config) interfaces.UserAdapter {
	userSrvConfig := userClient.NewConfiguration()
	userSrvConfig.Servers[0].URL = cfg.UserUrl
	client := userClient.NewAPIClient(userSrvConfig)
	return &userAdapter{client}
}

func (u *userAdapter) GetAccounts(ctx context.Context, userId string) ([]userClient.PiUserApplicationModelsUserTradingAccountInfoWithExternalAccounts, error) {
	req := u.client.UserMigrationAPI.InternalGetTradingAccountV2(ctx)
	req = req.UserId(userId)
	acc, accRes, err := u.client.UserMigrationAPI.InternalGetTradingAccountV2Execute(req)
	if err != nil || accRes.StatusCode != http.StatusOK {
		var c *int
		if accRes != nil {
			c = &accRes.StatusCode
		}
		statusCodeErr := fmt.Errorf("error: %v User Service Status Code: %d, userId: %q", err, c, userId)
		return nil, adapterError.NewExternalServiceError(statusCodeErr)
	}

	tradingAccounts := acc.Data
	return tradingAccounts, nil
}

func (u *userAdapter) GetTradingAccounts(ctx context.Context, userId string) ([]domain.TradingAccount, error) {
	accounts, err := u.GetAccounts(ctx, userId)
	if err != nil {
		return nil, adapterError.NewExternalServiceError(err)
	}

	var bondAccounts = make(map[string]domain.TradingAccount)
	for _, acc := range accounts {
		if !acc.HasTradingAccounts() {
			continue
		}

		for _, tradingAccount := range acc.TradingAccounts {
			product, ok := productMap[tradingAccount.GetProduct()]
			if !ok {
				continue
			}

			tradingAccountNo := tradingAccount.GetTradingAccountNo()
			if _, exist := bondAccounts[tradingAccountNo]; !exist || product == domain.Bond {
				bondAccounts[tradingAccountNo] = *domain.NewTradingAccount(acc.GetCustomerCode(), tradingAccount.GetTradingAccountNo(), product)
			}
		}
	}

	return lo.Values(bondAccounts), nil
}

func (u *userAdapter) GetTradingAccountsByCustCode(ctx context.Context, custCode string) ([]domain.TradingAccount, error) {
	req := u.client.UserTradingAccountAPI.GetUserTradingAccountInfoByUserId(ctx)
	req = req.CustomerCode(custCode)
	res, accRes, err := u.client.UserTradingAccountAPI.GetUserTradingAccountInfoByUserIdExecute(req)
	if err != nil || accRes.StatusCode != http.StatusOK {
		var c *int
		if accRes != nil {
			c = &accRes.StatusCode
		}
		validateError := fmt.Errorf("can't get trading account by cust code %v status %v with error \"%v\"", custCode, c, err)
		return nil, adapterError.NewExternalServiceError(validateError)
	}

	data := res.GetData()
	if !data.HasTradingAccounts() {
		return nil, fmt.Errorf("trading accounts of cust code %v can't be found", custCode)
	}

	var bondAccounts = make(map[string]domain.TradingAccount)
	for _, tradingAccount := range data.GetTradingAccounts() {
		pro := tradingAccount.GetProduct()
		product, ok := productMap[pro.GetName()]
		if !ok {
			continue
		}

		if _, ok := bondAccounts[tradingAccount.GetTradingAccountNo()]; !ok {
			bondAccounts[tradingAccount.GetTradingAccountNo()] = *domain.NewTradingAccount(data.GetCustomerCode(), tradingAccount.GetTradingAccountNo(), product)
		} else if product == domain.Bond {
			bondAccounts[tradingAccount.GetTradingAccountNo()] = *domain.NewTradingAccount(data.GetCustomerCode(), tradingAccount.GetTradingAccountNo(), product)
		}
	}

	return lo.Values(bondAccounts), nil
}
