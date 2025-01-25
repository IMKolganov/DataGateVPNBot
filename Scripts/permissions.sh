#!/bin/bash

# Set variables
CONTAINER_NAMES=("datagatevpn_main_container" "datagatevpn_develop_container")
EASY_RSA_PATH="/etc/openvpn/easy-rsa"
OUTPUT_DIR="/etc/openvpn/clients"
TLS_AUTH_KEY="/etc/openvpn/easy-rsa/pki/ta.key"
CRL_PKI_PATH="/etc/openvpn/easy-rsa/pki/crl.pem"
CRL_OPENVPN_PATH="/etc/openvpn/crl.pem"
CA_CERT_PATH="/etc/openvpn/easy-rsa/pki/ca.crt"
CURRENT_USER=$(whoami)  # Get the current user

# Files that require specific permissions
FILES_TO_CONFIGURE=(
  "$TLS_AUTH_KEY"
  "$CRL_PKI_PATH"
  "$CRL_OPENVPN_PATH"
  "$CA_CERT_PATH"
)

# Function to get UID and GID from a container
get_container_ids() {
  local container_name=$1
  local container_uid
  local container_gid
  container_uid=$(docker exec "$container_name" id -u)
  container_gid=$(docker exec "$container_name" id -g)
  echo "$container_uid:$container_gid"
}

# Function to set permissions for a file
set_permissions() {
  local path=$1
  local uid=$2
  local gid=$3

  if [ -e "$path" ]; then
    echo "Setting permissions for $path"
    sudo chown "$uid:$gid" "$path"
    sudo chmod 664 "$path"
  else
    echo "Path $path does not exist. Skipping."
  fi
}

# Ensure both the current user and Docker containers have access to directories
ensure_user_permissions() {
  local path=$1
  local uid=$2
  local gid=$3

  if [ -e "$path" ]; then
    echo "Ensuring user and container permissions for $path"
    sudo setfacl -m u:"$CURRENT_USER":rw "$path"
    sudo setfacl -m u:"$uid":rw "$path"
  fi
}

# Process each container
for container_name in "${CONTAINER_NAMES[@]}"; do
  if docker ps --format "{{.Names}}" | grep -q "^$container_name$"; then
    echo "Processing container: $container_name"

    # Get UID and GID from the container
    container_ids=$(get_container_ids "$container_name")
    container_uid=${container_ids%%:*}
    container_gid=${container_ids##*:}

    echo "Retrieved container_uid=$container_uid and container_gid=$container_gid for container $container_name"

    # Set permissions for essential files
    for file in "${FILES_TO_CONFIGURE[@]}"; do
      if [ -e "$file" ]; then
        echo "Setting ownership and permissions for $file"
        sudo chown "$container_uid:$container_gid" "$file"
        sudo chmod 664 "$file"
        ensure_user_permissions "$file" "$container_uid" "$container_gid"
      else
        echo "File $file does not exist. Skipping."
      fi
    done

    # Set permissions for directories
    if [ -d "$EASY_RSA_PATH" ]; then
      echo "Setting ownership and permissions for $EASY_RSA_PATH"
      sudo chown -R "$container_uid:$container_gid" "$EASY_RSA_PATH"
      sudo chmod -R 775 "$EASY_RSA_PATH"
      ensure_user_permissions "$EASY_RSA_PATH" "$container_uid" "$container_gid"
    else
      echo "Directory $EASY_RSA_PATH does not exist. Skipping."
    fi

    if [ -d "$OUTPUT_DIR" ]; then
      echo "Setting ownership and permissions for $OUTPUT_DIR"
      sudo chown -R "$container_uid:$container_gid" "$OUTPUT_DIR"
      sudo chmod -R 775 "$OUTPUT_DIR"
      ensure_user_permissions "$OUTPUT_DIR" "$container_uid" "$container_gid"
    else
      echo "Directory $OUTPUT_DIR does not exist. Skipping."
    fi
  else
    echo "Container $container_name not found. Skipping."
  fi
done

echo "All permissions have been set."
