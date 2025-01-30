#!/bin/bash

# Specify the old and new schema names
OLD_SCHEMA="xgb_botvpnprod"
NEW_SCHEMA="xgb_botvpndev"

# Get the current directory (assumes the script is in the migrations folder)
MIGRATIONS_FOLDER=$(pwd)

# Check if the current folder contains migration files
if [ -z "$(ls -A "$MIGRATIONS_FOLDER"/*.cs 2>/dev/null)" ]; then
  echo "Error: No migration files found in $MIGRATIONS_FOLDER"
  exit 1
fi

# Loop through all .cs files in the migrations folder and replace the schema names
echo "Replacing schema '$OLD_SCHEMA' with '$NEW_SCHEMA' in migration files..."
for file in "$MIGRATIONS_FOLDER"/*.cs; do
  if [ -f "$file" ]; then
    # Replace all occurrences of OLD_SCHEMA with NEW_SCHEMA
    sed -i "s/$OLD_SCHEMA/$NEW_SCHEMA/g" "$file"
    echo "Updated schema in $file"
  fi
done

echo "Schema replacement completed."
