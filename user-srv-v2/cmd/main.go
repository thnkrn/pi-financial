package main

import (
	_ "ariga.io/atlas-provider-gorm/gormschema"
	"github.com/pi-financial/user-srv-v2/cmd/http"
)

// NewServerHTTP godoc
//
//	@title						Pi User Service V2
//	@version					1.0
//	@description				Contain Generic Information.
//	@termsOfService				http://swagger.io/terms/
//
//	@contact.name				API Support
//	@contact.url				http://www.swagger.io/support
//
//	@license.name				Apache 2.0
//	@license.url				http://www.apache.org/licenses/LICENSE-2.0.html
func main() {
	http.Run()
}
