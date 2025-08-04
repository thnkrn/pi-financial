package repository

import (
	"context"
	"time"

	"github.com/google/uuid"
	commondatabase "github.com/pi-financial/go-common/database"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type ChangeRequestRepository struct {
	db *gorm.DB
}

func NewChangeRequestRepository(db *gorm.DB) interfaces.ChangeRequestRepository {
	return &ChangeRequestRepository{db: db}
}

func (r *ChangeRequestRepository) Create(ctx context.Context, changeRequest *domain.ChangeRequest) (*uuid.UUID, error) {
	if err := r.db.Model(domain.ChangeRequest{}).WithContext(ctx).Create(&changeRequest).Error; err != nil {
		return nil, err
	}
	return &changeRequest.Id, nil
}

func (r *ChangeRequestRepository) Update(ctx context.Context, changeRequest *domain.ChangeRequest) error {
	return r.db.WithContext(ctx).Omit("UserInfo", "AuditLogs").Save(&changeRequest).Error
}

func (r *ChangeRequestRepository) FindById(ctx context.Context, id uuid.UUID) (*domain.ChangeRequest, error) {
	var changeRequest domain.ChangeRequest
	err := r.db.Model(domain.ChangeRequest{}).Preload("UserInfo").Preload("UserInfo.Accounts").WithContext(ctx).Where("id = ?", id).First(&changeRequest).Error
	if err != nil {
		return nil, err
	}
	return &changeRequest, nil
}

func (r *ChangeRequestRepository) FindByWithPagination(ctx context.Context, filters *domain.ChangeRequest, params commondatabase.PaginationParams) (*commondatabase.PaginationResult[domain.ChangeRequest], error) {
	var changeRequests []domain.ChangeRequest

	var result commondatabase.PaginationResult[domain.ChangeRequest]

	query := r.db.Model(domain.ChangeRequest{}).WithContext(ctx)

	if !filters.CreatedAt.IsZero() {
		query = query.Where("DATE(created_at) = DATE(?)", filters.CreatedAt)
	}

	filtersWithoutDate := *filters
	filtersWithoutDate.CreatedAt = time.Time{}

	query.Preload("UserInfo").Preload("UserInfo.Accounts").
		Where(&filtersWithoutDate).
		Scopes(commondatabase.Paginate(changeRequests, &params, &result, r.db)).
		Find(&changeRequests)

	result.Items = changeRequests
	result.ItemCount = len(result.Items)

	return &result, nil
}

// FindLatestByUserIdAndInfoType finds the latest change request for a specific user and info type.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to search for
//   - infoType: Info type to search for
//
// Returns:
//   - *domain.ChangeRequest: The latest change request for the user and info type, or nil if not found
//   - error: Error if database operation fails
//
// Implementation:
//  1. Queries the bo_change_requests table for the specified user ID and info type.
//  2. Orders by created_at DESC to get the most recent record.
//  3. Limits to 1 record to get only the latest.
//  4. Returns the latest change request or nil if not found.
//
// Error cases:
//   - Returns error if database operation fails
//   - Returns nil if no change request is found for the user and info type
func (r *ChangeRequestRepository) FindLatestByUserIdAndInfoType(ctx context.Context, userId uuid.UUID, infoType domain.ChangeRequestInfoType) (*domain.ChangeRequest, error) {
	var changeRequest domain.ChangeRequest
	err := r.db.Model(domain.ChangeRequest{}).
		WithContext(ctx).
		Where("user_id = ? AND info_type = ?", userId, infoType).
		Order("created_at DESC").
		Limit(1).
		First(&changeRequest).Error

	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, err
	}

	return &changeRequest, nil
}
