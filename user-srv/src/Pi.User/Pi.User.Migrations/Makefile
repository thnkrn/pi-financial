add:
	dotnet ef migrations add $(name) -s ../Pi.User.Worker --context UserDbContext

remove:
	dotnet ef migrations remove -s ../Pi.User.Worker --context UserDbContext

update-db:
	dotnet ef database update -s ../Pi.User.Worker --context UserDbContext

revert-db:
	dotnet ef database update 0 -s ../Pi.User.Worker --context UserDbContext
	
script:
	dotnet ef migrations script -s ../Pi.User.Worker --context UserDbContext --idempotent -o migration.sql

scaffold:
	dotnet ef dbcontext scaffold "Name=ConnectionStrings:BackofficeNDbContext" Microsoft.EntityFrameworkCore.SqlServer --table dbo.tcas -s ../Pi.User.Worker --context-dir ../Pi.User.Infrastructure --output-dir ../Pi.User/Domain/AggregatesModel/TradingAccountAggregate