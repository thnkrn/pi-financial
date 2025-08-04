#!/bin/bash
DIR="tmp"

if [ ! -d "$DIR" ]; then
  mkdir "$DIR"
fi

module=$1
log_file="tmp/migration-log-$module"
targetPath="$module/migrations/scripts"
config="./.sequelize-$module"
rm -f $log_file
npx sequelize-cli --options-path $config db:migrate >> $log_file
migrations=$(grep -e "^== .*\: migrating =*" $log_file)

if [ -z "$migrations"  ]; then
  exit;
fi

if [ ! -d "$targetPath/up" ]; then
  mkdir -p "$targetPath/up"
fi

if [ ! -d "$targetPath/down" ]; then
  mkdir -p "$targetPath/down"
fi

while read -r migration; do
  filename=$(echo "$migration" | sed -n 's/^== \(.*\)\:.*$/\1/p')

  if [ ! -f "$targetPath/up/$filename.sql" ]; then
    queries=$(sed -n "/$migration/,/== $filename: migrated .*/p" $log_file | sed -n 's/^Executing (default): \(.*\)/\1/p' | sed -e 's/INSERT INTO `SequelizeMeta` (`name`) VALUES (?);//g' | sed 's/\([^;\s]\)$/\1;/')
    queries+="\nINSERT INTO \`SequelizeMeta\` (\`name\`) VALUES ('$filename.js');"
    echo "$queries" >> "$targetPath/up/$filename.sql"
  fi
done <<< "$migrations"

while read -r migration; do
  undoLogs=$(npx sequelize-cli --options-path $config db:migrate:undo)
  echo "$undoLogs" >> "$log_file-undo"
  migrationName=$(echo "$undoLogs" | sed -n 's/^== \(.*\)\: reverting =*$/\1/p')

  if [ ! -f "$targetPath/down/$migrationName.sql" ]; then
    queries=$(sed -n "/== $migrationName: reverting =*/,/== $migrationName: reverted .*/p" "$log_file-undo" | sed -n 's/^Executing (default): \(.*\)/\1/p' | sed 's/\([^;\s]\)$/\1;/')
#    queries+="DELETE FROM \`SequelizeMeta\` (\`name\`) VALUES ('$migrationName.js');"
    echo "$queries" >> "$targetPath/down/$migrationName.sql"
  fi
done <<< "$migrations"

if [ -d "$DIR" ]; then
  rm -rf "tmp"
fi
