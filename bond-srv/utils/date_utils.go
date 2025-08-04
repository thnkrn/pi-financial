package utils

import (
	"encoding/json"
	"time"

	"github.com/pi-financial/bond-srv/internal/constants"
)

type DateTimeMS time.Time

func (dt DateTimeMS) MarshalJSON() ([]byte, error) {
	formatted := time.Time(dt).Format(constants.DateTimeMsFormat)
	return json.Marshal(formatted)
}

func AddBusinessDateTime(dateTime time.Time, year int, month int, date int) time.Time {
	for {
		dateTime = dateTime.AddDate(year, month, date)
		day := dateTime.Weekday()
		if day != time.Saturday && day != time.Sunday && !isPublicHolidays(dateTime) {
			return dateTime
		}
	}
}

func isPublicHolidays(dateTime time.Time) bool {
	for _, holiday := range holidays() {
		if holiday.Day() == dateTime.Day() && holiday.Month() == dateTime.Month() && holiday.Year() == dateTime.Year() {
			return true
		}
	}
	return false
}

func holidays() []time.Time {
	return []time.Time{
		time.Date(2024, 12, 5, 0, 0, 0, 0, time.UTC),
		time.Date(2024, 12, 10, 0, 0, 0, 0, time.UTC),
		time.Date(2024, 12, 31, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 1, 1, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 2, 12, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 4, 7, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 4, 14, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 4, 15, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 5, 1, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 5, 5, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 5, 12, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 6, 3, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 7, 10, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 7, 28, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 8, 12, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 10, 13, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 12, 5, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 12, 10, 0, 0, 0, 0, time.UTC),
		time.Date(2025, 10, 31, 0, 0, 0, 0, time.UTC),
	}
}
