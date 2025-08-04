package utils

import (
	"github.com/shopspring/decimal"
)

type Decimal struct {
	Value *decimal.Decimal
}

func (d Decimal) MarshalJSON() ([]byte, error) {
	//decimal -> string without quote -> number
	if d.Value == nil {
		return []byte(decimal.Zero.String()), nil
	}
	result := []byte(d.Value.String())
	return result, nil
}

func (d *Decimal) UnmarshalJSON(data []byte) error {
	value, err := decimal.NewFromString(string(data))
	d.Value = Ptr(value)
	return err
}
