init:
	./init.sh
	
generate-client gen generate: 
	rm -f -R ${PWD}/src/Pi.GloablEquities/Pi.Client.some-client-name/Api
	rm -f -R ${PWD}/src/Pi.GloablEquities/Pi.Client.some-client-name/Client
	rm -f -R ${PWD}/src/Pi.IdentityService/Pi.Client.some-client-name/Model
	rm -f -R ${PWD}/codegen/tmp/some-client-name
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp-netcore \
		-i /local/codegen/specs/some-client-name.yml \
		-o /local/codegen/tmp/some-client-name/ \
		-c /local/codegen/generator-config.json
	rm ${PWD}/codegen/tmp/sirius/Pi.Client.some-client-name/Pi.Client.some-client-name.csproj
	cp -r ${PWD}/codegen/tmp/sirius/Pi.Client.some-client-name ${PWD}/src/Pi.GloablEquities
	rm -f -R ${PWD}/codegen/tmp/some-client-name

build:
	dotnet build ${PWD}/src/Pi.GloablEquities/Pi.GloablEquities.sln

restore:
	dotnet restore
	
run:
	docker compose -f ${PWD}/src/Pi.GloablEquities/docker-compose.yml up

SERVICE_NAME=Pi.GlobalEquities
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
gen-go-client:
	rm -f -R ${PWD}/go-client
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
			-g go \
			-i /local/codegen/go-swagger.json \
			-o /local/go-client \
			-c /local/codegen/go-client-generator-config.json \
			--git-user-id pi-financial \
			--git-repo-id global-equities/go-client
		cd go-client && $(GOCMD) mod tidy && $(GOCMD) fmt ./... && goimports -w .