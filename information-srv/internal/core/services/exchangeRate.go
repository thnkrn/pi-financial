package services

import (
	"context"
	"encoding/json"
	"fmt"
	"sort"
	"time"

	"github.com/pi-financial/information-srv/internal/core/domain/exchangeRate"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/driver/log"
)

type ExchangeRateService struct {
	cache  ports.CacheRepository
	logger log.Logger
	repo   ports.ExchangeRateRepository
}

func NewExchangeRateService(repo ports.ExchangeRateRepository, cache ports.CacheRepository, logger log.Logger) *ExchangeRateService {
	return &ExchangeRateService{
		cache,
		logger,
		repo,
	}
}

const cacheTTL = 4 * time.Hour
const dateFormat = "2006-01-02"

func (srv *ExchangeRateService) GetExchangeRate(
	ctx context.Context,
	currency string,
	startDate time.Time,
	endDate time.Time,
) ([]exchangeRate.ExchangeRate, error) {
	var allRates []exchangeRate.ExchangeRate

	// Iterate over each month in the date range
	begin, _ := getMonthRange(startDate)
	_, end := getMonthRange(endDate)
	for current := begin; current.Before(end) || current.Equal(end); current = current.AddDate(0, 1, 0) {
		startOfMonth, endOfMonth := getMonthRange(current)

		cacheKey := generateCacheKey(currency, startOfMonth)
		monthlyRates, err := srv.getDataFromCache(cacheKey)
		if err != nil {
			srv.logger.Error(fmt.Sprintf("Failed to get data from cache for key: %s, error: %v", cacheKey, err))
		} else if monthlyRates != nil {
			allRates = append(allRates, monthlyRates...)
			continue
		}

		rates, err := srv.repo.GetExchangeRate(ctx, currency, startOfMonth, endOfMonth)
		if err != nil {
			return nil, err
		}
		if len(rates) > 0 {
			if err := srv.storeDataInCache(cacheKey, rates); err != nil {
				return nil, err
			}
		}

		allRates = append(allRates, rates...)
	}

	// Filter the results to include only the dates within the requested range
	filteredRates := filterAndSortResults(allRates, startDate, endDate)

	return filteredRates, nil
}

func getMonthRange(current time.Time) (time.Time, time.Time) {
	startOfMonth := time.Date(current.Year(), current.Month(), 1, 0, 0, 0, 0, current.Location())
	endOfMonth := startOfMonth.AddDate(0, 1, 0).Add(-time.Nanosecond)
	return startOfMonth, endOfMonth
}

func generateCacheKey(currency string, startOfMonth time.Time) string {
	return fmt.Sprintf("exchangeRate:%s:%s", currency, startOfMonth.Format("2006-01"))
}

func (srv *ExchangeRateService) getDataFromCache(cacheKey string) ([]exchangeRate.ExchangeRate, error) {
	var monthlyRates []exchangeRate.ExchangeRate
	ratesString, err := srv.cache.Get(cacheKey)
	if err != nil {
		return nil, err
	}
	if ratesString == "" {
		return nil, nil
	}
	if err := json.Unmarshal([]byte(ratesString), &monthlyRates); err != nil {
		return nil, err
	}
	return monthlyRates, nil
}

func (srv *ExchangeRateService) storeDataInCache(cacheKey string, rates []exchangeRate.ExchangeRate) error {
	ratesString, err := json.Marshal(rates)
	if err != nil {
		return err
	}

	return srv.cache.Set(cacheKey, string(ratesString), cacheTTL)
}

func filterAndSortResults(allRates []exchangeRate.ExchangeRate, from, to time.Time) []exchangeRate.ExchangeRate {
	var filteredRates []exchangeRate.ExchangeRate
	for _, rate := range allRates {
		rateDate, err := time.Parse(dateFormat, rate.Date)
		if err != nil {
			continue
		}
		if (rateDate.After(from) || rateDate.Equal(from)) && (rateDate.Before(to) || rateDate.Equal(to)) {
			filteredRates = append(filteredRates, rate)
		}
	}

	sort.Slice(filteredRates, func(i, j int) bool {
		dateI, _ := time.Parse(dateFormat, filteredRates[i].Date)
		dateJ, _ := time.Parse(dateFormat, filteredRates[j].Date)
		return dateI.Before(dateJ)
	})

	return filteredRates
}
