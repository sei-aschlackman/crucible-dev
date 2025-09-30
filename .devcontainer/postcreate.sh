#!/bin/bash

# Show git dirty status in zsh prompt
git config devcontainers-theme.show-dirty 1

chmod +x .devcontainer/clone-repos.sh
.devcontainer/clone-repos.sh

dotnet tool install -g Aspire.Cli