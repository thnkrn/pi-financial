package driver

import (
	"net/url"

	"github.com/pi-financial/pi-sso-v2/config"
	client "github.com/pi-financial/user-srv-v2/client"
)

func CreateUserSrvV2Client(cfg config.Config) *client.APIClient {
	userConf := client.NewConfiguration()
	parsedURL, _ := url.Parse(cfg.UserSrvV2Host)
	userConf.Host = parsedURL.String()
	userConf.Scheme = "http"
	userApiClient := client.NewAPIClient(userConf)

	return userApiClient
}
