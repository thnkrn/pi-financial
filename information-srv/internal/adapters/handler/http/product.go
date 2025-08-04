package http

import (
	"net/http"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/pkg"
)

type ProductHandler struct {
	srv ports.ProductService
}

func NewProductHandler(srv ports.ProductService) *ProductHandler {
	return &ProductHandler{
		srv,
	}
}

// GetProducts godoc
// @Summary 		Get All Products
// @Description 	Retrieve a complete list of all products.
// @Tags 			Product
// @Accept 			json
// @Produce 		json
// @Param id query string false "Id"
// @Param name query string false "Name"
// @Param accountTypeCode query string false "AccountTypeCode"
// @Param accountType query string false "AccountType"
// @Param exchangeMarketId query string false "ExchangeMarketId"
// @Param suffix query string false "Suffix"
// @Param transactionType query string false "TransactionType"
// @Success 		200 {object} pkg.Response{data=[]product.Product}
// @Failure 		400 {object} pkg.Response
// @Failure 		500 {object} pkg.Response
// @Router 			/internal/product [get]
// @Router 			/secure/product [get]
func (ph *ProductHandler) GetProducts(ctx echo.Context) error {
	var req dto.GetProductByFiltersRequest
	if err := ctx.Bind(&req); err != nil {
		return ctx.JSON(http.StatusBadRequest, pkg.Response{})
	}

	products, err := ph.srv.GetProducts(ctx.Request().Context(), req)
	if err != nil {
		return echo.NewHTTPError(http.StatusInternalServerError, err.Error())
	}

	return ctx.JSON(http.StatusOK, pkg.Response{
		Title:  "Ok",
		Data:   products,
		Detail: "",
		Status: 200,
	})
}
