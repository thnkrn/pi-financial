package main

import (
	_ "ariga.io/atlas-provider-gorm/gormschema"
	"github.com/pi-financial/go-boilerplate/cmd/http"
)

func main() {
	http.Run()
}

// @title Swagger Example API
// @version 0.0.1
// @description This is a sample server Petstore server.
// @securityDefinitions.apikey ApiKeyAuth
// @in header
// @name Authorization
// @host localhost:8080
// @BasePath /
