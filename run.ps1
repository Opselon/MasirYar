# --- Helper Functions ---
function Print-Help {
    Write-Host "Usage: .\run.ps1 [command]"
    Write-Host ""
    Write-Host "Commands:"
    Write-Host "  up        Start all services."
    Write-Host "  down      Stop all services."
    Write-Host "  logs      Follow logs for a service. Usage: .\run.ps1 logs [service-name]"
    Write-Host "  prune     Stop and remove all containers, networks, and volumes."
    Write-Host "  config    Run the configuration wizard again."
    Write-Host "  test      Run all automated tests."
    Write-Host ""
    Write-Host "If no command is provided, this help message is displayed."
}

function Run-ConfigWizard {
    Write-Host "--- Configuration Wizard ---"
    Write-Host "This wizard will help you create the .env file."
    Write-Host ""

    # Example: Prompt for a database password
    $db_password = Read-Host -Prompt "Enter the database password [default: secret]"
    if ([string]::IsNullOrWhiteSpace($db_password)) {
        $db_password = "secret"
    }

    $jwt_secret_key = Read-Host -Prompt "Enter the JWT secret key [default: YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!]"
    if ([string]::IsNullOrWhiteSpace($jwt_secret_key)) {
        $jwt_secret_key = "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!"
    }

    Write-Host ""
    Write-Host "Creating .env file..."
    Set-Content -Path ".env" -Value "DATABASE_PASSWORD=$db_password"
    Add-Content -Path ".env" -Value "JWT_SECRET_KEY=$jwt_secret_key"
    Write-Host "--- .env file created ---"
    Get-Content .env
    Write-Host "-------------------------"
    Write-Host ""
    Write-Host "Configuration complete!"
}

# --- Main Script ---

# Check for .env file, if not found, run the wizard
if (-not (Test-Path ".env")) {
    Write-Host "Welcome! It looks like this is your first time running the project."
    Run-ConfigWizard
}

# Load environment variables from .env file
Get-Content .env | ForEach-Object {
    if ($_ -match "^(.*?)=(.*)$") {
        Set-Item -Path "env:$($Matches[1])" -Value $Matches[2]
    }
}

# Command handling
param (
    [string]$Command,
    [string]$ServiceName
)

switch ($Command) {
    "up" {
        Write-Host "Starting all services..."
        docker-compose -f docker/docker-compose.yml up -d --build
    }
    "down" {
        Write-Host "Stopping all services..."
        docker-compose -f docker/docker-compose.yml down
    }
    "logs" {
        if ([string]::IsNullOrWhiteSpace($ServiceName)) {
            Write-Host "Error: Service name not provided."
            Write-Host "Usage: .\run.ps1 logs [service-name]"
            exit 1
        }
        Write-Host "Following logs for $ServiceName..."
        docker-compose -f docker/docker-compose.yml logs -f $ServiceName
    }
    "prune" {
        $confirm = Read-Host -Prompt "Are you sure you want to remove all containers, networks, and volumes? This action is irreversible. [y/N]"
        if ($confirm -eq "y") {
            Write-Host "Pruning the environment..."
            docker-compose -f docker/docker-compose.yml down -v --remove-orphans
        } else {
            Write-Host "Prune operation cancelled."
        }
    }
    "config" {
        Run-ConfigWizard
    }
    "test" {
        Write-Host "Running all tests..."
        ./run_tests.sh
    }
    default {
        Print-Help
    }
}
