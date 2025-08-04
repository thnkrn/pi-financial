package driver

import (
	"net/url"

	client "github.com/pi-financial/onboard-srv/go-client"
	"github.com/pi-financial/pi-sso-v2/config"
)

func CreateOnboardSrvClient(cfg config.Config) *client.APIClient {
	onboardConf := client.NewConfiguration()
	parsedURL, _ := url.Parse(cfg.OnboardSrvHost)
	onboardConf.Host = parsedURL.String()
	onboardConf.Scheme = "http"
	onboardApiClient := client.NewAPIClient(onboardConf)

	return onboardApiClient
}
