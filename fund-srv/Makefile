generate-client-fc gen-fc generate-fc:
	rm -f -R ${PWD}/src/Pi.Financial.FundService/Pi.Financial.Client.FundConnext/Api
	rm -f -R ${PWD}/src/Pi.Financial.FundService/Pi.Financial.Client.FundConnext/Client
	rm -f -R ${PWD}/src/Pi.Financial.FundService/Pi.Financial.Client.FundConnext/Model
	rm -f -R ${PWD}/codegen/tmp/fund-connext
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp \
		-i /local/codegen/specs/fund-connext.yml \
		-o /local/codegen/tmp/fund-connext/ \
		-c /local/codegen/generator-config.json
	rm ${PWD}/codegen/tmp/fund-connext/Pi.Financial.Client.FundConnext/Pi.Financial.Client.FundConnext.csproj
	cp -r ${PWD}/codegen/tmp/fund-connext/Pi.Financial.Client.FundConnext ${PWD}/src/Pi.Financial.FundService
	rm -f -R ${PWD}/codegen/tmp/fund-connext
	dotnet format ${PWD}/src/Pi.Financial.FundService/Pi.Financial.Client.FundConnext/Pi.Financial.Client.FundConnext.csproj

generate-client-it-bo gen-it-bo generate-it-bo:
	rm -f -R ${PWD}/src/Pi.Financial.FundService/Pi.Financial.Client.PiItBackoffice/Api
	rm -f -R ${PWD}/src/Pi.Financial.FundService/Pi.Financial.Client.PiItBackoffice/Client
	rm -f -R ${PWD}/src/Pi.Financial.FundService/Pi.Financial.Client.PiItBackoffice/Model
	rm -f -R ${PWD}/codegen/tmp/pi-it-backoffice-api
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp \
		-i /local/codegen/specs/pi-it-backoffice-api.yml \
		-o /local/codegen/tmp/pi-it-backoffice-api/ \
		-c /local/codegen/pi-internal-generator-config.json
	rm ${PWD}/codegen/tmp/pi-it-backoffice-api/Pi.Financial.Client.PiItBackoffice/Pi.Financial.Client.PiItBackoffice.csproj
	cp -r ${PWD}/codegen/tmp/pi-it-backoffice-api/Pi.Financial.Client.PiItBackoffice ${PWD}/src/Pi.Financial.FundService
	rm -f -R ${PWD}/codegen/tmp/pi-it-backoffice-api
	dotnet format ${PWD}/src/Pi.Financial.FundService/Pi.Financial.Client.PiItBackoffice/Pi.Financial.Client.PiItBackoffice.csproj

build:
	dotnet build ${PWD}/src/Pi.Financial.FundService/Pi.Financial.FundService.sln

restore:
	dotnet restore

run:
	docker compose -f ${PWD}/src/Pi.Financial.FundService/docker-compose.yml up

test:
	dotnet test ${PWD}/src/Pi.Financial.FundService/Pi.Financial.FundService.sln --configuration Release --no-restore --verbosity normal

lint:
	dotnet format ${PWD}/src/Pi.Financial.FundService/Pi.Financial.FundService.sln

SERVICE_NAME=Pi.Financial.FundService
generate-swagger:
	rm -f ${PWD}/codegen/go-swagger.json
	dotnet new tool-manifest 2>/dev/null || true
	dotnet tool install Swashbuckle.AspNetCore.Cli --version 6.4.0
	dotnet build ${PWD}/src/$(SERVICE_NAME)/$(SERVICE_NAME).sln
	cd ./src/$(SERVICE_NAME)/$(SERVICE_NAME).API/ && \
		ASPNETCORE_ENVIRONMENT=Development \
			dotnet tool run swagger tofile \
				--output ${PWD}/codegen/go-swagger.json \
				bin/Debug/net8.0/$(SERVICE_NAME).API.dll v1

GOCMD=go
# Run local and copy swagger json then paste in /codegen/swagger.json
gen-go-client: generate-swagger
	rm -f -R ${PWD}/go-client
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
			-g go \
			-i /local/codegen/go-swagger.json \
			-o /local/go-client \
			-c /local/codegen/go-client-generator-config.json \
			--git-user-id pi-financial \
			--git-repo-id fund-srv/go-client
		cd go-client && $(GOCMD) mod tidy && $(GOCMD) fmt ./... && goimports -w .