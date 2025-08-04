package handler

import (
	"fmt"
	"net/http"

	"github.com/labstack/echo/v4"

	config "github.com/pi-financial/go-boilerplate/config"
	log "github.com/pi-financial/go-boilerplate/internal/driver/log"
	handler "github.com/pi-financial/go-boilerplate/internal/handler/example"
	middleware "github.com/pi-financial/go-boilerplate/internal/middleware"
)

type Middlewares struct {
	ErrorHandler *middleware.ErrorHandler
}

type Handlers struct {
	ExampleHandler *handler.ExampleHandler
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
		return fmt.Errorf("Error")
	})

	e.GET("/examples", handlers.ExampleHandler.FindAll)

	log.Info(cfg.DBHost)

	log.Info("server started")

	return &ServerHTTP{e}
}

func (sh *ServerHTTP) Start() {
	sh.app.Start(":8080")
}
