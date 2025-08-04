package client

import (
	"context"
	"fmt"
	"net/url"
	"strings"

	"github.com/pi-financial/onboard-srv-v2/config"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	goclient "github.com/pi-financial/user-srv-v2/client"
)

type userSrvV2Client struct {
	Log       port.Logger
	Config    config.Config
	UserSrvV2 *goclient.APIClient
}

func NewUserSrvV2Client(log port.Logger, config config.Config) port.UserSrvV2Client {
	host, err := url.Parse(config.Client.UserSrvV2Host)
	log.Info(fmt.Sprintf("User service v2 host: %s", host.String()))
	if err != nil {
		panic(err)
	}

	return &userSrvV2Client{
		Log:    log,
		Config: config,
		UserSrvV2: goclient.NewAPIClient(&goclient.Configuration{
			Scheme: "http",
			Servers: []goclient.ServerConfiguration{
				{
					URL: host.String(),
				},
			},
		}),
	}
}

func (c *userSrvV2Client) GetTradingAccountWithMarketingInfoByCustomerCodes(ctx context.Context, customerCodes []string) (_ []goclient.DtoTradingAccountsMarketingInfo, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetTradingAccountWithMarketingInfoByCustomerCodes from user-srv-v2: %w", err)
		}
	}()

	resp, _, err := c.UserSrvV2.TradingAccountAPI.
		InternalV1TradingAccountsMarketingInfosGet(ctx).
		CustomerCodes(strings.Join(customerCodes, ",")).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("get trading accounts with marketing id for customer codes %q: %w", customerCodes, err)
	}

	return resp.Data, nil
}

func (c *userSrvV2Client) GetUserInfoByCustomerCode(ctx context.Context, customerCode string) (_ []goclient.DtoUserInfo, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetUserInfoByCustomerCode from user-srv-v2: %w", err)
		}
	}()

	resp, _, err := c.UserSrvV2.UserAPI.
		InternalV1UsersGet(ctx).
		AccountId(customerCode).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("et user info for customer codes %q: %w", customerCode, err)
	}

	return resp.Data, nil
}
