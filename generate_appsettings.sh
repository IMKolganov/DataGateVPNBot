#!/bin/bash

if [ -f .env ]; then
  export $(grep -v '^#' .env | xargs)
fi

envsubst < appsettings.json.template > appsettings.json
echo "Файл appsettings.json completed."

envsubst < appsettings.json.template > appsettings.Development.json
echo "Файл appsettings.Development.json completed."

git add appsettings.json.template