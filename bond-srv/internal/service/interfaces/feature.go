package interfaces

import "context"

type FeatureService interface {
	IsOn(ctx context.Context, featureName string) bool
	IsOff(ctx context.Context, featureName string) bool
}
