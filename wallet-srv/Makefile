init:
	./init.sh
	
gen-exante-user-management:
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.ExanteUserManagement/Api
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.ExanteUserManagement/Client
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.ExanteUserManagement/Model
	rm -f -R ${PWD}/codegen/tmp/ExanteUserManagement
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp-netcore \
		-i /local/codegen/specs/exante-user-management.yml \
		-o /local/codegen/tmp/ExanteUserManagement/ \
		-c /local/codegen/exante-user-management-generator-config.json
	rm ${PWD}/codegen/tmp/ExanteUserManagement/Pi.Client.ExanteUserManagement/Pi.Client.ExanteUserManagement.csproj
	cp -r ${PWD}/codegen/tmp/ExanteUserManagement/Pi.Client.ExanteUserManagement ${PWD}/src/Pi.WalletService
	rm -f -R ${PWD}/codegen/tmp/ExanteUserManagement
	dotnet format ${PWD}/src/Pi.WalletService/Pi.Client.ExanteUserManagement/Pi.Client.ExanteUserManagement.csproj

gen-exante-trade: 
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.ExanteTrade/Api
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.ExanteTrade/Client
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.ExanteTrade/Model
	rm -f -R ${PWD}/codegen/tmp/ExanteTrade
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp \
		-i /local/codegen/specs/exante-trade.yml \
		-o /local/codegen/tmp/ExanteTrade/ \
		-c /local/codegen/exante-trade-generator-config.json
	rm ${PWD}/codegen/tmp/ExanteTrade/Pi.Client.ExanteTrade/Pi.Client.ExanteTrade.csproj
	cp -r ${PWD}/codegen/tmp/ExanteTrade/Pi.Client.ExanteTrade ${PWD}/src/Pi.WalletService
	rm -f -R ${PWD}/codegen/tmp/ExanteTrade
	dotnet format ${PWD}/src/Pi.WalletService/Pi.Client.ExanteTrade/Pi.Client.ExanteTrade.csproj

gen-cgs-bank:
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.CgsBank/Api
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.CgsBank/Client
	rm -f -R ${PWD}/src/Pi.WalletService/Pi.Client.CgsBank/Model
	rm -f -R ${PWD}/codegen/tmp/CgsBank
	docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
		-g csharp-netcore \
		-i /local/codegen/specs/cgs-bank.yml \
		-o /local/codegen/tmp/CgsBank/ \
		-c /local/codegen/cgs-bank-generator-config.json
	rm ${PWD}/codegen/tmp/CgsBank/Pi.Client.CgsBank/Pi.Client.CgsBank.csproj
	cp -r ${PWD}/codegen/tmp/CgsBank/Pi.Client.CgsBank ${PWD}/src/Pi.WalletService
	rm -f -R ${PWD}/codegen/tmp/CgsBank
	dotnet format ${PWD}/src/Pi.WalletService/Pi.Client.CgsBank/Pi.Client.CgsBank.csproj

build:
	dotnet build ${PWD}/src/Pi.WalletService/Pi.WalletService.sln

restore:
	dotnet restore
	
run:
	docker compose -f ${PWD}/src/Pi.WalletService/docker-compose.yml up