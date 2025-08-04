package middleware

import (
	"fmt"
	"net/http"

	"github.com/labstack/echo/v4"

	errorx "github.com/pi-financial/go-common/errorx"
	result "github.com/pi-financial/go-common/result"

	adapterError "github.com/pi-financial/bond-srv/internal/adapter/error"
	handlerError "github.com/pi-financial/bond-srv/internal/handler/error"
	repositoryError "github.com/pi-financial/bond-srv/internal/repository/error"
	serviceError "github.com/pi-financial/bond-srv/internal/service/error"
)

type EchoErrorHandler struct {
}

func NewEchoErrorHandler() *EchoErrorHandler {
	return &EchoErrorHandler{}
}

func (e *EchoErrorHandler) ErrorHandlerMiddleware(err error, ctx echo.Context) {
	var httpCode int
	var message string
	var errorCode string

	switch e := err.(type) {
	case *echo.HTTPError:
		httpCode = e.Code
		errorCode = errorx.SERVER_COMMON_ERROR
		message = e.Error()
	case *handlerError.BadRequestError:
		httpCode = http.StatusBadRequest
		errorCode = errorx.VALIDATION_ERROR
		message = e.Error()
	case *serviceError.BusinessError:
		httpCode = http.StatusBadRequest
		errorCode = errorx.VALIDATION_ERROR
		message = e.Error()
	case *serviceError.NotFoundError:
		httpCode = http.StatusNotFound
		errorCode = errorx.SERVER_COMMON_ERROR
		message = e.Error()
	case *repositoryError.InternalServiceError:
		httpCode = http.StatusInternalServerError
		errorCode = errorx.SERVER_COMMON_ERROR
		message = e.Error()
	case *adapterError.ExternalServiceError:
		httpCode = http.StatusInternalServerError
		errorCode = errorx.EXTERNAL_SERVICE_ERROR
		message = e.Error()
	case *adapterError.ValidationError:
		httpCode = http.StatusInternalServerError
		errorCode = errorx.VALIDATION_ERROR
		message = e.Error()
	default:
		httpCode = http.StatusInternalServerError
		errorCode = errorx.SERVER_COMMON_ERROR
		message = http.StatusText(http.StatusInternalServerError)
	}

	if !ctx.Response().Committed {
		err := result.ErrorResult(ctx, errorCode, message, httpCode)
		if err != nil {
			fmt.Printf("Error: %v sending a JSON response with status code.", err)
		}
	}
}
