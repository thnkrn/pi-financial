package onboard_repository

import (
	"context"

	onboardservice "github.com/pi-financial/onboard-srv/go-client"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"
)

type OnboardRepository struct {
	onboardApiClient onboardservice.APIClient
	logger           log.Logger
}

func NewOnboardRepository(logger log.Logger, onboardApiClient onboardservice.APIClient) *OnboardRepository {
	return &OnboardRepository{
		onboardApiClient: onboardApiClient,
		logger:           logger,
	}
}

func (r *OnboardRepository) GlobalEquityMapping(ctx context.Context, customerCode string) error {
	_, err := r.onboardApiClient.GlobalEquityAPI.InternalGlobalEquityAccountMapping(ctx).
		PiOnboardServiceAPIModelsMappingGlobalEquityAccountDto(onboardservice.PiOnboardServiceAPIModelsMappingGlobalEquityAccountDto{
			CustomerCode: *onboardservice.NewNullableString(&customerCode),
		}).Execute()
	if err != nil {
		r.logger.Error(ctx, "onboardRepository.GlobalEquityMapping Error Get: %v\n", zap.Error(err))
		return err
	}

	return nil
}
