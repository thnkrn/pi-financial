package handler

import (
	"net/http"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	"github.com/pi-financial/onboard-srv-v2/internal/errmsg"
	"github.com/pi-financial/onboard-srv-v2/internal/handler/dto"
)

type MetaTraderHandler struct {
	l  port.Logger
	ms port.MetaTraderService
}

func NewMetaTraderHandler(l port.Logger, ls port.MetaTraderService) *MetaTraderHandler {
	return &MetaTraderHandler{l, ls}
}

// Register MT4 MT5 registers both MT4 and MT5 trading accounts
//
//	@Summary		Register MT4 and MT5 Trading Accounts
//	@Description	Registers MT4 and MT5 trading accounts based on the provided customer codes and effective dates
//	@ID				RegisterMetaTrader
//	@Tags			MetaTrader
//	@Accept			json
//	@Produce		json
//	@Param			lang	header		string							true	"Locale: en_US or th_TH"
//	@Param			data	body		dto.RegisterMetaTraderRequest	true	"MT4 and MT5 registration data"
//	@Success		200		{object}	dto.BaseResponse				"Success response status"
//	@Failure		400		{object}	dto.BaseResponse				"Invalid input data"
//	@Failure		500		{object}	dto.BaseResponse				"Internal server error"
//	@Router			/secure/meta-trader [post]
func (h *MetaTraderHandler) RegisterMetaTrader(c echo.Context) error {
	ctx := c.Request().Context()

	// Default language is th_TH
	locale := dto.LanguageTH
	if dto.Locale(c.Request().Header.Get("lang")) == dto.LanguageEN {
		locale = dto.LanguageEN
	}

	var request dto.RegisterMetaTraderRequest
	if err := c.Bind(&request); err != nil {
		return err
	}

	if err := c.Validate(&request); err != nil {
		return err
	}

	data := make([]domain.CreateMetaTraderRequest, len(request.Data))
	for i, v := range request.Data {
		data[i] = domain.CreateMetaTraderRequest{
			TradingAccount: v.TradingAccount,
			EffectiveDate:  v.EffectiveDate,
			Platform:       v.Platform,
		}
	}

	if err := h.ms.CreateMetaTrader(ctx, data); err != nil {
		h.l.Error(err.Error())
		return err
	}

	if err := h.ms.SendMetaTraderCreatedNotificationEmail(ctx, data, locale); err != nil {
		h.l.Error(err.Error())
		return err
	}

	return c.JSON(http.StatusOK, dto.BaseResponse{
		Code: 0,
	})
}

// GetMetaTraders handles the request to get MT4 and MT5 records by date filter
//
//	@Summary		Get MT4 and MT5 trading accounts records
//	@Description	Retrieves a list of MT4 and MT5 trading accounts filtered by effective date range
//	@ID				GetMetaTraders
//	@Tags			MetaTrader
//	@Accept			json
//	@Produce		json
//	@Param			start_date	query		string											true	"Start date in YYYYMMDD format"
//	@Param			end_date	query		string											true	"End date in YYYYMMDD format"
//	@Param			is_exported	query		bool											false	"export status filter"
//	@Success		200			{object}	dto.BaseResponseWithData[MetaTraderResponseDto]	"Success response with MT4 and MT5 records"
//	@Failure		400			{object}	dto.BaseResponse								"Invalid date range or query parameters"
//	@Failure		500			{object}	dto.BaseResponse								"Internal server error"
//	@Router			/internal/meta-trader [get]
func (h *MetaTraderHandler) GetMetaTraders(c echo.Context) error {
	ctx := c.Request().Context()

	var query dto.GetMetaTraderFilter
	if err := c.Bind(&query); err != nil {
		return err
	}

	if err := c.Validate(&query); err != nil {
		return err
	}

	if query.StartDate > query.EndDate {
		return errmsg.MetaTraderInvalidDateFilter
	}

	mt4Data, mt5Data, err := h.ms.GetMetaTrader(ctx, &domain.GetMetaTraderFilter{
		StartDate:  query.StartDate,
		EndDate:    query.EndDate,
		IsExported: query.IsExported,
	})
	if err != nil {
		return err
	}

	response := &dto.MetaTraderResponseDto{}
	for _, v := range mt4Data {
		response.MT4 = append(response.MT4, dto.MT4Dto{
			TradingAccount: v.TradingAccount,
			EffectiveDate:  v.EffectiveDate,
			ServiceType:    v.ServiceType,
			PackageType:    v.PackageType,
			IsEnable:       "Y",
			IsExported:     v.IsExported,
		})
	}

	for _, v := range mt5Data {
		response.MT5 = append(response.MT5, dto.MT5Dto{
			TradingAccount: v.TradingAccount,
			EffectiveDate:  v.EffectiveDate,
			ServiceType:    v.ServiceType,
			PackageType:    v.PackageType,
			IsEnable:       "Y",
			IsExported:     v.IsExported,
		})
	}

	return c.JSON(http.StatusOK, dto.BaseResponseWithData[dto.MetaTraderResponseDto]{
		Data: *response,
		BaseResponse: dto.BaseResponse{
			Code: 0,
		},
	})
}

// UpdateExported updates MT4 and MT5 records as exported
//
//	@Summary		Update MT4 and MT5 Trading Accounts as exported
//	@Description	Update MT4 and MT5 trading accounts as exported based on the provided trading account numbers
//	@ID				UpdateExported
//	@Tags			MetaTrader
//	@Accept			json
//	@Produce		json
//	@Param			data	body		dto.UpdateMetaTraderRequest	true	"MT4 and MT5 trading accounts to update as exported"
//	@Success		200		{object}	dto.BaseResponse			"Success response indicating records are updated as exported"
//	@Failure		400		{object}	dto.BaseResponse			"Invalid input data"
//	@Failure		500		{object}	dto.BaseResponse			"Internal server error"
//	@Router			/internal/meta-trader/exported [put]
func (h *MetaTraderHandler) UpdateExported(c echo.Context) error {
	ctx := c.Request().Context()

	var request dto.UpdateMetaTraderRequest
	if err := c.Bind(&request); err != nil {
		return err
	}

	if err := c.Validate(&request); err != nil {
		return err
	}

	if err := h.ms.UpdateMetaTrader(ctx, &domain.UpdateMetaTraderRequest{
		TradingAccounts: request.TradingAccounts,
		Platform:        request.Platform,
	}); err != nil {
		return err
	}

	return c.JSON(http.StatusOK, dto.BaseResponse{
		Code: 0,
	})
}
