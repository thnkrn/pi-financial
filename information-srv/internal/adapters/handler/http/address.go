package http

import (
	"net/http"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/pkg"
)

type AddressHandler struct {
	srv ports.AddressService
}

func NewAddressHandler(srv ports.AddressService) *AddressHandler {
	return &AddressHandler{
		srv,
	}
}

// GetProvinces godoc
// @Summary 		Get All Provinces
// @Description 	Retrieve a complete list of all provinces.
// @Tags 			Address
// @Accept 			json
// @Produce 		json
// @Success 		200 {object} pkg.Response{data=[]address.Province}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/address/province [get]
// @Router 			/secure/address/province [get]
func (ah *AddressHandler) GetProvinces(ctx echo.Context) error {
	provinces, err := ah.srv.GetProvinces(ctx.Request().Context())
	if err != nil {
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   provinces,
		Detail: "",
		Status: 200,
	})
}

// GetAddresses godoc
// @Summary 		Get All Addresses
// @Description 	Retrieve a complete list of all available addresses.
// @Tags 			Address
// @Accept 			json
// @Produce 		json
// @Success 		200 {object} pkg.Response{data=[]address.Address}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/address [get]
// @Router 			/secure/address [get]
func (ah *AddressHandler) GetAddresses(ctx echo.Context) error {
	addresses, err := ah.srv.GetAddresses(ctx.Request().Context())
	if err != nil {
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   addresses,
		Detail: "",
		Status: 200,
	})
}

type getAddressesByZipCodeReq struct {
	ZipCode int `param:"zipCode"`
}

// GetAddressesByZipCode godoc
// @Summary 		Get Addresses by Zip Code
// @Description 	Retrieve a list of addresses associated with a specified Zip Code.
// @Tags 			Address
// @Accept 			json
// @Produce 		json
// @Param 			zipCode		path	int		true	"The Zip Code to retrieve addresses"
// @Success 		200 {object} pkg.Response{data=[]address.Address}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/address/zip-code/{zipCode} [get]
// @Router 			/secure/address/zip-code/{zipCode} [get]
func (ah *AddressHandler) GetAddressesByZipCode(ctx echo.Context) error {
	req := getAddressesByZipCodeReq{}
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{})
	}

	addresses, err := ah.srv.GetAddressesByZipCode(ctx.Request().Context(), req.ZipCode)
	if err != nil {
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   addresses,
		Detail: "",
		Status: 200,
	})
}

type getAddressesByProvinceReq struct {
	Province string `param:"province"`
	Lang     string `query:"lang"`
}

// GetAddressesByProvince godoc
// @Summary 		Get Addresses by Province
// @Description 	Retrieve a list of addresses associated with a specified Province.
// @Tags 			Address
// @Accept 			json
// @Produce 		json
// @Param			province	path	string	true	"The Province to retrieve addresses"
// @Param			lang		query	string	false	"Language preference for response data. Omit for Thai (default) or specify 'en' for English"
// @Success 		200 {object} pkg.Response{data=[]address.Address}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/address/province/{province} [get]
// @Router 			/secure/address/province/{province} [get]
func (handler *AddressHandler) GetAddressesByProvince(ctx echo.Context) error {
	req := getAddressesByProvinceReq{}
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{})
	}

	addresses, err := handler.srv.GetAddressesByProvince(ctx.Request().Context(), req.Province, req.Lang)
	if err != nil {
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   addresses,
		Detail: "",
		Status: 200,
	})
}
