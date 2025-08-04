package driver

import (
	"net/url"

	goclient "github.com/pi-financial/otp-srv/go-client"
	"github.com/pi-financial/pi-sso-v2/config"
)

func CreateOtpSrvClient(cfg config.Config) *goclient.APIClient {
	otpConf := goclient.NewConfiguration()
	parsedURL, _ := url.Parse(cfg.OtpSrvHost)
	otpConf.Host = parsedURL.String()
	otpConf.Scheme = "http"
	otpApiClient := goclient.NewAPIClient(otpConf)

	return otpApiClient
}
