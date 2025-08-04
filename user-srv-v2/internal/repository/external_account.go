package repository

import (
	"context"
	"fmt"

	"github.com/google/uuid"

	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type ExternalAccountRepository struct {
	db  *gorm.DB
	Log logger.Logger
}

func NewExternalAccountRepository(db *gorm.DB, log logger.Logger) interfaces.ExternalAccountRepository {
	return &ExternalAccountRepository{
		db:  db,
		Log: log,
	}
}

func (r *ExternalAccountRepository) Create(ctx context.Context, externalAccount *domain.ExternalAccount) error {
	return r.db.WithContext(ctx).Create(externalAccount).Error
}

func (r *ExternalAccountRepository) UpsertByTradeAccountId(ctx context.Context, tradeAccountId uuid.UUID, externalAccount *domain.ExternalAccount) (*domain.ExternalAccount, error) {
	var updatedExternalAccount domain.ExternalAccount

	result := r.db.WithContext(ctx).
		Where(domain.ExternalAccount{TradeAccountId: tradeAccountId}).
		Assign(domain.ExternalAccount{
			Value:      externalAccount.Value,
			ProviderId: externalAccount.ProviderId,
		}).
		Attrs(domain.ExternalAccount{Id: externalAccount.Id}).
		FirstOrCreate(&updatedExternalAccount)

	if result.Error != nil {
		r.Log.Error(fmt.Sprintf("Error upserting external account account %+v for id %s with error: %+v", externalAccount, tradeAccountId, result.Error))
		return nil, constants.ErrUpsertExternalAccount
	}

	return &updatedExternalAccount, nil
}

func (r *ExternalAccountRepository) FindByTradeAccountId(ctx context.Context, tradeAccountId uuid.UUID) ([]domain.ExternalAccount, error) {
	var externalAccounts []domain.ExternalAccount
	result := r.db.WithContext(ctx).Where("trade_account_id = ?", tradeAccountId).Find(&externalAccounts)
	if result.Error != nil {
		return nil, result.Error
	}
	return externalAccounts, nil
}
