package http

import (
	"log"

	"github.com/pi-financial/user-srv-v2/config"
	"github.com/pi-financial/user-srv-v2/di"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

func Run() {
	cfg, err := config.LoadConfig()
	if err != nil {
		log.Fatal("cannot load config: ", err)
	}

	tracer.Start(tracer.WithEnv(cfg.DDEnv), tracer.WithService(cfg.DDService), tracer.WithServiceVersion(cfg.DDVersion), tracer.WithAgentAddr(cfg.DDAgentHost+":"+cfg.DDTraceAgentPort))

	server, err := di.InitializeAPI(cfg)
	if err != nil {
		log.Fatal("cannot start server: ", err)
	} else {
		server.Start()
	}
}
