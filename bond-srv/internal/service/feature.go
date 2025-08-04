package service

import (
	"context"

	middleware "github.com/pi-financial/bond-srv/internal/middleware"
	interfaces "github.com/pi-financial/bond-srv/internal/service/interfaces"
)

type FeatureService struct {
	growthbook *middleware.GrowthBookClient
}

func NewFeatureService(growthbook *middleware.GrowthBookClient) interfaces.FeatureService {
	return &FeatureService{
		growthbook,
	}
}

func (s *FeatureService) IsOn(ctx context.Context, featureName string) bool {
	return s.growthbook.Client.EvalFeature(ctx, featureName).On
}

func (s *FeatureService) IsOff(ctx context.Context, featureName string) bool {
	return s.growthbook.Client.EvalFeature(ctx, featureName).Off
}
