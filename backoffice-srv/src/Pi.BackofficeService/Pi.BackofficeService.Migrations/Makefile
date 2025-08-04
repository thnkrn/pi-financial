## Available DB (context)
# BackofficeDbContext
# TicketDbContext

## (output) example
# ErrorDb

## (name) example
# AddErrorTable

add:
	dotnet ef migrations add $(name) -s ../Pi.BackofficeService.Worker --context $(context) -o ./Migrations/$(output)

list:
	dotnet ef migrations list -s ../Pi.BackofficeService.Worker --context TicketDbContext
	dotnet ef migrations list -s ../Pi.BackofficeService.Worker --context BackofficeDbContext
	dotnet ef migrations list -s ../Pi.BackofficeService.Worker --context DataSeedingDbContext

add-ticket-db:
	dotnet ef migrations add $(name) -s ../Pi.BackofficeService.Worker --context TicketDbContext -o ./Migrations/TicketDb

add-bo-db:
	dotnet ef migrations add $(name) -s ../Pi.BackofficeService.Worker --context BackofficeDbContext -o ./Migrations/BackofficeDb

add-seed:
	dotnet ef migrations add $(name) -s ../Pi.BackofficeService.Worker --context DataSeedingDbContext -o ./Migrations/DataSeeding

remove:
	dotnet ef migrations remove -s ../Pi.BackofficeService.Worker --context $(context)

remove-bo-db:
	dotnet ef migrations remove -s ../Pi.BackofficeService.Worker --context BackofficeDbContext

update-db:
	dotnet ef database update -s ../Pi.BackofficeService.Worker --context TicketDbContext
	dotnet ef database update -s ../Pi.BackofficeService.Worker --context BackofficeDbContext
	dotnet ef database update -s ../Pi.BackofficeService.Worker --context DataSeedingDbContext

revert-db:
	dotnet ef database update 0 -s ../Pi.BackofficeService.Worker --context $(context)

script:
	dotnet ef migrations script -s ../Pi.BackofficeService.Worker --idempotent --context TicketDbContext -o migration_ticket.sql
	dotnet ef migrations script -s ../Pi.BackofficeService.Worker --idempotent --context BackofficeDbContext -o migration_backoffice.sql
	dotnet ef migrations script -s ../Pi.BackofficeService.Worker --idempotent --context DataSeedingDbContext -o migration_seeding.sql
