package handler

import (
	"errors"

	"github.com/google/uuid"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/utils"

	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/logger"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
)

type ChangeRequestHandler struct {
	ChangeRequestService serviceinterface.ChangeRequestService
	Validator            *validator.Validate
	Log                  logger.Logger
}

func NewChangeRequestHandler(changeRequestService serviceinterface.ChangeRequestService, log logger.Logger) *ChangeRequestHandler {
	return &ChangeRequestHandler{
		ChangeRequestService: changeRequestService,
		Validator:            validator.New(),
		Log:                  log,
	}
}

// CreateChangeRequest godoc
//
// @Summary      Create a change request for user info
// @Description  Create a change request for user info. If all fields have no change, returns 400.
// @Tags         change-request
// @Accept       json
// @Produce      json
// @Param        request body dto.CreateChangeRequireInfoRequest true "Change Request"
// @Success      204  "No Content"
// @Failure		 400			{object}	result.ResponseError
// @Failure		 500			{object}	result.ResponseError
// @Router       /internal/v1/change-requests [post]
func (h *ChangeRequestHandler) CreateChangeRequest(c echo.Context) error {
	var req dto.CreateChangeRequireInfoRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := h.Validator.Struct(req); err != nil {
		return utils.ValidationErrorHandler(c, err)
	}

	err := h.ChangeRequestService.ProcessChangeRequest(c.Request().Context(), req)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	return result.HttpResult(c, nil, nil)
}

// InsertChangeRequestAction godoc
//
// @Summary      Insert a change request action.
// @Description  Insert a change request action.
// @Tags         change-request
// @Accept       json
// @Produce      json
// @Param        changeRequestId  path    string          true  "Change request id"
// @Param        request         body    dto.AuditAction  true  "Change request action"
// @Success      204            "No Content"
// @Failure      400            {object}  result.ResponseError
// @Failure      500            {object}  result.ResponseError
// @Router       /internal/v1/change-requests/{changeRequestId}/action [post]
func (h *ChangeRequestHandler) InsertChangeRequestAction(c echo.Context) error {
	changeRequestId, err := uuid.Parse(c.Param("changeRequestId"))
	if err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidUUIDFormat)
	}

	var req dto.AuditAction
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := h.Validator.Struct(req); err != nil {
		return utils.ValidationErrorHandler(c, err)
	}

	id, err := h.ChangeRequestService.InsertAuditAction(c.Request().Context(), changeRequestId, req)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	return result.HttpResult(c, id, nil)
}

// GetChangeRequest godoc
//
// @Summary      Get a change request for user info
// @Description  Get a change request for user info.
// @Tags         change-request
// @Accept       json
// @Produce      json
// @Param        params query dto.GetChangeRequestParams true "Change Request"
// @Success      200    {object} result.ResponseSuccess{data=dto.GetChangeRequestResponse}
// @Failure      400    {object} result.ResponseError
// @Failure      500    {object} result.ResponseError
// @Router       /internal/v1/change-requests [get]
func (h *ChangeRequestHandler) GetChangeRequest(c echo.Context) error {
	var req dto.GetChangeRequestParams
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := validator.New().Struct(req); err != nil {
		var validationErrors validator.ValidationErrors
		if errors.As(err, &validationErrors) {
			for _, e := range validationErrors {
				switch e.Field() {
				case "InfoType":
					return result.ParamErrorResult(c, errors.New("infoType must be one of: PersonalInfo, AddressInfo, ContactInfo"))
				case "Status":
					return result.ParamErrorResult(c, errors.New("status must be one of: Pending, Approved, Rejected, Cancelled"))
				case "Date":
					return result.ParamErrorResult(c, errors.New("date must be in YYYY-MM-DD format"))
				default:
					return result.ParamErrorResult(c, errors.New("invalid parameter: "+e.Field()))
				}
			}
		}

		return result.ParamErrorResult(c, err)
	}

	data, err := h.ChangeRequestService.GetChangeRequest(c.Request().Context(), req)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	return result.HttpResult(c, data, nil)
}

// GetChangeRequestById godoc
//
// @Summary      Get a change request by id
// @Description  Get a change request by id
// @Tags         change-request
// @Accept       json
// @Produce      json
// @Param        changeRequestId  path    string          true  "Change request id"
// @Success      200    {object} result.ResponseSuccess{data=dto.GetChangeRequestByIdResponse}
// @Failure      400    {object} result.ResponseError
// @Failure      500    {object} result.ResponseError
// @Router       /internal/v1/change-requests/{changeRequestId} [get]
func (h *ChangeRequestHandler) GetChangeRequestById(c echo.Context) error {
	id, err := uuid.Parse(c.Param("changeRequestId"))
	if err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidUUIDFormat)
	}

	data, err := h.ChangeRequestService.GetChangeRequestById(c.Request().Context(), id)
	return result.HttpResult(c, data, err)
}

// GetChangeRequestAction godoc
//
// @Summary      Get audit log by change request id
// @Description  Get audit log by change request id
// @Tags         change-request
// @Accept       json
// @Produce      json
// @Param        changeRequestId  path    string          true  "Change request id"
// @Param        params query dto.GetChangeRequestActionParams true "Change Request"
// @Success      200    {object} result.ResponseSuccess{data=[]dto.GetChangeRequestActionResponse}
// @Failure      400    {object} result.ResponseError
// @Failure      500    {object} result.ResponseError
// @Router       /internal/v1/change-requests/{changeRequestId}/action [get]
func (h *ChangeRequestHandler) GetChangeRequestAction(c echo.Context) error {
	id, err := uuid.Parse(c.Param("changeRequestId"))
	if err != nil {
		return result.ParamErrorResult(c, constants.ErrInvalidUUIDFormat)
	}

	var req dto.GetChangeRequestActionParams
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := validator.New().Struct(req); err != nil {
		var validationErrors validator.ValidationErrors
		if errors.As(err, &validationErrors) {
			for _, e := range validationErrors {
				switch e.Field() {
				case "Action":
					return result.ParamErrorResult(c, errors.New("action must be one of: Pending, Approved, Rejected, Cancelled"))
				case "Date":
					return result.ParamErrorResult(c, errors.New("date must be in YYYY-MM-DD format"))
				default:
					return result.ParamErrorResult(c, errors.New("invalid parameter: "+e.Field()))
				}
			}
		}

		return result.ParamErrorResult(c, err)
	}

	data, err := h.ChangeRequestService.GetChangeRequestAction(c.Request().Context(), id, req)
	return result.HttpResult(c, data, err)
}
