package mysql

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
	"time"
)

type structureNoteCashMovement struct {
	CustomerCode     *string          `gorm:"column:custcode;type:varchar(255)"`
	TradingAccountNo *string          `gorm:"column:trading_account_no;type:varchar(255)"`
	ExchangeMarketId *string          `gorm:"column:exchange_market_id;type:varchar(255)"`
	CustomerType     *string          `gorm:"column:customer_type;type:varchar(255)"`
	CustomerSubType  *string          `gorm:"column:customer_sub_type;type:varchar(255)"`
	AccountType      *string          `gorm:"column:account_type;type:varchar(255)"`
	AccountTypeCode  *string          `gorm:"column:account_type_code;type:varchar(255)"`
	SubAccount       *string          `gorm:"column:sub_account;type:varchar(255)"`
	TransactionDate  *time.Time       `gorm:"column:transaction_date;type:date"`
	SettlementDate   *time.Time       `gorm:"column:settlement_date;type:date"`
	TransactionType  *string          `gorm:"column:transaction_type;type:varchar(255)"`
	Currency         *string          `gorm:"column:currency;type:varchar(255)"`
	Amount           *decimal.Decimal `gorm:"column:amount;type:decimal(65,8)"`
	Note             *string          `gorm:"column:note;type:varchar(255)"`
	Description      *string          `gorm:"column:description;type:varchar(255)"`
	DateKey          *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"column:created_at;type:datetime"`
}

type StructuredNoteCashMovementRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewStructuredNoteCashMovementRepository(logger log.Logger, db *gorm.DB) *StructuredNoteCashMovementRepository {
	return &StructuredNoteCashMovementRepository{
		logger: logger,
		db:     db,
	}
}

func (r *StructuredNoteCashMovementRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.StructuredNote, error) {
	subQuery := r.db.Table("structure_note_cash_movement").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []structureNoteCashMovement
	err := r.db.Table("structure_note_cash_movement").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		return nil, err
	}

	snapshots := make([]model.StructuredNote, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, r.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (r *StructuredNoteCashMovementRepository) mapToPublicModel(s structureNoteCashMovement) model.StructuredNote {
	return model.StructuredNote{
		CustomerCode:    s.CustomerCode,
		AccountNo:       s.TradingAccountNo,
		TransactionDate: s.TransactionDate,
		SettlementDate:  s.SettlementDate,
		Currency:        s.Currency,
		Amount:          s.Amount,
		Note:            s.Note,
		Description:     s.Description,
		DateKey:         s.DateKey,
		CreatedAt:       s.CreatedAt,
	}
}
