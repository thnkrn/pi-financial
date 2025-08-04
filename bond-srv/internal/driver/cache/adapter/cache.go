package adapter

import (
	"time"

	"github.com/patrickmn/go-cache"
	"github.com/pi-financial/bond-srv/internal/constants"
)

type CacheAdapter struct {
	cache *cache.Cache
}

func NewCache() *CacheAdapter {
	c := cache.New(constants.BondCacheDuration, constants.BondCacheDuration)
	return &CacheAdapter{c}
}

func (c *CacheAdapter) Get(key string) (any, bool) {
	result, found := c.cache.Get(key)
	return result, found
}

func (c *CacheAdapter) Set(key string, value any, duration time.Duration) {
	c.cache.Set(key, value, duration)
}
