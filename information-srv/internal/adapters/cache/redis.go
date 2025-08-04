package cache

import (
	"context"
	"crypto/tls"
	"fmt"
	"time"

	"github.com/go-redis/redis/v8"
	"github.com/pi-financial/information-srv/internal/driver/log"
	"github.com/spf13/viper"
)

type RedisCacheRepository struct {
	client    *redis.Client
	connected bool
	logger    log.Logger
}

// KeySpacePrefix is the prefix for all keys stored in Redis
// Access String = "on ~information::* +@all"
var keySpacePrefix = "information::"

func IsRedisDisabled() bool {
	return !viper.GetBool("REDIS_ENABLED")
}

func NewRedisCacheRepository(logger log.Logger) *RedisCacheRepository {
	if IsRedisDisabled() {
		logger.Error("Redis is currently disabled")
		return nil
	}

	client := redis.NewClient(&redis.Options{
		Addr:     viper.GetString("REDIS_HOST"),
		Username: viper.GetString("REDIS_USER"),
		Password: viper.GetString("REDIS_PASSWORD"),
		DB:       viper.GetInt("REDIS_DB"),
	})

	if viper.GetBool("REDIS_SSL") {
		client.Options().TLSConfig = &tls.Config{
			MinVersion: tls.VersionTLS12,
		}
	}

	// Ping the Redis server to check the connection
	connected := false
	ctx := context.Background()
	if err := client.Ping(ctx).Err(); err != nil {
		logger.Error(fmt.Sprintf("Failed to ping Redis server: %s [%s]", client.Options().Addr, err))
		// panic(err)
	} else {
		logger.Info(fmt.Sprintf("Successfully connected to Redis server at %s", client.Options().Addr))
		connected = true
	}
	return &RedisCacheRepository{client: client, connected: connected, logger: logger}
}

func (repository *RedisCacheRepository) Get(key string) (string, error) {
	if IsRedisDisabled() || !repository.connected {
		return "", nil
	}

	ctx := context.Background()
	keySpace := fmt.Sprintf("%s%s", keySpacePrefix, key)
	val, err := repository.client.Get(ctx, keySpace).Result()
	if err != nil && err != redis.Nil {
		panic(err)
	}
	return val, nil
}

func (repository *RedisCacheRepository) Set(key string, value string, ttl time.Duration) error {
	if IsRedisDisabled() || !repository.connected {
		return nil
	}

	ctx := context.Background()
	keySpace := fmt.Sprintf("%s%s", keySpacePrefix, key)
	err := repository.client.Set(ctx, keySpace, value, ttl).Err()
	if err != nil {
		return err
	}
	return nil
}
