package services

import (
	"context"
	"encoding/json"
	"fmt"
	"time"

	"github.com/pi-financial/information-srv/internal/core/domain/calendar"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/driver/log"
)

type CalendarService struct {
	cache  ports.CacheRepository
	logger log.Logger
	repo   ports.CalendarRepository
}

var dateLayout string = "2006-01-02"

func NewCalendarService(repo ports.CalendarRepository, cache ports.CacheRepository, logger log.Logger) *CalendarService {
	return &CalendarService{
		cache,
		logger,
		repo,
	}
}

func (srv *CalendarService) GetHolidays(ctx context.Context, year int) ([]calendar.Holiday, error) {
	// local cache control to avoid unnecessary calls to the repository (BOT)
	cacheKey := fmt.Sprintf("holidays-%d", year)
	holidays, err := srv.tryGetCachedHolidays(cacheKey)
	if err != nil {
		return nil, err
	}
	// return cached holidays if found
	if holidays != nil {
		return holidays, nil
	}

	// fetch holidays from the repository
	holidays, err = srv.repo.GetHolidays(ctx, year)
	if err != nil {
		return nil, err
	}

	err = srv.setCacheHolidays(cacheKey, holidays)
	if err != nil {
		return nil, err
	}
	return holidays, nil
}

func (srv *CalendarService) tryGetCachedHolidays(cacheKey string) ([]calendar.Holiday, error) {
	cachedValue, err := srv.cache.Get(cacheKey)
	if err != nil {
		return nil, err
	}

	if cachedValue != "" {
		var holidays []calendar.Holiday
		if err := json.Unmarshal([]byte(cachedValue), &holidays); err != nil {
			return nil, err
		}
		return holidays, nil
	}
	return nil, nil
}

func (srv *CalendarService) setCacheHolidays(cacheKey string, holidays []calendar.Holiday) error {
	// Cache when result is not empty
	if len(holidays) > 0 {
		holidaysJSON, err := json.Marshal(holidays)
		if err != nil {
			return err
		}

		if err := srv.cache.Set(cacheKey, string(holidaysJSON), time.Hour*2); err != nil {
			return err
		}
	}
	return nil
}

func (srv *CalendarService) GetNextBusinessDay(ctx context.Context, date time.Time) (*time.Time, error) {
	holidays, err := srv.GetHolidays(ctx, date.Year())
	if err != nil {
		return nil, fmt.Errorf("failed to get holidays: %w", err)
	}

	nextDate, err := srv.findNextBusinessDay(holidays, date)
	if err != nil {
		return nil, err
	}
	if nextDate == nil {
		return nil, fmt.Errorf("next business day not found")
	}

	// check if nextDate extends to the next year, if so, fetch holidays for the next year
	if nextDate.Year() != date.Year() {
		nextHolidays, err := srv.GetHolidays(ctx, nextDate.Year())
		if err != nil {
			return nil, err
		}

		// find the next business day after the next year's holidays
		concatHoliday := append(holidays, nextHolidays...)
		nextDate, err = srv.findNextBusinessDay(concatHoliday, (*nextDate).AddDate(0, 0, -1))
		if err != nil {
			return nil, err
		}
		if nextDate == nil {
			return nil, fmt.Errorf("next business day not found")
		}
	}

	return nextDate, nil
}

func (srv *CalendarService) findNextBusinessDay(holidays []calendar.Holiday, date time.Time) (*time.Time, error) {
	loopLimit := 15
	loopCounter := 0
	nextDate := date

	for {
		nextDate = nextDate.AddDate(0, 0, 1)
		if !srv.checkHoliday(holidays, nextDate) {
			return &nextDate, nil
		}

		loopCounter++
		if loopCounter > loopLimit {
			return nil, fmt.Errorf("infinite loop detected: exceeded %d days", loopLimit)
		}
	}
}

func (srv *CalendarService) IsHoliday(ctx context.Context, date time.Time) (bool, error) {
	holidays, err := srv.GetHolidays(ctx, date.Year())
	if err != nil {
		return false, err
	}
	isHoliday := srv.checkHoliday(holidays, date)
	return isHoliday, nil
}

func (srv *CalendarService) checkHoliday(holidays []calendar.Holiday, date time.Time) bool {
	weekday := date.Weekday()
	if weekday == time.Saturday || weekday == time.Sunday {
		return true
	} else {
		dateStr := date.Format(dateLayout)
		for _, holiday := range holidays {
			if holiday.Date == dateStr {
				return true
			}
		}
	}
	return false
}
