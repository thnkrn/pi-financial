package utils

import (
	"errors"
	"github.com/go-playground/validator/v10"
	"github.com/pi-financial/go-common/result"

	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
)

func GetUserIdFromHeader(c echo.Context) (uuid.UUID, error) {
	userId := c.Request().Header.Get("user-id")
	if userId == "" {
		return uuid.Nil, errors.New("user-id is required")
	}
	if _, err := uuid.Parse(userId); err != nil {
		return uuid.Nil, errors.New("invalid user-id format, must be UUID")
	}

	return uuid.Parse(userId)
}

func GetOptionalUserIdFromHeader(c echo.Context) (uuid.UUID, error) {
	userId := c.Request().Header.Get("user-id")
	if userId == "" {
		return uuid.Nil, nil
	}
	if _, err := uuid.Parse(userId); err != nil {
		return uuid.Nil, errors.New("invalid user-id format, must be UUID")
	}

	return uuid.Parse(userId)
}

func ValidationErrorHandler(c echo.Context, err error) error {
	var validationErrors validator.ValidationErrors
	if errors.As(err, &validationErrors) {
		for _, e := range validationErrors {
			switch e.Tag() {
			case "required":
				return result.ParamErrorResult(c, errors.New(e.Field()+" is required"))
			case "oneof":
				return result.ParamErrorResult(c, errors.New(e.Field()+" must be one of "+e.Param()))
			default:
				return result.ParamErrorResult(c, errors.New("invalid value for: "+e.Field()))
			}
		}
	}
	return nil
}
