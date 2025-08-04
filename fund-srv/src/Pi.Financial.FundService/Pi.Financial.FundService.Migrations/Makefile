add:
	dotnet ef migrations add $(name) -s ../Pi.Financial.FundService.Worker --context FundDbContext

remove:
	dotnet ef migrations remove --context FundDbContext

update-db:
	dotnet ef database update -s ../Pi.Financial.FundService.Worker --context FundDbContext

revert-db:
	dotnet ef database update 0 --context FundDbContext

script:
	dotnet ef migrations script -s ../Pi.Financial.FundService.Worker --idempotent -o migration.sql --context FundDbContext