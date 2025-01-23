#!/bin/bash

if [ -f .env_dev_old ]; then
  export $(grep -v '^#' .env_dev_old | xargs)
fi

envsubst < appsettings.json.template > appsettings.Development_old.json
echo "File appsettings.Development_old.json is completed."

git add appsettings.json.template