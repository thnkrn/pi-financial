#!/bin/sh
read -p "Enter Project Name: " projName
read -p "Enter Db Name: " dbName

find ./src -type f | xargs sed -i ".bak" "s|service-name|${projName}|g"
find ./src -name "*.bak" | xargs rm 
find ./src -type f | xargs sed -i ".bak" "s|service_name|${projName}|g"
find ./src -name "*.bak" | xargs rm 
find ./src -type f | xargs sed -i ".bak" "s|db[-_]name|${dbName}|g"
find ./src -name "*.bak" | xargs rm 
find ./.github -type f | xargs sed -i ".bak" "s|service[-_]name|${projName}|g"
find ./.github -name "*.bak" | xargs rm 
find Makefile -type f | xargs sed -i ".bak" "s|service[-_]name|${projName}|g" 
find ./ -name "*.bak" | xargs rm 
find ./ -name "*.DS_STORE" | xargs rm 
find ./ -name "*!efbundle" | xargs rm 
find ./ -name "*.DS_Store" | xargs rm 

mv ${PWD}/src/Pi.service-name/Pi.service-name.API/Pi.service-name.API.csproj ${PWD}/src/Pi.service-name/Pi.service-name.API/Pi.${projName}.API.csproj
mv ${PWD}/src/Pi.service-name/Pi.service-name.API ${PWD}/src/Pi.service-name/Pi.${projName}.API

#mv ${PWD}/src/Pi.service-name/Pi.service-name.Application/Pi.service-name.Application.csproj ${PWD}/src/Pi.service-name/Pi.service-name.Application/Pi.${projName}.Application.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.Application ${PWD}/src/Pi.service-name/Pi.${projName}.Application

#mv ${PWD}/src/Pi.service-name/Pi.service-name.Domain/Pi.service-name.Domain.csproj ${PWD}/src/Pi.service-name/Pi.service-name.Domain/Pi.${projName}.Domain.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.Domain ${PWD}/src/Pi.service-name/Pi.${projName}.Domain

#mv ${PWD}/src/Pi.service-name/Pi.service-name.Infrastructure/Pi.service-name.Infrastructure.csproj ${PWD}/src/Pi.service-name/Pi.service-name.Infrastructure/Pi.${projName}.Infrastructure.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.Infrastructure ${PWD}/src/Pi.service-name/Pi.${projName}.Infrastructure

#mv ${PWD}/src/Pi.service-name/Pi.service-name.IntegrationEvents/Pi.service-name.IntegrationEvents.csproj ${PWD}/src/Pi.service-name/Pi.service-name.IntegrationEvents/Pi.${projName}.IntegrationEvents.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.IntegrationEvents ${PWD}/src/Pi.service-name/Pi.${projName}.IntegrationEvents

#mv ${PWD}/src/Pi.service-name/Pi.service-name.Migrations/Pi.service-name.Migrations.csproj ${PWD}/src/Pi.service-name/Pi.service-name.Migrations/Pi.${projName}.Migrations.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.Migrations ${PWD}/src/Pi.service-name/Pi.${projName}.Migrations

mv ${PWD}/src/Pi.service-name/Pi.service-name.Worker/Pi.service-name.Worker.csproj ${PWD}/src/Pi.service-name/Pi.service-name.Worker/Pi.${projName}.Worker.csproj
mv ${PWD}/src/Pi.service-name/Pi.service-name.Worker ${PWD}/src/Pi.service-name/Pi.${projName}.Worker

#mv ${PWD}/src/Pi.service-name/Pi.service-name.API.Tests/Pi.service-name.API.Tests.csproj ${PWD}/src/Pi.service-name/Pi.service-name.API.Tests/Pi.${projName}.API.Tests.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.API.Tests ${PWD}/src/Pi.service-name/Pi.${projName}.API.Tests

#mv ${PWD}/src/Pi.service-name/Pi.service-name.Application.Tests/Pi.service-name.Application.Tests.csproj ${PWD}/src/Pi.service-name/Pi.service-name.Application.Tests/Pi.${projName}.Application.Tests.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.Application.Tests ${PWD}/src/Pi.service-name/Pi.${projName}.Application.Tests

#mv ${PWD}/src/Pi.service-name/Pi.service-name.Domain.Tests/Pi.service-name.Domain.Tests.csproj ${PWD}/src/Pi.service-name/Pi.service-name.Domain.Tests/Pi.${projName}.Domain.Tests.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.Domain.Tests ${PWD}/src/Pi.service-name/Pi.${projName}.Domain.Tests

#mv ${PWD}/src/Pi.service-name/Pi.service-name.Infrastructure.Tests/Pi.service-name.Infrastructure.Tests.csproj ${PWD}/src/Pi.service-name/Pi.service-name.Infrastructure.Tests/Pi.${projName}.Infrastructure.Tests.csproj
#mv ${PWD}/src/Pi.service-name/Pi.service-name.Infrastructure.Tests ${PWD}/src/Pi.service-name/Pi.${projName}.Infrastructure.Tests

mv ${PWD}/src/Pi.service-name/Pi.service-name.sln ${PWD}/src/Pi.service-name/Pi.${projName}.sln
mv ${PWD}/src/Pi.service-name ${PWD}/src/Pi.${projName}

mv ${PWD}/efbundle ${PWD}/src/Pi.${projName}/Pi.${projName}.Migrations/efbundle

dotnet restore ${PWD}/src/Pi.${projName}/Pi.${projName}.sln
