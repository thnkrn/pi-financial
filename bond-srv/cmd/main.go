package main

import (
	"context"
	"log"
	"os"
	"os/signal"
	"syscall"
	"time"

	_ "ariga.io/atlas-provider-gorm/gormschema"
	"github.com/pi-financial/bond-srv/cmd/api"
	"github.com/pi-financial/bond-srv/config"
	"github.com/pi-financial/bond-srv/di"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

var httpApi *api.ServerHTTP

func main() {

	signalCtx, signalCtxStop, cancel := listenStopSignal()
	defer signalCtxStop()

	httpApi = newHTTPApi(cancel)

	go httpApi.Start()

	<-signalCtx.Done()

	shutdownGracefully()
}

func newHTTPApi(cf context.CancelFunc) *api.ServerHTTP {
	ddCfg, ddcfgErr := config.LoadDatadogConfig()
	if ddcfgErr != nil {
		log.Fatal("cannot load datadog config:", ddcfgErr)
	}

	addDatadogTracer(ddCfg)

	cfg, configErr := config.LoadConfig()
	if configErr != nil {
		log.Fatal("cannot load config: ", configErr)
	}

	server, diErr := di.InitializeAPI(cfg)
	if diErr != nil {
		log.Fatal("cannot create server: ", diErr)
	}

	server.App.Server.RegisterOnShutdown(cf)

	return server
}

func addDatadogTracer(cfg config.DatadogConfig) {
	tracer.Start(
		tracer.WithEnv(cfg.DDEnv),
		tracer.WithService(cfg.DDService),
		tracer.WithServiceVersion(cfg.DDVersion),
		tracer.WithAgentAddr(cfg.DDAgentHost+":"+cfg.DDTraceAgentPort),
	)
}

func listenStopSignal() (context.Context, context.CancelFunc, context.CancelFunc) {
	ctx, cancel := context.WithCancel(context.Background())

	signalCtx, signalCtxStop := signal.NotifyContext(ctx,
		os.Interrupt, // interrupt = SIGINT = Ctrl+C
		syscall.SIGQUIT,
		syscall.SIGTERM,
	)

	return signalCtx, signalCtxStop, cancel
}

func shutdownGracefully() {
	shutdownCtx, shutdownRelease := context.WithTimeout(context.Background(), 10*time.Second)
	defer shutdownRelease()

	httpApi.Shutdown(shutdownCtx)

	if err := shutdownCtx.Err(); err != nil {
		log.Fatalln("Error when shutdown: ", err)
	} else {
		log.Println("Gracefully shutdown the server")
	}
}
