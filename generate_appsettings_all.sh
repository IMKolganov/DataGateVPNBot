#!/bin/bash
if [ -f .env ]; then
  export $(grep -v '^#' .env | xargs)
fi

envsubst < appsettings.json.template > appsettings.json
echo "File appsettings.json is completed."

if [ -f .env_dev ]; then
  export $(grep -v '^#' .env_dev | xargs)
fi

envsubst < appsettings.json.template > appsettings.Development.json
echo "File appsettings.Development.json is completed."

git add appsettings.json.template
