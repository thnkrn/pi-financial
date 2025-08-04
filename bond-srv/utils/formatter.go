package utils

import (
	"time"

	"github.com/shopspring/decimal"
)

func FormatStringToDecimal(value *string) decimal.Decimal {
	if value != nil {
		formattedValue, err := decimal.NewFromString(*value)
		if err != nil {
			return decimal.Zero
		}
		return formattedValue
	}
	return decimal.Zero
}

func FormatStringToTime(value *string, format string) *time.Time {
	if value != nil {
		dateTime, err := time.Parse(format, *value)
		if err != nil {
			return nil
		}
		return &dateTime
	}
	return nil
}
