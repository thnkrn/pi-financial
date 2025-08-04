package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http"
	"github.com/pi-financial/information-srv/internal/adapters/repositories"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/core/services"
)

var CalendarSet = wire.NewSet(
	wire.Bind(new(ports.CalendarService), new(*services.CalendarService)),
	services.NewCalendarService,

	wire.Bind(new(ports.CalendarRepository), new(*repositories.HolidayRepository)),
	repositories.NewHolidayepository,

	http.NewCalendarHandler,
)
