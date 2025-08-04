#!/bin/sh
# read -p "Enter Project Name: " projName

template=TemplateProject
projName=GlobalEquities

find ./.github -type f | xargs sed "s|${template}|${projName}|g"
find ./src -type f | xargs sed "s|${template}|${projName}|g"
find Makefile -type f | xargs sed "s|${template}|${projName}|g" 
find ./ -name "*.DS_STORE" | xargs rm
find ./ -name "*.DS_Store" | xargs rm 

mv ${PWD}/src/Pi.${template}.API/Pi.${template}.API.csproj ${PWD}/src/Pi.${projName}.API/Pi.${projName}.API.csproj
mv ${PWD}/src/Pi.${template}.API ${PWD}/src/Pi.${projName}.API
mv ${PWD}/src/Pi.${template}.Worker/Pi.${template}.Worker.csproj ${PWD}/src/Pi.${projName}.Worker/Pi.${projName}.Worker.csproj
mv ${PWD}/src/Pi.${template}.Worker ${PWD}/src/Pi.${projName}.Worker
mv ${PWD}/src/Pi.${template}.sln ${PWD}/src/Pi.${projName}.sln

dotnet restore ${PWD}/src/Pi.${projName}.sln