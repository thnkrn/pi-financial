package adapter

import (
	"context"
	"fmt"
	"net/http"
	"strings"

	config "github.com/pi-financial/bond-srv/config"
	adapterError "github.com/pi-financial/bond-srv/internal/adapter/error"
	interfaces "github.com/pi-financial/bond-srv/internal/adapter/interfaces"
	domain "github.com/pi-financial/bond-srv/internal/domain/account"
	userClientV2 "github.com/pi-financial/user-srv-v2/client"
	"github.com/samber/lo"
)

type userV2Adapter struct {
	userClientV2 *userClientV2.APIClient
}

// NOTE: always use to lower case for product name
// NOTE: in userV2 Product names are changed to use pascal case (in v1 they use camel case)
// NOTE: so let use always convert to lower case in this file and ignore camel case and pascal case
var productMapV2 = map[string]domain.Product{
	"cash":        domain.Cash,
	"cashbalance": domain.CashBalance,
	"bond":        domain.Bond,
}

func NewUserV2Adapter(cfg config.Config) interfaces.UserV2Adapter {
	userSrvV2Config := userClientV2.NewConfiguration()
	userSrvV2Config.Servers[0].URL = cfg.UserV2Url
	userClientV2 := userClientV2.NewAPIClient(userSrvV2Config)

	return &userV2Adapter{userClientV2}
}

func (u *userV2Adapter) GetTradingAccounts(ctx context.Context, custCode *string, userId string) ([]domain.TradingAccount, error) {
	req := u.userClientV2.TradingAccountAPI.InternalV1TradingAccountsGet(ctx)
	req = req.UserId(userId)
	acc, accRes, err := u.userClientV2.TradingAccountAPI.InternalV1TradingAccountsGetExecute(req)

	if err != nil || accRes.StatusCode != http.StatusOK {
		var statusCode *int
		if accRes != nil {
			statusCode = &accRes.StatusCode
		}
		statusCodeErr := fmt.Errorf("error: %v User Service Status Code: %d, userId: %q", err, statusCode, userId)
		return nil, adapterError.NewExternalServiceError(statusCodeErr)
	}

	tradingAccounts := acc.Data
	bondAccounts := make(map[string]domain.TradingAccount)
	for _, acc := range tradingAccounts {
		if custCode != nil && *acc.CustomerCode != *custCode {
			continue
		}

		if !acc.HasTradingAccounts() {
			continue
		}

		for _, tradingAccount := range acc.TradingAccounts {
			product, ok := productMapV2[strings.ToLower(tradingAccount.GetProductName())]
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
