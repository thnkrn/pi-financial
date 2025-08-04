package handler

import (
	"github.com/go-playground/validator/v10"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type SuitabilityTestHandler struct {
	SuitabilityTestService serviceinterface.SuitabilityTestService
}

func NewSuitabilityTestHandler(suitabilityTestService serviceinterface.SuitabilityTestService) *SuitabilityTestHandler {
	return &SuitabilityTestHandler{
		SuitabilityTestService: suitabilityTestService,
	}
}

// CreateNewSuitabilityTest godoc
//
//	@Summary		Create new suitability test for user.
//	@Description	Create new suitability test for user.
//	@Tags			suitability-test
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string						true	"User ID"
//	@Param			request	body		dto.SuitabilityTestRequest	true	"Suitability Test Create Request"
//	@Success		200		{object}	result.ResponseSuccess
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/suitability-test [post]
func (h *SuitabilityTestHandler) CreateNewSuitabilityTest(c echo.Context) error {
	var req dto.SuitabilityTestRequest
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

	err = h.SuitabilityTestService.CreateSuitabilityTest(c.Request().Context(), userId, req)
	return result.HttpResult(c, nil, err)
}

// GetSuitabilityTestsByUserId godoc
//
//	@Summary		Get all suitability tests for user or create if not exists.
//	@Description	Get all suitability tests for user or create if not exists.
//	@Tags			suitability-test
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string	true	"User ID"
//	@Success		200		{object}	result.ResponseSuccess{data=[]dto.SuitabilityTestResponse}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/suitability-tests [get]
func (h *SuitabilityTestHandler) GetSuitabilityTestsByUserId(c echo.Context) error {
	userIdStr := c.Request().Header.Get("user-id")
	if userIdStr == "" {
		return result.ParamErrorResult(c, constants.ErrUserIdRequired)
	}

	userId, userIdErr := uuid.Parse(userIdStr)
	if userIdErr != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidUUIDFormat)
	}

	suitabilityTests, err := h.SuitabilityTestService.GetSuitabilityTestsByUserId(c.Request().Context(), userId)
	return result.HttpResult(c, suitabilityTests, err)
}
