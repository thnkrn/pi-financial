package ports

import (
	"context"
	"time"

	"github.com/pi-financial/information-srv/internal/core/domain/calendar"
)

type CalendarRepository interface {
	GetHolidays(ctx context.Context, year int) ([]calendar.Holiday, error)
}

type CalendarService interface {
	GetHolidays(ctx context.Context, year int) ([]calendar.Holiday, error)
	GetNextBusinessDay(ctx context.Context, date time.Time) (*time.Time, error)
	IsHoliday(ctx context.Context, date time.Time) (bool, error)
}
