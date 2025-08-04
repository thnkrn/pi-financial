package repository

import (
	"context"
	"fmt"

	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type UserAccountRepository struct {
	db  *gorm.DB
	Log logger.Logger
}

func NewUserAccountRepository(db *gorm.DB, log logger.Logger) interfaces.UserAccountRepository {
	return &UserAccountRepository{
		db:  db,
		Log: log,
	}
}

func (r *UserAccountRepository) Create(ctx context.Context, userAccount *domain.UserAccount) error {
	return r.db.WithContext(ctx).Create(userAccount).Error
}

func (r *UserAccountRepository) Update(ctx context.Context, userAccount *domain.UserAccount) error {
	return r.db.WithContext(ctx).Save(userAccount).Error
}

func (r *UserAccountRepository) FindById(ctx context.Context, id string) (*domain.UserAccount, error) {
	var userAccount domain.UserAccount
	result := r.db.WithContext(ctx).Where("id = ?", id).First(&userAccount)

	if result.Error != nil {
		return &userAccount, result.Error
	}

	return &userAccount, nil
}

func (r *UserAccountRepository) UpsertById(ctx context.Context, id string, data *domain.UserAccount) (*domain.UserAccount, error) {
	var userAccount domain.UserAccount
	result := r.db.WithContext(ctx).
		Where(domain.UserAccount{Id: data.Id}).
		Assign(domain.UserAccount{
			UserId:          data.UserId,
			UserAccountType: data.UserAccountType,
			Status:          data.Status}).
		FirstOrCreate(&userAccount)

	if result.Error != nil {
		return nil, fmt.Errorf("in UpsertById with user account id %q and account %+v: %w: %w",
			id, data, constants.ErrUpsertUserAccount, result.Error)
	}

	return &userAccount, nil
}

func (r *UserAccountRepository) FindByUserId(ctx context.Context, userId string) ([]domain.UserAccount, error) {
	var userAccount []domain.UserAccount
	result := r.db.WithContext(ctx).Where("user_id = ?", userId).Find(&userAccount)

	if result.Error != nil {
		return nil, fmt.Errorf("in FindByUserId %q: %w: %w", userId, constants.ErrFindUserAccountByUserId, result.Error)
	}

	return userAccount, nil
}

func (r *UserAccountRepository) FindByCustomerCode(ctx context.Context, customerCode string) (*domain.UserAccount, error) {
	var userAccount domain.UserAccount
	result := r.db.WithContext(ctx).
		Where(domain.UserAccount{
			Id:              customerCode,
			UserAccountType: domain.Freewill,
		}).
		First(&userAccount)

	if result.Error != nil {
		r.Log.Error(fmt.Sprintf("Error finding user account with customer code %s using account type %s with error: %+v", customerCode, domain.Freewill, result.Error))
		return nil, constants.ErrFindUserAccountByCustomerCode
	}

	return &userAccount, nil
}
