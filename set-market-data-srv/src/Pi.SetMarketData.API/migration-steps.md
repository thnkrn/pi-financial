# EFCore migration steps
### Verify that the C#.NET runtime is installed correctly by running the following command:
`dotnet --version`

### Install the dotnet-ef tool globally by running the following command:
`dotnet tool install --global dotnet-ef`

### To change the migration assembly to match your target project, update your DbContext configuration in the ServiceRegistrationExtensions.cs file of the Pi.SetMarketData.API project:
`services.AddDbContext<TimescaleContext>(options => options.UseNpgsql(configuration.GetConnectionString("TimescaleDb"), b => b.MigrationsAssembly("Pi.SetMarketData.API")));`

### Open Terminal and cd to `../Pi.SetMarketData.API`

### After updating the migration assembly, try running the migration command again from the Pi.SetMarketData.API project directory:
`dotnet ef migrations add InitialCreate` and `dotnet ef database update`

## Optional
### Or check the list of migrations to identify the previous migration:
`dotnet ef migrations list`

### And replace PreviousMigrationName with the name of the migration before, such as `20240517065233_InitialCreate`:
`dotnet ef database update PreviousMigrationName`

### After creating the migration, you can apply it to the database using the following command:
`dotnet ef database update`

### To remove the migrations, you can use the following command:
`dotnet ef migrations remove`