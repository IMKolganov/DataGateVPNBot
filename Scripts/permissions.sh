#!/bin/bash

# Set variables
CONTAINER_NAMES=("datagatevpn_main_container" "datagatevpn_develop_container")
EASY_RSA_PATH="/etc/openvpn/easy-rsa"
OUTPUT_DIR="/etc/openvpn/clients"
TLS_AUTH_KEY="/etc/openvpn/easy-rsa/pki/ta.key"
CRL_PKI_PATH="/etc/openvpn/easy-rsa/pki/crl.pem"
CRL_OPENVPN_PATH="/etc/openvpn/crl.pem"
CA_CERT_PATH="/etc/openvpn/easy-rsa/pki/ca.crt"
CURRENT_USER=${SUDO_USER:-$(whoami)}  # Get the user who called sudo

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
    echo "Setting ownership and permissions for $path"
    sudo chown "$uid:$gid" "$path"
    sudo chmod 664 "$path"
    sudo setfacl -m u:"$CURRENT_USER":rw "$path"  # Add access for the current user
  else
    echo "Path $path does not exist. Skipping."
  fi
}

# Function to ensure directories are accessible
ensure_directory_permissions() {
  local path=$1
  local uid=$2
  local gid=$3

  if [ -d "$path" ]; then
    echo "Setting ownership and permissions for $path"
    sudo chown -R "$uid:$gid" "$path"
    sudo chmod -R 775 "$path"
    sudo setfacl -R -m u:"$CURRENT_USER":rwx "$path"  # Add recursive access for the current user
  else
    echo "Directory $path does not exist. Skipping."
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
      set_permissions "$file" "$container_uid" "$container_gid"
    done

    # Set permissions for directories
    ensure_directory_permissions "$EASY_RSA_PATH" "$container_uid" "$container_gid"
    ensure_directory_permissions "$OUTPUT_DIR" "$container_uid" "$container_gid"
  else
    echo "Container $container_name not found. Skipping."
  fi
done

echo "All permissions have been set."
