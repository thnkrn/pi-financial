package client

import (
	"context"
	"fmt"
	"net/url"

	"github.com/pi-financial/go-common/logger"
	goclient "github.com/pi-financial/it-data-api-client/go-client"
	"github.com/pi-financial/user-srv-v2/config"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
)

type ItDataClient struct {
	ItDataService *goclient.APIClient
	Log           logger.Logger
	ApiKey        string
}

func NewItDataClient(log logger.Logger, cfg config.Config) interfaces.ItDataClient {
	apiUrl, err := url.Parse(cfg.ItDataSrvHost)
	log.Info(fmt.Sprintf("ItDataSrvHost: %s", apiUrl.String()))
	if err != nil {
		panic(err)
	}

	apiKey := cfg.ItDataSrvApiKey
	if apiKey == "" {
		log.Error("no api key for IT data service in .env (name must be IT_DATA_SRV_API_KEY)")
		panic(constants.ErrItDataSrvInvalidApiKey)
	}

	return &ItDataClient{
		Log:    log,
		ApiKey: apiKey,
		ItDataService: goclient.NewAPIClient(&goclient.Configuration{
			Scheme: "http",
			Servers: []goclient.ServerConfiguration{
				{
					// WARN: If the url have a colon (e.g. <host>:<port>), MUST
					// include the http:// prefix (e.g. http://<host>:<port>).
					// Without the prefix, goclient will remove the <host> from the url.
					URL: apiUrl.String(),
				},
			},
		}),
	}
}

func (i ItDataClient) GetAtsBankAccounts(ctx context.Context, customerCode string) ([]goclient.AtsInfoDetail, error) {
	result, _, err := i.ItDataService.SSODBAPI.ApiSSODBChkAtsGet(ctx).
		Custcode(customerCode).
		ApiKey(i.ApiKey).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetAtsBankAccounts customer code %q: client %q: %w: %w",
			customerCode, "ItDataService.SSODBAPI.ApiSSODBChkAtsGet", constants.ErrItDataSrvGetAtsBankAccounts, err)
	}

	return result.Data.Data, nil
}

func (i ItDataClient) GetKyc(ctx context.Context, cardId, customerCode *string) ([]goclient.KycDetail, error) {
	request := i.ItDataService.SSODBAPI.ApiSSODBKycInfoGet(ctx).
		ApiKey(i.ApiKey)

	if cardId != nil {
		request = request.CardId(*cardId)
	}

	if customerCode != nil {
		request = request.Custcode(*customerCode)
	}

	result, _, err := request.Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetKyc card id %q, customer code %q: client %q: %w: %w",
			*cardId, *customerCode, "ItDataService.SSODBAPI.ApiSSODBKycInfoGet", constants.ErrItDataSrvGetKyc, err)
	}

	if result.Data == nil {
		return []goclient.KycDetail{}, nil
	}

	return result.Data.Data, nil
}

func (i ItDataClient) GetSuitTest(ctx context.Context, customerCode string) ([]goclient.SuitTestDetail, error) {
	result, _, err := i.ItDataService.SSODBAPI.ApiSSODBSuitTestGet(ctx).
		Custcode(customerCode).
		ApiKey(i.ApiKey).
		Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetSuitTest customer code %q: client %q: %w: %w",
			customerCode, "ItDataService.SSODBAPI.ApiSSODBSuitTestGet", constants.ErrItDataSrvGetSuitTest, err)
	}

	if result.Data == nil {
		return []goclient.SuitTestDetail{}, nil
	}

	return result.Data.Data, nil
}

func (i ItDataClient) GetAddress(ctx context.Context, cardId, customerCode *string) ([]goclient.DatumAddrInfoModel, error) {
	request := i.ItDataService.SSODBAPI.ApiSSODBAddrInfoGet(ctx).
		ApiKey(i.ApiKey)

	if cardId != nil {
		request = request.CardId(*cardId)
	}

	if customerCode != nil {
		request = request.Custcode(*customerCode)
	}

	result, _, err := request.Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetAddress card id %q, customer code %q: client %q: %w: %w",
			*cardId, *customerCode, "ItDataService.SSODBAPI.ApiSSODBAddrInfoGet", constants.ErrItDataSrvGetAddress, err)
	}

	if result.Data == nil {
		return []goclient.DatumAddrInfoModel{}, nil
	}

	return result.Data.Data, nil
}

