package constants

import "github.com/pi-financial/go-common/errorx"

var (
	ErrInvalidUUIDFormat = errorx.NewErrCodeMsg("PRT0001", "Invalid UUID format")
	ErrUserNotFound      = errorx.NewErrCodeMsg("PRT0002", "User Not Found")
)
