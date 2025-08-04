package repository

import (
	"context"
	"fmt"
	"strings"

	"github.com/google/uuid"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
	"gorm.io/gorm"
)

type UserInfoRepository struct {
	db *gorm.DB
}

func NewUserInfoRepository(db *gorm.DB) interfaces.UserInfoRepository {
	return &UserInfoRepository{db: db}
}

func (r *UserInfoRepository) Update(ctx context.Context, userInfo *domain.UserInfo) error {
	return r.db.WithContext(ctx).Omit("Accounts", "Devices").Save(userInfo).Error
}

func (r *UserInfoRepository) FindById(ctx context.Context, Id string) (*domain.UserInfo, error) {
	var user *domain.UserInfo
	result := r.db.
		WithContext(ctx).
		Preload("Devices", "is_active = 1").
		Preload("Devices.NotificationPreference").
		Preload("Accounts").
		Preload("Accounts.TradeAccounts").
		Where("id = ?", Id).
		First(&user)
	if result.Error != nil {
		return nil, result.Error
	}

	return user, nil
}

func (r *UserInfoRepository) FindByFilterScopes(
	ctx context.Context,
	filter dto.UserInfoQueryFilter) ([]domain.UserInfo, error) {
	var users []domain.UserInfo

	result := r.db.WithContext(ctx).
		Limit(100).
		Preload("Devices", "is_active = 1").
		Preload("Devices.NotificationPreference", func(db *gorm.DB) *gorm.DB {
			return db.Session(&gorm.Session{}).Clauses()
		}).
		Preload("Accounts.TradeAccounts").
		Scopes(
			r.withIds(filter.Ids),
			r.withAccountId(filter.AccountId),
			r.withUserId(filter.UserId),
			r.withCitizenId(filter.CitizenId),
			r.withEmail(filter.Email),
			r.withPhoneNumber(filter.PhoneNumber),
			r.withFirstName(filter.FirstName),
			r.withLastName(filter.LastName),
		).
		Find(&users)

	if result.Error != nil {
		return nil, fmt.Errorf("in FindByFilterScopes with filters %+v: %w: %w",
			filter, constants.ErrFindingUserInfo, result.Error)
	}

	return users, nil
}

// Scope methods

func (r *UserInfoRepository) withIds(ids string) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if ids == "" {
			return db
		}

		return db.Where("id IN (?)", strings.Split(ids, ","))
	}
}

func (r *UserInfoRepository) withUserId(userId uuid.UUID) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if userId == uuid.Nil {
			return db
		}

		return db.Where("user_infos.id = ?", userId.String())
	}
}

func (r *UserInfoRepository) withAccountId(accountId string) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if accountId == "" {
			return db
		}

		return db.
			Joins("JOIN user_accounts ON user_infos.id = user_accounts.user_id").
			Where("user_accounts.id = ?", accountId)
	}
}

func (r *UserInfoRepository) withCitizenId(citizenId string) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if citizenId == "" {
			return db
		}

		return db.Where("citizen_id_hash = ?", utils.Hash(citizenId))
	}
}

func (r *UserInfoRepository) withEmail(email string) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if email == "" {
			return db
		}

		return db.Where("email_hash = ?", utils.Hash(email))
	}
}

func (r *UserInfoRepository) withPhoneNumber(phoneNumber string) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if phoneNumber == "" {
			return db
		}

		return db.Where("phone_number_hash = ?", utils.Hash(phoneNumber))
	}
}

func (r *UserInfoRepository) withFirstName(firstName string) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if firstName == "" {
			return db
		}

		return db.Where("firstname_th_hash = ? OR firstname_en_hash = ?", utils.Hash(firstName), utils.Hash(firstName))
	}
}

func (r *UserInfoRepository) withLastName(lastName string) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if lastName == "" {
			return db
		}

		return db.Where("lastname_th_hash = ? OR lastname_en_hash = ?", utils.Hash(lastName), utils.Hash(lastName))
	}
}

func (r *UserInfoRepository) FindByEmail(ctx context.Context, email string) (*domain.UserInfo, error) {
	var user *domain.UserInfo
	result := r.db.WithContext(ctx).Where("email_hash = ?", utils.Hash(email)).First(&user)
	if result.Error != nil {
		return nil, result.Error
	}

	return user, nil
}

func (r *UserInfoRepository) FindByCitizenId(ctx context.Context, citizenId string) (*domain.UserInfo, error) {
	var user *domain.UserInfo
	result := r.db.WithContext(ctx).Where("citizen_id_hash = ?", utils.Hash(citizenId)).First(&user)
	if result.Error != nil {
		return nil, result.Error
	}

	return user, nil
}

func (r *UserInfoRepository) FindByPhoneNumber(ctx context.Context, phoneNumber string) (*domain.UserInfo, error) {
	var user *domain.UserInfo
	result := r.db.WithContext(ctx).Where("phone_number_hash = ?", utils.Hash(phoneNumber)).First(&user)
	if result.Error != nil {
		return nil, result.Error
	}

	return user, nil
}

func (r *UserInfoRepository) Create(ctx context.Context, userInfo *domain.UserInfo) error {
	return r.db.WithContext(ctx).Create(userInfo).Error
}

func (r *UserInfoRepository) FindByMarketingId(ctx context.Context, marketingId string) ([]domain.UserInfo, error) {
	var users []domain.UserInfo
	result := r.db.WithContext(ctx).
		Preload("Accounts").
		Preload("Accounts.TradeAccounts").
		Joins("JOIN user_accounts ON user_infos.id = user_accounts.user_id").
		Joins("JOIN trade_accounts ON trade_accounts.user_account_id = user_accounts.id").
		Where("trade_accounts.marketing_id = ?", marketingId).
		Group("trade_accounts.user_account_id").
		Having("COUNT(DISTINCT trade_accounts.user_account_id) = 1").
		Find(&users)

	if result.Error != nil {
		return nil, result.Error
	}

	return users, nil
}