func (i ItDataClient) GetAccount(ctx context.Context, cardId, customerCode *string) ([]goclient.DatumAccountInfoV2Model, error) {
	request := i.ItDataService.SSODBAPI.ApiSSODBAccountInfoV2Get(ctx).
		ApiKey(i.ApiKey)

	if cardId != nil {
		request = request.CardId(*cardId)
	}

	if customerCode != nil {
		request = request.Custcode(*customerCode)
	}

	result, _, err := request.Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetAccount card id %q, customer code %q: client %q: %w: %w",
			*cardId, *customerCode, "ItDataService.SSODBAPI.ApiSSODBAccountInfoV2Get", constants.ErrItDataSrvGetAccount, err)
	}

	if result.Data == nil {
		return []goclient.DatumAccountInfoV2Model{}, nil
	}

	return result.Data.Data, nil
}

func (i ItDataClient) GetCustomerInfo(ctx context.Context, cardId, customerCode *string) ([]goclient.DatumCustInfoV2, error) {
	request := i.ItDataService.SSODBAPI.ApiSSODBCustInfoV2Get(ctx).
		ApiKey(i.ApiKey)

	if cardId != nil {
		request = request.CardId(*cardId)
	}

	if customerCode != nil {
		request = request.Custcode(*customerCode)
	}

	result, _, err := request.Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetCustomerInfo card id %q, customer code %q: client %q: %w: %w",
			*cardId, *customerCode, "ItDataService.SSODBAPI.ApiSSODBCustInfoV2Get", constants.ErrItDataSrvGetAccount, err)
	}

	if result.Data == nil {
		return []goclient.DatumCustInfoV2{}, nil
	}

	return result.Data.Data, nil
}

func (i ItDataClient) GetCustomerInfoOthers(ctx context.Context, cardId, customerCode *string) ([]goclient.DatasCustInfoOthers, error) {
	request := i.ItDataService.SSODBAPI.ApiSSODBCustInFoOthersGet(ctx).
		ApiKey(i.ApiKey)

	if cardId != nil {
		request = request.CardId(*cardId)
	}

	if customerCode != nil {
		request = request.Custcode(*customerCode)
	}

	result, _, err := request.Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetCustomerInfoOthers card id %q, customer code %q: client %q: %w: %w",
			*cardId, *customerCode, "ItDataService.SSODBAPI.ApiSSODBCustInFoOthersGet", constants.ErrItDataSrvGetAccount, err)
	}

	if result.Data == nil {
		return []goclient.DatasCustInfoOthers{}, nil
	}

	return result.Data.Data, nil
}

func (i ItDataClient) GetFrontName(ctx context.Context, cardId, customerCode *string) ([]goclient.FrontNameDetail, error) {
	request := i.ItDataService.SSODBAPI.ApiSSODBFrontNameGet(ctx).
		ApiKey(i.ApiKey)

	if cardId != nil {
		request = request.CardId(*cardId)
	}

	if customerCode != nil {
		request = request.Custcode(*customerCode)
	}

	result, _, err := request.Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetFrontName card id %q, customer code %q: client %q: %w: %w",
			*cardId, *customerCode, "ItDataService.SSODBAPI.ApiSSODBFrontNameGet", constants.ErrItDataSrvGetAccount, err)
	}

	if result.Data == nil {
		return []goclient.FrontNameDetail{}, nil
	}

	return result.Data.Data, nil
}

func (i ItDataClient) GetSuitChoice(ctx context.Context, cardId, customerCode *string) ([]goclient.SuitChoiceDetail, error) {
	request := i.ItDataService.SSODBAPI.ApiSSODBSuitChoiceGet(ctx).
		ApiKey(i.ApiKey)

	if customerCode != nil {
		request = request.Custcode(*customerCode)
	}

	result, _, err := request.Execute()

	if err != nil {
		return nil, fmt.Errorf("in GetSuitTestQuestions card id %q, customer code %q: client %q: %w: %w",
			*cardId, *customerCode, "ItDataService.SSODBAPI.ApiSSODBSuitChoiceGet", constants.ErrItDataSrvGetAccount, err)
	}

	if result.Data == nil {
		return []goclient.SuitChoiceDetail{}, nil
	}

	return result.Data.Data, nil
}
