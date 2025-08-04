package utils

import (
	"strings"
	"time"

	constants "github.com/pi-financial/user-srv-v2/const"
)

type DateOnly time.Time

func (d *DateOnly) UnmarshalJSON(b []byte) error {
	s := strings.Trim(string(b), "\"")
	t, err := time.Parse("2006-01-02", s)
	if err != nil {
		return constants.ErrInvalidDate
	}
	*d = DateOnly(t)

	return nil
}

// isAfterDate returns true if date a is after date b, comparing only year, month, and day (ignoring timezone and time-of-day).
func IsAfterDate(a, b time.Time) bool {
	y1, m1, d1 := a.Date()
	y2, m2, d2 := b.Date()
	if y1 > y2 {
		return true
	}
	if y1 < y2 {
		return false
	}
	if m1 > m2 {
		return true
	}
	if m1 < m2 {
		return false
	}
	return d1 > d2
}
