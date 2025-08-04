add:
	dotnet ef migrations add $(name) -s ../Pi.StructureNotes.Worker

remove:
	dotnet ef migrations remove -s ../Pi.StructureNotes.Worker

update-db:
	dotnet ef database update -s ../Pi.StructureNotes.Worker

revert-db:
	dotnet ef database update 0 -s ../Pi.StructureNotes.Worker

list:
	dotnet ef migrations list -s ../Pi.StructureNotes.Worker

script:
	dotnet ef migrations script -s ../Pi.StructureNotes.Worker --idempotent -o migration.sql