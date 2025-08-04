package port

import (
	"context"

	client "github.com/pi-financial/onboard-srv-v2/internal/client/dto"
	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"
	"github.com/pi-financial/onboard-srv-v2/internal/handler/dto"
)

type MetaTraderService interface {
	CreateMetaTrader(context.Context, []domain.CreateMetaTraderRequest) error
	GetMetaTrader(context.Context, *domain.GetMetaTraderFilter) ([]domain.MT4, []domain.MT5, error)
	UpdateMetaTrader(context.Context, *domain.UpdateMetaTraderRequest) error
	SendMetaTraderCreatedNotificationEmail(context.Context, []domain.CreateMetaTraderRequest, dto.Locale) error
}

type UserSrvV2Service interface {
	GetTradingAccountWithMarketingInfoByCustomerCodes(ctx context.Context, customerCodes []string) ([]client.TradingAccountsMarketingInfo, error)
	GetUserInfoByCustomerCode(ctx context.Context, customerCode string) ([]client.UserInfo, error)
}

type EmployeeService interface {
	GetEmployeeInfoById(ctx context.Context, employeeId string) (*client.EmployeeInfo, error)
}

type NotificationService interface {
	SendEmail(ctx context.Context, emailData client.SendEmailRequestData) (*client.NotificationTicket, error)
}
