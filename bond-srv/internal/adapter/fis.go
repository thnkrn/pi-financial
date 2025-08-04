package adapter

import (
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"net/http"
	"net/url"
	"sync"
	"time"

	"github.com/pi-financial/bond-srv/config"
	"github.com/pi-financial/bond-srv/internal/adapter/interfaces"
	"github.com/pi-financial/bond-srv/internal/constants"
	"github.com/pi-financial/bond-srv/internal/domain/instrument"
	"github.com/pi-financial/bond-srv/internal/driver/cache"
	"github.com/pi-financial/bond-srv/internal/driver/log"
	"github.com/pi-financial/bond-srv/internal/dto"
	"github.com/pi-financial/bond-srv/utils"

	adapterError "github.com/pi-financial/bond-srv/internal/adapter/error"
)

type fisApi struct {
	client  *http.Client
	baseUrl *url.URL
	logger  log.Logger
	cache   cache.Cache
}

type priceResponse struct {
	Data   map[string]detail `json:"Data"`
	Api    string            `json:"API"`
	Asof   string            `json:"asof"`
	Status int64             `json:"status"`
}

type detail struct {
	Coupon                *string `json:"Coupon"`
	Maturity              *string `json:"Maturity"`
	TRIS                  *string `json:"TRIS"`
	FITCH                 *string `json:"FITCH"`
	StaticSpread          *string `json:"Static Spread(bp)"`
	MarketYieldPercentage *string `json:"Market Yield(%)"`
	CleanPricePercentage  *string `json:"Clean Price %"`
	AiPercentage          *string `json:"AI %"`
	ModifiedDuration      *string `json:"Modified Duration*"`
}

const (
	asOfDateFormat     = "2006-01-02"
	maturityDateFormat = "02-Jan-2006"
)

func NewFisApi(cfg config.Config, client *http.Client, logger log.Logger, cache cache.Cache) interfaces.MarketDataAdapter {
	fisBaseUrl := cfg.FisUrl
	fisUrl, err := url.Parse(fisBaseUrl)
	if err != nil {
		errMessage := fmt.Sprintf("Cannot set Fis Base Url: %v", fisBaseUrl)
		logger.Error(context.Background(), errMessage, err)
	}

	return &fisApi{client, fisUrl, logger, cache}
}

func (f *fisApi) GetSymbols(ctx context.Context, from time.Time, days int) (map[instrument.Symbol]instrument.Symbol, error) {
	ch, err := f.getHistoriesCh(ctx, from, days)
	if err != nil {
		return nil, adapterError.NewExternalServiceError(err)
	}

	symbols := make(map[instrument.Symbol]instrument.Symbol)
	for res := range ch {
		if res.Err != nil {
			continue
		}

		for symbol := range res.Data.Bonds {
			if symbol == "" {
				continue
			}

			if _, ok := symbols[symbol]; !ok {
				symbols[symbol] = symbol
			}
		}
	}

	return symbols, nil
}

func (f *fisApi) GetBondsHistories(ctx context.Context, from time.Time, days int) (histories []instrument.BondHistory, err error) {
	ch, err := f.getHistoriesCh(ctx, from, days)
	if err != nil {
		return nil, adapterError.NewExternalServiceError(err)
	}

	histories = make([]instrument.BondHistory, 0)
	for res := range ch {
		if res.Err != nil {
			continue
		}

		histories = append(histories, res.Data)
	}

	return histories, nil
}

func (f *fisApi) getHistoriesCh(ctx context.Context, from time.Time, days int) (<-chan dto.ChannelResult[instrument.BondHistory], error) {
	if days <= 0 {
		err := errors.New("getHistoriesCh days must be greater than zero")
		return nil, adapterError.NewValidationError(err)
	}

	var wg sync.WaitGroup
	ch := make(chan dto.ChannelResult[instrument.BondHistory], days)

	date := from
	for i := 1; i <= days; i++ {
		date = utils.AddBusinessDateTime(date, 0, 0, -1)
		wg.Add(1)
		currentDate := date
		isLatest := i == 1
		go func(dateTime time.Time, isLastest bool) {
			defer wg.Done()
			his, err := f.getBondHistory(ctx, dateTime, isLastest)

			ch <- dto.ChannelResult[instrument.BondHistory]{
				Data: func() instrument.BondHistory {
					if his == nil {
						return instrument.BondHistory{}
					}
					return *his
				}(),
				Err: err,
			}
		}(currentDate, isLatest)
	}

	go func() {
		wg.Wait()
		close(ch)
	}()

	return ch, nil
}

