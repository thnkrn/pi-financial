package ssodb

import (
	"fmt"
	"strings"
)

type Platform string

const (
	Email  Platform = "email"
	Mobile Platform = "mobile"
)

func (e Platform) Ptr() *Platform {
	return &e
}

func (e Platform) FromString(s string) (*Platform, error) {
	switch strings.ToLower(s) {
	case string(Email):
		return Email.Ptr(), nil
	case string(Mobile):
		return Mobile.Ptr(), nil
	default:
		return nil, fmt.Errorf("failed to parse OtpPlatform; %s", s)
	}
}
