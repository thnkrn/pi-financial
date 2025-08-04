package service

import (
	"context"

	gb "github.com/growthbook/growthbook-golang"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
)

type FeatureService struct {
	growthbook *gb.Client
}

func NewFeatureService(growthbook *gb.Client) interfaces.FeatureService {
	return &FeatureService{
		growthbook: growthbook,
	}
}

func (s *FeatureService) IsOn(ctx context.Context, featureName string) bool {
	return s.growthbook.EvalFeature(ctx, featureName).On
}

func (s *FeatureService) IsOff(ctx context.Context, featureName string) bool {
	return s.growthbook.EvalFeature(ctx, featureName).Off
}
