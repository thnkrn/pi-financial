package repository

import (
	"context"

	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	"gorm.io/gorm"
)

type transactionRepository struct {
	DB *gorm.DB
}

func NewTransactionRepository(db *gorm.DB) port.TransactionRepository {
	return &transactionRepository{DB: db}
}

func (r *transactionRepository) Transaction(ctx context.Context, txFunc func(mt4 port.MT4Repository, mt5 port.MT5Repository) error) error {
	return runInTx(r.DB, func(tx *gorm.DB) error {
		mt4Repo := NewMT4Repository(tx)
		mt5Repo := NewMT5Repository(tx)

		return txFunc(mt4Repo, mt5Repo)
	})
}

func runInTx(db *gorm.DB, fn func(tx *gorm.DB) error) error {
	tx := db.Begin()
	if tx.Error != nil {
		return tx.Error
	}

	if err := fn(tx); err != nil {
		tx.Rollback()
		return err
	}

	return tx.Commit().Error
}
