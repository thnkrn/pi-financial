package repository

import (
	"context"

	"github.com/pi-financial/go-common/logger"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type DocumentRepository struct {
	db  *gorm.DB
	Log logger.Logger
}

func NewDocumentRepository(db *gorm.DB, log logger.Logger) interfaces.DocumentRepository {
	return &DocumentRepository{
		db:  db,
		Log: log,
	}
}

func (r *DocumentRepository) Create(ctx context.Context, document *domain.Document) error {
	return r.db.WithContext(ctx).Create(&document).Error
}

func (r *DocumentRepository) FindByUserId(ctx context.Context, userId string, documentType *string) ([]domain.Document, error) {
	var documents []domain.Document
	err := r.db.WithContext(ctx).Where("user_id = ?", userId).Order("created_at desc").Find(&documents).Error
	if documentType != nil {
		err = r.db.WithContext(ctx).Where("user_id = ?", userId).Where("document_type = ?", *documentType).Order("created_at desc").First(&documents).Error
	}
	return documents, err
}