func (f *fisApi) getBondHistory(ctx context.Context, dateTime time.Time, isLatest bool) (history *instrument.BondHistory, err error) {
	baseUrl := *f.baseUrl
	queryParam := baseUrl.Query()

	queryParam.Set("api", "price")

	loc, _ := time.LoadLocation("Asia/Bangkok")
	utcPlus7time := dateTime.In(loc).Format(asOfDateFormat)

	bondHistory, found := f.getBondHistoryCache(ctx, utcPlus7time)
	if found {
		return &bondHistory, nil
	}

	queryParam.Set("asof", utcPlus7time)
	baseUrl.RawQuery = queryParam.Encode()
	url := baseUrl.String()
	req, err := http.NewRequestWithContext(ctx, "GET", url, nil)
	if err != nil {
		return history, adapterError.NewExternalServiceError(err)
	}
	response, err := f.client.Do(req)
	if err != nil {
		ErrorMes := fmt.Sprintf("Cannot get FIS priceResponse, Error: %v", err.Error())
		f.logger.Info(ctx, ErrorMes)
		return history, adapterError.NewExternalServiceError(err)
	}

	if response.StatusCode != http.StatusOK {
		ErrorMes := fmt.Sprintf("Cannot get FIS priceResponse, StatusCode: %v", response.Status)
		f.logger.Info(ctx, ErrorMes)
		return history, err
	}
	defer response.Body.Close()

	var priceResp priceResponse
	if err := json.NewDecoder(response.Body).Decode(&priceResp); err != nil {
		return nil, err
	}

	asOf, err := time.Parse(asOfDateFormat, priceResp.Asof)
	if err != nil {
		timeError := errors.New("can't get \"AsOf\" at GetBondHistory")
		return history, adapterError.NewExternalServiceError(timeError)
	}

	bonds := map[instrument.Symbol]instrument.BondInstrument{}
	for sym, d := range priceResp.Data {
		m := utils.FormatStringToTime(d.Maturity, maturityDateFormat)
		var mat time.Time
		if m != nil {
			mat = *m
		}

		bonds[instrument.Symbol(sym)] = instrument.BondInstrument{
			Maturity:              mat,
			AsOf:                  asOf,
			Coupon:                utils.FormatStringToDecimal(d.Coupon),
			StaticSpread:          utils.FormatStringToDecimal(d.StaticSpread),
			MarketYieldPercentage: utils.FormatStringToDecimal(d.MarketYieldPercentage),
			CleanPricePercentage:  utils.FormatStringToDecimal(d.CleanPricePercentage),
			AiPercentage:          utils.FormatStringToDecimal(d.AiPercentage),
			ModifiedDuration:      utils.FormatStringToDecimal(d.ModifiedDuration),
			Symbol:                instrument.Symbol(sym),
			TRIS:                  utils.GetValueOrDefault(d.TRIS),
			FITCH:                 utils.GetValueOrDefault(d.FITCH),
		}
	}

	history = &instrument.BondHistory{
		AsOf:  asOf,
		Bonds: bonds,
	}

	if isLatest {
		f.addBondHistoryCache(utcPlus7time, *history, constants.LatestBondCacheDuration)
	} else {
		f.addBondHistoryCache(utcPlus7time, *history, constants.BondCacheDuration)
	}

	return history, nil
}

func (f *fisApi) getBondHistoryCache(ctx context.Context, queryDate string) (instrument.BondHistory, bool) {
	key := f.bondHistoryCacheKey(queryDate)
	result, found := f.cache.Get(key)
	if !found {
		return instrument.BondHistory{}, false
	}

	history, ok := result.(instrument.BondHistory)
	if !ok {
		logMsg := fmt.Sprintf("Cannot convert to Bond Histories cache key: %v, value: %v", key, result)
		f.logger.Info(ctx, logMsg)
		return instrument.BondHistory{}, false
	}

	return history, true
}

func (f *fisApi) addBondHistoryCache(queryDate string, history instrument.BondHistory, duration time.Duration) {
	key := f.bondHistoryCacheKey(queryDate)
	f.cache.Set(key, history, duration)
}

func (f *fisApi) bondHistoryCacheKey(queryDate string) string {
	return fmt.Sprintf("bond::bond_history_%v", queryDate)
}
