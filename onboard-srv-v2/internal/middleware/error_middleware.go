package middleware

import (
	"strings"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/onboard-srv-v2/internal/handler/dto"
	"github.com/pi-financial/onboard-srv-v2/pkg/errconst"
)

type ErrorMiddleware struct {
}

func NewErrorMiddelware() *ErrorMiddleware {
	return &ErrorMiddleware{}
}

func (e *ErrorMiddleware) EchoErrorMiddleware(err error, c echo.Context) {
	var m *errconst.ErrorResponse

	switch e := err.(type) {
	case *errconst.ErrorResponse:
		m = e
	case *echo.HTTPError:
		errMsg, ok := e.Message.(string)
		if !ok {
			errMsg = ""
		}
		m = errconst.NewError(e.Code).AppendMessage("ONB0000", strings.ToLower(errMsg))
	default:
		m = errconst.ErrorInternalServer.AppendError("ONB0000", e)
	}

	if !c.Response().Committed {
		_ = c.JSON(m.HttpStatus, dto.BaseErrorResponse{
			BaseResponse: dto.BaseResponse{
				Code: m.HttpStatus,
			},
			Title:  m.Title,
			Detail: m.Detail,
		})
	}
}
