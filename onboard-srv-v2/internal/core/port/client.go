package port

import (
	"context"

	employeesrvgoclient "github.com/pi-financial/employee-srv/go-client"
	notificationsrvgoclient "github.com/pi-financial/notification-srv/go-client"
	usersrvv2goclient "github.com/pi-financial/user-srv-v2/client"
)

type UserSrvV2Client interface {
	GetTradingAccountWithMarketingInfoByCustomerCodes(ctx context.Context, customerCodes []string) ([]usersrvv2goclient.DtoTradingAccountsMarketingInfo, error)
	GetUserInfoByCustomerCode(ctx context.Context, customerCode string) ([]usersrvv2goclient.DtoUserInfo, error)
}

type EmployeeClient interface {
	GetEmployeeInfoById(ctx context.Context, employeeId string) (*employeesrvgoclient.PiEmployeeServiceDomainAggregatesModelEmployeeInfoAggregateEmployeeInfo, error)
}

type NotificationClient interface {
	SendEmail(ctx context.Context, request notificationsrvgoclient.EmailRequestDto) (*notificationsrvgoclient.NotificationTicket, error)
}
