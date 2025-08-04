package adapter

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"net/url"
	"path"

	"github.com/pi-financial/bond-srv/config"
	"github.com/pi-financial/bond-srv/internal/adapter/interfaces"
	"github.com/pi-financial/bond-srv/internal/domain/instrument"
	"github.com/pi-financial/bond-srv/internal/driver/log"
	"github.com/shopspring/decimal"
)

type PortfolioV2Response struct {
	Code string     `json:"code"`
	Data []BondData `json:"data"`
	Msg  string     `json:"msg"`
}

type BondData struct {
	AssetName    string `json:"assetName"`
	CouponRate   string `json:"couponRate"`
	CreatedAt    string `json:"createdAt"`
	CustomerCode string `json:"customerCode"`
	DateKey      string `json:"dateKey"`
	InitialDate  string `json:"initialDate"`
	Issuer       string `json:"issuer"`
	MarketType   string `json:"marketType"`
	MarketValue  string `json:"marketValue"`
	MaturityDate string `json:"maturityDate"`
	TotalCost    string `json:"totalCost"`
}

type portfolioV2Api struct {
	baseUrl *url.URL
	logger  log.Logger
}

func NewPortfolioV2Api(cfg config.Config, logger log.Logger) interfaces.PortfolioV2Adapter {
	portfolioV2BaseUrl := cfg.PortfolioV2Url
	portfolioV2Url, err := url.Parse(portfolioV2BaseUrl)
	if err != nil {
		errMessage := fmt.Sprintf("Cannot set PortfolioV2 Base Url: %v", portfolioV2Url)
		logger.Error(context.Background(), errMessage, err)
	}

	return &portfolioV2Api{portfolioV2Url, logger}
}

func (p *portfolioV2Api) GetPositions(ctx context.Context, custCode string) (result map[instrument.Symbol]decimal.Decimal, err error) {
	baseUrl := *p.baseUrl
	baseUrl.Path = path.Join(baseUrl.Path, "internal/v1/bond-summary", custCode)

	resp, err := http.Get(baseUrl.String())
	if err != nil {
		p.logger.Error(ctx, "Cannot get result from portfolio V2", err)
		return nil, err
	}

	if resp.StatusCode != http.StatusOK {
		ErrorMes := fmt.Sprintf("Cannot get PortfolioV2Response, StatusCode: %v", resp.Status)
		p.logger.Info(ctx, ErrorMes)
		return result, err
	}
	defer resp.Body.Close()

	var portfolioV2Response PortfolioV2Response
	if err := json.NewDecoder(resp.Body).Decode(&portfolioV2Response); err != nil {
		return nil, err
	}

	results := make(map[instrument.Symbol]decimal.Decimal)
	for _, position := range portfolioV2Response.Data {
		marketVal, err := decimal.NewFromString(position.MarketValue)
		if err != nil {
			errMes := fmt.Sprintf("Cannot convert market value from portfolioV2 to decimal. Value: %v", position.MarketValue)
			p.logger.Error(ctx, errMes, err)
			return nil, err
		}
		if value, exists := results[instrument.Symbol(position.AssetName)]; exists {
			results[instrument.Symbol(position.AssetName)] = value.Add(marketVal)
		} else {
			results[instrument.Symbol(position.AssetName)] = marketVal
		}
	}

	return results, nil
}
