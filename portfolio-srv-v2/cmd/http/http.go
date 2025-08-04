package http

import (
	"log"

	"github.com/pi-financial/portfolio-srv-v2/config"
	"github.com/pi-financial/portfolio-srv-v2/di"
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
