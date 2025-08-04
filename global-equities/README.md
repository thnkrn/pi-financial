# .NET Service

## Setup
* ./init.sh
* Put your service name `MyService`

## Environment Variables
| ENV  | Possible Values | Description  |
|---|---|---|
| ASPNETCORE_ENVIRONMENT | "Development", "Staging", "Production" - default | For API project
| DOTNET_ENVIRONMENT | "Development", "Staging", "Production" - default | For Worker project
| USE_REMOTE_CONFIG | "true", "false" - default | To use remote config from AWS Parameter Store
| REMOTE_CONFIG_LIFETIME_MS | "any number", "1000" - default | Reload remote config after x ms
