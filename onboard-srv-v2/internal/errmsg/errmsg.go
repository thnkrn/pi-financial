package errmsg

import "github.com/pi-financial/onboard-srv-v2/pkg/errconst"

var (

	// system error
	InternalServer = errconst.ErrorInternalServer.AppendMessage("ONB0001", "Internal server error")
	BadRequest     = errconst.ErrorBadRequest.AppendMessage("ONB0002", "Bad Request")
	Unauthorized   = errconst.ErrorUnauthorized.AppendMessage("ONB0003", "Unauthorized")
	Forbidden      = errconst.ErrorForbidden.AppendMessage("ONB0004", "Forbidden")
	DataNotFound   = errconst.ErrorNotFound.AppendMessage("ONB0005", "Not Found")

	// meta trader error
	MetaTraderRegisterFailed    = errconst.ErrorInternalServer.AppendMessage("ONB0006", "Register meta trader failed")
	MetaTraderInvalidDateFilter = errconst.ErrorBadRequest.AppendMessage("ONB0007", "Invalid Date Filter")
)
