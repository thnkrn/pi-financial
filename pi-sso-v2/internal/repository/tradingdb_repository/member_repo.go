package tradingdb_repository

import (
	"github.com/pi-financial/pi-sso-v2/internal/domain/tradingdb"
	"gorm.io/gorm"
)

type MemberRepository struct {
	db *gorm.DB
}

func NewMemberRepository(db *gorm.DB) *MemberRepository {
	return &MemberRepository{db}
}

func (r *MemberRepository) GetAllMembers() ([]tradingdb.Member, error) {
	var members []tradingdb.Member
	err := r.db.Find(&members).Error
	return members, err
}

func (r *MemberRepository) GetByUsername(username string) ([]tradingdb.Member, error) {
	var members []tradingdb.Member
	err := r.db.Where("Username = ?", username).Find(&members).Error
	return members, err
}
