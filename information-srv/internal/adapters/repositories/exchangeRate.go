package repositories

import (
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"time"

	"github.com/pi-financial/information-srv/internal/core/domain/exchangeRate"
	"github.com/pi-financial/information-srv/internal/driver/log"
	"github.com/spf13/viper"
)

type ExchangeRateRepository struct {
	client   http.Client
	clientId string
	host     string
	logger   log.Logger
}

type ReferenceRateResponse struct {
	Result ReferenceRateResult `json:"result"`
}

type ReferenceRateResult struct {
	Data ReferenceRateResultDetail `json:"data"`
}

type ReferenceRateResultDetail struct {
	Detail []exchangeRate.ReferenceRate `json:"data_detail"`
}

type ExchangeRateResponse struct {
	Result ExchangeRateResult `json:"result"`
}

type ExchangeRateResult struct {
	Data ExchangeRateResultDetail `json:"data"`
}

type ExchangeRateResultDetail struct {
	Detail []exchangeRate.ExchangeRate `json:"data_detail"`
}

func NewExchangeRateRepository(logger log.Logger) *ExchangeRateRepository {
	host := viper.GetString("BOT_HOST")
	clientId := viper.GetString("BOT_CLIENT_ID")

	return &ExchangeRateRepository{
		client:   http.Client{},
		clientId: clientId,
		host:     host,
		logger:   logger,
	}
}

func (repo *ExchangeRateRepository) query(url string) ([]byte, error) {
	if repo.host == "" {
		return nil, fmt.Errorf("host haven't been configured")
	}
	if repo.clientId == "" {
		return nil, fmt.Errorf("clientId haven't been configured")
	}

	req, err := http.NewRequest("GET", url, nil)
	if err != nil {
		return nil, fmt.Errorf("error creating request: %v", err)
	}

	req.Header.Add("X-IBM-Client-Id", repo.clientId)
	resp, err := repo.client.Do(req)
	if err != nil {
		return nil, fmt.Errorf("error sending request: %v", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode == http.StatusUnauthorized {
		var errorResp ErrorResponse
		body, err := io.ReadAll(resp.Body)
		if err != nil {
			return nil, fmt.Errorf("error reading body: %v", err)
		}

		err = json.Unmarshal(body, &errorResp)
		if err != nil {
			return nil, fmt.Errorf("error unmarshalling body: %v", err)
		}
		return nil, fmt.Errorf("%s - %s", errorResp.HttpCode, errorResp.MoreInformation)
	} else if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("error response: %s", resp.Status)
	}

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("error reading body: %v", err)
	}

	return body, nil
}

func (repo *ExchangeRateRepository) GetReferenceRate(ctx context.Context, from time.Time, to time.Time) ([]exchangeRate.ReferenceRate, error) {
	fromStr := from.Format(format)
	toStr := to.Format(format)
	url := fmt.Sprintf("%s/Stat-ReferenceRate/v2/DAILY_REF_RATE/?start_period=%s&end_period=%s", repo.host, fromStr, toStr)
	body, err := repo.query(url)
	if err != nil {
		repo.logger.Error(fmt.Sprintf("error query: %v", err))
		return []exchangeRate.ReferenceRate{}, nil
	}

	var apiResponse ReferenceRateResponse
	err = json.Unmarshal(body, &apiResponse)
	if err != nil {
		repo.logger.Error(fmt.Sprintf("error unmarshalling body: %v", err))
		return []exchangeRate.ReferenceRate{}, nil
	}

	if len(apiResponse.Result.Data.Detail) == 1 && apiResponse.Result.Data.Detail[0].Rate == "" {
		return []exchangeRate.ReferenceRate{}, nil
	}

	return apiResponse.Result.Data.Detail, nil
}

func (repo *ExchangeRateRepository) GetExchangeRate(ctx context.Context, currency string, from time.Time, to time.Time) ([]exchangeRate.ExchangeRate, error) {
	fromStr := from.Format(format)
	toStr := to.Format(format)
	url := fmt.Sprintf("%s/Stat-ExchangeRate/v2/DAILY_AVG_EXG_RATE/?currency=%s&start_period=%s&end_period=%s", repo.host, currency, fromStr, toStr)
	body, err := repo.query(url)
	if err != nil {
		repo.logger.Error(fmt.Sprintf("error query: %v", err))
		return nil, err
	}

	var apiResponse ExchangeRateResponse
	err = json.Unmarshal(body, &apiResponse)
	if err != nil {
		repo.logger.Error(fmt.Sprintf("error unmarshalling body: %v", err))
		return nil, err
	}

	if len(apiResponse.Result.Data.Detail) == 1 && apiResponse.Result.Data.Detail[0].Date == "" {
		return []exchangeRate.ExchangeRate{}, nil
	}

	return apiResponse.Result.Data.Detail, nil
}
