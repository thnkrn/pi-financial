package di

import (
	"github.com/google/wire"

	"github.com/pi-financial/bond-srv/internal/driver/growthbook"
	service "github.com/pi-financial/bond-srv/internal/service"
)

var GrowthbookSet = wire.NewSet(
	growthbook.ConnectGrowthbook,
	service.NewFeatureService,
)
