package http

import (
	"net/http"
	"time"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/pkg"
)

type CalendarHandler struct {
	srv ports.CalendarService
}

func NewCalendarHandler(srv ports.CalendarService) *CalendarHandler {
	return &CalendarHandler{
		srv,
	}
}

type getHolidaysReq struct {
	Year int `param:"year"`
}

type holidaysResponse struct {
	Name string `json:"name"`
	Date string `json:"date"`
}

// GetHolidays godoc
// @Summary 		Get Holidays
// @Description 	Retrieve a list of holidays for a specified year. If no year is provided, defaults to the current year.
// @Tags 			Calendar
// @Accept 			json
// @Produce 		json
// @Param 			year	path	int		false	"The year to retrieve holidays"
// @Success 		200 {object} pkg.Response{data=[]holidaysResponse}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/calendar/holidays [get]
// @Router 			/secure/calendar/holidays [get]
// @Router 			/internal/calendar/holidays/{year} [get]
// @Router 			/secure/calendar/holidays/{year} [get]
func (handler *CalendarHandler) GetHolidays(ctx echo.Context) error {
	req := getHolidaysReq{}
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: "Year is required"})
	}

	if req.Year == 0 {
		req.Year = time.Now().Year()
	}
	holidays, err := handler.srv.GetHolidays(ctx.Request().Context(), req.Year)

	if err != nil {
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	response := make([]holidaysResponse, 0)
	for _, holiday := range holidays {
		response = append(response, holidaysResponse{holiday.Name, holiday.Date})
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   response,
		Detail: "",
		Status: 200,
	})
}

type getNextBusinessDayReq struct {
	Date string `param:"date"`
}

// GetNextBusinessDay godoc
// @Summary			Get Next Business Day
// @Description		Retrieve the next business day following a specified date.
// @Tags			Calendar
// @Accept			json
// @Produce			json
// @Param			date	path	string	true	"The date to start from to find the next business day (format: YYYY-MM-DD)"
// @Success			200 {object} pkg.Response{data=string}
// @Failure			400 {object} pkg.Response
// @Failure			500 {object} pkg.Response
// @Router			/internal/calendar/next-business-day/{date} [get]
// @Router			/secure/calendar/next-business-day/{date} [get]
func (hh *CalendarHandler) GetNextBusinessDay(ctx echo.Context) error {
	req := getNextBusinessDayReq{}
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: "Date is required"})
	}

	date, err := time.Parse("2006-01-02", req.Date)
	if err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: "Invalid date format"})
	}

	nextDate, err := hh.srv.GetNextBusinessDay(ctx.Request().Context(), date)
	if err != nil {
		return ctx.JSON(http.StatusInternalServerError, pkg.Response{
			Title:  "Internal Server Error",
			Data:   "",
			Detail: err.Error(),
			Status: 500,
		})
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   nextDate.Format("2006-01-02"),
		Detail: "",
		Status: 200,
	})
}

type getIsHolidayReq struct {
	Date string `param:"date"`
}

// IsHoliday godoc
// @Summary			Check if Date is a Holiday
// @Description		Determine if the specified date is a recognized holiday.
// @Tags			Calendar
// @Accept			json
// @Produce			json
// @Param			date	path	string	true	"The date to check if it is a holiday (format: YYYY-MM-DD)"		format(date)
// @Success			200 {object} pkg.Response{data=bool}
// @Failure			400 {object} pkg.Response
// @Failure			500 {object} pkg.Response
// @Router			/internal/calendar/is-holiday/{date} [get]
// @Router			/secure/calendar/is-holiday/{date} [get]
func (handler *CalendarHandler) IsHoliday(ctx echo.Context) error {
	req := getIsHolidayReq{}
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: "Date is required"})
	}

	date, err := time.Parse("2006-01-02", req.Date)
	if err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: "Invalid date format"})
	}

	isHoliday, err := handler.srv.IsHoliday(ctx.Request().Context(), date)
	if err != nil {
		return ctx.JSON(http.StatusInternalServerError, pkg.Response{
			Title:  "Internal Server Error",
			Data:   "",
			Detail: err.Error(),
			Status: 500,
		})
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   isHoliday,
		Detail: ",",
		Status: 200,
	})
}
