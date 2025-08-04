package api

import (
	"log"

	"github.com/pi-financial/information-srv/di"
	"github.com/pi-financial/information-srv/internal/adapters/config"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

func Run() {
	cfg, configErr := config.LoadConfig()
	if configErr != nil {
		log.Fatal("cannot load config: ", configErr)
	}
	if cfg.DatadogConfig.DDLogsEnabled {
		log.Printf("Datadog logs enabled")
		tracer.Start(tracer.WithEnv(cfg.DatadogConfig.DDEnv), tracer.WithService(cfg.DatadogConfig.DDService), tracer.WithServiceVersion(cfg.DatadogConfig.DDVersion), tracer.WithAgentAddr(cfg.DatadogConfig.DDAgentHost+":"+cfg.DatadogConfig.DDTraceAgentPort))
	}

	server, diErr := di.InitializeAPI(cfg)
	if diErr != nil {
		log.Fatal("cannot start server: ", diErr)
	} else {
		server.Start()
	}
}
