package handler

import (
	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterfaces "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type DebugHandler struct {
	Validator      *validator.Validate
	FeatureService serviceinterfaces.FeatureService
}

func NewDebugHandler(featureService serviceinterfaces.FeatureService) *DebugHandler {
	return &DebugHandler{
		Validator:      validator.New(),
		FeatureService: featureService,
	}
}

// GetHash godoc
//
//	@Summary		Get hash of input string
//	@Description	Convert input string to hash value
//	@Tags			debug
//	@Accept			json
//	@Produce		json
//	@Param			request	body		dto.HashRequest	true	"String to hash"
//	@Success		200		{object}	result.ResponseSuccess{data=dto.HashResponse}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/debug/hash [post]
func (h *DebugHandler) GetHash(c echo.Context) error {
	var req dto.HashRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := h.Validator.Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	hash := utils.Hash(req.Input)
	response := dto.HashResponse{
		Hash: hash,
	}

	return result.HttpResult(c, response, nil)
}

type TryFeatureServiceParam struct {
	FeatureSwitchName string `query:"featureSwitchName"`
}

// TryFeatureService godoc
//
//	@Summary		Try feature service
//	@Description	Try feature service
//	@Tags			debug
//	@Accept			json
//	@Produce		json
//	@Param			featureSwitchName	query		string	true	"Feature Switch Name"
//	@Success		200					{object}	result.ResponseSuccess{data=bool}
//	@Failure		400					{object}	result.ResponseError
//	@Failure		500					{object}	result.ResponseError
//	@Router			/internal/v1/debug/try-feature-service [get]
func (h *DebugHandler) TryFeatureService(c echo.Context) error {
	var params TryFeatureServiceParam
	if err := c.Bind(&params); err != nil {
		return result.ParamErrorResult(c, err)
	}

	r := h.FeatureService.IsOn(c.Request().Context(), params.FeatureSwitchName)

	return result.HttpResult(c, r, nil)
}

// TryFeatureServiceWithHeaders godoc
//
//	@Summary		Try feature service with headers
//	@Description	Try feature service with headers
//	@Tags			debug
//	@Accept			json
//	@Produce		json
//	@Param			featureSwitchName	query		string	true	"Feature Switch Name"
//	@Param			user-id				header		string	false	"User ID"
//	@Param			deviceId			header		string	false	"Device ID"
//	@Param			random				header		string	false	"Random"
//	@Success		200					{object}	result.ResponseSuccess{data=bool}
//	@Failure		400					{object}	result.ResponseError
//	@Failure		500					{object}	result.ResponseError
//	@Router			/internal/v1/debug/try-feature-service/with-headers [get]
func (h *DebugHandler) TryFeatureServiceWithHeaders(c echo.Context) error {
	var params TryFeatureServiceParam
	if err := c.Bind(&params); err != nil {
		return result.ParamErrorResult(c, err)
	}

	r := h.FeatureService.IsOn(c.Request().Context(), params.FeatureSwitchName)

	return result.HttpResult(c, r, nil)
}
