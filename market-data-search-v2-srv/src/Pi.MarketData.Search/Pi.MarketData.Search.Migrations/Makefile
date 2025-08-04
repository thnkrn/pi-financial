add:
	dotnet ef migrations add $(name) -s ../Pi.Market.Data.Search.Worker

remove:
	dotnet ef migrations remove -s ../Pi.Market.Data.Search.Worker

update-db:
	dotnet ef database update -s ../Pi.Market.Data.Search.Worker

revert-db:
	dotnet ef database update 0 -s ../Pi.Market.Data.Search.Worker

script:
	dotnet ef migrations script -s ../Pi.Market.Data.Search.Worker --idempotent -o migration.sql