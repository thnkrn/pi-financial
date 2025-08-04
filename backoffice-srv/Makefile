init:
	./init.sh
	
gen-wallet-service: 
	rm -f -R ${PWD}/src/Pi.BackofficeService/Pi.Client.WalletService/Api
	rm -f -R ${PWD}/src/Pi.BackofficeService/Pi.Client.WalletService/Client
	rm -f -R ${PWD}/src/Pi.IdentityService/Pi.Client.WalletService/Model
	rm -f -R ${PWD}/codegen/tmp/WalletService
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp-netcore \
		-i /local/codegen/specs/wallet-service.yaml \
		-o /local/codegen/tmp/WalletService/ \
		-c /local/codegen/wallet-service-generator-config.json
	rm ${PWD}/codegen/tmp/WalletService/Pi.Client.WalletService/Pi.Client.WalletService.csproj
	cp -r ${PWD}/codegen/tmp/WalletService/Pi.Client.WalletService ${PWD}/src/Pi.BackofficeService
	rm -f -R ${PWD}/codegen/tmp/WalletService

build:
	dotnet build ${PWD}/src/Pi.BackofficeService/Pi.BackofficeService.sln

restore:
	dotnet restore
	
run:
	docker compose -f ${PWD}/src/Pi.BackofficeService/docker-compose.yml up
format:
	dotnet format --verify-no-changes src/Pi.BackofficeService/Pi.BackofficeService.sln
