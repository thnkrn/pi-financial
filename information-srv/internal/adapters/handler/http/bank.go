package http

import (
	"net/http"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/pkg"
)

type BankHandler struct {
	srv ports.BankService
}

func NewBankHandler(srv ports.BankService) *BankHandler {
	return &BankHandler{
		srv,
	}
}

// GetBanks godoc
// @Summary 		Get All Banks
// @Description 	Retrieve a complete list of all banks.
// @Tags 			Bank
// @Accept 			json
// @Produce 		json
// @Param id query string false "Id"
// @Param shortName query string false "ShortName"
// @Param code query string false "Code"
// @Success 		200 {object} pkg.Response{data=[]bank.Bank}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/bank [get]
// @Router 			/secure/bank [get]
func (ph *BankHandler) GetBanks(ctx echo.Context) error {
	var req dto.GetBankByFiltersRequest
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{})
	}

	banks, err := ph.srv.GetBanks(ctx.Request().Context(), req)
	if err != nil {
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   banks,
		Detail: "",
		Status: 200,
	})
}
