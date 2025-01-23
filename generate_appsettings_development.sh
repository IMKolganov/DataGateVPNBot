#!/bin/bash

if [ -f .env ]; then
  export $(grep -v '^#' .env_dev | xargs)
fi

envsubst < appsettings.json.template > appsettings.Development.json
echo "File appsettings.Development.json is completed."

git add appsettings.json.template