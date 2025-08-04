package handler

import "github.com/labstack/echo/v4"

func InitializeRoutes(e *echo.Echo, h *Handlers) {
	secure := e.Group("/secure")
	internal := e.Group("/internal")
	registerSecureMetaTraderRoutes(secure, h)
	registerInternalMetaTraderRoutes(internal, h)
}

func registerSecureMetaTraderRoutes(g *echo.Group, h *Handlers) {
	g = g.Group("/meta-trader")
	g.POST("", h.MetaTraderHandler.RegisterMetaTrader)
}

func registerInternalMetaTraderRoutes(g *echo.Group, h *Handlers) {
	g = g.Group("/meta-trader")
	g.GET("", h.MetaTraderHandler.GetMetaTraders)
	g.PUT("/exported", h.MetaTraderHandler.UpdateExported)
}
