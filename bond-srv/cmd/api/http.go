package api

import (
	"context"
	"net/http"

	"github.com/labstack/echo/v4"
	echoSwagger "github.com/swaggo/echo-swagger"
	echotrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/labstack/echo.v4"

	config "github.com/pi-financial/bond-srv/config"
	_ "github.com/pi-financial/bond-srv/docs"
	log "github.com/pi-financial/bond-srv/internal/driver/log"
	handler "github.com/pi-financial/bond-srv/internal/handler"
	middleware "github.com/pi-financial/bond-srv/internal/middleware"
)

// NewServerHTTP godoc
//	@title						Bond API
//	@version					1.0.0
//	@description				Serve Bond accounting, trading and market data.
//	@termsOfService				http://swagger.io/terms/
//	@securityDefinitions.apikey	ApiKeyAuth
//	@in							header
//	@name						Authorization
//
//	@contact.name				API Support
//	@contact.url				http://www.swagger.io/support
//	@contact.email				support@swagger.io
//
//	@license.name				Apache 2.0
//	@license.url				http://www.apache.org/licenses/LICENSE-2.0.html
//
//	@host						localhost:8080
//	@BasePath					/
//
//	@externalDocs.description	OpenAPI
//	@externalDocs.url			https://swagger.io/resources/open-api/

type Middlewares struct {
	ErrorHandler     *middleware.EchoErrorHandler
	HeaderValidation *middleware.EchoHeaderValidation
	Context          *middleware.EchoContext
	GrowthBookClient *middleware.GrowthBookClient
}

type Handlers struct {
	AccountHandler    *handler.AccountHandler
	MarketDataHandler *handler.BondHandler
}

type ServerHTTP struct {
	App *echo.Echo
	log log.Logger
}

func NewServerHTTP(log log.Logger, middlewares *Middlewares, handler Handlers, cfg config.Config) *ServerHTTP {
	e := echo.New()
	e.HTTPErrorHandler = middlewares.ErrorHandler.ErrorHandlerMiddleware
	e.Use(middlewares.Context.ContextMiddleware)
	e.Use(echotrace.Middleware(), middlewares.GrowthBookClient.HandleGrowthbook(log, cfg))

	sh := &ServerHTTP{e, log}
	sh.addSwagger()
	sh.addHealthChecks()
	sh.addAccountRoutes(handler.AccountHandler, middlewares.HeaderValidation.HeaderValidationMiddleware(middleware.UserId))
	sh.addBondRoutes(handler.MarketDataHandler)

	return sh
}

func (sh *ServerHTTP) Start() {
	err := sh.App.Start(":8080")
	if err != nil && err != http.ErrServerClosed {
		sh.log.Fatal(context.Background(), "unexpected shutdown the server: ", err)
	}
}

func (sh *ServerHTTP) Shutdown(ctx context.Context) {
	err := sh.App.Shutdown(ctx)
	if err != nil {
		sh.log.Fatal(context.Background(), "unexpected shutdown the server: ", err)
	}
}

func (sh *ServerHTTP) addSwagger() {
	sh.App.GET("/swagger/*", echoSwagger.WrapHandler)
}

func (sh *ServerHTTP) addHealthChecks() {
	sh.App.GET("/", func(c echo.Context) error {
		return c.String(http.StatusOK, "Healthy")
	})
}

func (sh *ServerHTTP) addAccountRoutes(handler *handler.AccountHandler, middleware echo.MiddlewareFunc) {
	sh.App.GET("/secure/accounts/summary", handler.GetAccountSummary, middleware)
	sh.App.GET("/secure/accounts/overview", handler.GetAccountOverview, middleware)
	sh.App.GET("/internal/accounts/overview", handler.GetAccountOverview, middleware)
}

func (sh *ServerHTTP) addBondRoutes(handler *handler.BondHandler) {
	sh.App.GET("/internal/bonds/symbols", handler.GetBondList)
}
