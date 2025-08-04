add:
	dotnet ef migrations add $(name) -s ../Pi.SetService.Worker --context=SetDbContext

add-number:
	dotnet ef migrations add $(name) -s ../Pi.SetService.Worker --context=NumberGeneratorDbContext

remove:
	dotnet ef migrations remove -s ../Pi.SetService.Worker --context=SetDbContext

update-db:
	dotnet ef database update -s ../Pi.SetService.Worker --context=SetDbContext
	dotnet ef database update -s ../Pi.SetService.Worker --context=NumberGeneratorDbContext

revert-db:
	dotnet ef database update 0 -s ../Pi.SetService.Worker --context=SetDbContext

script:
	dotnet ef migrations script -s ../Pi.SetService.Worker  --context=SetDbContext --idempotent -o migration.sql
	dotnet ef migrations script -s ../Pi.SetService.Worker  --context=NumberGeneratorDbContext --idempotent -o migration_number.sql
