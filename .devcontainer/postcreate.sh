#!/bin/bash

# required for those running ARM builds, possibly confined only to Angular 16. Try back later.
sudo apt-get update
sudo apt-get install -y python3-dev

# Show git dirty status in zsh prompt
git config devcontainers-theme.show-dirty 1

sudo chown -R $(whoami): /home/vscode/.microsoft
sudo chown -R $(whoami): /mnt/data/

chmod +x .devcontainer/clone-repos.sh
.devcontainer/clone-repos.sh

dotnet tool install -g Aspire.Cli
dotnet dev-certs https --trust
dotnet restore
