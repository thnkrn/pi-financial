package handler

import (
	"context"
	"fmt"
	"net"
	"net/http"

	"github.com/labstack/echo/v4"
	echoMiddleware "github.com/labstack/echo/v4/middleware"
	"github.com/pi-financial/onboard-srv-v2/config"
	"github.com/pi-financial/onboard-srv-v2/internal/middleware"
	echoSwagger "github.com/swaggo/echo-swagger"
	"go.opentelemetry.io/contrib/instrumentation/github.com/labstack/echo/otelecho"
	echotrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/labstack/echo.v4"
)

type Handlers struct {
	MetaTraderHandler *MetaTraderHandler
}

type Middlewares struct {
	ErrorMiddleware *middleware.ErrorMiddleware
}

func NewServerHTTP(ctx context.Context, middlewares *Middlewares, handlers *Handlers) *http.Server {
	e := echo.New()
	e.Validator = NewValidator()
	// Middleware
	e.Use(echotrace.Middleware())
	e.Use(otelecho.Middleware("onboard-srv-v2"))
	e.Pre(echoMiddleware.RemoveTrailingSlash())
	e.Use(echoMiddleware.Logger())
	e.Use(echoMiddleware.Recover())
	e.Use(echoMiddleware.CORS())
	e.HTTPErrorHandler = middlewares.ErrorMiddleware.EchoErrorMiddleware

	if !config.Get().App.IsProduction {
		e.GET("/swagger/*", echoSwagger.WrapHandler)
	}

	e.GET("/", HealthCheck)
	InitializeRoutes(e, handlers)

	return &http.Server{
		Addr:    fmt.Sprintf("0.0.0.0:%s", config.Get().Server.Port),
		Handler: e,
		// cancel context with stop signal
		BaseContext: func(_ net.Listener) context.Context { return ctx },
	}
}

func HealthCheck(c echo.Context) error {
	return c.JSON(http.StatusOK, map[string]interface{}{
		"data": "Onboard Service V2 is up and running",
	})
}
