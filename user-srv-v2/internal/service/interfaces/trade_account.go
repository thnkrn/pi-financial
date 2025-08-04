package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type TradeAccountService interface {
	GetDepositWithdrawableTradingAccounts(ctx context.Context, userId string) ([]dto.DepositWithdrawTradingAccountResponse, error)
	GetTradingAccountByUserId(ctx context.Context, userId string, status string) ([]dto.TradeAccountResponse, error)
	CreateTradingAccount(ctx context.Context, customerCode string, req []dto.CreateTradingAccountRequest) error
	GetTradingAccountWithMarketingInfoByCustomerCodes(ctx context.Context, customerCodes []string) ([]dto.TradingAccountsMarketingInfo, error)
}
