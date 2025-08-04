package client

import (
	"context"
	"fmt"
	"net/url"

	goclient "github.com/pi-financial/notification-srv/go-client"
	"github.com/pi-financial/onboard-srv-v2/config"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
)

type notificationClient struct {
	Log             port.Logger
	Config          config.Config
	NotificationSrv *goclient.APIClient
}

func NewNotificationClient(log port.Logger, config config.Config) port.NotificationClient {
	host, err := url.Parse(config.Client.NotificationSrvHost)
	log.Info(fmt.Sprintf("Notification service host: %s", host.String()))
	if err != nil {
		panic(err)
	}

	return &notificationClient{
		Log:    log,
		Config: config,
		NotificationSrv: goclient.NewAPIClient(&goclient.Configuration{
			Scheme: "http",
			Servers: []goclient.ServerConfiguration{
				{
					URL: host.String(),
				},
			},
		}),
	}
}

func (c *notificationClient) SendEmail(ctx context.Context, request goclient.EmailRequestDto) (_ *goclient.NotificationTicket, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in SendEmail to notification-srv: %w", err)
		}
	}()

	resp, _, err := c.NotificationSrv.EmailAPI.
		InternalEmailPost(ctx).
		EmailRequestDto(request).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("send email: %w", err)
	}

	return resp.Data, nil
}
