package main

import (
	"log"

	_ "ariga.io/atlas-provider-gorm/gormschema"
	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/di"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

// NewServerHTTP godoc
// @title           Pi SSO V2
// @version         2.0
// @description     Contain Generic Information.
// @termsOfService  http://swagger.io/terms/
//
// @contact.name   API Support
// @contact.url    http://www.swagger.io/support
// @contact.email  pongtorn.po@pi.financial
//
// @license.name  Apache 2.0
// @license.url   http://www.apache.org/licenses/LICENSE-2.0.html
//
// @host      sso.identity.nonprod.pi.internal
// @BasePath  /
//
// @securityDefinitions.apikey BearerAuth
// @in header
// @name Authorization
// @description use a Bearer token for authorization. Format: "Bearer {token}"
//
// @externalDocs.description  OpenAPI
// @externalDocs.url          https://swagger.io/resources/open-api/
func main() {
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
