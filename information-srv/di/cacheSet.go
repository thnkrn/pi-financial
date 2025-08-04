package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/cache"
	"github.com/pi-financial/information-srv/internal/core/ports"
)

var CacheSet = wire.NewSet(
	wire.Bind(new(ports.CacheRepository), new(*cache.RedisCacheRepository)),
	cache.NewRedisCacheRepository,
)
