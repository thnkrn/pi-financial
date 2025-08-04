package repositories

import (
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"

	"github.com/pi-financial/information-srv/internal/core/domain/calendar"
	"github.com/pi-financial/information-srv/internal/driver/log"
	"github.com/spf13/viper"
)

type HolidayRepository struct {
	client   http.Client
	clientId string
	host     string
	logger   log.Logger
}

type HolidayResult struct {
	Data []calendar.Holiday `json:"data"`
}

type HolidayResponse struct {
	Result HolidayResult `json:"result"`
}

type ErrorResponse struct {
	HttpCode        string `json:"httpCode"`
	HttpMessage     string `json:"httpMessage"`
	MoreInformation string `json:"moreInformation"`
}

var format = "2006-01-02"

func NewHolidayepository(logger log.Logger) *HolidayRepository {
	host := viper.GetString("BOT_HOST")
	clientId := viper.GetString("BOT_CLIENT_ID")

	return &HolidayRepository{
		client:   http.Client{},
		clientId: clientId,
		host:     host,
		logger:   logger,
	}
}

func (repo *HolidayRepository) query(url string) ([]byte, error) {
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

func (repo *HolidayRepository) GetHolidays(ctx context.Context, year int) ([]calendar.Holiday, error) {
	url := fmt.Sprintf("%s/financial-institutions-holidays?year=%d", repo.host, year)
	body, err := repo.query(url)
	if err != nil {
		repo.logger.Error(fmt.Sprintf("error query: %v", err))
		return []calendar.Holiday{}, nil
	}

	var apiResponse HolidayResponse
	err = json.Unmarshal(body, &apiResponse)
	if err != nil {
		repo.logger.Error(fmt.Sprintf("error unmarshalling body: %v", err))
		return []calendar.Holiday{}, nil
	}

	return apiResponse.Result.Data, nil
}
