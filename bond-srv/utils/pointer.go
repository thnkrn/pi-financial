package utils

import "github.com/shopspring/decimal"

func Ptr[T decimal.Decimal](d T) *T {
	return &d
}

func GetValueOrDefault[T decimal.Decimal | string](d *T) T {
	if d != nil {
		return *d
	}
	var defaultRes T
	return defaultRes
}
