#!/bin/bash
set -euo pipefail

MANIFEST=.devcontainer/repos.json

jq -c '.repositories[]' $MANIFEST | while read repo; do
    NAME=$(echo $repo | jq -r .name)
    GROUP=$(echo $repo | jq -r .group)
    URL=$(echo $repo | jq -r .url)    
    TARGET="/mnt/data/crucible/$GROUP/$NAME"

    if [ ! -d "$TARGET" ]; then
        echo "Cloning $NAME..."
        git clone "$URL" "$TARGET"
    else
        echo "$NAME already exists, skipping."
    fi
done
