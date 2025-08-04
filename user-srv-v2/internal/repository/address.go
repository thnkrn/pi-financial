package repository

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type AddressRepository struct {
	db *gorm.DB
}

func NewAddressRepository(db *gorm.DB) interfaces.AddressRepository {
	return &AddressRepository{db: db}
}

func (r *AddressRepository) Create(ctx context.Context, address *domain.Address) error {
	return r.db.WithContext(ctx).Create(address).Error
}

func (r *AddressRepository) Update(ctx context.Context, address *domain.Address) error {
	return r.db.WithContext(ctx).Save(address).Error
}

func (r *AddressRepository) FindByUserId(ctx context.Context, userId string) (*domain.Address, error) {
	var address domain.Address
	result := r.db.WithContext(ctx).Where("user_id = ?", userId).First(&address)
	if result.Error != nil {
		return nil, result.Error
	}
	return &address, nil
}

func (r *AddressRepository) UpsertByUserId(ctx context.Context, id uuid.UUID, address *domain.Address) error {
	existingAddress, err := r.FindByUserId(ctx, id.String())
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return r.Create(ctx, address)
		}
		return err
	}

	address.Id = existingAddress.Id
	return r.Update(ctx, address)
}
