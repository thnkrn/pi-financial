package handler

import (
	"github.com/go-playground/validator/v10"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/go-common/result"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	serviceinterface "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type AddressHandler struct {
	AddressService serviceinterface.AddressService
	Validator      *validator.Validate
}

func NewAddressHandler(addressService serviceinterface.AddressService) *AddressHandler {
	return &AddressHandler{
		AddressService: addressService,
		Validator:      validator.New(),
	}
}

// GetAddressByUserId godoc
//
//	@Summary		Get address by user id or create if not exists
//	@Description	Get address by user id or create if not exists
//	@Tags			address
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string	true	"User ID"
//	@Success		200		{object}	result.ResponseSuccess{data=dto.Address}
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/address [get]
func (h *AddressHandler) GetAddressByUserId(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	address, err := h.AddressService.GetAddressByUserId(c.Request().Context(), userId.String())
	return result.HttpResult(c, address, err)
}

// CreateAddressByUserId godoc
//
//	@Summary		Create or update address by user id
//	@Description	Create or update address by user id
//	@Tags			address
//	@Accept			json
//	@Produce		json
//	@Param			user-id	header		string		true	"User ID"
//	@Param			request	body		dto.Address	true	"Address request"
//	@Success		200		{object}	result.ResponseSuccess
//	@Failure		400		{object}	result.ResponseError
//	@Failure		500		{object}	result.ResponseError
//	@Router			/internal/v1/address [post]
func (h *AddressHandler) CreateAddressByUserId(c echo.Context) error {
	userId, err := utils.GetUserIdFromHeader(c)
	if err != nil {
		return result.ParamErrorResult(c, err)
	}

	var req dto.Address
	if err := c.Bind(&req); err != nil {
		return result.ParamErrorResult(c, err)
	}
	if err := h.Validator.Struct(req); err != nil {
		return result.ParamErrorResult(c, err)
	}

	err = h.AddressService.UpsertAddress(c.Request().Context(), userId.String(), &req)
	return result.HttpResult(c, nil, err)
}
