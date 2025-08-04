package growthbook

import (
	"context"
	"fmt"
	"time"

	gb "github.com/growthbook/growthbook-golang"
	config "github.com/pi-financial/user-srv-v2/config"
)

func ConnectGrowthbook(cfg config.Config) (_ *gb.Client, err error) {
	client, err := gb.NewClient(
		context.Background(),
		gb.WithApiHost(fmt.Sprintf("http://%s", cfg.GrowthbookHost)),
		gb.WithClientKey(cfg.GrowthBookApiKey),
		gb.WithPollDataSource(5*time.Second),
	)
	if err != nil {
		return nil, fmt.Errorf("initialize client: %w", err)
	}

	defer func() {
		if closeErr := client.Close(); closeErr != nil {
			err = fmt.Errorf("close growthbook client: %w: %w", closeErr, err)
		}
	}()

	if err := client.EnsureLoaded(context.Background()); err != nil {
		return nil, fmt.Errorf("ensure growthbook client is loaded: %w", err)
	}

	return client, nil
}
