package ports

import "time"

type CacheRepository interface {
	Get(key string) (string, error)
	Set(key string, value string, ttl time.Duration) error
}
