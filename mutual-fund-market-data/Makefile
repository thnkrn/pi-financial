SERVICE_NAME=Pi.FundMarketData
GIT_USER_ID=pi-financial
GIT_REPO_ID=mutual-fund-market-data
GOCMD=go

init:
	./init.sh

generate-client gen generate:
	rm -f -R ${PWD}/src/Pi.FundMarketData/Pi.Client.some-client-name/Api
	rm -f -R ${PWD}/src/Pi.FundMarketData/Pi.Client.some-client-name/Client
	rm -f -R ${PWD}/src/Pi.IdentityService/Pi.Client.some-client-name/Model
	rm -f -R ${PWD}/codegen/tmp/some-client-name
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp-netcore \
		-i /local/codegen/specs/some-client-name.yml \
		-o /local/codegen/tmp/some-client-name/ \
		-c /local/codegen/generator-config.json
	rm ${PWD}/codegen/tmp/sirius/Pi.Client.some-client-name/Pi.Client.some-client-name.csproj
	cp -r ${PWD}/codegen/tmp/sirius/Pi.Client.some-client-name ${PWD}/src/Pi.FundMarketData
	rm -f -R ${PWD}/codegen/tmp/some-client-name

build:
	dotnet build ${PWD}/src/Pi.FundMarketData/Pi.FundMarketData.sln

restore:
	dotnet restore

run:
	docker compose -f ${PWD}/src/Pi.FundMarketData/docker-compose.yml up

generate-swagger:
	rm -f ${PWD}/codegen/go-swagger.yaml
	dotnet new tool-manifest 2>/dev/null || true
	dotnet tool install Swashbuckle.AspNetCore.Cli --version 6.4.0
	dotnet build ${PWD}/src/$(SERVICE_NAME)/$(SERVICE_NAME).sln
	cd ./src/$(SERVICE_NAME)/$(SERVICE_NAME).API/ && \
		ASPNETCORE_ENVIRONMENT=Development \
			dotnet tool run swagger tofile \
				--yaml \
				--output ${PWD}/codegen/go-swagger.yaml \
				bin/Debug/net8.0/$(SERVICE_NAME).API.dll v1

# Run local and copy swagger json then paste in /codegen/swagger.json
gen-go-client: generate-swagger
	rm -f -R ${PWD}/go-client
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
			-g go \
			-i /local/codegen/go-swagger.yaml \
			-o /local/go-client \
			-c /local/codegen/go-client-generator-config.json \
			--git-user-id $(GIT_USER_ID) \
			--git-repo-id $(GIT_REPO_ID)/go-client
		cd go-client && $(GOCMD) mod tidy && $(GOCMD) fmt ./... && goimports -w .