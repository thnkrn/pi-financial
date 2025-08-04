add:
	dotnet ef migrations add $(name) -s ../Pi.TfexService.Worker --context TfexDbContext -o ./Migrations/TfexDb

list:
	dotnet ef migrations list -s ../Pi.TfexService.Worker --context TfexDbContext

remove:
	dotnet ef migrations remove -s ../Pi.TfexService.Worker --context TfexDbContext

update-db:
	dotnet ef database update -s ../Pi.TfexService.Worker --context TfexDbContext

revert-db: lastGoodMigrationName ?= 0
revert-db:
	dotnet ef database update $(lastGoodMigrationName) -s ../Pi.TfexService.Worker --context TfexDbContext

script:
	dotnet ef migrations script -s ../Pi.TfexService.Worker --idempotent -o migration.sql --context TfexDbContext