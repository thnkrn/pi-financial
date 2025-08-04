package repository

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type KycRepository struct {
	db *gorm.DB
}

func NewKycRepository(db *gorm.DB) interfaces.KycRepository {
	return &KycRepository{db: db}
}

func (r *KycRepository) Create(ctx context.Context, kyc *domain.Kyc) error {
	return r.db.WithContext(ctx).Create(kyc).Error
}

func (r *KycRepository) Update(ctx context.Context, kyc *domain.Kyc) error {
	return r.db.WithContext(ctx).Save(kyc).Error
}

func (r *KycRepository) GetByUserId(ctx context.Context, userId uuid.UUID) (*domain.Kyc, error) {
	kyc := &domain.Kyc{}
	if err := r.db.WithContext(ctx).Where("user_id = ?", userId).Order("review_date DESC").First(kyc).Error; err != nil {
		return nil, err
	}
	return kyc, nil
}

func (r *KycRepository) UpsertById(ctx context.Context, id uuid.UUID, kyc *domain.Kyc) error {
	return r.db.WithContext(ctx).Where("id = ?", id).Assign(kyc).FirstOrCreate(kyc).Error
}

func (r *KycRepository) UpsertByUserId(ctx context.Context, id uuid.UUID, kyc *domain.Kyc) error {
	return r.db.WithContext(ctx).Where("user_id = ?", id).Assign(domain.Kyc{
		ReviewDate:  kyc.ReviewDate,
		ExpiredDate: kyc.ExpiredDate,
	}).FirstOrCreate(kyc).Error
}
