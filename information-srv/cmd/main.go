package main

import (
	"github.com/pi-financial/information-srv/cmd/api"
)

// NewServerHTTP godoc
// @title           Information API
// @version         1.0
// @description     Contain Generic Information.
// @termsOfService  http://swagger.io/terms/
//
// @contact.name   API Support
// @contact.url    http://www.swagger.io/support
// @contact.email  support@swagger.io
//
// @license.name  Apache 2.0
// @license.url   http://www.apache.org/licenses/LICENSE-2.0.html
//
// @host
// @BasePath  /
//
// @externalDocs.description  OpenAPI
// @externalDocs.url          https://swagger.io/resources/open-api/
func main() {
	api.Run()
}
