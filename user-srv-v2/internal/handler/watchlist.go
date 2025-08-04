package handler

import (
	"errors"
	"strings"

	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/errorx"
	"github.com/pi-financial/go-common/logger"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type WatchlistHandler struct {
	WatchlistService interfaces.WatchlistService
	Validator        *validator.Validate
	Log              logger.Logger
}

func NewWatchlistHandler(
	watchlistService interfaces.WatchlistService,
	log logger.Logger) *WatchlistHandler {
	return &WatchlistHandler{
		WatchlistService: watchlistService,
		Validator:        validator.New(),
		Log:              log,
	}
}

// CreateWatchlist godoc
//
//	@Summary		Create or delete watchlist item.
//	@Description	Create or delete watchlist item.
//	@Tags			watchlist
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string					true	"User ID"
//	@Param			request	body		dto.OptWatchlistRequest	true	"Watchlist operation request"
//	@Success		200		{object}	result.ResponseSuccess{data=dto.OptWatchlistResponse}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/secure/v1/watchlists [post]
func (h *WatchlistHandler) CreateWatchlist(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	var req dto.OptWatchlistRequest
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := h.Validator.Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	err = h.WatchlistService.CreateWatchlist(c.Request().Context(), userId.String(), &req)
	return result.HttpResult(c, nil, err)
}

// GetWatchlistByUserId godoc
//
//	@Summary		Get all watchlists for a user.
//	@Description	Get all watchlists for a user.
//	@Tags			watchlist
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string	true	"User ID"
//	@Param			venue	query		string	false	"Get watchlist request"
//	@Success		200		{object}	result.ResponseSuccess{data=[]dto.Watchlist}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/watchlists [get]
func (h *WatchlistHandler) GetWatchlistByUserId(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	var req dto.GetWatchlistParam
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	if err := h.Validator.Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	venue := strings.ToLower(req.Venue)

	watchlist, err := h.WatchlistService.GetWatchlistByUserId(c.Request().Context(), userId, venue)

	if err != nil {
		h.Log.Error(err.Error())

		var customErr *errorx.ErrorMsg
		if errors.As(err, &customErr) {
			err = customErr
		}
	}

	return result.HttpResult(c, watchlist, err)
}
