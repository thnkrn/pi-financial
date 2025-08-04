package http

import (
	"log"

	"github.com/pi-financial/go-boilerplate/config"
	"github.com/pi-financial/go-boilerplate/di"
)

func Run() {
	cfg, configErr := config.LoadConfig()
	if configErr != nil {
		log.Fatal("cannot load config: ", configErr)
	}
	server, diErr := di.InitializeAPI(cfg)
	if diErr != nil {
		log.Fatal("cannot start server: ", diErr)
	} else {
		server.Start()
	}
}
