#!/bin/bash

if [ -f .env ]; then
  export $(grep -v '^#' .env | xargs)
fi

envsubst < appsettings.json.template > appsettings.json
echo "File appsettings.json is completed."

git add appsettings.json.template