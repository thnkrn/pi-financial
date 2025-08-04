package repository

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type UserHierarchyRepository struct {
	db *gorm.DB
}

func NewUserHierarchyRepository(db *gorm.DB) interfaces.UserHierarchyRepository {
	return &UserHierarchyRepository{db: db}
}

func (r *UserHierarchyRepository) Create(ctx context.Context, userHierarchy *domain.UserHierarchy) error {
	return r.db.WithContext(ctx).Create(userHierarchy).Error
}

func (r *UserHierarchyRepository) Update(ctx context.Context, userHierarchy *domain.UserHierarchy) error {
	return r.db.WithContext(ctx).Save(userHierarchy).Error
}

func (r *UserHierarchyRepository) Delete(ctx context.Context, userHierarchy *domain.UserHierarchy) error {
	return r.db.WithContext(ctx).Delete(userHierarchy).Error
}

func (r *UserHierarchyRepository) FindById(ctx context.Context, id string) (domain.UserHierarchy, error) {
	var userHierarchy domain.UserHierarchy
	result := r.db.WithContext(ctx).Where("id = ?", id).First(&userHierarchy)
	if result.Error != nil {
		return userHierarchy, result.Error
	}
	return userHierarchy, nil
}

func (r *UserHierarchyRepository) FindByUserId(ctx context.Context, userId string) ([]domain.UserHierarchy, error) {
	var userHierarchies []domain.UserHierarchy
	result := r.db.WithContext(ctx).Where("user_id = ?", userId).Find(&userHierarchies)
	if result.Error != nil {
		return nil, result.Error
	}
	return userHierarchies, nil
}
