add:
	dotnet ef migrations add $(name) -s ../Pi.WalletService.Worker --context WalletDbContext -o ./Migrations/WalletDb

list:
	dotnet ef migrations list -s ../Pi.WalletService.Worker --context WalletDbContext

remove:
	dotnet ef migrations remove -s ../Pi.WalletService.Worker --context WalletDbContext

update-db:
	dotnet ef database update -s ../Pi.WalletService.Worker --context WalletDbContext

revert-db: lastGoodMigrationName ?= 0
revert-db:
	dotnet ef database update $(lastGoodMigrationName) -s ../Pi.WalletService.Worker --context WalletDbContext

script:
	dotnet ef migrations script -s ../Pi.WalletService.Worker --idempotent -o migration.sql --context WalletDbContext
