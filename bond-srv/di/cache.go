package di

import (
	"github.com/google/wire"

	cache "github.com/pi-financial/bond-srv/internal/driver/cache"

	adapter "github.com/pi-financial/bond-srv/internal/driver/cache/adapter"
)

var CacheSet = wire.NewSet(
	adapter.NewCache,
	wire.Bind(new(cache.Cache), new(*adapter.CacheAdapter)),
)
