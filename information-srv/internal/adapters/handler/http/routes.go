package http

import "github.com/labstack/echo/v4"

func InitRoutes(e *echo.Echo, handler *Handlers) {
	secure := e.Group("/secure")
	internal := e.Group("/internal")

	registerCalendarRoutes(secure, handler)
	registerCalendarRoutes(internal, handler)
	registerAddressRoutes(secure, handler)
	registerAddressRoutes(internal, handler)
	registerExchangeRateRoutes(secure, handler)
	registerExchangeRateRoutes(internal, handler)
	registerProductRoutes(secure, handler)
	registerProductRoutes(internal, handler)
	registerBankRoutes(secure, handler)
	registerBankRoutes(internal, handler)
	registerBankBranchRoutes(secure, handler)
	registerBankBranchRoutes(internal, handler)
}

func registerCalendarRoutes(group *echo.Group, handler *Handlers) {
	group.GET("/calendar/holidays/:year", handler.CalendarHandler.GetHolidays)
	group.GET("/calendar/holidays", handler.CalendarHandler.GetHolidays)
	group.GET("/calendar/next-business-day/:date", handler.CalendarHandler.GetNextBusinessDay)
	group.GET("/calendar/is-holiday/:date", handler.CalendarHandler.IsHoliday)
}

func registerAddressRoutes(group *echo.Group, handler *Handlers) {
	group.GET("/address", handler.AddressHandler.GetAddresses)
	group.GET("/address/province", handler.AddressHandler.GetProvinces)
	group.GET("/address/zip-code/:zipCode", handler.AddressHandler.GetAddressesByZipCode)
	group.GET("/address/province/:province", handler.AddressHandler.GetAddressesByProvince)
}

func registerExchangeRateRoutes(group *echo.Group, handler *Handlers) {
	group.GET("/exchange-rate", handler.ExchangeRateHandler.GetExchangeRate)

}

func registerProductRoutes(group *echo.Group, handler *Handlers) {
	group.GET("/product", handler.ProductHandler.GetProducts)

}

func registerBankRoutes(group *echo.Group, handler *Handlers) {
	group.GET("/bank", handler.BankHandler.GetBanks)
}

func registerBankBranchRoutes(group *echo.Group, handler *Handlers) {
	group.GET("/bank-branch", handler.BankBranchHandler.GetBankBranches)
}
