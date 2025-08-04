package service

import (
	"context"
	"fmt"

	goclient "github.com/pi-financial/notification-srv/go-client"
	client "github.com/pi-financial/onboard-srv-v2/internal/client/dto"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	"github.com/pi-financial/onboard-srv-v2/internal/handler/dto"
	"github.com/samber/lo"
)

type notificationService struct {
	NotificationClient port.NotificationClient
	Log                port.Logger
}

func NewNotificationService(
	NotificationClient port.NotificationClient,
	Log port.Logger) port.NotificationService {
	return &notificationService{
		NotificationClient: NotificationClient,
		Log:                Log,
	}
}

func (s *notificationService) SendEmail(ctx context.Context, emailData client.SendEmailRequestData) (_ *client.NotificationTicket, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in SendEmail: %w", err)
		}
	}()

	sendEmailRequest := goclient.EmailRequestDto{
		UserId:       emailData.UserId,
		CustomerCode: emailData.CustomerCode,
		Recipients:   emailData.Recipents,
		TemplateId:   emailData.TemplateId,
		Language:     lo.Ternary(emailData.Language == dto.LanguageEN, goclient.LANGUAGE_ENGLISH, goclient.LANGUAGE_THAI),
		TitlePayload: emailData.TitlePayload,
		BodyPayload: []string{
			emailData.BodyPayload.CustomerFirstName,
			emailData.BodyPayload.CustomerFullName,
			emailData.BodyPayload.CustomerMobileNo,
			emailData.BodyPayload.MT5Set,
			emailData.BodyPayload.MT5Tfex,
			emailData.BodyPayload.MT4,
			emailData.BodyPayload.MarketingId,
			emailData.BodyPayload.EmployeeFullName,
			emailData.BodyPayload.MarketingEmail,
		},
	}

	resp, err := s.NotificationClient.SendEmail(ctx, sendEmailRequest)

	if err != nil {
		return nil, fmt.Errorf("send email: %w", err)
	}

	if resp == nil {
		return nil, nil
	}

	return &client.NotificationTicket{
		TicketId: *resp.TicketId.Get(),
	}, nil
}
