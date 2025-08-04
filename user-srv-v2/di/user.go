package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/user-srv-v2/internal/driver/client"
	"github.com/pi-financial/user-srv-v2/internal/handler"
	"github.com/pi-financial/user-srv-v2/internal/repository"
	"github.com/pi-financial/user-srv-v2/internal/service"
)

var UserSet = wire.NewSet(
	handler.NewUserInfoHandler,
	handler.NewWatchlistHandler,
	handler.NewUserAccountHandler,
	handler.NewBankAccountHandler,
	handler.NewTradingAccountHandler,
	handler.NewAddressHandler,
	handler.NewSuitabilityTestHandler,
	handler.NewKycHandler,
	handler.NewExternalAccountHandler,
	handler.NewDebugHandler,
	handler.NewChangeRequestHandler,
	service.NewUserInfoService,
	service.NewWatchlistService,
	service.NewBankAccountService,
	service.NewTradeAccountService,
	service.NewAddressService,
	service.NewUserAccountService,
	service.NewSuitabilityTestService,
	service.NewKycService,
	service.NewExternalAccountService,
	service.NewInformationService,
	service.NewItDataService,
	service.NewFeatureService,
	service.NewChangeRequestService,
	repository.NewUserInfoRepository,
	repository.NewWatchlistRepository,
	repository.NewAddressRepository,
	repository.NewBankAccountV2Repository,
	repository.NewExternalAccountRepository,
	repository.NewUserAccountRepository,
	repository.NewSuitabilityTestRepository,
	repository.NewKycRepository,
	repository.NewTradeAccountRepository,
	repository.NewUserHierarchyRepository,
	repository.NewDocumentRepository,
	repository.NewChangeRequestRepository,
	repository.NewAuditLogRepository,
	repository.NewChangeRequestInfoRepository,
	client.NewOnboardClient,
	client.NewInformationClient,
	client.NewItDataClient,
	client.NewS3Client,
	client.NewDopaClient,
)
