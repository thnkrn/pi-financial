package interfaces

import (
	"context"

	goclient "github.com/pi-financial/it-data-api-client/go-client"
)

type ItDataClient interface {
	GetAtsBankAccounts(ctx context.Context, customerCode string) ([]goclient.AtsInfoDetail, error)
	GetKyc(ctx context.Context, cardId, customerCode *string) ([]goclient.KycDetail, error)
	GetSuitTest(ctx context.Context, customerCode string) ([]goclient.SuitTestDetail, error)
	GetAddress(ctx context.Context, cardId, customerCode *string) ([]goclient.DatumAddrInfoModel, error)
	GetAccount(ctx context.Context, cardId, customerCode *string) ([]goclient.DatumAccountInfoV2Model, error)
	GetCustomerInfo(ctx context.Context, cardId, customerCode *string) ([]goclient.DatumCustInfoV2, error)
	GetCustomerInfoOthers(ctx context.Context, cardId, customerCode *string) ([]goclient.DatasCustInfoOthers, error)
	GetFrontName(ctx context.Context, cardId, customerCode *string) ([]goclient.FrontNameDetail, error)
	GetSuitChoice(ctx context.Context, cardId, customerCode *string) ([]goclient.SuitChoiceDetail, error)
}
