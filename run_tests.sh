#!/bin/bash
set -e # Exit immediately if a command exits with a non-zero status.

echo "--- Running .NET Unit & Integration Tests ---"
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
dotnet test src/IdentityService/IdentityService.sln
dotnet test src/NotificationService/NotificationService.sln
dotnet test src/CoachingService/CoachingService.sln

echo "--- Running Frontend E2E Tests ---"
cd src/frontend-web
npm ci
npx playwright install --with-deps
npx playwright test

echo "--- All tests passed successfully! ---"
