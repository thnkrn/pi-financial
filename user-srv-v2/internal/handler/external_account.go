package handler

import (
	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type ExternalAccountHandler struct {
	ExternalAccountService serviceinterface.ExternalAccountService
}

func NewExternalAccountHandler(externalAccountService serviceinterface.ExternalAccountService) *ExternalAccountHandler {
	return &ExternalAccountHandler{
		ExternalAccountService: externalAccountService,
	}
}

// CreateExternalAccount godoc
//
//	@Summary		Create or update external account for user.
//	@Description	Create or update external account for user.
//	@Tags			external-account
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string								true	"User ID"
//	@Param			request	body		dto.CreateExternalAccountRequest	true	"Create External Account Request"
//	@Success		200		{object}	result.ResponseSuccess
//	@Failure		400		{object}	result.ResponseError	"Validation failed"
//	@Router			/internal/v1/external-account [post]
func (h *ExternalAccountHandler) CreateExternalAccount(c echo.Context) error {
	var req dto.CreateExternalAccountRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := validator.New().Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	err = h.ExternalAccountService.CreateExternalAccount(c.Request().Context(), userId, req)

	return result.HttpResult(c, nil, err)
}
