package handler

import (
	"net/http"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/logger"
	"github.com/pi-financial/go-common/middleware"
	"github.com/pi-financial/user-srv-v2/config"
	_ "github.com/pi-financial/user-srv-v2/docs" // swagger docs
	echoswagger "github.com/swaggo/echo-swagger"
	echotrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/labstack/echo.v4"
)

type Middlewares struct {
	Logger *middleware.Logger
}

type Handlers struct {
	UserInfoHandler        *UserInfoHandler
	WatchlistHandler       *WatchlistHandler
	UserAccountHandler     *UserAccountHandler
	BankAccountHandler     *BankAccountHandler
	TradingAccountHandler  *TradingAccountHandler
	AddressHandler         *AddressHandler
	KycHandler             *KycHandler
	ExternalAccountHandler *ExternalAccountHandler
	SuitabilityTestHandler *SuitabilityTestHandler
	DebugHandler           *DebugHandler
	ChangeRequestHandler   *ChangeRequestHandler
}

type ServerHTTP struct {
	app *echo.Echo
}

func NewServerHTTP(log logger.Logger, middlewares *Middlewares, handlers Handlers, cfg config.Config) *ServerHTTP {
	e := echo.New()
	// e.HTTPErrorHandler = middlewares.ErrorHandler.EchoErrorHandler

	e.GET("/", func(c echo.Context) error {
		return c.JSON(http.StatusOK, map[string]string{"Project": "pi-user-v2", "Version": "1.0.0"})
	})

	e.Use(echotrace.Middleware(), middlewares.Logger.Handler(log))

	internal := e.Group("internal/v1")

	internal.POST("/debug/hash", handlers.DebugHandler.GetHash)
	internal.GET("/debug/try-feature-service", handlers.DebugHandler.TryFeatureService)
	internal.GET("/debug/try-feature-service/with-headers", handlers.DebugHandler.TryFeatureServiceWithHeaders)

	internal.POST("/users/migrate", handlers.UserInfoHandler.MigrateUser)
	internal.POST("/users/sync", handlers.UserInfoHandler.SyncUser)
	internal.GET("/users", handlers.UserInfoHandler.GetUserInfoByFilters)
	internal.PATCH("/user", handlers.UserInfoHandler.PatchUser)
	internal.POST("/users", handlers.UserInfoHandler.CreateUserInfo)
	internal.GET("/users/profile", handlers.UserInfoHandler.GetProfile)

	internal.GET("/watchlists", handlers.WatchlistHandler.GetWatchlistByUserId)

	internal.GET("/bank-account/deposit-withdraw", handlers.BankAccountHandler.GetBankAccountForDepositWithdraw)
	internal.GET("/bank-accounts", handlers.BankAccountHandler.GetBankAccountByUserId)
	internal.POST("/bank-account", handlers.BankAccountHandler.CreateBankAccount)

	internal.POST("/user-account", handlers.UserAccountHandler.LinkUserAccount)
	internal.GET("/user-accounts", handlers.UserAccountHandler.GetUserAccountsByFilters)
	internal.GET("/user-accounts/marketing/:marketingId", handlers.UserAccountHandler.GetUserAccountByMarketingId)
	internal.GET("/user-accounts/customer-info/:accountId", handlers.UserAccountHandler.GetCustomerInfoByAccountId)
	internal.GET("/users", handlers.UserInfoHandler.GetUserInfoByFilters)
	internal.PATCH("/users", handlers.UserInfoHandler.PatchUser)
	internal.POST("/users/:user-id/sub-users", handlers.UserInfoHandler.AddSubUser)
	internal.GET("/users/:user-id/sub-users", handlers.UserInfoHandler.GetSubUser)

	internal.GET("/trading-accounts", handlers.TradingAccountHandler.GetTradingAccountByUserId)
	internal.GET("/trading-accounts/marketing-infos", handlers.TradingAccountHandler.GetTradingAccountWithMarketingInfoByCustomerCodes)

	internal.GET("/address", handlers.AddressHandler.GetAddressByUserId)
	internal.POST("/address", handlers.AddressHandler.CreateAddressByUserId)
	internal.POST("/kycs", handlers.KycHandler.CreateKyc)
	internal.GET("/kycs", handlers.KycHandler.GetKycByUserId)
	internal.POST("/external-account", handlers.ExternalAccountHandler.CreateExternalAccount)

	internal.POST("/suitability-test", handlers.SuitabilityTestHandler.CreateNewSuitabilityTest)
	internal.GET("/suitability-tests", handlers.SuitabilityTestHandler.GetSuitabilityTestsByUserId)
	internal.POST("/trading-accounts/:customerCode", handlers.TradingAccountHandler.CreateTradingAccount)

	internal.POST("/change-requests", handlers.ChangeRequestHandler.CreateChangeRequest)
	internal.POST("/change-requests/:changeRequestId/action", handlers.ChangeRequestHandler.InsertChangeRequestAction)
	internal.GET("/change-requests/:changeRequestId/action", handlers.ChangeRequestHandler.GetChangeRequestAction)
	internal.GET("/change-requests", handlers.ChangeRequestHandler.GetChangeRequest)
	internal.GET("/change-requests/:changeRequestId", handlers.ChangeRequestHandler.GetChangeRequestById)

	secure := e.Group("secure/v1")
	secure.POST("/watchlists", handlers.WatchlistHandler.CreateWatchlist)
	secure.GET("/users", handlers.UserInfoHandler.GetUserInfo)
	secure.GET("/user-accounts", handlers.UserAccountHandler.GetUserAccountsByUserId)
	secure.GET("/trading-accounts/deposit-withdraw", handlers.TradingAccountHandler.GetDepositWithdrawableTradingAccounts)
	secure.GET("/trading-accounts", handlers.TradingAccountHandler.GetTradingAccountByUserId)

	internalV2 := e.Group("internal/v2")
	internalV2.GET("/bank-account/deposit-withdraw", handlers.BankAccountHandler.GetBankAccountsForDepositWithdraw)

	// Swagger endpoint
	e.GET("/swagger/*", echoswagger.WrapHandler)

	log.Info("server started")

	return &ServerHTTP{e}
}

func (sh *ServerHTTP) Start() {
	if err := sh.app.Start(":8080"); err != nil {
		panic(err)
	}
}
