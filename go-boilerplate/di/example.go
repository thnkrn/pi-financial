package di

import (
	"github.com/google/wire"

	handler "github.com/pi-financial/go-boilerplate/internal/handler/example"
	repository "github.com/pi-financial/go-boilerplate/internal/repository"
	service "github.com/pi-financial/go-boilerplate/internal/service"
)

var ExampleSet = wire.NewSet(
	repository.NewExampleRepository, service.NewExampleService, handler.NewExampleHandler,
)
