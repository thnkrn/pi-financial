package handler

import (
	"net/http"

	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"

	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
)

type CustomValidator struct {
	validator *validator.Validate
}

func NewValidator() *CustomValidator {
	cv := &CustomValidator{validator: validator.New()}

	_ = cv.validator.RegisterValidation("enum_metatrader", func(fl validator.FieldLevel) bool {
		return domain.MetaTraderPlatform(fl.Field().String()).IsValid()
	})

	return cv
}

func (cv *CustomValidator) Validate(i interface{}) error {
	if err := cv.validator.Struct(i); err != nil {
		// Optionally, you could return the error to give each route more control over the status code
		return echo.NewHTTPError(http.StatusBadRequest, err.Error())
	}
	return nil
}
