package handler

import (
	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type KycHandler struct {
	KycService serviceinterface.KycService
	Validator  *validator.Validate
}

func NewKycHandler(kycService serviceinterface.KycService) *KycHandler {
	return &KycHandler{
		KycService: kycService,
		Validator:  validator.New(),
	}
}

// GetKycByUserId godoc
//
//	@Summary		Get KYC for user or create if not exists.
//	@Description	Get KYC for user or create if not exists.
//	@Tags			kyc
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string	true	"User ID"
//	@Success		200		{object}	result.ResponseSuccess{data=dto.GetKycByUserIdResponse}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/kycs [get]
func (h *KycHandler) GetKycByUserId(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	kyc, err := h.KycService.GetByUserId(c.Request().Context(), userId.String())
	return result.HttpResult(c, kyc, err)
}

// CreateKyc godoc
//
//	@Summary		Create or update KYC for user.
//	@Description	Create or update KYC for user.
//	@Tags			kyc
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string					true	"User ID"
//	@Param			request	body		dto.CreateKycRequest	true	"KYC request"
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/kycs [post]
func (h *KycHandler) CreateKyc(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	var req dto.CreateKycRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := h.Validator.Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	err = h.KycService.CreateOrUpdate(c.Request().Context(), userId.String(), &req)
	return result.HttpResult(c, nil, err)
}
