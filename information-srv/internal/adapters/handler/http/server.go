package http

import (
	"net/http"

	"github.com/labstack/echo/v4"
	echoMiddleware "github.com/labstack/echo/v4/middleware"
	_ "github.com/pi-financial/information-srv/docs"
	"github.com/pi-financial/information-srv/internal/adapters/cache"
	log "github.com/pi-financial/information-srv/internal/driver/log"
	middleware "github.com/pi-financial/information-srv/internal/middleware"
	echoSwagger "github.com/swaggo/echo-swagger"
)

type Handlers struct {
	CalendarHandler     *CalendarHandler
	AddressHandler      *AddressHandler
	ExchangeRateHandler *ExchangeRateHandler
	ProductHandler      *ProductHandler
	BankHandler         *BankHandler
	BankBranchHandler   *BankBranchHandler
}

type Middlewares struct {
	ErrorHandler *middleware.ErrorHandler
}

type ServerHTTP struct {
	app *echo.Echo
}

// NewServerHTTP godoc
// @title           Information API
// @version         1.0
// @description     Contain Generic Information.
// @termsOfService  http://swagger.io/terms/
//
// @contact.name   API Support
// @contact.url    http://www.swagger.io/support
// @contact.email  support@swagger.io
//
// @license.name  Apache 2.0
// @license.url   http://www.apache.org/licenses/LICENSE-2.0.html
//
// @host
// @BasePath  /
//
// @externalDocs.description  OpenAPI
// @externalDocs.url          https://swagger.io/resources/open-api/
func NewServerHTTP(
	logger log.Logger,
	middlewares *Middlewares,
	handlers *Handlers,
	cache *cache.RedisCacheRepository,
) *ServerHTTP {
	e := echo.New()
	// remove trailing slash from request if exists
	e.Pre(echoMiddleware.RemoveTrailingSlash())
	// recover from panics
	e.Use(echoMiddleware.Recover())
	// log request
	e.Use(echoMiddleware.LoggerWithConfig(middleware.LoggerMiddlewareConfig()))
	e.Use(middleware.CacheMiddleware(cache))
	e.Use(middleware.TracerMiddleware())
	e.HTTPErrorHandler = middlewares.ErrorHandler.EchoErrorHandler

	e.GET("/", func(c echo.Context) error {
		return c.String(http.StatusOK, "Healthy")
	})
	e.GET("/swagger/*", echoSwagger.WrapHandler)

	e.POST("/cgs/v1/user/rsa", func(c echo.Context) error {
		return c.JSON(http.StatusOK, map[string]interface{}{
			"code":    "",
			"message": "",
			"response": map[string]interface{}{
				"rsaPubKey": "",
			},
		})
	})

	e.POST("/cgs/v1/account/kkp/qr/payment/callback", func(c echo.Context) error {
		return c.JSON(http.StatusOK, map[string]interface{}{
			"code":     "0",
			"message":  "OK",
			"response": "",
		})
	})

	e.GET("/cgs/v1/account/kkp/qr/payment/callback", func(c echo.Context) error {
		return c.JSON(http.StatusOK, map[string]interface{}{
			"code":     "0",
			"message":  "OK",
			"response": "",
		})
	})

	e.GET("/cgs/v1/user/rsa", func(c echo.Context) error {
		return c.JSON(http.StatusOK, map[string]interface{}{
			"code":    "",
			"message": "",
			"response": map[string]interface{}{
				"rsaPubKey": "",
			},
		})
	})

	InitRoutes(e, handlers)
	logger.Info("server started")

	return &ServerHTTP{e}
}

func (sh *ServerHTTP) Start() {
	_ = sh.app.Start(":8080")
}
