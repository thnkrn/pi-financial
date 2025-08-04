init:
	./init.sh

gen-pi-internal:
	rm -f -R ${PWD}/src/Pi.SetService/Pi.Client.PiInternal/Api
	rm -f -R ${PWD}/src/Pi.SetService/Pi.Client.PiInternal/Client
	rm -f -R ${PWD}/src/Pi.SetService/Pi.Client.PiInternal/Model
	rm -f -R ${PWD}/codegen/tmp/PiInternal
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp \
		-i /local/codegen/specs/pi-internal.yml \
		-o /local/codegen/tmp/PiInternal/ \
		-c /local/codegen/pi-internal-generator-config.json
	rm ${PWD}/codegen/tmp/PiInternal/Pi.Client.PiInternal/Pi.Client.PiInternal.csproj
	cp -r ${PWD}/codegen/tmp/PiInternal/Pi.Client.PiInternal ${PWD}/src/Pi.SetService
	rm -f -R ${PWD}/codegen/tmp/PiInternal
	dotnet format ${PWD}/src/Pi.SetService/Pi.Client.PiInternal/Pi.Client.PiInternal.csproj

build:
	dotnet build ${PWD}/src/Pi.SetService/Pi.SetService.sln

restore:
	dotnet restore
	
run:
	docker compose -f ${PWD}/src/Pi.SetService/docker-compose.yml up
	
SERVICE_NAME=Pi.SetService
generate-swagger:
	rm -f ${PWD}/codegen/go-swagger.json
	dotnet new tool-manifest 2>/dev/null || true
	dotnet tool install Swashbuckle.AspNetCore.Cli --version 6.9.0
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
			--git-repo-id set-srv/go-client
		cd go-client && $(GOCMD) mod tidy && $(GOCMD) fmt ./... && goimports -w .