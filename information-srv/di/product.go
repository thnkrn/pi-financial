package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http"
	"github.com/pi-financial/information-srv/internal/adapters/repositories"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/core/services"
)

var ProductSet = wire.NewSet(
	wire.Bind(new(ports.ProductService), new(*services.ProductService)),
	services.NewProductService,

	wire.Bind(new(ports.ProductRepository), new(*repositories.ProductRepository)),
	repositories.NewProductRepository,

	http.NewProductHandler,
)
