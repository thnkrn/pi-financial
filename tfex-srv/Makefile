init:
	./init.sh
	
generate-client gen generate: 
	rm -f -R ${PWD}/src/Pi.TfexService/Pi.Client.some-client-name/Api
	rm -f -R ${PWD}/src/Pi.TfexService/Pi.Client.some-client-name/Client
	rm -f -R ${PWD}/src/Pi.IdentityService/Pi.Client.some-client-name/Model
	rm -f -R ${PWD}/codegen/tmp/some-client-name
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp-netcore \
		-i /local/codegen/specs/some-client-name.yml \
		-o /local/codegen/tmp/some-client-name/ \
		-c /local/codegen/generator-config.json
	rm ${PWD}/codegen/tmp/sirius/Pi.Client.some-client-name/Pi.Client.some-client-name.csproj
	cp -r ${PWD}/codegen/tmp/sirius/Pi.Client.some-client-name ${PWD}/src/Pi.TfexService
	rm -f -R ${PWD}/codegen/tmp/some-client-name

build:
	dotnet build ${PWD}/src/Pi.TfexService/Pi.TfexService.sln

restore:
	dotnet restore
	
run:
	docker compose -f ${PWD}/src/Pi.TfexService/docker-compose.yml up