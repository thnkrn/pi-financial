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

type SuitabilityTestRepository struct {
	db  *gorm.DB
	Log logger.Logger
}

func NewSuitabilityTestRepository(
	db *gorm.DB,
	log logger.Logger) interfaces.SuitabilityTestRepository {
	return &SuitabilityTestRepository{
		db:  db,
		Log: log,
	}
}

func (r *SuitabilityTestRepository) CreateBatch(ctx context.Context, suitabilityTest []domain.SuitabilityTest) error {
	return r.db.WithContext(ctx).CreateInBatches(suitabilityTest, len(suitabilityTest)).Error
}

func (r *SuitabilityTestRepository) Create(ctx context.Context, userId uuid.UUID, suitabilityTest domain.SuitabilityTest) error {
	return r.db.WithContext(ctx).Create(&suitabilityTest).Error
}

func (r *SuitabilityTestRepository) FindAllByUserId(ctx context.Context, userId uuid.UUID) ([]domain.SuitabilityTest, error) {
	var suitabilityTests []domain.SuitabilityTest
	result := r.db.WithContext(ctx).
		Where(domain.SuitabilityTest{UserId: userId}).
		Find(&suitabilityTests)

	if result.Error != nil {
		r.Log.Error(fmt.Sprintf("Error finding suitability tests for user id %s with error: %+v", userId, result.Error))
		return nil, constants.ErrFindSuitabilityTestsByUserId
	}

	return suitabilityTests, nil
}

func (r *SuitabilityTestRepository) FindByUserId(ctx context.Context, userId uuid.UUID) (*domain.SuitabilityTest, error) {
	var suitabilityTest domain.SuitabilityTest
	result := r.db.WithContext(ctx).
		Where(domain.SuitabilityTest{UserId: userId}).
		Order("review_date desc").
		First(&suitabilityTest)

	if result.Error != nil {
		r.Log.Error(fmt.Sprintf("Error finding suitability test for user id %s with error: %+v", userId, result.Error))
		return nil, fmt.Errorf("in FindByUserId %q: %w: %w",
			userId, constants.ErrFindSuitabilityTestsByUserId, result.Error)
	}

	return &suitabilityTest, nil
}
