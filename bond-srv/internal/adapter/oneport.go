package adapter

import (
	"context"
	"fmt"
	"net/http"
	"sync"
	"time"

	domain "github.com/pi-financial/bond-srv/internal/domain/account"
	"github.com/pi-financial/bond-srv/internal/dto"
	"github.com/shopspring/decimal"

	config "github.com/pi-financial/bond-srv/config"
	interfaces "github.com/pi-financial/bond-srv/internal/adapter/interfaces"
	onePortClient "github.com/pi-financial/go-client/oneport/client"

	adapterError "github.com/pi-financial/bond-srv/internal/adapter/error"
)

type oneportAdapter struct {
	client *onePortClient.APIClient
	cfg    config.Config
}

func NewOnePortAdapter(cfg config.Config) interfaces.OneportAdapter {
	onePortConfig := onePortClient.NewConfiguration()
	onePortConfig.Servers[0].URL = cfg.OnePortUrl
	client := onePortClient.NewAPIClient(onePortConfig)
	return &oneportAdapter{client, cfg}
}

func (op *oneportAdapter) GetPositions(ctx context.Context, accountId string) ([]onePortClient.PiOnePortDb2ModelsAccountPosition, error) {

	if op.isInMaintenanceWindow() {
		return []onePortClient.PiOnePortDb2ModelsAccountPosition{}, nil
	}

	pos, posRes, err := op.client.AccountAPI.GetAccountPosition(ctx, accountId).Page(0).Execute()

	if err != nil || posRes.StatusCode != http.StatusOK {
		var c *int
		if posRes != nil {
			c = &posRes.StatusCode
		}
		statusCodeErr := fmt.Errorf("error: %v OnePort Status Code: %d, accountId: %q", err, c, accountId)
		return nil, adapterError.NewExternalServiceError(statusCodeErr)
	}

	positions := pos.Data
	return positions, nil
}

func (op *oneportAdapter) GetPositionsByTradingAccounts(ctx context.Context, accountsNo []string) (result map[string][]domain.Position, err error) {
	if op.isInMaintenanceWindow() {
		return map[string][]domain.Position{}, nil
	}

	var wg sync.WaitGroup
	ch := make(chan dto.ChannelResult[[]domain.Position], len(accountsNo))

	for _, accountNo := range accountsNo {
		wg.Add(1)
		go func(acc string) {
			defer wg.Done()

			positions, err := op.GetPositions(ctx, acc)

			var res []domain.Position

			if err == nil {
				for _, position := range positions {
					res = append(res, *domain.NewPosition(
						position.GetAccountNo(),
						position.GetSecSymbol(),
						decimal.NewFromFloat32(position.GetAvgPrice()),
						decimal.NewFromFloat32(position.GetStartVolume()),
						decimal.NewFromFloat32(position.GetAvaiVolume())))
				}
			}

			ch <- dto.ChannelResult[[]domain.Position]{
				Reference: acc,
				Data:      res,
				Err:       err,
			}
		}(accountNo)
	}

	go func() {
		wg.Wait()
		close(ch)
	}()

	result = make(map[string][]domain.Position)
	for res := range ch {
		if res.Err != nil {
			continue
		}

		result[res.Reference] = res.Data
	}

	return result, nil
}

func (op *oneportAdapter) GetAvailabilities(ctx context.Context, custCode string) ([]onePortClient.PiOnePortDb2ModelsAccountAvailable, error) {
	accountId := custCode + "1" //always custCode+1
	avia, availableRes, err := op.client.AccountAPI.GetAccountsAvailable(ctx, accountId).Page(0).Execute()

	if err != nil || availableRes.StatusCode != http.StatusOK {
		var c *int
		if availableRes != nil {
			c = &availableRes.StatusCode
		}
		statusCodeErr := fmt.Errorf("error: %v OnePort Status Code: %d, accountId: %q", err, c, accountId)
		return nil, adapterError.NewExternalServiceError(statusCodeErr)
	}

	available := avia.Data
	return available, nil
}

func (op *oneportAdapter) isInMaintenanceWindow() bool {
	thaiLoc, _ := time.LoadLocation("Asia/Bangkok")
	now := time.Now().In(thaiLoc)

	startTime, errStart := time.Parse(time.RFC3339, op.cfg.OneportMaintenanceStartDateTime)
	endTime, errEnd := time.Parse(time.RFC3339, op.cfg.OneportMaintenanceEndDateTime)

	if errStart == nil && errEnd == nil {
		startTime = startTime.In(thaiLoc)
		endTime = endTime.In(thaiLoc)
		return now.After(startTime) && now.Before(endTime)
	}

	return false
}
