package growthbook

import (
	"context"
	"fmt"

	gb "github.com/growthbook/growthbook-golang"
	config "github.com/pi-financial/bond-srv/config"
	middleware "github.com/pi-financial/bond-srv/internal/middleware"
)

func ConnectGrowthbook(cfg config.Config) (_ *middleware.GrowthBookClient, err error) {
	defer func() {
		if err != nil {
			panic(fmt.Errorf("in ConnectGrowthbook: %w", err))
		}
	}()

	var (
		host      = cfg.GrowthbookHost
		apiKey    = cfg.GrowthBookApiKey
		projectId = cfg.GrowthbookProjectId
		url       = fmt.Sprintf("http://%s/api/features/%s?project=%s", host, apiKey, projectId)
	)

	client, err := gb.NewClient(
		context.Background(),
		gb.WithUrl(url),
		gb.WithEnabled(true),
		gb.WithQaMode(false),
	)
	if err != nil {
		return nil, fmt.Errorf("initialize client: %w", err)
	}

	defer client.Close()

	// Manually get the feature names. More predictable behavior than
	// automatically polling from growthbook in the background.
	rawFeatureMap, err := middleware.GetRawFeatureMap(host, apiKey, projectId)
	if err != nil {
		return nil, fmt.Errorf("get feature map: %w", err)
	}

	// Set empty attributes because the real values will be injected from the
	// api controller middleware using request's header values.
	childClient, err := client.WithAttributes(gb.Attributes{})
	if err != nil {
		return nil, fmt.Errorf("initialize child client: %w", err)
	}

	// Set the available growthbook feature names.
	err = childClient.SetJSONFeatures(string(rawFeatureMap))
	if err != nil {
		return nil, fmt.Errorf("set features for client: %w", err)
	}

	return &middleware.GrowthBookClient{
		Client: childClient,
	}, nil
}
