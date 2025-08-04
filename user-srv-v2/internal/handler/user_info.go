package handler

import (
	"errors"
	"strings"

	"github.com/pi-financial/go-common/errorx"
	"github.com/pi-financial/go-common/logger"

	"github.com/go-playground/validator/v10"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type UserInfoHandler struct {
	UserInfoService serviceinterface.UserInfoService
	Validator       *validator.Validate
	Log             logger.Logger
}

func NewUserInfoHandler(
	userInfoService serviceinterface.UserInfoService,
	log logger.Logger) *UserInfoHandler {
	return &UserInfoHandler{
		UserInfoService: userInfoService,
		Validator:       validator.New(),
		Log:             log,
	}
}

// MigrateUser godoc
//
//	@Summary		Create new user with info from BPM
//	@Description	Create new user with info from BPM
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string													true	"User ID"
//	@Param			request	body		dto.MigrateUserRequest									true	"Migrate User Request"
//	@Success		200		{object}	result.ResponseSuccess{data=dto.MigrateUserResponse}	"Migrate User"
//	@Failure		400		{object}	result.ResponseError									"Validation failed"
//	@Router			/internal/v1/users/migrate [post]
func (h *UserInfoHandler) MigrateUser(c echo.Context) error {
	var req dto.MigrateUserRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	userId := c.Request().Header.Get("user-id")
	if userId == "" {
		return result.ParamErrorResult(c, errors.New("user-id is required"))
	}

	err := h.UserInfoService.MigrateUser(c.Request().Context(), userId, &req)
	return result.HttpResult(c, nil, err)
}

// GetUserInfo godoc
//
//	@Summary		Get user info.
//	@Description	Get user info.
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string										true	"User ID"
//	@Success		200		{object}	result.ResponseSuccess{data=dto.UserInfo}	"User Info"
//	@Failure		400		{object}	result.ResponseError						"Validation failed"
//	@Router			/secure/v1/users [get]
func (h *UserInfoHandler) GetUserInfo(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	userInfo, err := h.UserInfoService.GetUserInfo(c.Request().Context(), userId.String())
	return result.HttpResult(c, userInfo.ExcludeCitizenId(), err)
}

// GetUserInfoByFilters godoc
//
//	@Summary		Get user info by filters for multiple users.
//	@Description	Get user info by filters for multiple users.
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			request		query		dto.GetUserInfoByFiltersRequest	true	    "Filter parameters"
//	@Success		200			{object}	result.ResponseSuccess{data=[]dto.UserInfo}	"User Info"
//	@Failure		400			{object}	result.ResponseError						"Validation failed"
//	@Router			/internal/v1/users [get]
func (h *UserInfoHandler) GetUserInfoByFilters(c echo.Context) error {
	var req dto.GetUserInfoByFiltersRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if req.Ids != "" {
		ids := strings.Split(req.Ids, ",")
		for _, id := range ids {
			if _, err := uuid.Parse(id); err != nil {
				return result.ParamErrorResult(c, errors.New("invalid user-id format, must be UUID"))
			}
		}
	}

	userInfos, err := h.UserInfoService.GetUserInfoByFilters(c.Request().Context(), req)
	return result.HttpResult(c, userInfos, err)
}

// PatchUser godoc
//
//	@Summary		Update some fields for user.
//	@Description	Update some fields for user.
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string						true	"User ID"
//	@Param			request	body		dto.PatchUserInfoRequest	true	"Patch User Info Request"
//	@Success		200		{object}	result.ResponseSuccess
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/users [patch]
func (h *UserInfoHandler) PatchUser(c echo.Context) error {
	var req dto.PatchUserInfoRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	err = h.UserInfoService.UpdateUserInfo(c.Request().Context(), userId.String(), &req)
	if err != nil {
		// Log out the error chain to get the stack trace.
		h.Log.Error(err.Error())

		// Return custom error if it exists in the error chain.
		var customErr *errorx.ErrorMsg
		if errors.As(err, &customErr) {
			return result.ParamErrorResult(c, customErr)
		}

		return result.ParamErrorResult(c, err)
	}
	return result.HttpResult(c, nil, err)
}

// AddSubUser godoc
//
//	@Summary		Add sub-user to a user.
//	@Description	Add sub-user to a user.
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			user-id	path		string		true	"User ID"
//	@Param			body	body		[]string	true	"Map Sub User Request"
//	@Success		200		{object}	result.ResponseSuccess
//	@Failure		400		{object}	result.ResponseError
//	@Router			/internal/v1/users/{user-id}/sub-users [post]
func (h *UserInfoHandler) AddSubUser(c echo.Context) error {
	var req []string
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	err := h.UserInfoService.AddSubUser(c.Request().Context(), c.Param("user-id"), req)
	return result.HttpResult(c, nil, err)
}

// GetSubUsers godoc
//
//	@Summary		Get sub-users associated with a user.
//	@Description	Get sub-users associated with auser.
//	@Tags			user
//	@Accept			json
//	@Produce		json
//	@Param			user-id	path		string									true	"User ID"
//	@Success		200		{object}	result.ResponseSuccess{data=[]string}	"Sub User IDs"
//	@Failure		400		{object}	result.ResponseError
//	@Router			/internal/v1/users/{user-id}/sub-users [get]
func (h *UserInfoHandler) GetSubUser(c echo.Context) error {
	subUsers, err := h.UserInfoService.GetSubUser(c.Request().Context(), c.Param("user-id"))
	return result.HttpResult(c, subUsers, err)
}

// SyncUser godoc
//
// @Summary Synchronizes user data with external IT services.
// @Description Synchronizes user data with external IT services (create and upsert user info).
// @Tags			user
// @Accept			json
// @Produce		json
// @Param customerCode query string true "Customer Code (must be 7 characters long)" example:"ABC1234"
// @Param syncType query string true "Sync Type" Enums(kyc,suitTest,address,tradingAccount,userInfo,all)
// @Success 200 {object} result.ResponseSuccess
// @Failure 400 {object} result.ResponseError "Invalid parameters"
// @Failure 500 {object} result.ResponseError "Sync error"
// @Router /internal/v1/users/sync [post]
func (h *UserInfoHandler) SyncUser(c echo.Context) error {
	var params dto.SyncUserInfoParams
	if err := c.QueryParams().Get("customerCode"); err == "" {
		return result.ParamErrorResult(c, errors.New("customerCode is required"))
	}
	if err := c.QueryParams().Get("syncType"); err == "" {
		return result.ParamErrorResult(c, errors.New("syncType is required"))
	}
	params.CustomerCode = c.QueryParams().Get("customerCode")
	params.SyncType = dto.SyncUserInfoType(c.QueryParams().Get("syncType"))

	if err := validator.New().Struct(params); err != nil {
		var validationErrors validator.ValidationErrors
		if errors.As(err, &validationErrors) {
			for _, e := range validationErrors {
				switch e.Field() {
				case "CustomerCode":
					return result.ParamErrorResult(c, errors.New("customerCode must be 7 characters long"))
				case "SyncType":
					return result.ParamErrorResult(c, errors.New("syncType must be one of: kyc, suitTest, address, tradingAccount, userInfo, all"))
				default:
					return result.ParamErrorResult(c, errors.New("invalid parameter: "+e.Field()))
				}
			}
		}
		return result.ParamErrorResult(c, err)
	}

	err := h.UserInfoService.SyncUserInfo(c.Request().Context(), params.CustomerCode, params.SyncType)
	if err != nil {
		h.Log.Error(err.Error())

		var customErr *errorx.ErrorMsg
		if errors.As(err, &customErr) {
			return result.ParamErrorResult(c, customErr)
		}

		return result.ParamErrorResult(c, err)
	}

	return result.HttpResult(c, nil, err)
}

// CreateUserInfo godoc
//
// @Summary Create user info with the given details.
// @Description Create user info with the given details.
// @Tags user
// @Accept json
// @Produce json
// @Param request body dto.CreateUserInfoRequest true "Create User Info Request"
// @Success 200 {object} result.ResponseSuccess{data=dto.CreateUserInfoResponse} "Create User Info"
// @Failure 400 {object} result.ResponseError "Validation failed"
// @Router /internal/v1/users [post]
func (h *UserInfoHandler) CreateUserInfo(c echo.Context) error {
	var req dto.CreateUserInfoRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := validator.New().Struct(req); err != nil {
		var validationErrors validator.ValidationErrors
		if errors.As(err, &validationErrors) {
			for _, e := range validationErrors {
				switch e.Field() {
				case "Email":
					return result.ParamErrorResult(c, errors.New("email must be a valid email address"))
				case "DateOfBirth":
					return result.ParamErrorResult(c, errors.New("date of birth must be in YYYY-MM-DD format"))
				default:
					return result.ParamErrorResult(c, errors.New("invalid parameter: "+e.Field()))
				}
			}
		}

		return result.ParamErrorResult(c, err)
	}

	res, err := h.UserInfoService.CreateUserInfo(c.Request().Context(), &req)
	return result.HttpResult(c, res, err)
}

// GetProfile godoc
// @Summary Get user profile
// @Description Get user profile
// @Tags user
// @Accept json
// @Produce json
// @Param user-id header string true "User ID"
// @Success 200 {object} result.ResponseSuccess{data=dto.ProfileInfo} "User Profile"
// @Failure 400 {object} result.ResponseError "Validation failed"
// @Router /internal/v1/users/profile [get]
func (h *UserInfoHandler) GetProfile(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	res, err := h.UserInfoService.GetProfile(c.Request().Context(), userId.String())

	if err != nil {
		h.Log.Error(err.Error())

		var customErr *errorx.ErrorMsg
		if errors.As(err, &customErr) {
			return result.ParamErrorResult(c, customErr)
		}

		return result.ParamErrorResult(c, err)
	}

	return result.HttpResult(c, res, err)
}
