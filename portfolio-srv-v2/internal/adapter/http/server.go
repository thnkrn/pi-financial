package http

import (
	"fmt"
	"net/http"

	_ "github.com/pi-financial/portfolio-srv-v2/docs" // swagger docs
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/pi-financial/portfolio-srv-v2/internal/middleware"
	echoswagger "github.com/swaggo/echo-swagger"

	"github.com/labstack/echo/v4"

	config "github.com/pi-financial/portfolio-srv-v2/config"
)

type Middlewares struct {
	ErrorHandler *middleware.ErrorHandler
}

type Handlers struct {
	TotalPortfolioSummaryHandler *TotalPortfolioSummaryHandler
	FundSummaryHandler           *FundSummaryHandler
	BondSummaryHandler           *BondSummaryHandler
	ThaiEquitySummaryHandler     *ThaiEquitySummaryHandler
	TfexSummaryHandler           *TfexSummaryHandler
	CashSummaryHandler           *CashSummaryHandler
	GeSummaryHandler             *GeSummaryHandler
	StructuredHandler            *StructuredHandler
}

type ServerHTTP struct {
	app *echo.Echo
}

func NewServerHTTP(log log.Logger, middlewares *Middlewares, handlers Handlers, cfg config.Config) *ServerHTTP {
	e := echo.New()
	e.HTTPErrorHandler = middlewares.ErrorHandler.EchoErrorHandler

	e.GET("/", func(c echo.Context) error {
		return c.String(http.StatusOK, "Hello, World!")
	})

	e.GET("/error", func(c echo.Context) error {
		return fmt.Errorf("error")
	})

	// Swagger endpoint
	e.GET("/swagger/*", echoswagger.WrapHandler)

	registerFundSummary(e, handlers)
	registerBondSummary(e, handlers)
	registerThaiEquitySummary(e, handlers)
	registerTfexSummary(e, handlers)
	registerTotalPortfolio(e, handlers)
	registerCashSummary(e, handlers)
	registerGeSummary(e, handlers)
	registerStructuredSummary(e, handlers)
	log.Info(cfg.DBHost)

	log.Info("server started")

	return &ServerHTTP{e}
}

func (sh *ServerHTTP) Start() {
	_ = sh.app.Start(":8080")
}

func registerFundSummary(e *echo.Echo, handlers Handlers) {
	e.GET("/internal/v1/fund-summary/:customer-code", handlers.FundSummaryHandler.GetFundSummaryByCustomerCode)
}

func registerBondSummary(e *echo.Echo, handlers Handlers) {
	e.GET("/internal/v1/bond-summary/:customer-code", handlers.BondSummaryHandler.GetBondSummaryByCustomerCode)
	e.GET("/internal/v1/bond-offshore-summary/:customer-code", handlers.BondSummaryHandler.GetBondOffshoreSummaryByCustomerCode)
}

func registerThaiEquitySummary(e *echo.Echo, handlers Handlers) {
	e.GET("/internal/v1/thai-equity-summary/:customer-code", handlers.ThaiEquitySummaryHandler.GetThaiEquitySummaryByCustomerCode)
}

func registerTfexSummary(e *echo.Echo, handlers Handlers) {
	e.GET("/internal/v1/tfex-daily-summary/:customer-code", handlers.TfexSummaryHandler.GetTfexDailySummaryByCustomerCode)
	e.GET("/internal/v1/tfex-daily/:customer-code", handlers.TfexSummaryHandler.GetTfexDailyByCustomerCode)
}

func registerTotalPortfolio(e *echo.Echo, handlers Handlers) {
	e.GET("/internal/v1/total-portfolio-summary/:user-id", handlers.TotalPortfolioSummaryHandler.GetTotalPortfolioSummaryByCustomerCode)
}

func registerCashSummary(e *echo.Echo, handlers Handlers) {
	e.GET("/internal/v1/cash-summary/:customer-code", handlers.CashSummaryHandler.GetCashSummaryByCustomerCode)
}

func registerGeSummary(e *echo.Echo, handlers Handlers) {
	e.GET("/internal/v1/global-equity-summary/:customer-code", handlers.GeSummaryHandler.GetGeSummaryByCustomerCode)
	e.GET("/internal/v1/global-equity-deposit-withdraw-summary/:customer-code", handlers.GeSummaryHandler.GetGeDepositWithdrawSummaryByCustomerCode)
	e.GET("/internal/v1/global-equity-dividend-summary/:customer-code", handlers.GeSummaryHandler.GetGeDividendSummaryByCustomerCode)
	e.GET("/internal/v1/global-equity-trade-summary/:customer-code", handlers.GeSummaryHandler.GetGeTradeSummaryByCustomerCode)
	e.GET("/internal/v1/global-equity-otc-summary/:customer-code", handlers.GeSummaryHandler.GetGeOtcByCustomerCode)
}

func registerStructuredSummary(e *echo.Echo, handlers Handlers) {
	e.GET("/internal/v1/structured-product-summary/:customer-code", handlers.StructuredHandler.GetStructuredProductSummaryByCustomerCode)
	e.GET("/internal/v1/structured-product-onshore-summary/:customer-code", handlers.StructuredHandler.GetStructuredProductOnshoreSummaryByCustomerCode)
	e.GET("/internal/v1/structure-note-cash-movement/:customer-code", handlers.StructuredHandler.GetStructuredNoteCashMovementByCustomerCode)
}
