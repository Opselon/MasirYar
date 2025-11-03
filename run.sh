#!/bin/bash

# --- Helper Functions ---
function print_help() {
    echo "Usage: ./run.sh [command]"
    echo ""
    echo "Commands:"
    echo "  up        Start all services."
    echo "  down      Stop all services."
    echo "  logs      Follow logs for a service. Usage: ./run.sh logs [service-name]"
    echo "  prune     Stop and remove all containers, networks, and volumes."
    echo "  config    Run the configuration wizard again."
    echo "  test      Run all automated tests."
    echo ""
    echo "If no command is provided, this help message is displayed."
}

function run_config_wizard() {
    echo "--- Configuration Wizard ---"
    echo "This wizard will help you create the .env file."
    echo ""

    # Example: Prompt for a database password
    read -p "Enter the database password [default: secret]: " db_password
    db_password=${db_password:-secret}

    read -p "Enter the JWT secret key [default: YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!]: " jwt_secret_key
    jwt_secret_key=${jwt_secret_key:-YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!}

    echo ""
    echo "Creating .env file..."
    echo "DATABASE_PASSWORD=${db_password}" > .env
    echo "JWT_SECRET_KEY=${jwt_secret_key}" >> .env
    echo "--- .env file created ---"
    cat .env
    echo "-------------------------"
    echo ""
    echo "Configuration complete!"
}

# --- Main Script ---

# Check for .env file, if not found, run the wizard
if [ ! -f .env ]; then
    echo "Welcome! It looks like this is your first time running the project."
    run_config_wizard
fi

# Load environment variables from .env file
export $(grep -v '^#' .env | xargs)

# Command handling
case "$1" in
    up)
        echo "Starting all services..."
        sudo docker compose -f docker/docker-compose.yml up --build --exit-code-from migration-runner --remove-orphans
        sudo docker compose -f docker/docker-compose.yml up -d
        ;;
    down)
        echo "Stopping all services..."
        sudo docker compose -f docker/docker-compose.yml down
        ;;
    logs)
        if [ -z "$2" ]; then
            echo "Error: Service name not provided."
            echo "Usage: ./run.sh logs [service-name]"
            exit 1
        fi
        echo "Following logs for $2..."
        sudo docker compose -f docker/docker-compose.yml logs -f $2
        ;;
    prune)
        read -p "Are you sure you want to remove all containers, networks, and volumes? This action is irreversible. [y/N] " confirm
        if [[ $confirm =~ ^[Yy]$ ]]; then
            echo "Pruning the environment..."
            sudo docker compose -f docker/docker-compose.yml down -v --remove-orphans
        else
            echo "Prune operation cancelled."
        fi
        ;;
    config)
        run_config_wizard
        ;;
    test)
        echo "Running all tests..."
        ./run_tests.sh
        ;;
    *)
        print_help
        ;;
esac
