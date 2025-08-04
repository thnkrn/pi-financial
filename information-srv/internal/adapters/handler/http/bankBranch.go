package http

import (
	"net/http"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/pkg"
)

type BankBranchHandler struct {
	srv ports.BankBranchService
}

func NewBankBranchHandler(srv ports.BankBranchService) *BankBranchHandler {
	return &BankBranchHandler{
		srv,
	}
}

// GetBanks godoc
// @Summary 		Get All BankBranches
// @Description 	Retrieve a complete list of all bank branches.
// @Tags 			BankBranch
// @Accept 			json
// @Produce 		json
// @Param bankCode query string false "BankCode"
// @Param bankBranchCode query string false "BankBranchCode"
// @Success 		200 {object} pkg.Response{data=[]bankBranch.BankBranch}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/bank-branch [get]
// @Router 			/secure/bank-branch [get]
func (ph *BankBranchHandler) GetBankBranches(ctx echo.Context) error {
	var req dto.GetBankBranchByFiltersRequest
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{})
	}

	bankBranches, err := ph.srv.GetBankBranches(ctx.Request().Context(), req)
	if err != nil {
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   bankBranches,
		Detail: "",
		Status: 200,
	})
}
