package main

import (
	"context"
	"fmt"
	"os"
	"os/signal"
	"syscall"

	_ "ariga.io/atlas-provider-gorm/gormschema"
	"github.com/pi-financial/onboard-srv-v2/cmd/api"
	"github.com/pi-financial/onboard-srv-v2/config"
	"github.com/pi-financial/onboard-srv-v2/di"
	_ "github.com/pi-financial/onboard-srv-v2/docs"

	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

// NewServerHTTP godoc
//
//	@title			Onboard Service V2
//	@version		1.0
//	@description	Onboard service V2
//	@termsOfService	http://swagger.io/terms/
//
//	@contact.name	API Support
//	@contact.url	http://www.swagger.io/support
//	@contact.email	support@swagger.io
//
//	@license.name	Apache 2.0
//	@license.url	http://www.apache.org/licenses/LICENSE-2.0.html
//
//	@host
//	@BasePath					/
//
//	@externalDocs.description	OpenAPI
//	@externalDocs.url			https://swagger.io/resources/open-api/
func main() {
	err := config.LoadConfig()
	if err != nil {
		panic(err)
	}
	ctx, cancel := context.WithCancel(context.Background())
	signal, stop := signal.NotifyContext(ctx, os.Interrupt, syscall.SIGTERM, syscall.SIGQUIT)
	defer stop()

	// Config Datadog
	if config.Get().Datadog.DDLogsEnabled {
		tracer.Start(
			tracer.WithEnv(config.Get().Datadog.DDEnv),
			tracer.WithService(config.Get().Datadog.DDService),
			tracer.WithServiceVersion(config.Get().Datadog.DDVersion),
			tracer.WithAgentAddr(config.Get().Datadog.DDAgentHost+":"+config.Get().Datadog.DDTraceAgentPort),
		)
	}

	// Dependencies Injection
	hs, l := di.InitializeApp(signal)
	defer func() {
		syncErr := l.Sync()
		if syncErr != nil {
			fmt.Printf("error syncing logger: %v", syncErr)
		}
	}()
	hs.RegisterOnShutdown(cancel)

	api.StartServer(hs, l)
}
