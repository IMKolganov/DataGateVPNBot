#!/bin/bash

HOOKS_DIR=".git/hooks"
SOURCE_HOOKS_DIR="hooks"

if [ ! -d "$HOOKS_DIR" ]; then
  echo "Error: .git/hooks not found. Make sure this script is run from the root of a Git repository."
  exit 1
fi

cp "$SOURCE_HOOKS_DIR/pre-commit" "$HOOKS_DIR/pre-commit"
chmod +x "$HOOKS_DIR/pre-commit"

echo "Git hooks have been set up successfully!"
