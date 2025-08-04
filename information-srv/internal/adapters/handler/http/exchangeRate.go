package http

import (
	"fmt"
	"net/http"
	"strconv"
	"strings"
	"time"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/pkg"
)

type ExchangeRateHandler struct {
	srv ports.ExchangeRateService
}

func NewExchangeRateHandler(srv ports.ExchangeRateService) *ExchangeRateHandler {
	return &ExchangeRateHandler{
		srv,
	}
}

var format = "2006-01-02"
var invalidaDateMsg = "Invalid date format"

type getExchangeRateReq struct {
	From         string `query:"from"`
	To           string `query:"to"`
	FromCurrency string `query:"fromCur"`
	ToCurrency   string `query:"toCur"`
}

type exchangeRateResponse struct {
	Date           string  `json:"date"`
	BuyingSight    float32 `json:"buying_sight"`
	BuyingTransfer float32 `json:"buying_transfer"`
	Selling        float32 `json:"selling"`
	MidRate        float32 `json:"mid_rate"`
}

// GetExchangeRate godoc
// @Summary 		Get Exchange Rates
// @Description 	Retrieve the exchange rate for a specified date.
// @Tags 			Exchange Rate
// @Accept 			json
// @Produce 		json
// @Param 			from		query	string	true	"Start date to retrieve the exchange rate (format: YYYY-MM-DD)."	format(from)
// @Param 			to			query	string	true	"End date to retrieve the exchange rate (format: YYYY-MM-DD)."		format(to)
// @Param 			fromCur		query	string	false	"Source currency code"												default(USD)
// @Param 			toCur		query	string	false	"Destination currency code"											default(THB)
// @Success 		200 {object} pkg.Response{data=[]exchangeRateResponse}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/exchange-rate		[get]
// @Router 			/secure/exchange-rate		[get]
func (handler *ExchangeRateHandler) GetExchangeRate(ctx echo.Context) error {
	req := getExchangeRateReq{}
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: "Date is required"})
	}

	from, err := time.Parse(format, req.From)
	if err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: invalidaDateMsg})
	}
	to, err := time.Parse(format, req.To)
	if err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: invalidaDateMsg})
	}

	if req.ToCurrency != "THB" && req.ToCurrency != "" {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{Status: 400, Detail: "Invalid currency code, only THB is supported"})
	}

	fromCurrency := req.FromCurrency
	if fromCurrency == "" {
		fromCurrency = "USD"
	}

	rates, err := handler.srv.GetExchangeRate(ctx.Request().Context(), req.FromCurrency, from, to)
	if err != nil {
		if strings.Contains(err.Error(), "400 Bad Request") {
			return echo.NewHTTPError(http.StatusBadRequest, err.Error())
		}
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	response := make([]exchangeRateResponse, 0)
	for _, rate := range rates {
		response = append(response, exchangeRateResponse{
			Date:           rate.Date,
			BuyingSight:    parseFloat32(rate.BuyingSight),
			BuyingTransfer: parseFloat32(rate.BuyingTransfer),
			Selling:        parseFloat32(rate.Selling),
			MidRate:        parseFloat32(rate.MidRate),
		})
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   response,
		Detail: fmt.Sprintf("%s-THB", fromCurrency),
		Status: 200,
	})
}

func parseFloat32(value string) float32 {
	parsedValue, err := strconv.ParseFloat(value, 32)
	if err != nil {
		return 0
	}
	return float32(parsedValue)
}
