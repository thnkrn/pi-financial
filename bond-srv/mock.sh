#!/bin/bash

# Base directory for mocks
MOCKS_DIR="./mocks"

# Find all interface files
find ./internal -type f -path "*/interfaces/*.go" | while read -r file; do
    # Extract relevant parts of the path
    dir_part=$(echo "$file" | sed -E 's#./internal/(.+)/interfaces/(.+)\.go#\1#')
    file_part=$(echo "$file" | sed -E 's#.*/interfaces/(.+)\.go#\1_mock.go#')

    # Construct the target directory
    target_dir="$MOCKS_DIR/$dir_part"
    mkdir -p "$target_dir"

    # Generate the mock file
    mockgen -source="$file" -destination="$target_dir/$file_part" -package=$(basename "$target_dir")
    
    echo "Generated mock: $target_dir/$file_part"
done
