#!/bin/bash
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
sudo -E dotnet test src/IdentityService/IdentityService.sln
